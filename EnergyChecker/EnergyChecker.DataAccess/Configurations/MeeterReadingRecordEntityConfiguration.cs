using EnergyChecker.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EnergyChecker.DataAccess.Configurations;

public class MeeterReadingRecordEntityConfiguration : IEntityTypeConfiguration<MeeterReadingRecordEntity>
{
    public void Configure(EntityTypeBuilder<MeeterReadingRecordEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedOnAdd();
        builder.HasOne(x => x.Account).WithMany()
            .HasForeignKey(x => x.AccountId);

        // builder.HasIndex(u => new { u.AccountId, u.MeterReadValue, u.MeeterReadingDateTime })
        //     .IsUnique();
        
        builder.HasIndex(x => x.CompositeKeyString).IsUnique();
    }
}