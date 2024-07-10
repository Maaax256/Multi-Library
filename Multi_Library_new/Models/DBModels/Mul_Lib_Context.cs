using System;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Multi_Library.Interfaces;
using Multi_Library_new.Models.DBModels;

#nullable disable

namespace Multi_Library.Models
{
    public partial class Mul_Lib_Context : DbContext
    {
        private IHttpContextAccessor Context { get; }
        private HttpContext HttpContext => Context.HttpContext;
        private ClaimsPrincipal User => HttpContext.User;

        public Mul_Lib_Context(DbContextOptions options, IHttpContextAccessor context)
            : base(options)
        {
            Context = context;
        }

        public virtual DbSet<Album> Albums { get; set; }
        public virtual DbSet<AuthorSong> AuthorSongs { get; set; }
        public virtual DbSet<Cover> Covers { get; set; }
        public virtual DbSet<Song> Songs { get; set; }
        public virtual DbSet<UserTable> UserTables { get; set; }
        public virtual DbSet<VideoClip> VideoClips { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                string role, password;

                //role = "connector";
                //password = "1234";

                //if (User.Identities.Count() > 1)
                //{
                string roleint = User.FindFirst(ClaimsIdentity.DefaultRoleClaimType)?.Value;

                switch (roleint)
                {
                    case "1":
                        { password = "4758"; role = "author"; break; }
                    case "2":
                        { password = "5927"; role = "admin"; break;}
                    case "0":
                        password = "1425"; role = "authorized_user"; break;
                    default:
                    {
                        password = "1234";
                        role = "guest";
                        break;
                    }    
                }
                     //User.FindFirst("Password")?.Value ?? "";
                //}
                //if (role == null)
                //{
                //    role = "connector";
                //    password = "1234";
                //}

                optionsBuilder.UseNpgsql($"Host=localhost;Port=5432;ConnectionIdleLifetime=10;Database=courseDB;Username={role};Password={password}");
            }
        }

        public IQueryable<LoginModel> Check_what_role(string login) =>
        Set<LoginModel>().FromSqlRaw($"select * from check_what_role('{login}');");

        public IQueryable<Song> Get_author_singles(int userId) =>
        Set<Song>().FromSqlRaw($"select * from get_author_singles({userId});");

        public void Remove_song(int songId) =>
        Database.ExecuteSqlInterpolated($"call remove_song({songId})");

        public void Change_user_info(int userId, string name, bool isAuthor, string description) =>
        Database.ExecuteSqlInterpolated($"call change_user_info({userId}, {name}, {isAuthor}, {description})");


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LoginModel>().HasNoKey().ToView(null);

            //modelBuilder.HasPostgresEnum(null, "user_type", new[] { "Admin", "Author", "User" });

            modelBuilder.Entity<Album>(entity =>
            {
                entity.ToTable("album");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.DateCreate)
                .HasColumnType("date")
                .HasColumnName("date_create");
                //.HasDefaultValue("current_data");
            });

            modelBuilder.Entity<AuthorSong>(entity =>
            {
                entity.HasKey(e => new { e.SongId, e.AuthorId })
                    .HasName("pk_author_song");

                entity.ToTable("author_song");

                entity.Property(e => e.SongId).HasColumnName("song_id").HasColumnType("integer");

                entity.Property(e => e.AuthorId).HasColumnName("author_id").HasColumnType("integer");

                entity.HasOne(d => d.Author)
                    .WithMany(p => p.AuthorSongs)
                    .HasForeignKey(d => d.AuthorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("author_song_author_id_fkey");

                entity.HasOne(d => d.Song)
                    .WithMany(p => p.AuthorSongs)
                    .HasForeignKey(d => d.SongId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("author_song_song_id_fkey");
            });

            modelBuilder.Entity<Cover>(entity =>
            {
                entity.ToTable("cover");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Link)
                    .IsRequired()
                    .HasColumnName("link");

                entity.Property(e => e.DateCreate)
                    .HasColumnType("date")
                    .HasColumnName("date_create");
                    //.HasDefaultValue("current_timestamp");
            });

            modelBuilder.Entity<Song>(entity =>
            {
                entity.ToTable("song");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Link)
                    .IsRequired()
                    .HasColumnName("link");

                entity.Property(e => e.Lyrics).HasColumnName("lyrics");

                entity.Property(e => e.DateCreate)
                    .HasColumnType("date")
                    .HasColumnName("date_create");
                    //.HasDefaultValue("current_timestamp");

                entity.Property(e => e.AlbumId).HasColumnName("album_id").HasColumnType("integer");

                entity.Property(e => e.CoverId).HasColumnName("cover_id").HasColumnType("integer");

                entity.HasOne(d => d.Album)
                    .WithMany(p => p.Songs)
                    .HasForeignKey(d => d.AlbumId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("song_album_id_fkey");

                entity.HasOne(d => d.Cover)
                    .WithMany(p => p.Songs)
                    .HasForeignKey(d => d.CoverId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("song_cover_id_fkey");
            });

            _ = modelBuilder.Entity<UserTable>(entity =>
            {
                entity.ToTable("user_table");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.PasswordHash)
                .IsRequired()
                .HasColumnName("password_hash")
                .HasColumnType("text");

                entity.Property(e => e.UserType)
                .IsRequired()
                .HasColumnType("integer")
                .HasColumnName("u_type");
                //.HasConversion(v => v.ToString(), v => Enum.Parse<user_type>(v));

                entity.Property(e => e.DateCreate)
                    .HasColumnType("date")
                    .HasColumnName("date_create");
                //.HasDefaultValue("current_timestamp");
            });

            modelBuilder.Entity<VideoClip>(entity =>
            {
                entity.ToTable("video_clip");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Link)
                    .IsRequired()
                    .HasColumnName("link");

                entity.Property(e => e.DateCreate)
                    .HasColumnType("date")
                    .HasColumnName("date_create");
                    //.HasDefaultValue("current_timestamp");

                entity.Property(e => e.SongId).HasColumnName("song_id").HasColumnType("integer");

                entity.HasIndex(e => e.SongId, "video_clip_song_id_key")
                    .IsUnique();

                entity.HasOne(d => d.Song)
                    .WithOne(p => p.VideoClip)
                    .HasForeignKey<VideoClip>(d => d.SongId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("video_clip_song_id_fkey");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
