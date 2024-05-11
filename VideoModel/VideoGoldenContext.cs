using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace VideoModel;

public partial class VideoGoldenContext : IdentityDbContext<VideoUser>
{
    public VideoGoldenContext()
    {
    }

    public VideoGoldenContext(DbContextOptions<VideoGoldenContext> options)
        : base(options)
    {
    }

    public virtual DbSet<ChatRoom> ChatRooms { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<RegisteredUser> RegisteredUsers { get; set; }

    public virtual DbSet<Video> Videos { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (optionsBuilder.IsConfigured)
        {
            return;
        }
        IConfigurationBuilder builder = new ConfigurationBuilder().AddJsonFile("appsettings.json");
        var config = builder.Build();
        optionsBuilder
            .UseLazyLoadingProxies()
            .UseSqlServer(config.GetConnectionString("DefaultConnection"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Message>(entity =>
        {
            entity.Property(e => e.MessageId).ValueGeneratedOnAdd();

            entity.HasOne(d => d.ChatRoom).WithMany()
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Messages_ChatRooms");
        });

        modelBuilder.Entity<RegisteredUser>(entity =>
        {
            entity.HasMany(d => d.ChatRooms).WithMany(p => p.RegisteredUsers)
                .UsingEntity<Dictionary<string, object>>(
                    "UsersChatRoom",
                    r => r.HasOne<ChatRoom>().WithMany()
                        .HasForeignKey("ChatRoomId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UsersChatRooms_ChatRooms"),
                    l => l.HasOne<RegisteredUser>().WithMany()
                        .HasForeignKey("Username")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_UsersChatRooms_RegisteredUsers"),
                    j =>
                    {
                        j.HasKey("Username", "ChatRoomId");
                        j.ToTable("UsersChatRooms");
                        j.IndexerProperty<string>("Username").HasMaxLength(50);
                    });
        });

        modelBuilder.Entity<Video>(entity =>
        {
            entity.Property(e => e.Username).HasDefaultValue("");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
