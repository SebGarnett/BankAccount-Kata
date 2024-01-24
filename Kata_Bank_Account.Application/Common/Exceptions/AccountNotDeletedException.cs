using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kata_Bank_Account.Application.Common.Exceptions
{
    public class AccountNotDeletedException: Exception
    {
        public AccountNotDeletedException(IDictionary<string, string[]> errors): base("The product cannot be deleted")
        {
        }

        public AccountNotDeletedException(string message): base (message)
        {
        }
    }
}
