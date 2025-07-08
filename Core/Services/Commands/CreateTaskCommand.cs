using ProjectManagementSystem.Core.Entities;
using ProjectManagementSystem.Core.Enums;
using ProjectManagementSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Core.Services.Commands
{
    internal class CreateTaskCommand : ICommand
    {
        public string Name => "create-task";
        public string Description => "Cоздать задачу";
        public Role RequiredRole => Role.Manager;

        private readonly ITaskService _taskService;
        private readonly IUserRepository _userRepository;

        public CreateTaskCommand(ITaskService taskService, IUserRepository userRepository)
        {
            _taskService = taskService;
            _userRepository = userRepository;
        }

        public void Execute(User currentUser)
        {
            Console.WriteLine("\n=== Создание новой задачи ===");

            // Ввод данных
            Console.Write("ID проекта: ");
            var projectId = Console.ReadLine();

            Console.Write("Название задачи: ");
            var title = Console.ReadLine();

            Console.Write("Описание: ");
            var description = Console.ReadLine();

            // Выбор исполнителя
            Console.WriteLine("\nДоступные сотрудники:");
            var employees = _userRepository.GetAll()
                .Where(u => u.Role == Role.Employee)
                .ToList();

            foreach (var emp in employees)
            {
                Console.WriteLine($"{emp.Id}: {emp.Login}");
            }

            Console.Write("ID исполнителя: ");
            if (!int.TryParse(Console.ReadLine(), out var assigneeId))
            {
                Console.WriteLine("Некорректный ID");
                return;
            }

            try
            {
                var task = _taskService.CreateTask(projectId, title, description, assigneeId);
                Console.WriteLine($"\nЗадача создана! ID: {task.Id}");
                Console.WriteLine($"Проект: {task.ProjectId}");
                Console.WriteLine($"Исполнитель: {task.AssigneeId}");
                Console.WriteLine($"Статус: {task.Status}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }
}
