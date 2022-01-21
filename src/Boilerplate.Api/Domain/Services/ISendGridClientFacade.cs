using System;
using System.Threading.Tasks;

namespace Boilerplate.Api.Domain.Services;

public interface ISendGridClientFacade
{
    Task SendEmail(SendGridMailDto mailDto);
}