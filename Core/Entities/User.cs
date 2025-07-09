using ProjectManagementSystem.Core.Enums;
using System.Text.Json.Serialization;

namespace ProjectManagementSystem.Core.Entities
{

    internal abstract class User
    {
        public int Id { get; set; }
        public string Login { get; set; }
        public string PasswordHash { get; set; }
        public abstract Role Role { get; }

        public User() { }

        [JsonConstructor]
        public User(int id, string login, string passwordHash)
        {
            Id = id;
            Login = login;
            PasswordHash = passwordHash;
        }
    }
}
