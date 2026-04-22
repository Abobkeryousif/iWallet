# 💳 iWallet

A **production-ready digital wallet system** built with **.NET**, designed using **Clean Architecture** and powered by a modern **DevOps ecosystem** including CI/CD, Docker, and Kubernetes with GitOps deployment.

---

## 🚀 Overview

**iWallet** is a scalable backend system that simulates a real-world **fintech wallet platform**, focusing on secure financial transactions, strict business rules, and high-performance processing.

This project demonstrates **real-world engineering practices** including:

* 🔐 Secure authentication using JWT
* 💸 Transaction processing with financial integrity
* 🛡️ Limit enforcement (per transaction & daily)
* ⚡ In-memory caching for performance optimization
* 🐳 Containerization using Docker
* 🔄 Advance Automated CI/CD pipelines
* ☸️ Kubernetes deployment using GitOps (ArgoCD)

---

## 🏗️ Architecture

The system is designed using **Clean Architecture** principles to ensure **scalability, maintainability, and separation of concerns**.

---

### 🧩 Domain Layer (Core)

The core of the system where all **business rules and domain models** are defined:

* Core Entities:

  * `User`
  * `Wallet`
  * `Transaction`
  * `LedgerEntry`
  * `Beneficiery`


  * Strongly-typed enums (`TransactionType`, `TransactionStatus`, `WalletStatus`,`WalletType`)

---

### ⚙️ Application Layer

Responsible for **use case orchestration and business workflows**:

* Transaction processing:

  * Transfer
  * Deposit
  * Withdrawal

* Limit validation engine:

  * Per-transaction limit enforcement
  * Daily limit validation (based on transaction aggregation)

* Coordinates between domain and infrastructure

* Fully asynchronous for better scalability

---

### 🏗️ Infrastructure Layer

Handles **external systems and persistence concerns**:

* Database integration using Entity Framework Core
* Repository pattern implementation
* Optimized queries for financial operations
* Aggregation queries for daily transaction calculations

---

### 🌐 API Layer

Exposes the system through **secure RESTful APIs**:

* Clean and well-structured endpoints
* JWT-based authentication & authorization
* Stateless architecture for scalability
* Validation and centralized error handling

---

## ⚡ Performance & Optimization

🌐 Distributed Caching with Redis

To support scalability and consistency across multiple instances, a Distributed Cache using Redis is implemented.

Why Redis?

🚀 High-performance in-memory data store
🔗 Shared cache across multiple services/instances
⏱️ Supports expiration (TTL) for dynamic data like limits

Use Cases in iWallet:

🔁 Idempotency Handling (Using Redis)

To ensure safe and repeatable operations (especially in financial transactions), Idempotency is implemented.

💡 Concept:

If the same request is sent multiple times (e.g., due to retry or network issues), it should only be processed once.

⚙️ How It Works:
Client sends request with Idempotency Key
Backend checks Redis:
✅ If key exists → return cached response (no DB operation)

❌ If not → process request normally

Store result in Redis with a TTL (e.g.)
🎯 Benefits:
🛡️ Prevents duplicate transactions
🔄 Safe retries for clients
⚡ Faster responses for repeated requests
🔗 Combined Impact



* 🚀 **In-Memory Caching** for limit validation:

  * Reduces database load
  * Improves response time
  * Applied to:

    * Per Transaction Limits
    * Daily Limits

* Async/Await across the system for high concurrency handling

By combining:

In-Memory Cache ⚡
Distributed Cache (Redis) 🌐
Idempotency 🔁

The system achieves:

High performance 🚀
Strong consistency 🔒
Fault-tolerant transaction handling 💪

---

📒 Ledger & Audit Trail

All financial operations are recorded using a Ledger Entry system, ensuring full traceability of every transaction.

Each transaction generates corresponding ledger entries, providing a clear audit trail of fund movements and enabling accurate tracking, transparency, and easier debugging.

---

## 🔄 CI/CD & DevOps

### 🔧 GitHub Actions

A fully automated CI/CD pipeline that:

* Builds the application
* Runs automated tests
* Jobs Timeout || Jobs Cocurrency
* Reusable Workflow || Custom actions
* Caching
* Builds Docker images
* Deploy docker image to GHCR (Github Container Registery)
* Auto override docker image tag in GitOps Repo


Ensuring:

* Code quality
* Continuous integration
* Reliable delivery

---

### 🐳 Docker

* Containerized .NET application
* Optimized Dockerfile for production environments

---

### ☸️ Kubernetes

* Deployed on a Kubernetes cluster
* Designed for scalability and resilience

---

### 🚀 ArgoCD (GitOps)

* Continuous deployment using GitOps approach
* Automatic synchronization between repository and cluster
* Declarative infrastructure management

---

## 🧠 Key Engineering Concepts

