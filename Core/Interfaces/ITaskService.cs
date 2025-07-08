using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Core.Interfaces
{
    internal interface ITaskService
    {
        Task CreateTask(string projectId, string title, string description, int assigneeId);
        void ChangeTaskStatus(int taskId, TaskStatus newStatus);
        IEnumerable<Task> GetTasksForUser(int userId);
    }
}
