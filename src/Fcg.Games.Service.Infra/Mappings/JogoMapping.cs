using Fcg.Games.Service.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Fcg.Games.Service.Infra.Mappings;
public class JogoMapping : IEntityTypeConfiguration<JogoEntity>
{
    public void Configure(EntityTypeBuilder<JogoEntity> builder)
    {
        builder.ToTable("Jogos");

        builder.HasKey(j => j.Id);

        builder.Property(j => j.Nome)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(j => j.Descricao)
            .HasMaxLength(500);

        builder.Property(j => j.Preco)
            .IsRequired()
            .HasColumnType("decimal(10,2)");

        builder.Property(j => j.Ativo)
            .IsRequired();

        builder.HasMany(j => j.Promocoes)
            .WithOne(p => p.Jogo)
            .HasForeignKey(p => p.JogoId);
    }
}
