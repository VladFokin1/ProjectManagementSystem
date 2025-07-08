using ProjectManagementSystem.Core.Enums;
using ProjectManagementSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Core.Services
{
    internal class CommandRegistry
    {
        private readonly IEnumerable<ICommand> _commands;

        public CommandRegistry(IEnumerable<ICommand> commands)
        {
            _commands = commands;
        }

        public IEnumerable<ICommand> GetAvailableCommands(Role userRole)
        {
            return _commands.Where(cmd =>
                cmd.RequiredRole == Role.Any ||
                cmd.RequiredRole == userRole);
        }

        public ICommand GetCommand(string name)
        {
            return _commands.FirstOrDefault(c =>
                c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        }
    }
}
