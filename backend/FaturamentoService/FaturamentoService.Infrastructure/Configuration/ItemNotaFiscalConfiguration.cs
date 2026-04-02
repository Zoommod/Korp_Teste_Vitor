using System;
using FaturamentoService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaturamentoService.Infrastructure.Configuration;

public sealed class ItemNotaFiscalConfiguration : IEntityTypeConfiguration<ItemNotaFiscal>
{
    public void Configure(EntityTypeBuilder<ItemNotaFiscal> builder)
    {
        builder.ToTable("ItensNotaFiscal");

        builder.HasKey(i => i.Id);

        builder.Property(i => i.CodigoProduto)
            .IsRequired()
            .HasMaxLength(50);
        
        builder.Property(i => i.Quantidade)
            .IsRequired();
    }
}
