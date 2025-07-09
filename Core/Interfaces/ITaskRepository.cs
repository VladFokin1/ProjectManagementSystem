using Task = ProjectManagementSystem.Core.Entities.Task;

namespace ProjectManagementSystem.Core.Interfaces
{
    internal interface ITaskRepository
    {
        Task GetById(int id);
        IEnumerable<Task> GetByProjectId(string projectId);
        IEnumerable<Task> GetByAssignee(int assigneeId);
        void Add(Task task);
        void Update(Task task);
        void Save();
    }
}
