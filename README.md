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

* Rich domain modeling:

  * Strongly-typed enums (`TransactionType`, `TransactionStatus`, `WalletStatus`)

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

* 🚀 **In-Memory Caching** for limit validation:

  * Reduces database load
  * Improves response time
  * Applied to:

    * Per Transaction Limits
    * Daily Limits

* Optimized LINQ queries for aggregation

* Async/Await across the system for high concurrency handling

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

## 📌 Example: Daily Limit Calculation

```csharp
public async Task<decimal> GetTransactionsTodayTotalAsync(int userId)
{
    var today = DateTime.UtcNow.Date;
    var tomorrow = today.AddDays(1);

    return await _context.Transactions
        .Where(t =>
            t.FromWalletId == userId &&
            t.CreatedAt >= today &&
            t.CreatedAt < tomorrow &&
            t.Status == TransactionStatus.Success)
        .SumAsync(t => (decimal?)t.Amount) ?? 0;
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

It showcases practical implementation of:

* Modern Backend system design
* CI/CD pipelines and automation
* Cloud-native deployment using Kubernetes and GitOps

The project serves as both a **learning platform** and a **professional portfolio** to demonstrate real-world engineering practices.

