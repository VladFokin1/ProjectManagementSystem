using ProjectManagementSystem.Core.Entities;
using ProjectManagementSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Core.Services
{
    internal class DatabaseAuthenticator : IAuthenticator
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public User Authenticate(string login, string password)
        {
            throw new NotImplementedException();
        }
    }
}
