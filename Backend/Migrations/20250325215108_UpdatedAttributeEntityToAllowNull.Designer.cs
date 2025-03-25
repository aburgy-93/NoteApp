﻿// <auto-generated />
using System;
using Backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace Backend.Migrations
{
    [DbContext(typeof(NoteDbContext))]
    [Migration("20250325215108_UpdatedAttributeEntityToAllowNull")]
    partial class UpdatedAttributeEntityToAllowNull
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.3")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("Backend.Models.AttributeEntity", b =>
                {
                    b.Property<int>("AttributeId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("AttributeId"));

                    b.Property<string>("AttributeName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("AttributeId");

                    b.HasIndex("AttributeName")
                        .IsUnique();

                    b.ToTable("Attributes");
                });

            modelBuilder.Entity("Backend.Models.Note", b =>
                {
                    b.Property<int>("NoteId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("NoteId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime");

                    b.Property<string>("NoteText")
                        .HasColumnType("nvarchar(255)");

                    b.Property<int?>("ProjectId")
                        .HasColumnType("int");

                    b.HasKey("NoteId");

                    b.HasIndex("ProjectId");

                    b.ToTable("Notes");
                });

            modelBuilder.Entity("Backend.Models.NoteAttributeJoin", b =>
                {
                    b.Property<int>("NoteId")
                        .HasColumnType("int");

                    b.Property<int>("AttributeId")
                        .HasColumnType("int");

                    b.HasKey("NoteId", "AttributeId");

                    b.HasIndex("AttributeId");

                    b.ToTable("NoteAttributes");
                });

            modelBuilder.Entity("Backend.Models.Project", b =>
                {
                    b.Property<int>("ProjectId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ProjectId"));

                    b.Property<string>("ProjectName")
                        .IsRequired()
                        .HasColumnType("nvarchar(25)");

                    b.HasKey("ProjectId");

                    b.HasIndex("ProjectName")
                        .IsUnique();

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("Backend.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("UserId"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("datetime2");

                    b.Property<DateTime>("LastLoginAt")
                        .HasColumnType("datetime2");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");
                });

            modelBuilder.Entity("Backend.Models.Note", b =>
                {
                    b.HasOne("Backend.Models.Project", "Project")
                        .WithMany("Notes")
                        .HasForeignKey("ProjectId")
                        .OnDelete(DeleteBehavior.SetNull);

                    b.Navigation("Project");
                });

            modelBuilder.Entity("Backend.Models.NoteAttributeJoin", b =>
                {
                    b.HasOne("Backend.Models.AttributeEntity", "Attribute")
                        .WithMany("NoteAttributes")
                        .HasForeignKey("AttributeId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Backend.Models.Note", "Note")
                        .WithMany("NoteAttributes")
                        .HasForeignKey("NoteId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Attribute");

                    b.Navigation("Note");
                });

            modelBuilder.Entity("Backend.Models.AttributeEntity", b =>
                {
                    b.Navigation("NoteAttributes");
                });

            modelBuilder.Entity("Backend.Models.Note", b =>
                {
                    b.Navigation("NoteAttributes");
                });

            modelBuilder.Entity("Backend.Models.Project", b =>
                {
                    b.Navigation("Notes");
                });
#pragma warning restore 612, 618
        }
    }
}
