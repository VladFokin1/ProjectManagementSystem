namespace ProjectManagementSystem.Core.Exceptions
{
    internal class RepositoryException : Exception
    {
        public RepositoryException(string message) : base(message) { }
        public RepositoryException(string message, Exception inner) : base(message, inner) { }
    }
}
