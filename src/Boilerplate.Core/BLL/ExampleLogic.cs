using System.Threading.Tasks;
using Boilerplate.Core.Database;
using Boilerplate.Core.Database.Entities;
using Boilerplate.Core.Utils;
using Boilerplate.Core.Utils.Exceptions;
using Boilerplate.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Boilerplate.Core.BLL
{
    public interface IExampleLogic
    {
        Task<ExampleEntity> CreateExample(string name);
        Task<ExampleEntity[]> GetExamples();
    }

    /// <summary>
    /// All document-oriented logic should be put here.
    /// </summary>
    public class ExampleLogic : IExampleLogic
    {
        private readonly ILogger<ExampleLogic> _logger;
        private readonly IAppDbContext _appDbContext;
        private readonly Appsettings _appsettings;

        public ExampleLogic(
            ILogger<ExampleLogic> logger,
            IOptionsMonitor<Appsettings> appsettings,
            IAppDbContext appDbContext)
        {
            this._logger = logger;
            this._appDbContext = appDbContext;
            this._appsettings = appsettings.CurrentValue;
        }

        public async Task<ExampleEntity[]> GetExamples()
        {
            using (new TimedOperation(_logger, "Fectching examples from DB"))
            {
                var examples = await _appDbContext.Examples
                    .ToArrayAsync();

                return examples;
            }
        }

        public async Task<ExampleEntity> CreateExample(string name)
        {
            using (new TimedOperation(_logger, "Creating example in DB"))
            {
                var example = new ExampleEntity
                {
                    Name = name
                };

                await _appDbContext.Examples.AddAsync(example);

                // this is where some business logic might disallow the operation and throw an error
                // the BusinessRuleException will get caught by the exception middleware and formatted for the client
                if (true == false)
                    throw new BusinessRuleException(
                        nameof(BusinessRuleErrorConstants.THIS_IS_AN_ERROR_EXAMPLE),
                        BusinessRuleErrorConstants.THIS_IS_AN_ERROR_EXAMPLE);

                await _appDbContext.SaveChangesAsync();

                return example;
            }
        }
    }
}