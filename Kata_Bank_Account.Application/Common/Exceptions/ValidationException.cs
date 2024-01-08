namespace Kata_Bank_Account.Application.Common.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(IDictionary<string, string[]> errors)
        : base( "One or more validation exceptions have occured")
        {
        }

        public ValidationException(string message)
        :base(message)
        {
        }
    }
}
