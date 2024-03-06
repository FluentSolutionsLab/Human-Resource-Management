﻿// <auto-generated />
using System;
using HRManagement.Modules.Staff.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace HRManagement.Modules.Staff.Data.Migrations
{
    [DbContext(typeof(StaffDbContext))]
    [Migration("20240305215738_ModifyRoleIdColumnType")]
    partial class ModifyRoleIdColumnType
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder);

            modelBuilder.Entity("HRManagement.Modules.Staff.Models.Employee", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid?>("ManagerId")
                        .HasColumnType("uniqueidentifier")
                        .HasColumnName("ManagerId");

                    b.Property<int>("RoleId")
                        .HasColumnType("int")
                        .HasColumnName("RoleId");

                    b.HasKey("Id");

                    b.HasIndex("ManagerId");

                    b.HasIndex("RoleId");

                    b.ToTable("Employee", "PersonnelManagement");
                });

            modelBuilder.Entity("HRManagement.Modules.Staff.Models.Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("Id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"));

                    b.Property<int?>("ReportsToId")
                        .HasColumnType("int")
                        .HasColumnName("ReportsToId");

                    b.HasKey("Id");

                    b.HasIndex("ReportsToId");

                    b.ToTable("Role", "PersonnelManagement");
                });

            modelBuilder.Entity("HRManagement.Modules.Staff.Models.Employee", b =>
                {
                    b.HasOne("HRManagement.Modules.Staff.Models.Employee", "Manager")
                        .WithMany("ManagedEmployees")
                        .HasForeignKey("ManagerId");

                    b.HasOne("HRManagement.Modules.Staff.Models.Role", "Role")
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.OwnsOne("HRManagement.Modules.Staff.Models.ValueObjects.ValueDate", "BirthDate", b1 =>
                        {
                            b1.Property<Guid>("EmployeeId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<DateTime>("Date")
                                .HasColumnType("date")
                                .HasColumnName("DateOfBirth");

                            b1.HasKey("EmployeeId");

                            b1.ToTable("Employee", "PersonnelManagement");

                            b1.WithOwner()
                                .HasForeignKey("EmployeeId");
                        });

                    b.OwnsOne("HRManagement.Modules.Staff.Models.ValueObjects.ValueDate", "HireDate", b1 =>
                        {
                            b1.Property<Guid>("EmployeeId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<DateTime>("Date")
                                .HasColumnType("date")
                                .HasColumnName("HireDate");

                            b1.HasKey("EmployeeId");

                            b1.ToTable("Employee", "PersonnelManagement");

                            b1.WithOwner()
                                .HasForeignKey("EmployeeId");
                        });

                    b.OwnsOne("HRManagement.Modules.Staff.Models.ValueObjects.ValueDate", "TerminationDate", b1 =>
                        {
                            b1.Property<Guid>("EmployeeId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<DateTime>("Date")
                                .HasColumnType("date")
                                .HasColumnName("TerminationDate");

                            b1.HasKey("EmployeeId");

                            b1.ToTable("Employee", "PersonnelManagement");

                            b1.WithOwner()
                                .HasForeignKey("EmployeeId");
                        });

                    b.OwnsOne("HRManagement.Modules.Staff.Models.Name", "Name", b1 =>
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

                            b1.ToTable("Employee", "PersonnelManagement");

                            b1.WithOwner()
                                .HasForeignKey("EmployeeId");
                        });

                    b.OwnsOne("HRManagement.Modules.Staff.Models.ValueObjects.EmailAddress", "EmailAddress", b1 =>
                        {
                            b1.Property<Guid>("EmployeeId")
                                .HasColumnType("uniqueidentifier");

                            b1.Property<string>("Email")
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("Email");

                            b1.HasKey("EmployeeId");

                            b1.ToTable("Employee", "PersonnelManagement");

                            b1.WithOwner()
                                .HasForeignKey("EmployeeId");
                        });

                    b.Navigation("BirthDate")
                        .IsRequired();

                    b.Navigation("EmailAddress")
                        .IsRequired();

                    b.Navigation("HireDate")
                        .IsRequired();

                    b.Navigation("Manager");

                    b.Navigation("Name")
                        .IsRequired();

                    b.Navigation("Role");

                    b.Navigation("TerminationDate");
                });

            modelBuilder.Entity("HRManagement.Modules.Staff.Models.Role", b =>
                {
                    b.HasOne("HRManagement.Modules.Staff.Models.Role", "ReportsTo")
                        .WithMany()
                        .HasForeignKey("ReportsToId");

                    b.OwnsOne("HRManagement.Modules.Staff.Models.ValueObjects.RoleName", "Name", b1 =>
                        {
                            b1.Property<int>("RoleId")
                                .HasColumnType("int");

                            b1.Property<string>("Value")
                                .HasColumnType("nvarchar(max)")
                                .HasColumnName("Name");

                            b1.HasKey("RoleId");

                            b1.ToTable("Role", "PersonnelManagement");

                            b1.WithOwner()
                                .HasForeignKey("RoleId");
                        });

                    b.Navigation("Name")
                        .IsRequired();

                    b.Navigation("ReportsTo");
                });

            modelBuilder.Entity("HRManagement.Modules.Staff.Models.Employee", b =>
                {
                    b.Navigation("ManagedEmployees");
                });
#pragma warning restore 612, 618
        }
    }
}
