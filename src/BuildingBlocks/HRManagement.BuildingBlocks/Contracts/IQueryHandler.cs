using MediatR;

namespace HRManagement.BuildingBlocks.Contracts;

public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
}

public interface IQuery<out TResult> : IRequest<TResult>
{
}