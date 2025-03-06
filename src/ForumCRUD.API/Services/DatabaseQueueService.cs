using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MySqlConnector;

namespace ForumCRUD.API.Services
{
    public class DatabaseQueueService
    {
        private readonly SemaphoreSlim _dbSemaphore;
        private readonly ILogger<DatabaseQueueService> _logger;
        private static readonly Random _random = new Random();

        public DatabaseQueueService(SemaphoreSlim dbSemaphore, ILogger<DatabaseQueueService> logger)
        {
            _dbSemaphore = dbSemaphore;
            _logger = logger;
        }

        /// <summary>
        /// Executes a database operation while respecting the connection limit
        /// </summary>
        /// <typeparam name="T">The return type of the database operation</typeparam>
        /// <param name="operation">The database operation to execute</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        /// <returns>The result of the database operation</returns>
        public async Task<T> ExecuteWithConnectionLimitAsync<T>(Func<Task<T>> operation, CancellationToken cancellationToken = default)
        {
            const int maxRetries = 3;
            int retryCount = 0;
            
            while (true)
            {
                try
                {
                    // Wait for a connection to become available with a timeout
                    _logger.LogInformation("Waiting for database connection (attempt {RetryCount}/{MaxRetries})...", 
                        retryCount + 1, maxRetries);
                    
                    bool acquired = await _dbSemaphore.WaitAsync(TimeSpan.FromSeconds(60), cancellationToken);
                    
                    if (!acquired)
                    {
                        _logger.LogWarning("Failed to acquire database connection within timeout period");
                        throw new TimeoutException("Failed to acquire database connection within the timeout period");
                    }
                    
                    _logger.LogInformation("Database connection acquired");

                    // Execute the operation with a timeout
                    using var timeoutCts = new CancellationTokenSource(TimeSpan.FromSeconds(25));
                    using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(timeoutCts.Token, cancellationToken);
                    
                    try
                    {
                        return await operation();
                    }
                    catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested)
                    {
                        _logger.LogWarning("Database operation timed out");
                        throw new TimeoutException("Database operation timed out");
                    }
                    catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.TooManyUserConnections && retryCount < maxRetries)
                    {
                        retryCount++;
                        int delayMs = (int)Math.Pow(2, retryCount) * 1000 + _random.Next(500); // Exponential backoff with jitter
                        _logger.LogWarning(ex, "Too many MySQL connections, retrying in {DelayMs}ms (attempt {RetryCount}/{MaxRetries})", 
                            delayMs, retryCount, maxRetries);
                        
                        // Release before retry
                        _dbSemaphore.Release();
                        await Task.Delay(delayMs, cancellationToken);
                        continue; // Retry
                    }
                }
                catch (Exception ex)
                {
                    if (ex is MySqlException mysqlEx && mysqlEx.ErrorCode == MySqlErrorCode.TooManyUserConnections && retryCount < maxRetries)
                    {
                        retryCount++;
                        int delayMs = (int)Math.Pow(2, retryCount) * 1000 + _random.Next(500); // Exponential backoff with jitter
                        _logger.LogWarning(ex, "Too many MySQL connections, retrying in {DelayMs}ms (attempt {RetryCount}/{MaxRetries})", 
                            delayMs, retryCount, maxRetries);
                        
                        // If we had already acquired the semaphore, release it
                        if (_dbSemaphore.CurrentCount < 5)
                        {
                            _dbSemaphore.Release();
                        }
                        
                        await Task.Delay(delayMs, cancellationToken);
                        continue; // Retry
                    }
                    
                    _logger.LogError(ex, "Error executing database operation");
                    throw;
                }
                finally
                {
                    // Always release the semaphore when done, but only if we're not retrying
                    if (retryCount >= maxRetries || retryCount == 0)
                    {
                        _dbSemaphore.Release();
                        _logger.LogInformation("Database connection released");
                    }
                }
                
                // If we get here, we either succeeded or exhausted retries
                break;
            }
            
            // We shouldn't reach here, but just in case
            throw new InvalidOperationException("Unexpected execution path in database connection handling");
        }

        /// <summary>
        /// Executes a void database operation while respecting the connection limit
        /// </summary>
        /// <param name="operation">The database operation to execute</param>
        /// <param name="cancellationToken">Optional cancellation token</param>
        public async Task ExecuteWithConnectionLimitAsync(Func<Task> operation, CancellationToken cancellationToken = default)
        {
            const int maxRetries = 3;
            int retryCount = 0;
            
            while (true)
            {
                try
                {
                    // Wait for a connection to become available
                    _logger.LogInformation("Waiting for database connection (attempt {RetryCount}/{MaxRetries})...", 
                        retryCount + 1, maxRetries);
                    
                    bool acquired = await _dbSemaphore.WaitAsync(TimeSpan.FromSeconds(60), cancellationToken);
                    
                    if (!acquired)
                    {
                        _logger.LogWarning("Failed to acquire database connection within timeout period");
                        throw new TimeoutException("Failed to acquire database connection within the timeout period");
                    }
                    
                    _logger.LogInformation("Database connection acquired");

                    // Execute the operation
                    try
                    {
                        await operation();
                        return; // Success!
                    }
                    catch (MySqlException ex) when (ex.ErrorCode == MySqlErrorCode.TooManyUserConnections && retryCount < maxRetries)
                    {
                        retryCount++;
                        int delayMs = (int)Math.Pow(2, retryCount) * 1000 + _random.Next(500); // Exponential backoff with jitter
                        _logger.LogWarning(ex, "Too many MySQL connections, retrying in {DelayMs}ms (attempt {RetryCount}/{MaxRetries})", 
                            delayMs, retryCount, maxRetries);
                        
                        // Release before retry
                        _dbSemaphore.Release();
                        await Task.Delay(delayMs, cancellationToken);
                        continue; // Retry
                    }
                }
                catch (Exception ex)
                {
                    if (ex is MySqlException mysqlEx && mysqlEx.ErrorCode == MySqlErrorCode.TooManyUserConnections && retryCount < maxRetries)
                    {
                        retryCount++;
                        int delayMs = (int)Math.Pow(2, retryCount) * 1000 + _random.Next(500); // Exponential backoff with jitter
                        _logger.LogWarning(ex, "Too many MySQL connections, retrying in {DelayMs}ms (attempt {RetryCount}/{MaxRetries})", 
                            delayMs, retryCount, maxRetries);
                        
                        // If we had already acquired the semaphore, release it
                        if (_dbSemaphore.CurrentCount < 5)
                        {
                            _dbSemaphore.Release();
                        }
                        
                        await Task.Delay(delayMs, cancellationToken);
                        continue; // Retry
                    }
                    
                    _logger.LogError(ex, "Error executing database operation");
                    throw;
                }
                finally
                {
                    // Always release the semaphore when done, but only if we're not retrying
                    if (retryCount >= maxRetries || retryCount == 0)
                    {
                        _dbSemaphore.Release();
                        _logger.LogInformation("Database connection released");
                    }
                }
                
                // If we get here, we either succeeded or exhausted retries
                break;
            }
            
            // We shouldn't reach here, but just in case
            throw new InvalidOperationException("Unexpected execution path in database connection handling");
        }
    }
}
