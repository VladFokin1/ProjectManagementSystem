
using ProjectManagementSystem.Core.Entities;
using ProjectManagementSystem.Core.Interfaces;
using ProjectManagementSystem.Infrastructure.Logging;
using Task = ProjectManagementSystem.Core.Entities.Task;
using TaskStatus = ProjectManagementSystem.Core.Enums.TaskStatus;

namespace ProjectManagementSystem.Core.Services
{
    internal class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogger _logger;

        public TaskService(ITaskRepository taskRepository,
                           IUserRepository userRepository,
                           ILogger logger)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _logger = logger;
        }

        public void ChangeTaskStatus(int taskId, TaskStatus newStatus, User currentUser)
        {
            var task = _taskRepository.GetById(taskId)
                ?? throw new ArgumentException("Задача не найдена");

            if (task.AssigneeId != currentUser.Id)
                throw new Exception("Вы не являетесь исполнителем этой задачи");

            var oldStatus = task.Status;
            task.Status = newStatus;
            task.UpdatedAt = DateTime.Now;

            _taskRepository.Update(task);
            _taskRepository.Save();

            _logger.LogInformation($"Статус задачи {taskId} изменен с {oldStatus} на {newStatus}");
        }

        public Task CreateTask(string projectId, string title, string description, int assigneeId)
        {
            if (string.IsNullOrWhiteSpace(projectId))
                throw new ArgumentException("ID проекта обязательно");

            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Название задачи обязательно");

            if (_userRepository.GetById(assigneeId) == null)
                throw new ArgumentException("Исполнитель не найден");

            var task = new Task
            {
                ProjectId = projectId,
                Title = title,
                Description = description,
                AssigneeId = assigneeId,
                Status = TaskStatus.ToDo,
                CreatedAt = DateTime.Now
            };

            _taskRepository.Add(task);
            _taskRepository.Save();

            _logger.LogInformation($"Создана новая задача: {title} (ID: {task.Id})");
            return task;
        }

        public IEnumerable<Task> GetTasksForUser(int userId)
        {
            return _taskRepository.GetByAssignee(userId);
        }
    }
}
