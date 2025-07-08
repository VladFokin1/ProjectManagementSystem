using ProjectManagementSystem.Core.Entities;
using ProjectManagementSystem.Core.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Core.Interfaces
{
    internal interface IUserService
    {
        User Register(string login, string password, Role role);
        IEnumerable<User> GetAllUsers();
        User GetUser(int userId);
        User GetByLogin(string login);
    }
}
