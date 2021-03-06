﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using User.API.Application;

namespace User.API.Migrations
{
    [DbContext(typeof(UserContext))]
    partial class UserContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("User.API.Models.AppUser", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("Age")
                        .HasColumnType("int");

                    b.Property<string>("Name")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("Phone")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("User.API.Models.BpFile", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<int>("AppUserId")
                        .HasColumnType("int");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("FileName")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("FromatFilePath")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.Property<string>("OriginFilePath")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Id");

                    b.ToTable("BpFiles");
                });

            modelBuilder.Entity("User.API.Models.UserProperty", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<int>("AppUserId")
                        .HasColumnType("int");

                    b.Property<string>("Value")
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<string>("Text")
                        .HasColumnType("longtext CHARACTER SET utf8mb4");

                    b.HasKey("Key", "AppUserId", "Value");

                    b.HasIndex("AppUserId");

                    b.ToTable("UserProperties");
                });

            modelBuilder.Entity("User.API.Models.UserTage", b =>
                {
                    b.Property<int>("AppUserId")
                        .HasColumnType("int");

                    b.Property<string>("Tag")
                        .HasColumnType("varchar(100) CHARACTER SET utf8mb4")
                        .HasMaxLength(100);

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime(6)");

                    b.HasKey("AppUserId", "Tag");

                    b.ToTable("UserTages");
                });

            modelBuilder.Entity("User.API.Models.UserProperty", b =>
                {
                    b.HasOne("User.API.Models.AppUser", null)
                        .WithMany("Properties")
                        .HasForeignKey("AppUserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
