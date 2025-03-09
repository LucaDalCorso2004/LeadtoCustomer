using System.Data.SqlClient;
using System.Data;

namespace LeadtoCustomer.Model
{
    public class UsersModel
    {
        private static bool VerifyPassword(string clearTextPassword, UserModel user)
        {
            return BCrypt.Net.BCrypt.Verify(clearTextPassword, user.PasswordHash);
        }
        public static string HashAndSaltPassword(string clearTextPassword)
        {
            return BCrypt.Net.BCrypt.HashPassword(clearTextPassword);
        }

        public static UserModel Authenticate(LoginModel login)
        {
            UserModel retUser;

            const string query = @"SELECT id, username, password_hash, role FROM users WHERE username=@Username";
            using (SqlConnection conn = new SqlConnection(Database.CONNECTION_STRING))
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@Username", login.Username);
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (!reader.Read())
                            return null;

                        int ordId = reader.GetInt32(reader.GetOrdinal("id"));
                        string ordUsername = reader.GetString(reader.GetOrdinal("username"));
                        string ordPasswordHash = reader.GetString(reader.GetOrdinal("password_hash"));
                        Roles ordRole = (Roles)Enum.Parse(typeof(Roles), reader.GetString(reader.GetOrdinal("role")), true);
                        retUser = new UserModel { Id = ordId, Username = ordUsername, PasswordHash = ordPasswordHash, Role = ordRole };

                        if (!VerifyPassword(login.Password, retUser))
                        {
                            return null;
                        }

                    }
                }
            }
            return retUser;
        }
    }
}
