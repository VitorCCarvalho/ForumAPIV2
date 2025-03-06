using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;
using MySqlConnector;
using System;
using System.Collections.Concurrent;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ForumCRUD.API.Data
{
    /// <summary>
    /// Interceptor that handles MySQL too many connections errors with automatic retry logic
    /// </summary>
    public class MySqlRetryConnectionInterceptor : DbConnectionInterceptor
    {
        private readonly ILogger<MySqlRetryConnectionInterceptor> _logger;
        private readonly Random _random = new Random();
        private readonly ConcurrentDictionary<string, int> _connectionRetryCount = new ConcurrentDictionary<string, int>();
        
        // Maximum number of retries for connection errors
        public int MaxRetries { get; set; } = 3;
        
        public MySqlRetryConnectionInterceptor(ILogger<MySqlRetryConnectionInterceptor> logger)
        {
            _logger = logger;
        }

        // Override basic DbConnection methods
        public override InterceptionResult ConnectionOpening(
            DbConnection connection,
            ConnectionEventData eventData,
            InterceptionResult result)
        {
            return result;
        }

        public override ValueTask<InterceptionResult> ConnectionOpeningAsync(
            DbConnection connection,
            ConnectionEventData eventData,
            InterceptionResult result,
            CancellationToken cancellationToken = default)
        {
            return new ValueTask<InterceptionResult>(result);
        }

        public override void ConnectionOpened(
            DbConnection connection,
            ConnectionEndEventData eventData)
        {
            // No action needed after successful connection
        }

        public override Task ConnectionOpenedAsync(
            DbConnection connection,
            ConnectionEndEventData eventData,
            CancellationToken cancellationToken = default)
        {
            // No action needed after successful connection
            return Task.CompletedTask;
        }

        public override void ConnectionClosed(
            DbConnection connection,
            ConnectionEndEventData eventData)
        {
            // Clean up retry count when connection closes
            if (connection.ConnectionString != null)
            {
                _connectionRetryCount.TryRemove(connection.ConnectionString, out _);
            }
        }

        public override Task ConnectionClosedAsync(
            DbConnection connection,
            ConnectionEndEventData eventData)
        {
            ConnectionClosed(connection, eventData);
            return Task.CompletedTask;
        }

        public override void ConnectionFailed(
            DbConnection connection,
            ConnectionErrorEventData eventData)
        {
            // Check if this is a MySQL connection error related to too many connections
            if (eventData.Exception is MySqlException mysqlException)
            {
                HandleConnectionFailure(connection, eventData, mysqlException);
            }
        }

        public override async Task ConnectionFailedAsync(
            DbConnection connection,
            ConnectionErrorEventData eventData,
            CancellationToken cancellationToken = default)
        {
            // Check if this is a MySQL connection error related to too many connections
            if (eventData.Exception is MySqlException mysqlException)
            {
                await HandleConnectionFailureAsync(connection, eventData, mysqlException, cancellationToken);
            }
        }
        
        private bool HandleConnectionFailure(
            DbConnection connection,
            ConnectionErrorEventData eventData,
            MySqlException mysqlException)
        {
            // Only handle connection limit errors
            if (mysqlException.ErrorCode != MySqlErrorCode.TooManyUserConnections)
            {
                return false;
            }

            // Get the connection id for tracking retries
            string connectionId = connection.ConnectionString ?? Guid.NewGuid().ToString();
            int retryCount = _connectionRetryCount.GetValueOrDefault(connectionId, 0);

            // Don't retry if we've exceeded max retries
            if (retryCount >= MaxRetries)
            {
                _connectionRetryCount.TryRemove(connectionId, out _);
                _logger.LogError(
                    mysqlException,
                    "MySQL connection error (too many connections) - max retries ({MaxRetries}) exhausted",
                    MaxRetries);
                return false;
            }

            // Increment retry count
            retryCount++;
            _connectionRetryCount[connectionId] = retryCount;

            // Calculate backoff time with jitter
            int delayMs = (int)Math.Pow(2, retryCount) * 1000 + _random.Next(500);
            
            _logger.LogWarning(
                mysqlException,
                "MySQL connection error (too many connections), retry {RetryCount}/{MaxRetries} after {DelayMs}ms",
                retryCount, MaxRetries, delayMs);
            
            // For sync connections, we can't do async delay, so we just return false
            // indicating that we're not handling the connection
            return false;
        }
        
        private async Task<bool> HandleConnectionFailureAsync(
            DbConnection connection,
            ConnectionErrorEventData eventData,
            MySqlException mysqlException,
            CancellationToken cancellationToken)
        {
            // Only handle connection limit errors
            if (mysqlException.ErrorCode != MySqlErrorCode.TooManyUserConnections)
            {
                return false;
            }

            // Get the connection id for tracking retries
            string connectionId = connection.ConnectionString ?? Guid.NewGuid().ToString();
            int retryCount = _connectionRetryCount.GetValueOrDefault(connectionId, 0);

            // Don't retry if we've exceeded max retries
            if (retryCount >= MaxRetries)
            {
                _connectionRetryCount.TryRemove(connectionId, out _);
                _logger.LogError(
                    mysqlException,
                    "MySQL connection error (too many connections) - max retries ({MaxRetries}) exhausted",
                    MaxRetries);
                return false;
            }

            // Increment retry count
            retryCount++;
            _connectionRetryCount[connectionId] = retryCount;

            // Calculate backoff time with jitter
            int delayMs = (int)Math.Pow(2, retryCount) * 1000 + _random.Next(500);
            
            _logger.LogWarning(
                mysqlException,
                "MySQL connection error (too many connections), retry {RetryCount}/{MaxRetries} after {DelayMs}ms",
                retryCount, MaxRetries, delayMs);

            // Wait before retry
            await Task.Delay(delayMs, cancellationToken);
            
            // Try to reopen the connection
            try
            {
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync(cancellationToken);
                    return true;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reopening connection on retry {RetryCount}", retryCount);
            }
            
            return false;
        }
    }
}
