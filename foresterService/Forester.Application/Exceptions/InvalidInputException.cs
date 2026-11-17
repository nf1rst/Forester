using System;

namespace Forester.Application.Exceptions;

[Serializable]
public class InvalidInputException : Exception
{
    public InvalidInputException() : base("Invalid input values") { }
    public InvalidInputException(string message) : base(message)
    {

    }
}
