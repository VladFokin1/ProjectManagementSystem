using ProjectManagementSystem.Core.Entities;
using ProjectManagementSystem.Core.Enums;

namespace ProjectManagementSystem.Core.Interfaces
{
    internal interface ICommand
    {
        string Name { get; }
        string Description { get; }
        Role RequiredRole { get; } // Роль, для которой доступна команда
        void Execute(User currentUser);
    }
}
