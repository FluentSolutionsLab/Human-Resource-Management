﻿// <auto-generated />
using System;
using HRManagement.Modules.Staff.Data;
using HRManagement.Modules.Staff.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HRManagement.Modules.Staff.Persistence.Migrations
{
    [DbContext(typeof(StaffDbContext))]
    [Migration("20230124035027_UpdateEmployeeTable3")]
    partial class UpdateEmployeeTable3
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.12")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("HRManagement.Modules.Staff.Domain.Employee.Employee", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("HireDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("HireDate");

                    b.Property<Guid?>("ReportsToId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("ReportsToId");

                    b.Property<byte>("RoleId")
                        .HasColumnType("tinyint")
                        .HasColumnName("RoleId");

                    b.Property<DateTime?>("TerminationDate")
                        .HasColumnType("datetime2")
                        .HasColumnName("TerminationDate");

                    b.HasKey("Id");

                    b.HasIndex("ReportsToId");

                    b.HasIndex("RoleId");

                    b.ToTable("Employee", (string)null);
                });

            modelBuilder.Entity("HRManagement.Modules.Staff.Domain.Role.Role", b =>
                {
                    b.Property<byte>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("tinyint")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<byte>("Id"), 1L, 1);

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("Name");

                    b.Property<byte?>("ReportsToId")
                        .HasColumnType("tinyint")
                        .HasColumnName("ReportsToId");

                    b.HasKey("Id");

                    b.HasIndex("ReportsToId");

                    b.ToTable("Role", (string)null);
                });

            modelBuilder.Entity("HRManagement.Modules.Staff.Domain.Employee.Employee", b =>
                {
                    b.HasOne("HRManagement.Modules.Staff.Domain.Employee.Employee", "ReportsTo")
                        .WithMany()
                        .HasForeignKey("ReportsToId");

                    b.HasOne("HRManagement.Modules.Staff.Domain.Role.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("HRManagement.Modules.Staff.Domain.Employee.DateOfBirth", "DateOfBirth", b1 =>
                        {
                            b1.Property<Guid>("EmployeeId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<DateTime>("Date")
                                .HasColumnType("datetime2")
                                .HasColumnName("DateOfBirth");

                            b1.HasKey("EmployeeId");

                            b1.ToTable("Employee");

                            b1.WithOwner()
                                .HasForeignKey("EmployeeId");
                        });

                    b.OwnsOne("HRManagement.Modules.Staff.Domain.Employee.EmailAddress", "EmailAddress", b1 =>
                        {
                            b1.Property<Guid>("EmployeeId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Email")
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("Email");

                            b1.HasKey("EmployeeId");

                            b1.ToTable("Employee");

                            b1.WithOwner()
                                .HasForeignKey("EmployeeId");
                        });

                    b.OwnsOne("HRManagement.Modules.Staff.Domain.Employee.Name", "Name", b1 =>
                        {
                            b1.Property<Guid>("EmployeeId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("FirstName")
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("FirstName");

                            b1.Property<string>("LastName")
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("LastName");

                            b1.HasKey("EmployeeId");

                            b1.ToTable("Employee");

                            b1.WithOwner()
                                .HasForeignKey("EmployeeId");
                        });

                    b.Navigation("DateOfBirth")
                        .IsRequired();

                    b.Navigation("EmailAddress")
                        .IsRequired();

                    b.Navigation("Name")
                        .IsRequired();

                    b.Navigation("ReportsTo");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("HRManagement.Modules.Staff.Domain.Role.Role", b =>
                {
                    b.HasOne("HRManagement.Modules.Staff.Domain.Role.Role", "ReportsTo")
                        .WithMany()
                        .HasForeignKey("ReportsToId");

                    b.Navigation("ReportsTo");
                });
#pragma warning restore 612, 618
        }
    }
}
