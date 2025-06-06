﻿// Data/ApplicationDbContext.cs
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

        public DbSet<GroupChat> GroupChats { get; set; }
        public DbSet<GroupChatUser> GroupChatUsers { get; set; }
        public DbSet<GroupMessage> GroupMessages { get; set; }

        public DbSet<ReportedPost> ReportedPosts { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ReportedPost>()
                .HasIndex(r => new { r.UserId, r.PostId })
                .IsUnique(); // Prevent duplicate reports

            builder.Entity<ReportedPost>()
    .HasOne(rp => rp.Post)
    .WithMany()
    .HasForeignKey(rp => rp.PostId)
    .OnDelete(DeleteBehavior.Restrict); // ✅ Safe: Prevents cascade path conflict

            builder.Entity<ReportedPost>()
                .HasOne(rp => rp.User)
                .WithMany()
                .HasForeignKey(rp => rp.UserId)
                .OnDelete(DeleteBehavior.Cascade); // Optional: delete reports if user is deleted


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
                                                    //         builder.Entity<Message>()
                                                    //.HasOne(m => m.SharedPost)
                                                    //.WithMany()
                                                    //.HasForeignKey(m => m.SharedPostId)
                                                    //.OnDelete(DeleteBehavior.SetNull);  // ✅ Recommended — unlink, don't delete

            builder.Entity<GroupChatUser>()
    .HasOne(gcu => gcu.GroupChat)
    .WithMany(gc => gc.Members)
    .HasForeignKey(gcu => gcu.GroupChatId)
     .OnDelete(DeleteBehavior.Restrict); ;

            builder.Entity<GroupChatUser>()
                .HasOne(gcu => gcu.User)
                .WithMany()
                .HasForeignKey(gcu => gcu.UserId);

            builder.Entity<GroupMessage>()
                .HasOne(gm => gm.GroupChat)
                .WithMany(gc => gc.Messages)
                .HasForeignKey(gm => gm.GroupChatId)
                .OnDelete(DeleteBehavior.Restrict); ;


            builder.Entity<GroupMessage>()
                .HasOne(gm => gm.Sender)
                .WithMany()
                .HasForeignKey(gm => gm.SenderId);

        }

    }
}