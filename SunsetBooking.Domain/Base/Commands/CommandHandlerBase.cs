namespace SunsetBooking.Domain.Base.Commands;

public interface ICommandHandler<in TCommand, TResult> where TCommand : CommandBase
{
    Task<TResult> HandleAsync(TCommand command);
}

public abstract class CommandHandlerBase<TCommand, TResult> : ICommandHandler<TCommand, TResult>
    where TCommand : CommandBase
{
    public abstract Task<TResult> HandleAsync(TCommand command);
}
