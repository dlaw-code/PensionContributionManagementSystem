# ElectricityBillPaymentSystem

An ASP.NET Core API application for managing user authentication, bill verification, bill payment, and wallet funding in an electricity bill payment system. This application supports SMS notifications and is designed using event-driven principles for handling updates to bills and payments.

---

## Prerequisites

- [.NET SDK 7.0](https://dotnet.microsoft.com/download/dotnet/7.0)
- [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads)
- [Visual Studio](https://visualstudio.microsoft.com/) or another C# IDE

## Setup and Run Instructions

### 1. Clone the Repository

Clone the project to your local machine and navigate to the project directory.

```bash
git clone https://your-repo-url/ElectricityBillPaymentSystem.git
cd ElectricityBillPaymentSystem

### 2\. Configure the Database Connection

Open Open appsettings.json and update the connection string to match your SQL Server configuration:

"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER_NAME;Database=ElectricityBillPaymentDB;User Id=YOUR_USERNAME;Password=YOUR_PASSWORD;TrustServerCertificate=True;"
}


### 3\. Apply Database Migrations

Open a terminal or command prompt, navigate to the ElectricityBillPaymentSystem.Data project directory, and run the following commands to apply the database migrations:

cd ElectricityBillPaymentSystem.Api
dotnet ef database update

### 4\. Run the Application

You can run the application using your IDE or via the command line.

#### Using Visual Studio

1.  Open the solution file (ElectricityBillPaymentSystem.sln) in Visual Studio.
    
2.  Set ElectricityBillPaymentSystem.Api as the startup project.
    
3.  Press F5 to build and run the application.
    

#### Using Command Line

Navigate to the ElectricityBillPaymentSystem.Api directory and run the following command:

dotnet run

### 5\. Access the Application

*   **API**:  Access the API at https://localhost:5001/ or http://localhost:5000.
.
    
*   **Swagger UI**: Explore the API endpoints at https://localhost:5001/swagger. to explore the API endpoints.


### Project Structure and API Endpoints
**AuthController**
This controller manages user registration and login.

POST /auth/register: Registers a new user with details in RegisterUserDto.
POST /auth/login: Authenticates a user and returns a token.

**ElectricityController**
This controller handles bill operations and wallet management.

POST /api/electricity/verify: Verifies a bill using details in CreateBillDTO and initiates a bill creation event.
POST /api/electricity/vend/{billId}/pay: Completes payment for a specified bill and triggers a payment event.
POST /api/electricity/wallets/{walletId}/add-funds: Adds funds to a user’s wallet.

 
**Event_Publishing**
Events are simulated with a mock notification service and can be integrated with real services (e.g., Amazon SNS) for production use.

bill_created: Published after successful bill verification.
payment_completed: Published upon successful bill payment.


Troubleshooting
---------------

*   Database Connection Issues: Ensure SQL Server is running and the connection string in appsettings.json is correct.
    
*   CORS Issues: Ensure CORS is properly configured in your ASP.NET Core application. You can modify the CORS settings in Program.cs if needed.
    
*   Port Conflicts: Ensure the ports you are using are not already in use on your machine.
    

Assumptions
-----------

*   The database is initialized with necessary tables and schema. If not, ensure migrations are applied successfully.
    
*   You have the required environment variables set up for your connection strings and other configurations.
    
*   The project uses default ports for SQL Server and the API. Adjust the ports in launchSettings.json or appsettings.json if necessary.