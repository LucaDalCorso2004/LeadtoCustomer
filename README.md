Deployment and Installation Instructions
##1. Prerequisites
Before you can set up the system, make sure you have the following installed:

Microsoft SQL Server (for database management)
Microsoft SQL Server Management Studio (SSMS)
Visual Studio (or another IDE for running the backend project)
.NET Core SDK (if not already included with your IDE)
##2. Database Setup
Open the Database file and change the name of your server. You can do this in the connection string within the configuration files.

To locate the file, open Microsoft SQL Server Management Studio (SSMS).

Open SSMS and log in to your SQL Server.
Find the connection string file in the project folder that specifies the server name and update it with your server's name.
![image](https://github.com/user-attachments/assets/b931038b-4bee-4524-9d01-3f27959fae27)


Once you’ve updated the connection string, restart the project to make sure the changes take effect.

##3. Backend Setup
After configuring the database, open the backend project in your preferred IDE (e.g., Visual Studio).

Click the green arrow (or press F5) to start the backend server.

##4. API Authentication and Customer Access
To interact with the Customer functions, you need to log in and obtain a JWT token.

In the top-right corner of the Swagger UI (or Postman), you’ll see an input field for the Bearer token. This is where you will paste the token.

You can get the token by logging in using valid user credentials (found in the Database.file).
Example token format: Bearer <your_token_here>
![image](https://github.com/user-attachments/assets/c8fa3ea6-de0e-4e57-9ede-26c5079c9c0b)

##5. Testing the System
After logging in and entering the token, you can now test the CRUD functionality (creating, updating, viewing, and deleting leads) through the API using Swagger UI or Postman.
Conclusion
With these steps, you should be able to set up, deploy, and interact with the Lead Management System.

##Notes:
Ensure that your database server is running and accessible before starting the backend.


