using ProjectManagementSystem.Core.Entities;
using ProjectManagementSystem.Core.Enums;
using ProjectManagementSystem.Core.Interfaces;
using ProjectManagementSystem.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Core.Services.Commands
{
    internal class RegisterUserCommand : ICommand
    {
        public string Name => "register-user";
        public string Description => "Зарегистрировать пользователя";
        public Role RequiredRole => Role.Manager;

        private readonly IUserService _userService;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger _logger;

        public RegisterUserCommand(
            IUserService userService,
            IPasswordHasher passwordHasher,
            ILogger logger)
        {
            _userService = userService;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public void Execute(User currentUser)
        {
            Console.WriteLine("\n=== Регистрация нового пользователя ===");

            // Ввод логина
            string login;
            while (true)
            {
                Console.Write("Логин: ");
                login = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(login))
                {
                    Console.WriteLine("Логин не может быть пустым");
                    continue;
                }

                if (_userService.GetByLogin(login) != null)
                {
                    Console.WriteLine("Пользователь с таким логином уже существует");
                    continue;
                }

                break;
            }

            // Ввод пароля
            string password;
            while (true)
            {
                Console.Write("Пароль: ");
                password = ReadPassword();

                if (password.Length < 6)
                {
                    Console.WriteLine("Пароль должен содержать не менее 6 символов");
                    continue;
                }

                Console.Write("Подтвердите пароль: ");
                var confirmPassword = ReadPassword();

                if (password != confirmPassword)
                {
                    Console.WriteLine("Пароли не совпадают");
                    continue;
                }

                break;
            }

            // Выбор роли
            Role role;
            while (true)
            {
                Console.WriteLine("\nВыберите роль:");
                Console.WriteLine("1 - Сотрудник");
                Console.WriteLine("2 - Управляющий");
                Console.Write("Ваш выбор: ");

                var choice = Console.ReadLine();
                switch (choice)
                {
                    case "1":
                        role = Role.Employee;
                        break;
                    case "2":
                        role = Role.Manager;
                        break;
                    default:
                        Console.WriteLine("Некорректный выбор");
                        continue;
                }
                break;
            }

            try
            {
                var newUser = _userService.Register(login, password, role);
                Console.WriteLine($"Пользователь '{newUser.Login}' успешно зарегистрирован!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка регистрации пользователя");
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }

        private string ReadPassword()
        {
            var password = new StringBuilder();
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
    }
}
