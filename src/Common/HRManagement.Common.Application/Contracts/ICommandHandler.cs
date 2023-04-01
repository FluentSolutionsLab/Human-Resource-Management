using MediatR;

namespace HRManagement.Common.Application.Contracts;

public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
}

public interface ICommand<out TResult> : IRequest<TResult>
{
}