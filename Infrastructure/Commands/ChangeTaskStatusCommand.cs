using ProjectManagementSystem.Core.Entities;
using ProjectManagementSystem.Core.Enums;
using ProjectManagementSystem.Core.Interfaces;
using Task = ProjectManagementSystem.Core.Entities.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProjectManagementSystem.Infrastructure.Logging;

namespace ProjectManagementSystem.Core.Services.Commands
{
    internal class ChangeTaskStatusCommand : ICommand
    {
        public string Name => "change-status";

        public string Description => "Изменить статус задачи";

        public Role RequiredRole => Role.Employee;

        private readonly ITaskService _taskService;
        private readonly ILogger _logger;

        public ChangeTaskStatusCommand(ITaskService taskService, ILogger logger)
        {
            _taskService = taskService;
            _logger = logger;
        }

        public void Execute(User currentUser)
        {

            IEnumerable<Task> tasks = _taskService.GetTasksForUser(currentUser.Id);

            Console.WriteLine("Выберите задачу для изменения:");

            int colWidth = 20;
            Console.WriteLine(
                "|{0}|{1}|{2}|{3}|",
                "ID".PadRight(colWidth),
                "Title".PadRight(colWidth),
                "Description".PadRight(colWidth),
                "Status".PadRight(colWidth),
                "Created At".PadRight(colWidth)
            );

            foreach (Task task in tasks)
            {
                Console.WriteLine(
                    "|{0}|{1}|{2}|{3}|",
                    task.Id.ToString().PadRight(colWidth),
                    task.Title.PadRight(colWidth),
                    task.Description.PadRight(colWidth),
                    task.Status.ToString().PadRight(colWidth),
                    task.CreatedAt.ToString().PadRight(colWidth)
                );
            }
            while (true)
            {
                Console.WriteLine("Введите ID задачи:");
                var taskID = Console.ReadLine();
                Task taskToUpdate = tasks.First(t => t.Id == int.Parse(taskID));
                if (taskToUpdate == null)
                {
                    Console.WriteLine("Такого задания не существует!");
                    continue;
                }
                Console.WriteLine("Выберите новый статус");
                Console.WriteLine("1 - ToDo");
                Console.WriteLine("2 - InProgress");
                Console.WriteLine("3 - Done");

                var choice = Console.ReadLine();
                Enums.TaskStatus newStatus;
                if (choice == "1")
                {
                    newStatus = Enums.TaskStatus.ToDo;
                }
                else if (choice == "2")
                {
                    newStatus = Enums.TaskStatus.InProgress;
                }
                else if (choice == "3")
                {
                    newStatus = Enums.TaskStatus.Done;
                }
                else
                {
                    Console.WriteLine("Такого статуса не существует!");
                    continue;
                }
                var oldStatus = taskToUpdate.Status;
                _taskService.ChangeTaskStatus(taskToUpdate.Id, newStatus, currentUser);
                _logger.LogInformation($"Пользователь {currentUser.Login} изменил статус задания с ID={taskToUpdate.Id} изменен с {oldStatus} на {newStatus}");
                break;
            }

            Console.WriteLine("Нажмите любую кнопку для продолжения...");
            Console.Read();


        }
    }
}
