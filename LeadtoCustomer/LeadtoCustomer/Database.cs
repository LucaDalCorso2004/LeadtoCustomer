using LeadtoCustomer.Model;
using System.Data;
using System.Data.SqlClient;

namespace LeadtoCustomer
{
    internal class Database
    {
        public static readonly string DATABASE_NAME = @"LeadtoCustomer";
        public static readonly string MASTER_CONNECTION_STRING = @"Server=LAPTOP-TSP1Q2RF\SQLEXPRESS; Integrated Security=True;";
        public static readonly string CONNECTION_STRING = $@"Server=LAPTOP-TSP1Q2RF\SQLEXPRESS; Database={DATABASE_NAME}; Integrated Security = True;";


        private static void CreateDatabaseIfNotExists()
        {
            string createDbQuery = $@"IF NOT EXISTS(SELECT * FROM sys.databases WHERE name = '{DATABASE_NAME}') CREATE DATABASE {DATABASE_NAME}";
            using (var masterConnection = new SqlConnection(MASTER_CONNECTION_STRING))
            {
                var command = new SqlCommand(createDbQuery, masterConnection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
        }

        private static void CreateTablesIfNotExists()
        {
            const string createLeadsDbQuery = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='leads' and xtype='U')
                BEGIN
                    CREATE TABLE leads (
                        id int IDENTITY(1,1) PRIMARY KEY,
                        Name nvarchar(50) NOT NULL,
                        Gender nvarchar(50) NOT NULL,
                        Address nvarchar(60) NOT NULL,
                        LeadSource nvarchar (60) NOT NULL,
                    )
                END";
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                var command = new SqlCommand(createLeadsDbQuery, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }

            const string createCustomersDbQuery = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='customers' and xtype='U')
                BEGIN
                    CREATE TABLE customers (
                        id int IDENTITY(1,1) PRIMARY KEY,
                        Name nvarchar(50) NOT NULL,
                        Gender nvarchar(50) NOT NULL,
                        Address nvarchar(60) NOT NULL,
                        Customersource nvarchar (60) NOT NULL,
                    )
                END";


            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                var command = new SqlCommand(createCustomersDbQuery, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
            const string createUsersDbQuery = @"IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='users' and xtype='U')
                BEGIN
                    CREATE TABLE users (
                        id int IDENTITY(1,1) PRIMARY KEY,
                        username nvarchar(50) NOT NULL,
                        role nvarchar(50) NOT NULL,
                        password_hash nvarchar(60) NOT NULL
                    )
                END";
            using (var connection = new SqlConnection(CONNECTION_STRING))
            {
                var command = new SqlCommand(createUsersDbQuery, connection);
                command.Connection.Open();
                command.ExecuteNonQuery();
            }
       
        }

        public static void Initialize()
        {
            CreateDatabaseIfNotExists();
            CreateTablesIfNotExists();
        }

        private static bool IsEmpty(string tableName)
        {
            string query = $"SELECT COUNT(*) FROM {tableName}";

            int count = 0;
            using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    count = Convert.ToInt32(cmd.ExecuteScalar());

                }
            }
            return count == 0;
        }

        private static void SeedUsers()
        {
            if (!IsEmpty("users"))
            {
                return;
            }

      

            var seedUsers = new List<Dictionary<string, string>>
            {
                new Dictionary<string, string>
                    {
                        { "username", "admin" },
                        { "password_hash", UsersModel.HashAndSaltPassword("adminpass") },
                        { "role", Roles.Administrators.ToString() }
                },
                new Dictionary<string, string>
                    {
                        { "username", "editor" },
                        { "password_hash", UsersModel.HashAndSaltPassword("Editorpass") },
                        { "role", Roles.Editor.ToString() }
                },
                  new Dictionary<string, string>
                    {
                        { "username", "Viewer" },
                        { "password_hash", UsersModel.HashAndSaltPassword("Viewerpass") },
                        { "role", Roles.Viewer.ToString() }
                },
            };

            using (SqlConnection conn = new SqlConnection(CONNECTION_STRING))
            {
                conn.Open();

                foreach (var data in seedUsers)
                {
                    const string query = "INSERT INTO users (username, password_hash, role) VALUES (@Username, @PasswordHash, @Role)";
                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@Username", data["username"]);
                        cmd.Parameters.AddWithValue("@PasswordHash", data["password_hash"]);
                        cmd.Parameters.AddWithValue("@Role", data["role"]);

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }


        public static void Seed()
        {
           
            SeedUsers();
        }



    }
}
