using MediatR;

namespace HRManagement.Modules.Personnel.Application.Contracts.Handlers;

public interface IQueryHandler<in TQuery, TResult> : IRequestHandler<TQuery, TResult> where TQuery : IQuery<TResult>
{
}

public interface IQuery<out TResult> : IRequest<TResult>
{
}