using AuthServer.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthServer.Data.Configuraitons;

public class UserRefreshTokenConfiguraiton : IEntityTypeConfiguration<UserRefreshToken>
{
    public void Configure(EntityTypeBuilder<UserRefreshToken> builder)
    {
        builder.HasKey(x => x.UserId);
        builder.Property(x => x.Code).IsRequired().HasMaxLength(200);

    }
}