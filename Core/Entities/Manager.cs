using ProjectManagementSystem.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Core.Entities
{
    internal class Manager : User
    {
        public override Role Role => Role.Manager;
    }
}
