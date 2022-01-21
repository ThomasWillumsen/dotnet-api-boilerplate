using System;
using System.Threading;
using System.Threading.Tasks;
using Boilerplate.Api.Domain.Services;
using MediatR;
using Boilerplate.Api.Infrastructure.Database;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;

namespace Boilerplate.Api.Domain.Commands.Emails;

public static class SendResetPasswordMail
{
    public record Command(string email, string fullName, Guid resetPasswordToken) : IRequest;

    public class Handler : MediatR.AsyncRequestHandler<Command>
    {
        private readonly AppDbContext _dbContext;
        private readonly SendGridSettings _sendgridSettings;
        private readonly ISendGridClientFacade _sendgridClient;

        public Handler(
            AppDbContext dbContext,
            IOptions<SendGridSettings> sendgridSettings,
            ISendGridClientFacade sendgridClient)
        {
            _dbContext = dbContext;
            _sendgridSettings = sendgridSettings.Value;
            _sendgridClient = sendgridClient;
        }

        protected override async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var dto = new SendGridMailDto(
                templateId: _sendgridSettings.ResetPasswordTemplateId,
                templateData: new
                {
                    fullName = request.fullName,
                    passwordToken = request.resetPasswordToken.ToString()
                },
                from: new EmailAddress(_sendgridSettings.SendFromEmail, _sendgridSettings.SendFromName),
                to: new EmailAddress(request.email, request.fullName),
                replyTo: null
            );

            await _sendgridClient.SendEmail(dto);
        }
    }
}