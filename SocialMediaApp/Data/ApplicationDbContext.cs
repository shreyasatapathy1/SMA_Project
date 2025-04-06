// Data/ApplicationDbContext.cs
using System.Reflection.Emit;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SocialMediaApp.Models;

namespace SocialMediaApp.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<FriendRequest> FriendRequests { get; set; }
        public DbSet<Message> Messages { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<PostLike> PostLikes { get; set; }

        public DbSet<PostComment> PostComments { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<FriendRequest>()
                .HasOne(fr => fr.Sender)
                .WithMany()
                .HasForeignKey(fr => fr.SenderId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete

            builder.Entity<FriendRequest>()
                .HasOne(fr => fr.Receiver)
                .WithMany()
                .HasForeignKey(fr => fr.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);  // Prevent cascade delete

            builder.Entity<Message>()
        .HasOne(m => m.Sender)
        .WithMany()
        .HasForeignKey(m => m.SenderId)
        .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<PostLike>()
    .HasIndex(pl => new { pl.UserId, pl.PostId })
    .IsUnique();

            builder.Entity<PostLike>()
    .HasOne(pl => pl.Post)
    .WithMany(p => p.Likes)
    .HasForeignKey(pl => pl.PostId)
    .OnDelete(DeleteBehavior.Restrict); // Prevent cascade loop

            builder.Entity<PostLike>()
                .HasOne(pl => pl.User)
                .WithMany()
                .HasForeignKey(pl => pl.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Optional: delete likes when user deleted

            builder.Entity<PostComment>()
        .HasOne(c => c.User)
        .WithMany()
        .HasForeignKey(c => c.UserId)
        .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<PostComment>()
                .HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Restrict); // ✅ Prevent cascade path conflict

            builder.Entity<Message>()
                .HasOne(m => m.SharedPost)
                .WithMany()
                .HasForeignKey(m => m.SharedPostId)
                .OnDelete(DeleteBehavior.Restrict); // To prevent cascade delete


        }

    }
}