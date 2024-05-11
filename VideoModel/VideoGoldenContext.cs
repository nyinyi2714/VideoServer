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

    public virtual DbSet<Video> Videos { get; set; }
    public DbSet<RegisteredUser> RegisteredUsers { get; set; }

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
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Video>()
            .HasOne(v => v.RegisteredUser)
            .WithMany(u => u.Videos)
            .HasForeignKey(v => v.Username);
        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
