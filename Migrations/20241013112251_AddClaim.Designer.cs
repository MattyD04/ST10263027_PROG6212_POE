﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using ST10263027_PROG6212_POE.Data;

#nullable disable

namespace ST10263027_PROG6212_POE.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20241013112251_AddClaim")]
    partial class AddClaim
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.10")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("ST10263027_PROG6212_POE.Models.AcademicManager", b =>
                {
                    b.Property<int>("ManagerID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ManagerID"));

                    b.Property<string>("ManagerNum")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("ManagerID");

                    b.ToTable("AcademicManagers");
                });

            modelBuilder.Entity("ST10263027_PROG6212_POE.Models.Claim", b =>
                {
                    b.Property<int>("ClaimID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("ClaimID"));

                    b.Property<string>("ClaimNum")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimStatus")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Comments")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ContentType")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int?>("CoordinatorID")
                        .HasColumnType("int");

                    b.Property<byte[]>("FileData")
                        .IsRequired()
                        .HasColumnType("varbinary(max)");

                    b.Property<string>("Filename")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<int>("LecturerID")
                        .HasColumnType("int");

                    b.Property<int?>("ManagerID")
                        .HasColumnType("int");

                    b.Property<DateTime>("SubmissionDate")
                        .HasColumnType("datetime2");

                    b.HasKey("ClaimID");

                    b.HasIndex("CoordinatorID");

                    b.HasIndex("LecturerID");

                    b.HasIndex("ManagerID");

                    b.ToTable("Claims");
                });

            modelBuilder.Entity("ST10263027_PROG6212_POE.Models.Lecturer", b =>
                {
                    b.Property<int>("LecturerId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("LecturerId"));

                    b.Property<double>("HourlyRate")
                        .HasColumnType("float");

                    b.Property<double>("HoursWorked")
                        .HasColumnType("float");

                    b.Property<string>("LecturerNum")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("LecturerId");

                    b.ToTable("Lecturers");
                });

            modelBuilder.Entity("ST10263027_PROG6212_POE.Models.ProgrammeCoordinator", b =>
                {
                    b.Property<int>("CoordinatorID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("CoordinatorID"));

                    b.Property<string>("CoordinatorNum")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("CoordinatorID");

                    b.ToTable("ProgrammeCoordinators");
                });

            modelBuilder.Entity("ST10263027_PROG6212_POE.Models.Claim", b =>
                {
                    b.HasOne("ST10263027_PROG6212_POE.Models.ProgrammeCoordinator", "Coordinator")
                        .WithMany()
                        .HasForeignKey("CoordinatorID");

                    b.HasOne("ST10263027_PROG6212_POE.Models.Lecturer", "Lecturer")
                        .WithMany()
                        .HasForeignKey("LecturerID")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("ST10263027_PROG6212_POE.Models.AcademicManager", "Manager")
                        .WithMany()
                        .HasForeignKey("ManagerID");

                    b.Navigation("Coordinator");

                    b.Navigation("Lecturer");

                    b.Navigation("Manager");
                });
#pragma warning restore 612, 618
        }
    }
}
