using ProjectManagementSystem.Core.Entities;
using ProjectManagementSystem.Core.Enums;
using ProjectManagementSystem.Core.Interfaces;

namespace ProjectManagementSystem.Core.Services.Commands
{
    internal class ShowAllUsersCommand : ICommand
    {
        public string Name => "show-all-users";
        public string Description => "Показать всех пользователей";
        public Role RequiredRole => Role.Manager;

        private readonly IUserService _userService;

        public ShowAllUsersCommand(IUserService userService)
        {
            _userService = userService;
        }

        public void Execute(User currentUser)
        {
            IEnumerable<User> users = _userService.GetAllUsers();
            int colWidth = 20;
            Console.WriteLine(
                "|{0}|{1}|{2}|",
                "ID".PadRight(colWidth),
                "Login".PadRight(colWidth),
                "Role".PadRight(colWidth)
            );

            foreach (User user in users)
            {
                Console.WriteLine(
                    "|{0}|{1}|{2}|",
                    user.Id.ToString().PadRight(colWidth),
                    user.Login.PadRight(colWidth),
                    user.Role.ToString().PadRight(colWidth)
                );
            }
            Console.WriteLine("Нажмите любую кнопку для продолжения...");
            Console.Read();
        }

    }
}
