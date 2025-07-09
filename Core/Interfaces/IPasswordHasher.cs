namespace ProjectManagementSystem.Core.Interfaces
{
    internal interface IPasswordHasher
    {
        string Hash(string password);
        bool Verify(string password, string hashedPassword);
    }
}
