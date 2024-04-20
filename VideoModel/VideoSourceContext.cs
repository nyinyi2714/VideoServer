using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace VideoModel;

public partial class VideoSourceContext : IdentityDbContext<VideoUser>
{
    public VideoSourceContext()
    {
    }

    public VideoSourceContext(DbContextOptions<VideoSourceContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChatRoom> ChatRooms { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Video> Videos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }
        IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        var config = builder.Build();
        optionsBuilder.UseSqlServer(config.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ChatRoom>(entity =>
        {
            entity.HasKey(e => e.ChatRoomId).HasName("PK__ChatRoom__69733CF70BA3BE0E");
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasKey(e => e.MessageId).HasName("PK__Messages__4808B993CF9DC4C0");

            entity.HasOne(d => d.ChatRoom).WithMany(p => p.Messages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_ChatRooms");

            entity.HasOne(d => d.Sender).WithMany(p => p.Messages)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_Users");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK_User");

            entity.HasMany(d => d.ChatRooms).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "UsersChatRoom",
                    r => r.HasOne<ChatRoom>().WithMany()
                        .HasForeignKey("ChatRoomId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UsersChatRooms_ChatRooms"),
                    l => l.HasOne<User>().WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UsersChatRooms_Users"),
                    j =>
                    {
                        j.HasKey("UserId", "ChatRoomId");
                        j.ToTable("UsersChatRooms");
                    });
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.HasKey(e => e.VideoId).HasName("PK_Video_1");

            entity.HasOne(d => d.User).WithMany(p => p.Videos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Video_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
