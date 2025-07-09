using ProjectManagementSystem.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Core.Entities
{
  
    internal class Manager : User
    {
        [JsonPropertyName("role")]
        public override Role Role => Role.Manager;

        public Manager() : base() { }

        [JsonConstructor]
        public Manager(int id, string login, string passwordHash) : base(id, login, passwordHash)
        {
        }
    }
}
