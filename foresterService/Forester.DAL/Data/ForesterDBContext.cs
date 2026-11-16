using System;
using System.Runtime.Intrinsics.X86;
using Forester.Domain.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Forester.DAL.Data;

public class ForesterDBContext : IdentityDbContext<User>
{
    public DbSet<User> Users { get; set; }
    public DbSet<Log> Logs { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Log>()
        .HasOne(l => l.LogType)
        .WithMany(lt => lt.Logs)
        .HasForeignKey(l => l.LogTypeID);

        builder.Entity<LogType>()
        .HasData(
           new LogType() { Id = 1, Name = "User Activity" });
    }
}