* Clean Architecture
* Repository Pattern
* Separation of Concerns
* Async/Await scalability
* Performance optimization using caching
* container and container orchestration
* DevOps automation (CI/CD)
* GitOps workflow

---

## 🛠️ Tech Stack

* .NET 10
* ASP.NET Core Web API
* Entity Framework Core
* SQL Server
* Docker
* Kubernetes
* ArgoCD
* GitHub Actions

---

## 📌 Example: Transfer between wallets

```csharp
 public async Task<string> TransferAsync(string toAccountNumber, decimal amount, int userId)
        {
            if (amount <= 0)
                throw new Exception("Invalid Amount");

            var senderWallet = await _context.Wallets.FirstOrDefaultAsync(u => u.UserId == userId);
            if (senderWallet == null || senderWallet.Status != WalletStatus.Active)
                throw new Exception("invalid sender wallet");

            var receiverWallet = await _context.Wallets.FirstOrDefaultAsync(an => an.WalletNumber == toAccountNumber);
            if (receiverWallet == null || receiverWallet.Status != WalletStatus.Active)
                throw new Exception("Invalid receiver wallet");

            if (senderWallet.Id == receiverWallet.Id)
                throw new Exception("you can't transfer to yourself");

            if (senderWallet.Balance < amount)
                throw new Exception("insufficient balance");

            // frud pervention and get value from cache

            var limit = await _limitService.GetUserLimitAsync(senderWallet.UserId);
            if (amount > limit.PerTransactionLimit)
                throw new Exception("Exceeded per transaction limit 5000");

            var todyTotal = await GetTransactionsTodayTotalAsync(senderWallet.UserId);
            if (todyTotal + amount > limit.DailyLimit)
                throw new Exception("You already Exceeded daily limit 200000");

            await _context.SaveChangesAsync();

            var reference = GenerateReference(TransactionType.Transfer);

            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                senderWallet.Balance -= amount;
                receiverWallet.Balance += amount;

                var transaction = new Transaction
                {
                    FromWalletId = senderWallet.Id,
                    ToWalletId = receiverWallet.Id,
                    Amount = amount,
                    TransactionType = TransactionType.Transfer,
                    Reference = reference,
                    Status = TransactionStatus.Success
                };

                await _context.Transactions.AddAsync(transaction);
                _context.SaveChanges();

                var senderLedger = new LedgerEntry
                {
                    WalletId = senderWallet.Id,
                    TransactionId = transaction.Id,
                    Debit = amount,
                    Credit = 0,
                    Particulars = $"Transfer to {senderWallet.WalletNumber}"
                };

                await _context.LedgerEntries.AddAsync(senderLedger);
                _context.SaveChanges();

                var receiverLedger = new LedgerEntry
                {
                    WalletId = receiverWallet.Id,
                    TransactionId = transaction.Id,
                    Debit = 0,
                    Credit = amount,
                    Particulars = $"Successfly receive transaction from {receiverWallet.WalletNumber}"
                };

                await _context.LedgerEntries.AddAsync(receiverLedger);
                _context.SaveChanges();

                senderLedger.UpdatedAt = DateTime.UtcNow;
                receiverWallet.UpdatedAt = DateTime.UtcNow;

                _context.Wallets.Update(senderWallet);
                _context.Wallets.Update(receiverWallet);

                await _context.SaveChangesAsync();

                await dbTransaction.CommitAsync();

                return $"Transfer Completed Successfly with Transaction Reference {reference}";
                }

            catch
            {
                await dbTransaction.RollbackAsync();
                throw;
            }
        }
```

---

## ⚖️ Design Decisions & Trade-offs

* Used **in-memory caching** for limits to reduce database pressure, with a trade-off of potential cache staleness (acceptable within short windows).
* Daily limits are calculated using **transaction aggregation** instead of storing counters to ensure accuracy and consistency.
* Chose **stateless APIs with JWT** to support horizontal scaling.
* Applied **Clean Architecture** to maintain long-term maintainability and testability.

---

## 🔮 Future Enhancements

* 🔐 Role-based access control (RBAC)
* ⚡ Distributed caching using Redis
* 📈 Analytics & reporting dashboard
* 🌍 Multi-currency support
* 🧾 Audit logging system
* 🚀 Background Jobs
* ☁️ Full cloud deployment on Azure using AKS(Azure Kubernetes service) with CI/CD and GitOps improvements

---

## 📂 Project Structure

```
iWallet
│
├── iWallet.Domain
├── iWallet.Application
├── iWallet.Infrastructure
└── iWallet.API
```

---

## 🤝 Contribution

Contributions are welcome! Feel free to fork the project and submit a pull request.

---

## 📄 License

This project is developed as part of my continuous learning journey in **Software Engineering and DevOps**.

The project serves as both a **learning platform** and a **professional portfolio** to demonstrate real-world engineering practices.

