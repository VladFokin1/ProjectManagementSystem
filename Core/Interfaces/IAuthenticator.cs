using ProjectManagementSystem.Core.Entities;

namespace ProjectManagementSystem.Core.Interfaces
{
    internal interface IAuthenticator
    {
        User Authenticate(string login, string password);
    }
}
