namespace SunsetBooking.Domain.Shared.Exceptions;

public class MissingAttributeException : Exception
{
    public MissingAttributeException(string attributeName, string resourceName) 
        : base($"{attributeName} value is not specified for type {resourceName}.") { }
}