using System.Data;

namespace LeadtoCustomer.Model
{
    public enum Roles { Administrators, Editor, Viewer }
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public Roles Role { get; set; }
    }
}
