using ProjectManagementSystem.Core.Entities;
using ProjectManagementSystem.Core.Enums;
using ProjectManagementSystem.Core.Interfaces;
using ProjectManagementSystem.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Core.Services
{
    internal class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger _logger;

        public UserService(IUserRepository userRepository,
                           IPasswordHasher passwordHasher,
                           ILogger logger)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
            _logger = logger;
        }

        public IEnumerable<User> GetAllUsers()
        {
            throw new NotImplementedException();
        }

        public User GetByLogin(string login)
        {
            throw new NotImplementedException();
        }

        public User GetUser(int userId)
        {
            throw new NotImplementedException();
        }

        public User Register(string login, string password, Role role)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new ArgumentException("Логин не может быть пустым");

            if (string.IsNullOrWhiteSpace(password) || password.Length < 6)
                throw new ArgumentException("Пароль должен содержать минимум 6 символов");

            if (_userRepository.GetByLogin(login) != null)
                throw new InvalidOperationException("Пользователь с таким логином уже существует");

            User newUser = role switch
            {
                Role.Employee => new Employee(),
                Role.Manager => new Manager(),
                _ => throw new ArgumentException("Недопустимая роль")
            };

            newUser.Login = login;
            newUser.PasswordHash = _passwordHasher.Hash(password);

            _userRepository.Add(newUser);

            _logger.LogInformation($"Зарегистрирован новый пользователь: {login} ({role})");

            return newUser;
        }
    }
}
