using HRManagement.Modules.Staff.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Modules.Staff.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Role", "PersonnelManagement").HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn().HasColumnName("Id");
        builder.OwnsOne(x => x.Name, x => { x.Property(xx => xx.Value).HasColumnName("Name"); })
            .Navigation(x => x.Name)
            .IsRequired();
        builder.HasOne(x => x.ReportsTo).WithMany();
        builder.Property<byte?>("ReportsToId").HasColumnName("ReportsToId");
    }
}