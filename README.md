# 🚀 User Management Azure Function

[![Azure Functions](https://img.shields.io/badge/Azure%20Functions-v4-0062AD?style=flat&logo=azure-functions&logoColor=white)](https://azure.microsoft.com/en-us/services/functions/)
[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![CI/CD](https://img.shields.io/badge/CI%2FCD-GitHub%20Actions-2088FF?style=flat&logo=github-actions&logoColor=white)](https://github.com/features/actions)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

> A modern, serverless user management API built with Azure Functions, Entity Framework Core, and the Repository Pattern. Features automatic CI/CD deployment and follows software engineering best practices.

---

## ✨ Features

- 🔐 **Complete CRUD Operations** - Create, Read, Update, Delete users
- 🏗️ **Clean Architecture** - Repository Pattern with proper separation of concerns
- 🗄️ **Entity Framework Core** - SQL Server integration with migrations
- 🚀 **Serverless Architecture** - Scale automatically with Azure Functions
- ⚡ **CI/CD Pipeline** - Automatic deployment via GitHub Actions
- 📁 **Organized Structure** - Clean folder organization for maintainability
- 🔄 **Isolated Worker Model** - .NET 8 with improved performance

---

## 🛠️ Tech Stack

| Technology | Purpose |
|------------|---------|
| **Azure Functions v4** | Serverless compute platform |
| **.NET 8.0** | Runtime framework (Isolated Worker) |
| **Entity Framework Core 9** | ORM for database operations |
| **SQL Server** | Relational database |
| **Application Insights** | Monitoring and telemetry |
| **GitHub Actions** | CI/CD automation |

---

## 📂 Project Structure

```
UserManagementAzFunction/
├── 📁 Data/
│   └── ApplicationDbContext.cs       # EF Core DbContext
├── 📁 Functions/
│   ├── HelloWorld.cs                 # Test endpoint
│   ├── UserRegistration.cs           # POST /api/RegisterUser
│   ├── GetAllUsers.cs                # GET /api/GetAllUsers
│   ├── GetUserById.cs                # GET /api/users/{id}
│   ├── UpdateUser.cs                 # PUT /api/users/{id}
│   └── DeleteUser.cs                 # DELETE /api/users/{id}
├── 📁 Models/
│   └── User.cs                       # User domain model
├── 📁 Repositories/
│   ├── IUserRepository.cs            # Repository interface
│   └── UserRepository.cs             # Repository implementation
├── 📁 Migrations/
│   └── [EF Core Migrations]          # Database schema versioning
├── 📄 Program.cs                     # Dependency injection setup
└── 📄 host.json                      # Function app configuration
```

---

## 🚀 API Endpoints

### Base URL (Local)
```
http://localhost:7071/api
```

### Base URL (Azure)
```
https://<your-function-app-name>.azurewebsites.net/api
```

### Available Endpoints

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| `GET` | `/helloworld` | Test endpoint | - |
| `POST` | `/RegisterUser` | Create new user | `UserRegistrationRequest` |
| `GET` | `/GetAllUsers` | Get all users | - |
| `GET` | `/users/{id}` | Get user by ID | - |
| `PUT` | `/users/{id}` | Update user | `UpdateUserRequest` |
| `DELETE` | `/users/{id}` | Delete user | - |

---

## 🏃 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Azure Functions Core Tools v4](https://docs.microsoft.com/azure/azure-functions/functions-run-local)
- [SQL Server](https://www.microsoft.com/sql-server) or [Azure SQL Database](https://azure.microsoft.com/services/sql-database/)
- [Visual Studio Code](https://code.visualstudio.com/) or [Visual Studio 2022](https://visualstudio.microsoft.com/)

### Local Development Setup

1. **Clone the repository**
   ```bash
   git clone https://github.com/harshanaerandaperera/UserManagementAzFunction.git
   cd UserManagementAzFunction
   ```

2. **Configure local settings**
   
   Create `local.settings.json`:
   ```json
   {
     "IsEncrypted": false,
     "Values": {
       "AzureWebJobsStorage": "UseDevelopmentStorage=true",
       "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
       "SqlConnectionString": "Server=localhost;Database=UsersDB;Trusted_Connection=True;"
     }
   }
   ```

3. **Apply database migrations**
   ```bash
   dotnet ef database update
   ```

4. **Run locally**
   ```bash
   func start
   ```

5. **Test endpoints**
   
   Navigate to: `http://localhost:7071/api/helloworld`

---

## 🔧 Configuration

### Environment Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `SqlConnectionString` | Database connection string | `Server=...;Database=UsersDB;...` |
| `AzureWebJobsStorage` | Azure Storage connection | For local: `UseDevelopmentStorage=true` |
| `FUNCTIONS_WORKER_RUNTIME` | Runtime identifier | `dotnet-isolated` |

---

## 🚢 Deployment

### Automatic Deployment (CI/CD)

Every push to the `master` branch automatically triggers:

1. ✅ Build & compilation
2. ✅ Run tests (if available)
3. ✅ Package application
4. ✅ Deploy to Azure Functions

**Monitor deployments:** 
- [GitHub Actions](https://github.com/harshanaerandaperera/UserManagementAzFunction/actions)
- Azure Portal → Function App → Deployment Center

### Manual Deployment

```bash
func azure functionapp publish <your-function-app-name>
```

---

## 🏗️ Architecture

### Design Patterns

- **Repository Pattern** - Abstracts data access layer
- **Dependency Injection** - Loose coupling and testability
- **Isolated Worker Model** - Better performance and flexibility

### Key Benefits

✅ **Testable** - Mock repositories for unit testing  
✅ **Maintainable** - Clear separation of concerns  
✅ **Scalable** - Serverless auto-scaling  
✅ **Reliable** - Built-in retry policies and monitoring  

---

## 🧪 Testing

### Test Locally with Postman

Import the collection:
1. Open Postman
2. Create new collection: "User Management API"
3. Add requests using the endpoints above
4. Set base URL to `http://localhost:7071/api`

### Best Practices

- Never commit `local.settings.json` to source control
- Use Azure Key Vault for secrets in production
- Implement proper authentication/authorization for production use

---

## 📈 Monitoring

### Application Insights

Monitor your functions in real-time:
- Request rates and response times
- Failure rates and exceptions
- Custom metrics and logs

Access via: Azure Portal → Function App → Application Insights

---

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

## 👨‍💻 Author

**Harshana Erandaperera**

- GitHub: [@harshanaerandaperera](https://github.com/harshanaerandaperera)
- Email: harshanatxn@gmail.com

---

## 🙏 Acknowledgments

- Azure Functions Team for excellent documentation
- Entity Framework Core community
- Microsoft Learn for comprehensive tutorials

---

<div align="center">

**⭐ Star this repo if you find it helpful!**

Made with ❤️ using Azure Functions & .NET 8

</div>
