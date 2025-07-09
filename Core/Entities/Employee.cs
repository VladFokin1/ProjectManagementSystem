using ProjectManagementSystem.Core.Enums;
using System.Text.Json.Serialization;

namespace ProjectManagementSystem.Core.Entities
{

    internal class Employee : User
    {
        [JsonPropertyName("role")]
        public override Role Role => Role.Employee;

        public Employee() : base() { }

        [JsonConstructor]
        public Employee(int id, string login, string passwordHash) : base(id, login, passwordHash)
        {
        }
    }
}
