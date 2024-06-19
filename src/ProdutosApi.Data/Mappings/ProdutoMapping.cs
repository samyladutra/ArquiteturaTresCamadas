using ProdutosApi.Business.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ProdutosApi.Data.Mappings;
public class ProdutoMapping : IEntityTypeConfiguration<Produto>
{
    public void Configure(EntityTypeBuilder<Produto> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Nome)
            .IsRequired()
            .HasColumnType("varchar(200)");

        builder.Property(p => p.Descricao)
            .IsRequired()
            .HasColumnType("varchar(1000)");

        builder.ToTable("Produtos");
    }
}