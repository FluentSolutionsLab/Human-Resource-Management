using HRManagement.Modules.Personnel.Domain.Employee;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace HRManagement.Modules.Personnel.Persistence.Configurations;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.ToTable("Employee").HasKey(x => x.Id);
        builder.Property(x => x.Id);
        builder.OwnsOne(x => x.Name, x =>
        {
            x.Property(xx => xx.FirstName).HasColumnName("FirstName");
            x.Property(xx => xx.LastName).HasColumnName("LastName");
        }).Navigation(x => x.Name).IsRequired();
        builder.OwnsOne(x => x.EmailAddress, x => { x.Property(xx => xx.Email).HasColumnName("Email"); })
            .Navigation(x => x.EmailAddress).IsRequired();
        builder.OwnsOne(x => x.DateOfBirth,
            x =>
            {
                x.Property(xx => xx.Date).HasColumnName("DateOfBirth")
                    .HasConversion<DateOnlyConverter, DateOnlyComparer>();
            }).Navigation(x => x.DateOfBirth).IsRequired();
        builder.Property(x => x.HireDate).HasColumnName("HireDate").HasConversion<DateOnlyConverter, DateOnlyComparer>()
            .IsRequired();
        builder.Property(x => x.TerminationDate).HasColumnName("TerminationDate")
            .HasConversion<DateOnlyConverter, DateOnlyComparer>();
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