﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Multi_Library.Models;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Multi_Library_new.Migrations
{
    [DbContext(typeof(Mul_Lib_Context))]
    [Migration("20240501230155_NewMigration1")]
    partial class NewMigration1
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasPostgresEnum(null, "user_type", new[] { "Admin", "Author", "User" })
                .HasAnnotation("Relational:MaxIdentifierLength", 63)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            modelBuilder.Entity("Multi_Library.Models.Album", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("DateCreate")
                        .HasColumnType("date")
                        .HasColumnName("date_create");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.ToTable("album");
                });

            modelBuilder.Entity("Multi_Library.Models.AuthorSong", b =>
                {
                    b.Property<int>("SongId")
                        .HasColumnType("integer")
                        .HasColumnName("song_id");

                    b.Property<int>("AuthorId")
                        .HasColumnType("integer")
                        .HasColumnName("author_id");

                    b.HasKey("SongId", "AuthorId")
                        .HasName("pk_author_song");

                    b.HasIndex("AuthorId");

                    b.ToTable("author_song");
                });

            modelBuilder.Entity("Multi_Library.Models.Cover", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("DateCreate")
                        .HasColumnType("date")
                        .HasColumnName("date_create");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("link");

                    b.HasKey("Id");

                    b.ToTable("cover");
                });

            modelBuilder.Entity("Multi_Library.Models.Song", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<int?>("AlbumId")
                        .HasColumnType("integer")
                        .HasColumnName("album_id");

                    b.Property<int?>("CoverId")
                        .HasColumnType("integer")
                        .HasColumnName("cover_id");

                    b.Property<DateTime?>("DateCreate")
                        .HasColumnType("date")
                        .HasColumnName("date_create");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("link");

                    b.Property<string>("Lyrics")
                        .HasColumnType("text")
                        .HasColumnName("lyrics");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.HasKey("Id");

                    b.HasIndex("AlbumId");

                    b.HasIndex("CoverId");

                    b.ToTable("song");
                });

            modelBuilder.Entity("Multi_Library.Models.UserTable", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("DateCreate")
                        .HasColumnType("date")
                        .HasColumnName("date_create");

                    b.Property<string>("Description")
                        .HasColumnType("text")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)")
                        .HasColumnName("name");

                    b.Property<int>("UserType")
                        .HasColumnType("user_type")
                        .HasColumnName("u_type");

                    b.HasKey("Id");

                    b.ToTable("user_table");
                });

            modelBuilder.Entity("Multi_Library.Models.VideoClip", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer")
                        .HasColumnName("id")
                        .HasAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

                    b.Property<DateTime?>("DateCreate")
                        .HasColumnType("date")
                        .HasColumnName("date_create");

                    b.Property<string>("Link")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("link");

                    b.Property<int>("SongId")
                        .HasColumnType("integer")
                        .HasColumnName("song_id");

                    b.HasKey("Id");

                    b.HasIndex(new[] { "SongId" }, "video_clip_song_id_key")
                        .IsUnique();

                    b.ToTable("video_clip");
                });

            modelBuilder.Entity("Multi_Library.Models.AuthorSong", b =>
                {
                    b.HasOne("Multi_Library.Models.UserTable", "Author")
                        .WithMany("AuthorSongs")
                        .HasForeignKey("AuthorId")
                        .HasConstraintName("author_song_author_id_fkey")
                        .IsRequired();

                    b.HasOne("Multi_Library.Models.Song", "Song")
                        .WithMany("AuthorSongs")
                        .HasForeignKey("SongId")
                        .HasConstraintName("author_song_song_id_fkey")
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Song");
                });

            modelBuilder.Entity("Multi_Library.Models.Song", b =>
                {
                    b.HasOne("Multi_Library.Models.Album", "Album")
                        .WithMany("Songs")
                        .HasForeignKey("AlbumId")
                        .HasConstraintName("song_album_id_fkey");

                    b.HasOne("Multi_Library.Models.Cover", "Cover")
                        .WithMany("Songs")
                        .HasForeignKey("CoverId")
                        .HasConstraintName("song_cover_id_fkey");

                    b.Navigation("Album");

                    b.Navigation("Cover");
                });

            modelBuilder.Entity("Multi_Library.Models.VideoClip", b =>
                {
                    b.HasOne("Multi_Library.Models.Song", "Song")
                        .WithOne("VideoClip")
                        .HasForeignKey("Multi_Library.Models.VideoClip", "SongId")
                        .HasConstraintName("video_clip_song_id_fkey")
                        .IsRequired();

                    b.Navigation("Song");
                });

            modelBuilder.Entity("Multi_Library.Models.Album", b =>
                {
                    b.Navigation("Songs");
                });

            modelBuilder.Entity("Multi_Library.Models.Cover", b =>
                {
                    b.Navigation("Songs");
                });

            modelBuilder.Entity("Multi_Library.Models.Song", b =>
                {
                    b.Navigation("AuthorSongs");

                    b.Navigation("VideoClip");
                });

            modelBuilder.Entity("Multi_Library.Models.UserTable", b =>
                {
                    b.Navigation("AuthorSongs");
                });
#pragma warning restore 612, 618
        }
    }
}
