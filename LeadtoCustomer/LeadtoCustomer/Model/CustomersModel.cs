using System.Data.SqlClient;
using System.Data;

namespace LeadtoCustomer.Model
{
    public class CustomersModel
    {
        public static IEnumerable<CustomerModel> GetAllCustomers()
        {
            var allcustomers = new List<CustomerModel>();

            const string query = @"SELECT id, Name, Gender,Address,Customersource  FROM customers ORDER BY id";
            bool worked;
            do
            {
                worked = true;
                using (SqlConnection conn = new SqlConnection(Database.CONNECTION_STRING))
                {
                    conn.Open();
                    using (SqlTransaction transaction = conn.BeginTransaction(IsolationLevel.Serializable))
                    {
                        try
                        {
                            using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                            {
                                using (SqlDataReader reader = cmd.ExecuteReader())
                                {
                                    while (reader.Read())
                                    {
                                        int id = reader.GetInt32(reader.GetOrdinal("id"));
                                        string name = reader.GetString(reader.GetOrdinal("Name"));
                                        string gender = reader.GetString(reader.GetOrdinal("Gender"));
                                        string address = reader.GetString(reader.GetOrdinal("Address"));

                                        string Customersource = reader.GetString(reader.GetOrdinal("Customersource"));


                                        allcustomers.Add(new CustomerModel { Id = id, Name = name, Gender = gender, Address = address, CustomerSource = Customersource });

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {

                            try
                            {
                                transaction.Rollback();
                                if (ex.GetType() != typeof(Exception))
                                    worked = false;
                            }
                            catch (Exception ex2)
                            {

                                if (ex2.GetType() != typeof(Exception))
                                    worked = false;
                            }
                        }
                    }
                }
            } while (!worked);
            return allcustomers;
        }
        public static bool TransferLeadToCustomer(int leadId)
        {
            // 1. Lead aus der Datenbank holen
            LeadModel lead = GetLeadById(leadId);
            if (lead == null)
            {
                Console.WriteLine($"Lead mit ID {leadId} nicht gefunden.");
                return false;
            }

         
            CustomerModel customer = new CustomerModel
            {
                Name = lead.Name,
                Gender = lead.Gender,
                Address = lead.Address,
                CustomerSource = lead.LeadSource
            };

            bool isCustomerCreated = CreateCustomer(customer);
            if (!isCustomerCreated)
            {
                Console.WriteLine("Fehler beim Erstellen des Customers.");
                return false;
            }

         
            return true;
        }

        public static LeadModel GetLeadById(int leadId)
        {
            

            using (SqlConnection conn = new SqlConnection(Database.CONNECTION_STRING))
            {
                conn.Open();
                string query = "SELECT * FROM leads WHERE Id = @LeadId";
                SqlCommand cmd = new SqlCommand(query, conn);
                cmd.Parameters.AddWithValue("@LeadId", leadId);

                SqlDataReader reader = cmd.ExecuteReader();
                if (reader.Read())
                {
                    return new LeadModel
                    {
                        Id = (int)reader["Id"],
                        Name = reader["Name"].ToString(),
                        Gender = reader["Gender"].ToString(),
                        Address = reader["Address"].ToString(),
                        LeadSource = reader["LeadSource"].ToString()
                    };
                }
            }
            return null;
        }

        public static bool CreateCustomer(CustomerModel customer)
        {
          
            const string query = @"INSERT INTO customers (Name, Gender, Address, CustomerSource) 
                           VALUES (@Name, @Gender, @Address, @CustomerSource)";
            using (SqlConnection conn = new SqlConnection(Database.CONNECTION_STRING))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Name", customer.Name);
                    cmd.Parameters.AddWithValue("@Gender", customer.Gender);
                    cmd.Parameters.AddWithValue("@Address", customer.Address);
                    cmd.Parameters.AddWithValue("@CustomerSource", customer.CustomerSource);

                    int rowsAffected = cmd.ExecuteNonQuery();
                    return rowsAffected > 0;
                }
            }
        }
        public static void DeleteCustomer(int id)
        {
            using (var con = new SqlConnection(Database.CONNECTION_STRING))
            {
                con.Open();
                var sql =
                    "DELETE FROM customers WHERE Id = @Id";  

                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
        public static CustomerModel GetCustomerById(int id)
        {
            using (var con = new SqlConnection(Database.CONNECTION_STRING))
            {
                con.Open();
                var sql = "SELECT * FROM customers WHERE Id = @Id"; 
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new CustomerModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                Address = reader["Address"].ToString(),
                                CustomerSource = reader["CustomerSource"].ToString()
                            };
                        }
                        else
                        {
                            return null;
                        }
                    }
                }
            }

        }
        public static void Update(CustomerModel customer, SqlConnection conn, SqlTransaction transaction)
        {
            const string query = "UPDATE customers SET Name=@Name, Gender=@Gender, Address =@Address , CustomerSource=@CustomerSource  WHERE id=@Id";
            using (var cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@Name", customer.Name);
                cmd.Parameters.AddWithValue("@Gender", customer.Gender);
                cmd.Parameters.AddWithValue("@Id", customer.Id);
                cmd.Parameters.AddWithValue("@Address", customer.Address);
                cmd.Parameters.AddWithValue("@CustomerSource", customer.CustomerSource);

                // Execute the command
                cmd.ExecuteNonQuery();
            }
        }

        public static void Update(CustomerModel customer)
        {
            using (SqlConnection conn = new SqlConnection(Database.CONNECTION_STRING))
            {
                conn.Open();
                Update(customer, conn, null);
            }

        }
    }
}

