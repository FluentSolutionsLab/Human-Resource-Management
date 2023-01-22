using HRManagement.Modules.Personnel.Domain.Role;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace HRManagement.Modules.Personnel.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Role").HasKey(x => x.Id);
        builder.Property(x => x.Id).UseIdentityColumn().HasColumnName("Id");
        builder.Property(x => x.Name).HasColumnName("Name");
        builder.HasOne(x => x.ReportsTo).WithMany();
        builder.Property<byte?>("ReportsToId").HasColumnName("ReportsToId");
    }
}