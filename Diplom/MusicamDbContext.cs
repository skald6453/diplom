using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Diplom;

public partial class MusicamDbContext : DbContext
{
    public MusicamDbContext()
    {
    }

    public MusicamDbContext(DbContextOptions<MusicamDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Devices2> Devices2s { get; set; }

    public virtual DbSet<Songs2> Songs2s { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlite("Data Source=D:\\diplomFull\\Diplom\\bin\\Debug\\net7.0\\\\\\\\MusicamDB.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Devices2>(entity =>
        {
            entity.ToTable("Devices2");

            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<Songs2>(entity =>
        {
            entity.ToTable("Songs2");

            entity.Property(e => e.Id)
                .ValueGeneratedNever()
                .HasColumnName("id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
