using ProjectManagementSystem.Core.Entities;

namespace ProjectManagementSystem.Core.Interfaces
{
    internal interface IUserRepository
    {
        User GetById(int id);
        User GetByLogin(string login);
        IEnumerable<User> GetAll();
        void Add(User user);
        void Update(User user);
    }
}
