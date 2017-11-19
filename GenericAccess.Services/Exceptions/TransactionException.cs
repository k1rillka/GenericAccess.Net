using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenericAccess.Exceptions
{
    public class TransactionException : Exception
    {
        private const string DefaultMessage = "Exception has happened during transaction execution";

        public TransactionException()
            : base(DefaultMessage)
        { }

        public TransactionException(string message)
            : base(message)
        { }

        public TransactionException(Exception inner)
            : base(DefaultMessage, inner)
        { }

        public TransactionException(string message, Exception inner)
            : base(message, inner)
        { }
    }
}
