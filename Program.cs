using Microsoft.Extensions.DependencyInjection;
using ProjectManagementSystem.Core.Entities;
using ProjectManagementSystem.Core.Enums;
using ProjectManagementSystem.Core.Exceptions;
using ProjectManagementSystem.Core.Interfaces;
using ProjectManagementSystem.Core.Services;
using ProjectManagementSystem.Infrastructure.Logging;
using System.Reflection;
using System.Text;

namespace ProjectManagementSystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            using (var serviceProvider = ConfigureServices())
            {
                var authenticator = serviceProvider.GetService<IAuthenticator>();
                var commandProcessor = serviceProvider.GetService<CommandProcessor>();
                var commandRegistry = serviceProvider.GetService<CommandRegistry>();
                var logger = serviceProvider.GetService<ILogger>();

                User currentUser = null;
                logger.LogInformation("Приложение запущено");
                while (true)
                {
                    try
                    {
                        Console.Clear();
                        if (currentUser == null)
                        {

                            Console.WriteLine("=== Система управления задачами ===");
                            Console.WriteLine("Введите логин и пароль для входа");

                            Console.Write("Логин: ");
                            var login = Console.ReadLine();

                            Console.Write("Пароль: ");
                            var password = ReadPassword();

                            currentUser = authenticator.Authenticate(login, password);

                            Console.WriteLine($"\nДобро пожаловать, {currentUser.Login}!");
                            Console.WriteLine($"Ваша роль: {currentUser.Role}");
                            Console.WriteLine("Нажмите любую клавишу для продолжения...");
                            Console.ReadKey();
                        }
                        else
                        {
                            // Отображение меню на основе роли
                            DisplayMenu(currentUser, commandRegistry);

                            // Обработка команды
                            Console.Write("\nВведите команду: ");
                            var commandName = Console.ReadLine();

                            if (string.Equals(commandName, "logout", StringComparison.OrdinalIgnoreCase))
                            {
                                currentUser = null; // Сбрасываем авторизацию
                                continue;
                            }


                            if (string.Equals(commandName, "exit", StringComparison.OrdinalIgnoreCase))
                            {
                                Console.WriteLine("Выход");
                                break;
                            }

                            var command = commandRegistry.GetCommand(commandName);
                            if (command != null)
                            {


                                // Проверка прав доступа
                                if (command.RequiredRole != Role.Any &&
                                    command.RequiredRole != currentUser.Role)
                                {
                                    Console.WriteLine("Недостаточно прав для выполнения команды");
                                }
                                else
                                {
                                    command.Execute(currentUser);
                                }
                            }
                            else
                            {
                                Console.WriteLine("Неизвестная команда");
                            }
                        }
                    }
                    catch (AuthenticationException ex)
                    {
                        Console.WriteLine($"Ошибка аутентификации: {ex.Message}");
                        Console.ReadKey();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("Произошла непредвиденная ошибка");
                        logger.LogError(ex, "Непредвиденная ошибка");
                        Console.ReadKey();
                    }
                }
            }
        }

        private static ServiceProvider ConfigureServices()
        {
            var services = new ServiceCollection();

            // Регистрация зависимостей
            services.AddSingleton<ILogger, FileLogger>();
            services.AddSingleton<IAuthenticator, DatabaseAuthenticator>();
            services.AddSingleton<IUserRepository, JsonUserRepository>();
            services.AddSingleton<ITaskRepository, JsonTaskRepository>();
            services.AddSingleton<ITaskService, TaskService>();
            services.AddSingleton<IUserService, UserService>();
            services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();

            var encryptionKey = Encoding.UTF8.GetBytes("16-char-key-1234"); //хранить так небезопасно
            var iv = new byte[16]; // В реальном приложении должен быть уникальным
            services.AddSingleton<IDataEncryptor>(new AesDataEncryptor(encryptionKey, iv));

            services.AddSingleton<IUserRepository>(provider =>
            {
                var encryptor = provider.GetRequiredService<IDataEncryptor>();
                var hasher = provider.GetRequiredService<IPasswordHasher>();
                return new JsonUserRepository("users.json", encryptor, hasher);
            });

            services.AddSingleton<ITaskRepository>(provider =>
            {
                var encryptor = provider.GetRequiredService<IDataEncryptor>();
                var hasher = provider.GetRequiredService<IPasswordHasher>();
                return new JsonTaskRepository("tasks.json", encryptor, hasher);
            });

            var commandTypes = Assembly.GetExecutingAssembly()
                                       .GetTypes()
                                       .Where(t => typeof(ICommand).IsAssignableFrom(t) && !t.IsInterface);


            //регистрация команд
            foreach (var type in commandTypes)
            {
                services.AddTransient(type);
                services.AddTransient<ICommand>(provider =>
                    (ICommand)provider.GetRequiredService(type));
            }

            services.AddSingleton<CommandRegistry>(provider =>
            {
                var commands = provider.GetServices<ICommand>();
                return new CommandRegistry(commands);
            });

            // Регистрация процессора команд
            services.AddSingleton<CommandProcessor>();

            return services.BuildServiceProvider();
        }

        private static string ReadPassword()
        {
            var password = new System.Text.StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(intercept: true);
                if (key.Key == ConsoleKey.Enter)
                    break;

                if (key.Key == ConsoleKey.Backspace && password.Length > 0)
                {
                    password.Remove(password.Length - 1, 1);
                    Console.Write("\b \b");
                }
                else if (!char.IsControl(key.KeyChar))
                {
                    password.Append(key.KeyChar);
                    Console.Write("*");
                }
            }
            Console.WriteLine();
            return password.ToString();
        }

        private static void DisplayMenu(User user, CommandRegistry registry)
        {
            Console.WriteLine($"\n=== ГЛАВНОЕ МЕНЮ ({user.Login}, {user.Role}) ===");
            Console.WriteLine("Доступные команды:");

            var availableCommands = registry.GetAvailableCommands(user.Role);

            foreach (var command in availableCommands)
            {
                Console.WriteLine($"  {command.Name,-15} - {command.Description}");
            }
            Console.WriteLine("  logout          - Завершить работу");
            Console.WriteLine("  exit            - Завершить работу");
        }
    }
}