using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ForumCRUD.API.Data
{
    /// <summary>
    /// Custom execution strategy for MySQL that provides retry logic specifically for connection limit issues
    /// </summary>
    public class MySqlRetryExecutionStrategy : ExecutionStrategy
    {
        private readonly ILogger<MySqlRetryExecutionStrategy> _logger;
        private static readonly Random _random = new Random();

        public MySqlRetryExecutionStrategy(
            DbContext context,
            int maxRetryCount,
            TimeSpan maxRetryDelay,
            ILogger<MySqlRetryExecutionStrategy> logger) 
            : base(context, maxRetryCount, maxRetryDelay)
        {
            _logger = logger;
        }

        protected override bool ShouldRetryOn(Exception exception)
        {
            // Check for MySQL connection limit exceptions
            if (exception is MySqlException mysqlException)
            {
                bool shouldRetry = mysqlException.ErrorCode == MySqlErrorCode.TooManyUserConnections ||
                                   mysqlException.ErrorCode == MySqlErrorCode.LockWaitTimeout ||
                                   mysqlException.ErrorCode == MySqlErrorCode.LockDeadlock;

                if (shouldRetry)
                {
                    _logger.LogWarning(
                        mysqlException, 
                        "Detected retryable MySQL error: {ErrorCode} - {Message}", 
                        mysqlException.ErrorCode, 
                        mysqlException.Message);
                }
                
                return shouldRetry;
            }

            // Check for transient exceptions
            return exception is TimeoutException ||
                   IsTransientNetwork(exception);
        }

        protected override TimeSpan? GetNextDelay(Exception lastException)
        {
            var baseDelay = base.GetNextDelay(lastException);
            
            if (baseDelay == null)
            {
                return null;
            }
            
            // Add some jitter to prevent all retries happening at the exact same time
            int jitterMs = _random.Next(500);
            var delay = TimeSpan.FromMilliseconds(baseDelay.Value.TotalMilliseconds + jitterMs);
            
            _logger.LogInformation(
                "Retrying after MySQL error with delay of {Delay}ms (retry #{RetryCount})",
                delay.TotalMilliseconds,
                ExceptionsEncountered.Count);
            
            return delay;
        }

        private bool IsTransientNetwork(Exception ex)
        {
            // Check if it's a transient network-related exception
            string message = ex.Message.ToLower();
            return message.Contains("connection") && 
                   (message.Contains("refused") || 
                    message.Contains("timeout") || 
                    message.Contains("reset") ||
                    message.Contains("broken pipe"));
        }
    }

    /// <summary>
    /// Factory for creating MySQL retry execution strategies
    /// </summary>
    public class MySqlRetryExecutionStrategyFactory : IExecutionStrategyFactory
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly int _maxRetryCount;
        private readonly TimeSpan _maxRetryDelay;

        public MySqlRetryExecutionStrategyFactory(
            ILoggerFactory loggerFactory,
            int maxRetryCount = 3,
            int maxRetryDelaySeconds = 30)
        {
            _loggerFactory = loggerFactory;
            _maxRetryCount = maxRetryCount;
            _maxRetryDelay = TimeSpan.FromSeconds(maxRetryDelaySeconds);
        }

        public ExecutionStrategy Create()
        {
            return new MySqlRetryExecutionStrategy(
                null,
                _maxRetryCount,
                _maxRetryDelay,
                _loggerFactory.CreateLogger<MySqlRetryExecutionStrategy>());
        }

        IExecutionStrategy IExecutionStrategyFactory.Create()
        {
            return Create();
        }

    }
}
