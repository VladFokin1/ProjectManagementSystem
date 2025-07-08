using ProjectManagementSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Core.Services
{
    internal class TaskService : ITaskService
    {
        private readonly ITaskRepository _taskRepository;
        public void ChangeTaskStatus(int taskId, TaskStatus newStatus)
        {
            throw new NotImplementedException();
        }

        public Task CreateTask(string projectId, string title, string description, int assigneeId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<Task> GetTasksForUser(int userId)
        {
            throw new NotImplementedException();
        }
    }
}
