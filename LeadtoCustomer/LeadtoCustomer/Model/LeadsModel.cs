using System.Data.SqlClient;
using System.Data;

namespace LeadtoCustomer.Model
{
    public class LeadsModel
    {
        public static bool CreateLeads(LeadModel lead)
        {
            const string query = @"INSERT INTO leads (Name, Gender, Address, LeadSource) 
                          VALUES (@Name, @Gender, @Address, @LeadSource)";
            bool worked = false;

            using (SqlConnection conn = new SqlConnection(Database.CONNECTION_STRING))
            {
                conn.Open();
                using (SqlTransaction transaction = conn.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
                        {

                            cmd.Parameters.AddWithValue("@Name", lead.Name);
                            cmd.Parameters.AddWithValue("@Gender", lead.Gender);
                            cmd.Parameters.AddWithValue("@Address", lead.Address);
                            cmd.Parameters.AddWithValue("@LeadSource", lead.LeadSource);

                            cmd.ExecuteNonQuery();
                        }

                        transaction.Commit();
                        worked = true;
                    }
                    catch (Exception ex)
                    {
                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception rollbackEx)
                        {
                            Console.WriteLine("Rollback failed: " + rollbackEx.Message);
                        }
                        Console.WriteLine("Insert failed: " + ex.Message);
                    }
                }
            }

            return worked;
        }


        public static IEnumerable<LeadModel> GetAllLeads()
        {
            var allleads = new List<LeadModel>();

            const string query = @"SELECT id, Name, Gender,Address,LeadSource  FROM leads ORDER BY id";
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

                                        string leadsource = reader.GetString(reader.GetOrdinal("LeadSource"));


                                        allleads.Add(new LeadModel { Id = id, Name = name, Gender = gender, Address = address, LeadSource = leadsource });

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
            return allleads;
        }



        public static void Update(LeadModel lead, SqlConnection conn, SqlTransaction transaction)
        {
            const string query = "UPDATE leads SET Name=@Name, Gender=@Gender, Address =@Address , LeadSource=@LeadSource  WHERE id=@Id";
            using (var cmd = new SqlCommand(query, conn, transaction))
            {
                cmd.Parameters.AddWithValue("@Name", lead.Name);
                cmd.Parameters.AddWithValue("@Gender", lead.Gender);
                cmd.Parameters.AddWithValue("@Id", lead.Id);
                cmd.Parameters.AddWithValue("@Address", lead.Address);
                cmd.Parameters.AddWithValue("@LeadSource", lead.LeadSource);

                // Execute the command
                cmd.ExecuteNonQuery();
            }
        }

        public static void Update(LeadModel lead)
        {
            using (SqlConnection conn = new SqlConnection(Database.CONNECTION_STRING))
            {
                conn.Open();
                Update(lead, conn, null);
            }

        }
        public static LeadModel GetLeadById(int id)
        {
            using (var con = new SqlConnection(Database.CONNECTION_STRING))
            {
                con.Open();
                var sql = "SELECT * FROM leads WHERE Id = @Id"; // Dein SQL Befehl
                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new LeadModel
                            {
                                Id = Convert.ToInt32(reader["Id"]),
                                Name = reader["Name"].ToString(),
                                Gender = reader["Gender"].ToString(),
                                Address = reader["Address"].ToString(),
                                LeadSource = reader["LeadSource"].ToString()
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
        public static  void DeleteLead(int id)
        {
            using (var con = new SqlConnection(Database.CONNECTION_STRING))
            {
                con.Open();
                var sql =
                    "DELETE FROM Leads WHERE Id = @Id";  // Dein SQL Befehl

                using (var cmd = new SqlCommand(sql, con))
                {
                    cmd.Parameters.AddWithValue("@Id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }


    }
}
