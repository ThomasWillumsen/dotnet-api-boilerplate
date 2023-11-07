using Boilerplate.Api.Domain.Services;
using MediatR;
using Boilerplate.Api.Infrastructure.Database;
using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using Boilerplate.Api.Infrastructure.Database.Entities;

namespace Boilerplate.Api.Domain.Commands.Emails;

public static class SendResetPasswordMail
{
    public record Command(string email, string fullName, Guid passwordToken) : IRequest;

    public class Handler : IRequestHandler<Command>
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

        public async Task Handle(Command request, CancellationToken cancellationToken)
        {
            var dto = new SendGridMailDto(
                templateId: _sendgridSettings.CreateNewPasswordTemplateId,
                templateData: new
                {
                    fullName = request.fullName,
                    createNewPasswordLink = _sendgridSettings.CreateNewPasswordLink
                        .Replace("{passwordToken}", request.passwordToken.ToString())
                },
                Guid.NewGuid(),
                from: new EmailAddress(_sendgridSettings.SendFromEmail, _sendgridSettings.SendFromName),
                to: new EmailAddress(request.email, request.fullName),
                replyTo: null
            );

            await _sendgridClient.SendEmail(dto);

            await _dbContext.EmailLogs.AddAsync(new EmailLogEntity(request.email, EmailTypeEnum.ResetPassword, dto.Reference));
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}