using ForumCRUD.API.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Amazon.RDS;

namespace ForumCRUD.API.Data;

public class ForumContext: IdentityDbContext<User>
{
    public ForumContext(DbContextOptions<ForumContext> opts) : base(opts) { }

    public DbSet<Forum> Forums { get; set; }
    public DbSet<FThread> Threads { get; set; }
    public DbSet<Post> Posts { get; set; }
    public DbSet<FThreadReaction> FThreadReaction { get; set; }
    public DbSet<PostReaction> PostReaction { get; set; }

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

        base.OnModelCreating(builder);
    }
}
