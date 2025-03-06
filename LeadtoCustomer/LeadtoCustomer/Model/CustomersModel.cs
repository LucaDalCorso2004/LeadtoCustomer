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

            // 2. Lead-Daten in Customer umwandeln
            CustomerModel customer = new CustomerModel
            {
                Name = lead.Name,
                Gender = lead.Gender,
                Address = lead.Address,
                CustomerSource = lead.LeadSource
            };

            // 3. Customer in die Kunden-Datenbank einfügen
            bool isCustomerCreated = CreateCustomer(customer);
            if (!isCustomerCreated)
            {
                Console.WriteLine("Fehler beim Erstellen des Customers.");
                return false;
            }

         

            // Falls alles erfolgreich war
            return true;
        }

        public static LeadModel GetLeadById(int leadId)
        {
            // Hier ein einfaches Beispiel, wie du ein Lead anhand der ID aus der Datenbank abholst.
            // Du könntest ADO.NET, Entity Framework oder Dapper verwenden.

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
            // Customer in die Kunden-Datenbank einfügen
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
    }
}

