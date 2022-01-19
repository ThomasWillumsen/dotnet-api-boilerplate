using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Boilerplate.Api.Domain.Services;

public class SendGridService : IMailService
{
    private readonly ILogger<SendGridService> _logger;
    private readonly ISendGridClient _sendGridClient;
    private readonly SendGridSettings _sendGridSettings;

    public SendGridService(
        ILogger<SendGridService> logger,
        IOptions<SendGridSettings> sendGridSettings,
        ISendGridClient sendGridClient)
    {
        _logger = logger;
        _sendGridClient = sendGridClient;
        _sendGridSettings = sendGridSettings.Value;
    }

    public async Task SendResetPasswordEmail(string email, string fullName, Guid resetPasswordToken)
    {
        var dto = new SendGridMailDto(
            templateId: _sendGridSettings.ResetPasswordTemplateId,
            templateData: new
            {
                fullName = fullName,
                passwordToken = resetPasswordToken.ToString()
            },
            from: new EmailAddress(_sendGridSettings.SendFromEmail, _sendGridSettings.SendFromName),
            to: new EmailAddress(email, fullName),
            replyTo: null
        );

        await SendEmail(dto);
    }

    protected virtual async Task SendEmail(SendGridMailDto mailDto)
    {
        await Task.Run(() => {});
        var msg = new SendGridMessage();
        msg.AddTo(mailDto.To.Email, mailDto.To.Name);
        if (mailDto.ReplyTo != null)
            msg.SetReplyTo(new EmailAddress(mailDto.ReplyTo.Email, mailDto.ReplyTo.Name));
        msg.SetFrom(mailDto.From.Email, mailDto.From.Name);
        msg.SetTemplateId(mailDto.TemplateId);
        msg.SetTemplateData(mailDto.TemplateData);

        // TODO: uncomment and write a sendgrid API key in appsettings.json
        // var response = await _sendGridClient.SendEmailAsync(msg);
        // if (!response.IsSuccessStatusCode)
        // {
        //     var content = await response.Body.ReadAsStringAsync();
        //     throw new Exception($"SendGrid invoked sending to with template {msg.TemplateId}, response {content} {response.Body}");
        // }
    }
}