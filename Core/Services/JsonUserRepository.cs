using ProjectManagementSystem.Core.Entities;
using ProjectManagementSystem.Core.Exceptions;
using ProjectManagementSystem.Core.Interfaces;
using ProjectManagementSystem.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Core.Services
{
    internal class JsonUserRepository : IUserRepository, IDisposable
    {
        private readonly string _filePath;
        private readonly IDataEncryptor _encryptor;
        private List<User> _users;
        private readonly object _lock = new object();
        private bool _disposed;
        private IPasswordHasher _passwordHasher;

        public JsonUserRepository(string filePath, IDataEncryptor encryptor, IPasswordHasher passwordHasher)
        {
            _filePath = filePath ?? throw new ArgumentNullException(nameof(filePath));
            _encryptor = encryptor ?? throw new ArgumentNullException(nameof(encryptor));
            _passwordHasher = passwordHasher;

            InitializeRepository();
            
        }

        private void InitializeRepository()
        {

            try
            {
                var directory = Path.GetDirectoryName(_filePath);
                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                if (!File.Exists(_filePath))
                {
                    _users = new List<User>();
                    File.WriteAllText(_filePath, _encryptor.Encrypt("[]")); // Пустой массив

                    // Добавляем администратора
                    var admin = new Manager
                    {
                        Id = 1,
                        Login = "admin",
                        PasswordHash = _passwordHasher.Hash("admin123")
                    };
                    _users.Add(admin);
                    Save(); // Сохраняем сразу
                    return;
                }

                // Читаем существующий файл
                var encryptedJson = File.ReadAllText(_filePath);
                var json = _encryptor.Decrypt(encryptedJson);
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Converters = { new UserConverter() }
                };

                _users = JsonSerializer.Deserialize<List<User>>(json, options) ?? new List<User>();

                // Проверяем, есть ли уже администратор
                if (!_users.Any(u => u.Login == "admin"))
                {
                    var admin = new Manager
                    {
                        Id = _users.Count + 1,
                        Login = "admin",
                        PasswordHash = _passwordHasher.Hash("admin123")
                    };
                    _users.Add(admin);
                    Save();
                }
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Ошибка инициализации репозитория", ex);
            }
        }

        public User GetById(int id)
        {
            lock (_lock)
            {
                return _users.FirstOrDefault(u => u.Id == id);
            }
        }

        public User GetByLogin(string login)
        {
            if (string.IsNullOrWhiteSpace(login))
                return null;

            lock (_lock)
            {
                return _users.FirstOrDefault(u =>
                    string.Equals(u.Login, login, StringComparison.OrdinalIgnoreCase));
            }
        }

        public IEnumerable<User> GetAll()
        {
            lock (_lock)
            {
                // Возвращаем копию для защиты от изменений извне
                return _users.ToList();
            }
        }

        public void Add(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            lock (_lock)
            {
                // Генерация ID (для простоты, в реальном приложении лучше GUID)
                user.Id = _users.Count > 0 ? _users.Max(u => u.Id) + 1 : 1;
                _users.Add(user);
                Save();
            }
        }

        public void Update(User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));

            lock (_lock)
            {
                var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
                if (existingUser == null)
                    throw new KeyNotFoundException($"Пользователь с ID {user.Id} не найден");

                // Обновляем только изменяемые поля
                existingUser.Login = user.Login;
                existingUser.PasswordHash = user.PasswordHash;

            }
        }

        public void Save()
        {
            lock (_lock)
            {
                try
                {
                    var options = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        Converters = {new UserConverter() }
                    };

                    var json = JsonSerializer.Serialize(_users, options);
                    var encryptedJson = _encryptor.Encrypt(json);

                    var tempFile = Path.GetTempFileName();
                    File.WriteAllText(_filePath + ".tmp", encryptedJson);
                    File.Delete(_filePath);
                    File.Move(_filePath + ".tmp", _filePath);
                }
                catch (Exception ex)
                {
                    throw new RepositoryException("Ошибка сохранения пользователей", ex);
                }
            }
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                Save(); // Сохраняем при завершении
                _disposed = true;
            }
        }
    }
}
