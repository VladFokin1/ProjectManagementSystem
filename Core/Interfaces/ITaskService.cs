using ProjectManagementSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Task = ProjectManagementSystem.Core.Entities.Task;

namespace ProjectManagementSystem.Core.Interfaces
{
    internal interface ITaskService
    {
        Task CreateTask(string projectId, string title, string description, int assigneeId);
        void ChangeTaskStatus(int taskId, ProjectManagementSystem.Core.Enums.TaskStatus newStatus, User currentUser);
        IEnumerable<Task> GetTasksForUser(int userId);
    }
}
