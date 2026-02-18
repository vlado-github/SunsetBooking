namespace SunsetBooking.Domain.Base.Queries;

public interface IQueryHandler<in TQuery, TResult> where TQuery : QueryBase
{
    Task<TResult> HandleAsync(TQuery query);
}

public abstract class QueryHandlerBase<TQuery, TResult> : IQueryHandler<TQuery, TResult>
    where TQuery : QueryBase
{
    public abstract Task<TResult> HandleAsync(TQuery query);
}
