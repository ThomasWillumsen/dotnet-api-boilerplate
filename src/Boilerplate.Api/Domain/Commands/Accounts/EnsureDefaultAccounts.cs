using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Options;

namespace Boilerplate.Api.Domain.Commands.Accounts;

public static class EnsureDefaultAccounts
{
    public record Command() : IRequest;

    public class Handler : AsyncRequestHandler<Command>
    {
        private readonly IMediator _mediator;
        private readonly Appsettings _appSettings;

        public Handler(
            IMediator mediator,
            IOptions<Appsettings> appSettings)
        {
            _mediator = mediator;
            _appSettings = appSettings.Value;
        }

        protected override async Task Handle(Command request, CancellationToken cancellationToken)
        {
            foreach (var defaultAcc in _appSettings.DefaultAccounts)
            {
                try
                {
                    await _mediator.Send(new CreateAccount.Command(defaultAcc.FullName, defaultAcc.Email));
                }
                catch (Exception) { }
            }
        }
    }
}