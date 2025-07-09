using ProjectManagementSystem.Core.Entities;
using ProjectManagementSystem.Core.Enums;
using ProjectManagementSystem.Core.Interfaces;
using Task = ProjectManagementSystem.Core.Entities.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Core.Services.Commands
{
    internal class ShowTasksCommand : ICommand
    {
        public string Name => "show-tasks";
        public string Description => "Показать мои задачи";
        public Role RequiredRole => Role.Employee;

        private readonly ITaskService _taskService;

        public ShowTasksCommand(ITaskService taskService)
        {
            _taskService = taskService;
        }

        public void Execute(User currentUser)
        {
            IEnumerable<Task> tasks = _taskService.GetTasksForUser(currentUser.Id);
            int colWidth = 20;
            Console.WriteLine(
                "|{0}|{1}|{2}|{3}|",
                "ID".PadRight(colWidth),
                "Title".PadRight(colWidth),
                "Status".PadRight(colWidth),
                "Due Date".PadRight(colWidth)
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
            Console.WriteLine("Нажмите любую кнопку для продолжения...");
            Console.Read();
        }
    }
}
