using System;
using System.Threading.Tasks;

namespace Boilerplate.Api.Domain.Services
{
    public interface IMailService
    {
        Task SendContactEmail(string fromEmail, string fullName, string comment);
        Task SendResetPasswordEmail(string email, string fullName, Guid resetPasswordToken);
    }
}