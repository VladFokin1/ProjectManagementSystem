using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Core.Interfaces
{
    internal interface IDataEncryptor
    {
        string Encrypt(string plainText);
        string Decrypt(string encryptedText);
    }
}
