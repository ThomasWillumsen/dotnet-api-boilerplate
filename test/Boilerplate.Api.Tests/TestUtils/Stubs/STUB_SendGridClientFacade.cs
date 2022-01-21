using System.Threading.Tasks;
using Boilerplate.Api.Domain.Services;

namespace Boilerplate.Api.Tests.TestUtils.Stubs;

public class STUB_SendGridClientFacade : ISendGridClientFacade
{

    Task ISendGridClientFacade.SendEmail(SendGridMailDto mailDto)
    {
        return Task.CompletedTask;
    }
}