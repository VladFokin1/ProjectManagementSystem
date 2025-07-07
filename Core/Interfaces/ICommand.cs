using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectManagementSystem.Core.Interfaces
{
    internal interface ICommand
    {
        string Name { get; }
        void Execute();
    }
}
