using ProjectManagementSystem.Core.Exceptions;
using ProjectManagementSystem.Core.Interfaces;
using ProjectManagementSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Text.Json;
using Task = ProjectManagementSystem.Core.Entities.Task;

namespace ProjectManagementSystem.Core.Services
{
    internal class JsonTaskRepository : ITaskRepository, IDisposable
    {
        private readonly string _filePath;
        private readonly IDataEncryptor _encryptor;
        private List<Task> _tasks;
        private readonly object _lock = new object();
        private bool _disposed;
        private IPasswordHasher _passwordHasher;

        public JsonTaskRepository(string filePath, IDataEncryptor encryptor, IPasswordHasher passwordHasher)
        {
            _filePath = Path.GetFullPath(filePath);
            _encryptor = encryptor;
            _passwordHasher = passwordHasher;

            var directory = Path.GetDirectoryName(_filePath);
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            InitializeRepository();

        }

        private void InitializeRepository()
        {
            try
            {
                if (!File.Exists(_filePath))
                {
                    _tasks = new List<Task>();
                    Save();
                    return;
                }

                var encryptedJson = File.ReadAllText(_filePath);
                var json = _encryptor.Decrypt(encryptedJson);
                _tasks = JsonSerializer.Deserialize<List<Task>>(json) ?? new List<Task>();
            }
            catch (Exception ex)
            {
                throw new RepositoryException("Ошибка инициализации репозитория задач", ex);
            }
        }

        public void Add(Task task)
        {
            task.Id = _tasks.Count > 0 ? _tasks.Max(t => t.Id) + 1 : 1;
            _tasks.Add(task);
        }

        
        public IEnumerable<Task> GetByAssignee(int assigneeId)
        {
            return _tasks.Where(t => t.AssigneeId == assigneeId).ToList();
        }

        public Task GetById(int id)
        {
            return _tasks.FirstOrDefault(t => t.Id == id);
        }

        public IEnumerable<Task> GetByProjectId(string projectId)
        {
            return _tasks.Where(t => t.ProjectId == projectId).ToList();
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
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    };

                    var json = JsonSerializer.Serialize(_tasks, options);
                    var encryptedJson = _encryptor.Encrypt(json);

                    var tempFile = Path.GetTempFileName();
                    File.WriteAllText(_filePath + ".tmp", encryptedJson);
                    File.Delete(_filePath);
                    File.Move(_filePath + ".tmp", _filePath);
                }
                catch (Exception ex)
                {
                    throw new RepositoryException("Ошибка сохранения задач", ex);
                }
            }
        }

        public void Update(Task task)
        {
            lock (_lock)
            {
                var existing = _tasks.FirstOrDefault(t => t.Id == task.Id);
                if (existing == null)
                    throw new KeyNotFoundException($"Задача с ID {task.Id} не найдена");

                existing.Title = task.Title;
                existing.Description = task.Description;
                existing.Status = task.Status;
                existing.UpdatedAt = DateTime.Now;
            }
        }

        public void Dispose()
        {
            Save();
        }

    }
}
