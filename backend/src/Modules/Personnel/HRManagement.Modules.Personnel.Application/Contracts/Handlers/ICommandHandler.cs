using MediatR;

namespace HRManagement.Modules.Personnel.Application.Contracts.Handlers;

public interface ICommandHandler<in TCommand, TResult> : IRequestHandler<TCommand, TResult>
    where TCommand : ICommand<TResult>
{
}

public interface ICommand<out TResult> : IRequest<TResult>
{
}