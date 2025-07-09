using ProjectManagementSystem.Core.Entities;
using ProjectManagementSystem.Core.Services.Commands;
using System.Windows.Input;

namespace ProjectManagementSystem.Core.Services
{
    internal class CommandProcessor
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly Dictionary<string, Type> _commands;

        public CommandProcessor(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _commands = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase)
            {
                ["login"] = typeof(CreateTaskCommand),
                //["my-tasks"] = typeof(ViewTasksCommand),
                // ... другие команды
            };
        }

        public void Process(string commandName, User currentUser)
        {
            if (string.IsNullOrWhiteSpace(commandName))
            {
                Console.WriteLine("Необходимо ввести команду");
                return;
            }

            if (_commands.TryGetValue(commandName, out var commandType))
            {
                try
                {
                    var command = (ICommand)_serviceProvider.GetService(commandType);
                    command.Execute(currentUser);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка выполнения команды: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine($"Неизвестная команда: {commandName}");
                Console.WriteLine("Введите 'help' для списка команд");
            }
        }
    }
}
