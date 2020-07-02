using System;
using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Boilerplate.Core.Utils
{
    /// <summary>
    /// Log how much time is spent performing some action
    /// </summary>
    public class TimedOperation : IDisposable
    {
        private readonly ILogger _logger;
        private readonly string _operationName;
        private readonly Stopwatch _stopwatch;

        public TimedOperation(ILogger logger, string operationName)
        {
            this._logger = logger;
            this._operationName = operationName;
            this._stopwatch = Stopwatch.StartNew();
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _logger.LogInformation("{OperationName} took {ElapsedMilliseconds} ms to complete", _operationName, _stopwatch.ElapsedMilliseconds);
        }
    }
}