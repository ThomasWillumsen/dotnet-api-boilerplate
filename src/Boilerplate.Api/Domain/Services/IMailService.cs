using System;
using System.Threading.Tasks;

namespace Boilerplate.Api.Domain.Services;

public interface IMailService
{
    Task SendResetPasswordEmail(string email, string fullName, Guid resetPasswordToken);
}