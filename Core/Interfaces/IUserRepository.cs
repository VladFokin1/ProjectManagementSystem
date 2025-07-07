using ProjectManagementSystem.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
