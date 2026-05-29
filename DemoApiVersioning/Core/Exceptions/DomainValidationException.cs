using System.Text;

namespace Core.Exceptions;

public sealed class DomainValidationException : Exception
{
    public DomainValidationException(string message) : base(message)
    {
        
    }
    
    public DomainValidationException(StringBuilder message) : base(message.ToString())
    {
        
    }
}
