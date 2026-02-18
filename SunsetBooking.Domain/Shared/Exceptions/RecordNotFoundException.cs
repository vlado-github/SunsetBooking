namespace SunsetBooking.Domain.Shared.Exceptions;

public class RecordNotFoundException : Exception
{
    public RecordNotFoundException(Guid recordId) : base($"Record with ID {recordId} not found.")
    {
        
    }
    
    public RecordNotFoundException(string recordId) : base($"Record with ID {recordId} not found.")
    {
        
    }
    
    public RecordNotFoundException(long recordId) : base($"Record with ID {recordId} not found.")
    {
        
    }
}