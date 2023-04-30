using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Diplom;

public partial class MusicamContext : DbContext
{
    public MusicamContext()
    {
    }

    public MusicamContext(DbContextOptions<MusicamContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Device> Devices { get; set; }

    public virtual DbSet<ExtraSong> ExtraSongs { get; set; }

    public virtual DbSet<Level> Levels { get; set; }

    public virtual DbSet<Score> Scores { get; set; }

    public virtual DbSet<Song> Songs { get; set; }

    public virtual DbSet<SoundBank> SoundBanks { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UsersRecording> UsersRecordings { get; set; }

    public virtual DbSet<UsersSong> UsersSongs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=Musicam");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Device>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("Id");
            entity.Property(e => e.Name)
                .HasMaxLength(1000)
                .IsUnicode(true);
        });

        modelBuilder.Entity<ExtraSong>(entity =>
        {
            entity.ToTable("Extra_songs");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Author)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.Duration).HasPrecision(5);
            entity.Property(e => e.Genre)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.LvlToAchieve).HasColumnName("Lvl_To_Achieve");
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsFixedLength();

            entity.HasOne(d => d.LvlToAchieveNavigation).WithMany(p => p.ExtraSongs)
                .HasForeignKey(d => d.LvlToAchieve)
                .HasConstraintName("FK_Extra_songs_Levels");
        });

        modelBuilder.Entity<Level>(entity =>
        {
            entity.HasKey(e => e.Level1);

            entity.Property(e => e.Level1)
                .ValueGeneratedNever()
                .HasColumnName("Level");
            entity.Property(e => e.UserId).HasColumnName("User_id");
            entity.Property(e => e._5StarSongs).HasColumnName("5_star_songs");

            entity.HasOne(d => d.User).WithMany(p => p.Levels)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_Levels_User");
        });

        modelBuilder.Entity<Score>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("Score");

            entity.Property(e => e.Score1).HasColumnName("Score");
            entity.Property(e => e.UserId).HasColumnName("User_id");

            entity.HasOne(d => d.SongNavigation).WithMany()
                .HasForeignKey(d => d.Song)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Score_Songs");

            entity.HasOne(d => d.User).WithMany()
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Score_User");
        });

        modelBuilder.Entity<Song>(entity =>
        {
            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Author)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.Difficulty).HasDefaultValueSql("((0))");
            entity.Property(e => e.Duration).HasPrecision(5);
            entity.Property(e => e.Genre)
                .HasMaxLength(50)
                .IsFixedLength();
            entity.Property(e => e.Name)
                .HasMaxLength(50)
                .IsFixedLength();
        });

        modelBuilder.Entity<SoundBank>(entity =>
        {
            entity.HasNoKey();

            entity.Property(e => e.File)
                .HasMaxLength(1000)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.Instrument)
                .HasMaxLength(20)
                .IsFixedLength();
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("User");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Sex)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.Timbre)
                .HasMaxLength(20)
                .IsFixedLength();
            entity.Property(e => e.UserName)
                .HasMaxLength(20)
                .IsFixedLength();
        });

        modelBuilder.Entity<UsersRecording>(entity =>
        {
            entity.HasKey(e => e.Name);

            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .IsFixedLength();
            entity.Property(e => e.File)
                .HasMaxLength(1000)
                .IsFixedLength();

            entity.HasOne(d => d.UserNavigation).WithMany(p => p.UsersRecordings)
                .HasForeignKey(d => d.User)
                .HasConstraintName("FK_UsersRecordings_User");
        });

        modelBuilder.Entity<UsersSong>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("UsersSong");

            entity.Property(e => e.Song)
                .HasMaxLength(100)
                .IsFixedLength();

            entity.HasOne(d => d.UserNavigation).WithMany()
                .HasForeignKey(d => d.User)
                .HasConstraintName("FK_UsersSong_User");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
