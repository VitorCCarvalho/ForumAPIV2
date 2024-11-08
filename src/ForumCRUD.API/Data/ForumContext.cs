using ForumCRUD.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Amazon.RDS;

namespace ForumCRUD.API.Data;

public class ForumContext: IdentityDbContext<User>
{
    public ForumContext(DbContextOptions<ForumContext> opts) : base(opts) { }

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
}
