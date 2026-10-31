<img width="1600" height="684" alt="bakry-stack" src="https://github.com/user-attachments/assets/2dc13159-945b-42fc-b931-585d561f259a" />

 # 💳 iWallet

A **production-ready digital wallet system** built with **.NET** and **Clean Architecture**, powered by a modern DevOps ecosystem featuring **CI/CD, Docker, Kubernetes, and GitOps**.

Integrated with a comprehensive **Observability Stack** featuring **OpenTelemetry, Prometheus, Grafana, Loki, and Jaeger** for application monitoring, centralized logging, distributed tracing, and performance analysis.

---

## 🚀 Overview

**iWallet** is a scalable backend system that simulates a real-world **fintech wallet platform**, focusing on secure financial transactions, strict business rules, and high-performance processing.

This project demonstrates **real-world engineering practices** including:

* 🔐 Secure authentication using JWT
* 💸 Transaction processing with financial integrity
* 💡 Handling Duplicate Transactions With Idempotency
* 🛡️ Limit enforcement (per transaction & daily)
* ⚡ Distributed & In-memory caching for performance optimization
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
* Fluent Validations and DTOs

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
* build advance deployment and services

---

### 🚀 ArgoCD (GitOps)

* Continuous deployment using GitOps approach
* Automatic synchronization between repository and cluster
* Declarative infrastructure management

---

## 📊 Application Monitoring & Observability

iWallet implements an observability stack to monitor application health, analyze performance, and troubleshoot issues across financial transaction workflows.

- **OpenTelemetry:** Instruments the .NET application to collect traces, metrics, and logs.
- **OpenTelemetry Collector:** Receives, processes, and exports telemetry through configurable pipelines.
- **Jaeger:** Tracks request execution paths and identifies slow operations and performance bottlenecks.
- **Prometheus:** Collects application performance and custom business metrics.
- **Grafana Loki:** Centralizes application logs and supports log analysis using LogQL.
- **Grafana:** Provides dashboards for monitoring application health and analyzing telemetry.

### 🎯 Key Benefits

- Monitor application performance and health.
- Trace requests to identify latency and performance bottlenecks.
- Investigate errors by analyzing logs, metrics, and distributed traces.
- Gain visibility into financial transaction workflows.

**Technologies:** OpenTelemetry, OpenTelemetry Collector, Jaeger, Prometheus, Grafana Loki, Grafana.

<img width="800" height="533" alt="otel" src="https://github.com/user-attachments/assets/33bcbf57-de30-4f30-bbe7-437da9a8ecc2" />


---

## 🧠 Key Engineering Concepts

* Clean Architecture
* Repository Pattern
* Separation of Concerns
* Async/Await scalability
* Performance optimization using caching (InMemory , Distributed)
* container and container orchestration
* Observability & Monitoring
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
* OpenTelemetry
* Jaeger
* Prometheus
* Loki
* Grafana

---

## 📌 Example: Transfer between wallets

