using System.Data.SqlClient;
using System.Data;
using System.Data.SqlClient;
namespace LeadtoCustomer.Model
{
    public class LedgersModel
    {
        public static bool CreateLedgers(LegerModel ledger)
        {
            const string query = @"INSERT INTO ledgers (Name, Gender, Adress, LederSource) 
                          VALUES (@Name, @Gender, @Adress, @LederSource)";
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
                            cmd.Parameters.AddWithValue("@Name", ledger.Name);
                            cmd.Parameters.AddWithValue("@Gender", ledger.Gender);
                            cmd.Parameters.AddWithValue("@Adress", ledger.Adress);
                            cmd.Parameters.AddWithValue("@LederSource", ledger.Leadsource);

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


        public static IEnumerable<LegerModel> GetAllLedgers()
        {
            var allLedgers = new List<LegerModel>();

            const string query = @"SELECT id, Name, Gender,Adress,LederSource  FROM ledgers ORDER BY id";
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
                                        string adress = reader.GetString(reader.GetOrdinal("Adress"));
                                    
                                        string lederSource = reader.GetString(reader.GetOrdinal("LederSource"));


                                        allLedgers.Add(new LegerModel { Id = id, Name = name, Gender = gender, Adress = adress, Leadsource = lederSource});

                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
                            //Console.WriteLine("  Message: {0}", ex.Message);

                            // Attempt to roll back the transaction.
                            try
                            {
                                transaction.Rollback();
                                if (ex.GetType() != typeof(Exception))
                                    worked = false;
                            }
                            catch (Exception ex2)
                            {
                                // Handle any errors that may have occurred on the server that would cause the rollback to fail.
                                //Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
                                //Console.WriteLine("  Message: {0}", ex2.Message);
                                if (ex2.GetType() != typeof(Exception))
                                    worked = false;
                            }
                        }
                    }
                }
            } while (!worked);
            return allLedgers;
        }

        //public static LedgersModel SelectOne(int id, SqlConnection conn, SqlTransaction? transaction)
        //{
        //    LedgersModel retLedger;
        //    const string query = @"SELECT id, name, balance FROM ledgers WHERE id=@Id";

        //    using (SqlCommand cmd = new SqlCommand(query, conn, transaction))
        //    {
        //        cmd.Parameters.AddWithValue("@Id", id);
        //        using (SqlDataReader reader = cmd.ExecuteReader())
        //        {
        //            if (!reader.Read())
        //                throw new Exception($"No Ledger with id {id}");

        //            int ordId = reader.GetInt32(reader.GetOrdinal("id"));
        //            string ordName = reader.GetString(reader.GetOrdinal("name"));
        //            decimal ordBalance = reader.GetDecimal(reader.GetOrdinal("balance"));

        //            retLedger = new LedgersModel { Id = ordId, Name = ordName, Balance = ordBalance };

        //        }
        //    }
        //    return retLedger;
        //}

        //public static LedgersModel SelectOne(int id)
        //{
        //    LedgersModel retLedger = null;
        //    bool worked;

        //    do
        //    {
        //        worked = true;
        //        using (SqlConnection conn = new SqlConnection(Database.CONNECTION_STRING))
        //        {
        //            conn.Open();
        //            using (SqlTransaction transaction = conn.BeginTransaction(IsolationLevel.Serializable))
        //            {
        //                try
        //                {
        //                    retLedger = SelectOne(id, conn, transaction);
        //                }
        //                catch (Exception ex)
        //                {
        //                    //Console.WriteLine("Commit Exception Type: {0}", ex.GetType());
        //                    //Console.WriteLine("  Message: {0}", ex.Message);

        //                    // Attempt to roll back the transaction.
        //                    try
        //                    {
        //                        transaction.Rollback();
        //                        if (ex.GetType() != typeof(Exception))
        //                            worked = false;
        //                    }
        //                    catch (Exception ex2)
        //                    {
        //                        // Handle any errors that may have occurred on the server that would cause the rollback to fail.
        //                        //Console.WriteLine("Rollback Exception Type: {0}", ex2.GetType());
        //                        //Console.WriteLine("  Message: {0}", ex2.Message);
        //                        if (ex2.GetType() != typeof(Exception))
        //                            worked = false;
        //                    }
        //                }
        //            }
        //        }
        //    } while (!worked);
        //    return retLedger;
        //}

        //public static void Update(LegerModel ledger, SqlConnection conn, SqlTransaction transaction)
        //{
        //    const string query = "UPDATE ledgers SET name=@Name, balance=@Balance WHERE id=@Id";
        //    using (var cmd = new SqlCommand(query, conn, transaction))
        //    {
        //        cmd.Parameters.AddWithValue("@Name", ledger.Name);
        //        cmd.Parameters.AddWithValue("@Balance", ledger.Balance);
        //        cmd.Parameters.AddWithValue("@Id", ledger.Id);

        //        // Execute the command
        //        cmd.ExecuteNonQuery();
        //    }
        //}

        //public static void Update(LedgersModel ledger)
        //{
        //    using (SqlConnection conn = new SqlConnection(Database.CONNECTION_STRING))
        //    {
        //        conn.Open();
        //        Update(ledger, conn, null);
        //    }

        //}
    }
}
