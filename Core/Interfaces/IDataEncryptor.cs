namespace ProjectManagementSystem.Core.Interfaces
{
    internal interface IDataEncryptor
    {
        string Encrypt(string plainText);
        string Decrypt(string encryptedText);
    }
}
