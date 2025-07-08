using ProjectManagementSystem.Core.Entities;
using ProjectManagementSystem.Core.Interfaces;
using ProjectManagementSystem.Infrastructure.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Core.Services
{
    internal class DatabaseAuthenticator : IAuthenticator
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;
        private readonly ILogger _logger;

        // Внедрение зависимостей через конструктор
        public DatabaseAuthenticator(
            IUserRepository userRepository,
            IPasswordHasher passwordHasher,
            ILogger logger)
        {
            _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
            _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public User Authenticate(string login, string password)
        {
            if (string.IsNullOrWhiteSpace(login))
                throw new AuthenticationException("Логин не может быть пустым");

            if (string.IsNullOrWhiteSpace(password))
                throw new AuthenticationException("Пароль не может быть пустым");

            try
            {
                var user = _userRepository.GetByLogin(login);

                if (user == null)
                {
                    _logger.LogWarning($"Попытка входа с несуществующим логином: {login}");
                    throw new AuthenticationException("Неверный логин или пароль");
                }

                if (!_passwordHasher.Verify(password, user.PasswordHash))
                {
                    _logger.LogWarning($"Неудачная попытка входа для пользователя: {login}");
                    throw new AuthenticationException("Неверный логин или пароль");
                }

                _logger.LogInformation($"Успешный вход пользователя: {login}");
                return user;
            }
            catch (Exception ex) when (ex is not AuthenticationException)
            {
                _logger.LogError(ex, $"Ошибка аутентификации для пользователя {login}");
                throw new AuthenticationException("Ошибка при попытке аутентификации", ex);
            }
        }
    }
}
