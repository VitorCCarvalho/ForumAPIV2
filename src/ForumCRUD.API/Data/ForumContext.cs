using ForumCRUD.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Amazon.RDS;
using Microsoft.Extensions.Logging;

namespace ForumCRUD.API.Data;

public class ForumContext: IdentityDbContext<User>
{
    private readonly ILogger<MySqlRetryConnectionInterceptor> _logger;

    public ForumContext(DbContextOptions<ForumContext> opts) : base(opts) { }

    public ForumContext(
        DbContextOptions<ForumContext> opts,
        ILogger<MySqlRetryConnectionInterceptor> logger) : base(opts)
    {
        _logger = logger;
    }

    public DbSet<Forum> forums { get; set; }
    public DbSet<FThread> threads { get; set; }
    public DbSet<Post> posts { get; set; }
    public DbSet<FThreadReaction> fthreadreaction { get; set; }
    public DbSet<PostReaction> postreaction { get; set; }

    public DbSet<FThreadImage> fthreadimage { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {

        builder.Entity<Post>()
                   .HasOne(post => post.Thread)
                   .WithMany(fthread => fthread.Posts)
                   .HasForeignKey(post => post.ThreadId);

        builder.Entity<Post>()
                   .HasOne(post => post.User)
                   .WithMany(user => user.Posts)
                   .HasForeignKey(post => post.UserId);

        builder.Entity<FThread>()
                   .HasOne(fthread => fthread.Forum)
                   .WithMany(forum => forum.Threads)
                   .HasForeignKey(fthread => fthread.ForumID);

        builder.Entity<FThread>()
                   .HasOne(fthread => fthread.User)
                   .WithMany(user => user.Threads)
                   .HasForeignKey(fthread => fthread.UserId);

        builder.Entity<FThreadReaction>()
                   .HasOne(fthreadReaction => fthreadReaction.User)
                   .WithMany(user => user.ThreadReactions)
                   .HasForeignKey(fthread => fthread.UserId);

        builder.Entity<FThreadReaction>()
                   .HasOne(fthreadReaction => fthreadReaction.User)
                   .WithMany(user => user.ThreadReactions)
                   .HasForeignKey(fthreadReaction => fthreadReaction.UserId);

        builder.Entity<FThreadReaction>()
                   .HasOne(fthreadReaction => fthreadReaction.FThread)
                   .WithMany(fthread => fthread.Reactions)
                   .HasForeignKey(fthreadReaction => fthreadReaction.ThreadId);

        builder.Entity<PostReaction>()
                   .HasOne(postReaction => postReaction.User)
                   .WithMany(user => user.PostReactions)
                   .HasForeignKey(postReaction => postReaction.UserId);

        builder.Entity<PostReaction>()
                   .HasOne(postReaction => postReaction.Post)
                   .WithMany(post => post.Reactions)
                   .HasForeignKey(postReaction => postReaction.PostId);

        builder.Entity<FThreadImage>()
                   .HasOne(fthreadImage => fthreadImage.FThread)
                   .WithMany(fthread => fthread.Images)
                   .HasForeignKey(fthreadImage => fthreadImage.FThreadId);

        base.OnModelCreating(builder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // This will be called after the initial configuration from Program.cs
        if (!optionsBuilder.IsConfigured)
        {
            // Apply default configuration if not already configured
            return;
        }

        // Add additional interceptors - this will be applied even if configured in Program.cs
        if (_logger != null)
        {
            optionsBuilder.AddInterceptors(new MySqlRetryConnectionInterceptor(_logger));
        }
    }

    /// <summary>
    /// Executes database operations with automatic retries for connection errors
    /// </summary>
    /// <typeparam name="TResult">The type of the result</typeparam>
    /// <param name="operation">The database operation to execute</param>
    /// <returns>The result of the operation</returns>
    public async Task<TResult> ExecuteWithRetryAsync<TResult>(Func<Task<TResult>> operation)
    {
        // Get the execution strategy from the context
        var strategy = Database.CreateExecutionStrategy();
        
        // Execute the operation with the strategy
        return await strategy.ExecuteAsync(operation);
    }

    /// <summary>
    /// Executes database operations with automatic retries for connection errors
    /// </summary>
    /// <param name="operation">The database operation to execute</param>
    public async Task ExecuteWithRetryAsync(Func<Task> operation)
    {
        // Get the execution strategy from the context
        var strategy = Database.CreateExecutionStrategy();
        
        // Execute the operation with the strategy
        await strategy.ExecuteAsync(operation);
    }
}