```csharp
  public async Task<Result<string>> TransferAsync(string toAccountNumber, decimal amount, int userId)
  {
      const string transferOperation = "transfer";
      using var activity = TransactionActivitySource.Source.StartActivity("transaction.transfer");

      activity?.AddEvent(new ActivityEvent("start.transfer.process"));
      

      Stopwatch st = Stopwatch.StartNew();

      _logger.LogInformation("Starring Transfer Process. UserId: {UserId}, ToAccount: {toAccount}, Amount: {Amount}",
          userId,
          toAccountNumber,
          amount);

      //activity.AddEvent();

      var senderWallet = await _context.Wallets.Include(w => w.User).FirstOrDefaultAsync(u => u.UserId == userId);
      if (senderWallet == null || senderWallet.Status != WalletStatus.Active)
      {
          Log.Warning("Sender wallet not found or not active. UserId: {UserId}", userId);
          activity?.SetTag("transaction.outcome", "failure");
          BusinessMetricsHelper.RecordBusinessOperation(module, transferOperation, "failure");

          return Result<string>.Failure(TransactionSharedErrors.InValidUserWallet());
      }

      var receiverWallet = await _context.Wallets.Include(w => w.User).FirstOrDefaultAsync(an => an.WalletNumber == toAccountNumber);
      if (receiverWallet == null || receiverWallet.Status != WalletStatus.Active)
      {
          Log.Warning("Receiver wallet not found or not active. UserId: {UserId}", userId);
          activity?.SetTag("transaction.outcome", "failure");

          BusinessMetricsHelper.RecordBusinessOperation(module, transferOperation, "failure");

          return Result<string>.Failure(TransferErrors.InValidReceiverWallet());
      }

      if (senderWallet.Id == receiverWallet.Id)
      {
          return Result<string>.Failure(TransferErrors.Conflict());
      }

      if (senderWallet.Balance < amount)
      {
          _logger.LogWarning("InSufficient Balance to transfer");
          activity?.SetTag("transaction.outcome", "failure");

          BusinessMetricsHelper.RecordBusinessOperation(module, transferOperation, "failure");

          return Result<string>.Failure(TransactionSharedErrors.InsufficientBalance());
      }

      activity?.SetTag("sender_wallet.id", senderWallet.Id);
      activity?.SetTag("receiver_wallet.id", receiverWallet.Id);

      // frud pervention and get value from cache

      var limit = await _limitService.GetUserLimitAsync(senderWallet.UserId);

      var todyTotal = await GetTransactionsTodayTotalAsync(senderWallet.UserId);
      if (todyTotal + amount > limit.DailyLimit)
      {
          _logger.LogWarning("user {UserId} is Exceeded Daily limit", userId);
          activity?.SetTag("transaction.outcome", "failure");

          BusinessMetricsHelper.RecordBusinessOperation(module, transferOperation, "failure");

          return Result<string>.Failure(TransferErrors.ExceededDailylimit());
      }

      await _context.SaveChangesAsync();

      var reference = GenerateReference(TransactionType.Transfer);


      _logger.LogInformation(
      "Database transaction start for transfer. FromWallet: {FromWallet}, ToWallet: {ToWallet}",
      senderWallet.WalletNumber,
      receiverWallet.WalletNumber);

      
      using var dbTransaction = await _context.Database.BeginTransactionAsync();

      activity?.AddEvent(new ActivityEvent("transfer.db_transaction.started"));

      try
      {
          senderWallet.Balance -= amount;
          receiverWallet.Balance += amount;

          activity?.AddEvent(
          new ActivityEvent("transfer.balances.updated"));

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

          activity?.AddEvent(new ActivityEvent("end.db.transaction.successfly"));

          var recepit = new TransferReceiptDto
          {
              TransactionReference = reference,
              SenderName = senderWallet.User.UserName,
              SenderWalletNumber = senderWallet.WalletNumber,
              ReceiverName = receiverWallet.User.UserName,
              ReceiveWalletNumber = receiverWallet.WalletNumber,
              Amount = amount,
              Fees = 0,
              Currency = "SAR",
              Date = DateTime.UtcNow,
              Status = transaction.Status.ToString(),
          };

          var path = Path.Combine("receipts/");

          if (!Directory.Exists(path))
              Directory.CreateDirectory(path);

          var logoPath = Path.Combine("wwwroot/assets", "iWallet-logo.png");

          var document = new TransferReceiptDocument(recepit, logoPath);

          document.GeneratePdf($"receipts/receipt-{recepit.TransactionReference}.pdf");

          _logger.LogInformation("Transfer completed successfully. Reference: {Reference}, Amount: {Amount}, Sender: {Sender}, Receiver: {Receiver}"
                  ,reference,amount,senderWallet.WalletNumber,receiverWallet.WalletNumber);

          activity?.SetTag("transaction.outcome", "success");
          activity?.AddEvent(new ActivityEvent("end.transfer.process.successfly"));

          BusinessMetricsHelper.RecordBusinessOperation(module, transferOperation, "success");
          BusinessMetricsHelper.RecordBusinessOperationDuration(module, transferOperation, st.ElapsedMilliseconds);


          return Result<string>.Success($"Transfer Completed Successfly with Transaction Reference {reference}");

      }

      catch (Exception ex)
      {
          await dbTransaction.RollbackAsync();

          _logger.LogError(ex.Message, "Transfer failed. UserId: {UserId}, Amount: {Amount}, Reference: {Reference}",userId ,amount ,reference);

          activity?.SetTag("transaction.outcome", "failure");
          activity?.SetStatus(ActivityStatusCode.Error,"Transfer failed");
          activity?.AddEvent(new ActivityEvent("transfer.failed"));

          throw;
      }
  }
```

---

## ⚖️ Design Decisions & Trade-offs

* Implemented Idempotency using Distributed Caching (Redis) to prevent duplicate financial transactions, with a trade-off of temporary storage overhead and key management complexity.
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

