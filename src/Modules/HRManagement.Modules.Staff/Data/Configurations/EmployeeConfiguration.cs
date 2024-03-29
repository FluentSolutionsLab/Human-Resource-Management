﻿using HRManagement.Modules.Staff.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HRManagement.Modules.Staff.Data.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employee", "PersonnelManagement").HasKey(x => x.Id);
        builder.Property(x => x.Id);
        builder.OwnsOne(x => x.Name, x =>
        {
            x.Property(xx => xx.FirstName).HasColumnName("FirstName");
            x.Property(xx => xx.LastName).HasColumnName("LastName");
        }).Navigation(x => x.Name).IsRequired();
        builder
            .OwnsOne(x => x.EmailAddress, x => { x.Property(xx => xx.Email).HasColumnName("Email"); })
            .Navigation(x => x.EmailAddress)
            .IsRequired();
        builder
            .OwnsOne(x => x.BirthDate,
                x =>
                {
                    x.Property(xx => xx.Date).HasColumnName("DateOfBirth")
                        .HasConversion<DateOnlyConverter, DateOnlyComparer>().HasColumnType("date");
                })
            .Navigation(x => x.BirthDate)
            .IsRequired();
        builder
            .OwnsOne(x => x.HireDate,
                x =>
                {
                    x.Property(xx => xx.Date).HasColumnName("HireDate")
                        .HasConversion<DateOnlyConverter, DateOnlyComparer>().HasColumnType("date");
                })
            .Navigation(x => x.HireDate)
            .IsRequired();
        builder
            .OwnsOne(x => x.TerminationDate,
                x =>
                {
                    x.Property(xx => xx.Date).HasColumnName("TerminationDate")
                        .HasConversion<DateOnlyConverter, DateOnlyComparer>().HasColumnType("date");
                })
            .Navigation(x => x.TerminationDate);
        builder.HasOne(x => x.Role).WithMany().IsRequired();
        builder.Property<int>("RoleId").HasColumnName("RoleId").IsRequired();
        builder.HasOne(x => x.Manager).WithMany();
        builder.Property<Guid?>("ManagerId").HasColumnName("ManagerId");
        builder.HasMany(x => x.ManagedEmployees).WithOne(x => x.Manager);
    }
}

public class DateOnlyConverter : ValueConverter<DateOnly, DateTime>
{
    public DateOnlyConverter() : base(
        dateOnly => dateOnly.ToDateTime(TimeOnly.MinValue),
        dateTime => DateOnly.FromDateTime(dateTime))
    {
    }
}

public class DateOnlyComparer : ValueComparer<DateOnly>
{
    public DateOnlyComparer() : base(
        (d1, d2) => d1.DayNumber == d2.DayNumber,
        d => d.GetHashCode())
    {
    }
}