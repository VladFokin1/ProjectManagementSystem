namespace ProjectManagementSystem.Infrastructure.Logging
{
    internal interface ILogger
    {
        void LogInformation(string message);
        void LogError(Exception ex, string message);
        void LogWarning(string message);
    }
}
