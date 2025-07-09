namespace ProjectManagementSystem.Infrastructure.Logging
{
    internal class FileLogger : ILogger
    {
        private readonly string _logFilePath;

        public FileLogger()
        {
            _logFilePath = "app.log";
        }

        public void LogError(Exception ex, string message)
        {
            WriteToFile($"[ERROR] {message} | Exception: {ex}");
        }

        public void LogInformation(string message)
        {
            WriteToFile($"[INFO] {message}");
        }

        public void LogWarning(string message)
        {
            WriteToFile($"[WARNING] {message}");
        }

        private void WriteToFile(string message)
        {
            try
            {
                string logMessage = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} | {message}{Environment.NewLine}";

                // Добавляем запись в конец файла
                File.AppendAllText(_logFilePath, logMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Не удалось записать лог: {ex.Message}");
            }
        }
    }
}
