using System;
using FaturamentoService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FaturamentoService.Infrastructure.Configuration;

public sealed class NotaFiscalConfiguration : IEntityTypeConfiguration<NotaFiscal>
{
    public void Configure(EntityTypeBuilder<NotaFiscal> builder)
    {
        builder.ToTable("NotasFiscais");

        builder.HasKey(n => n.Id);

        builder.Property(n => n.Numero)
            .ValueGeneratedOnAdd();
        
        builder.Property(n => n.Status)
            .IsRequired();
        
        builder.HasMany(n => n.Itens)
            .WithOne()
            .HasForeignKey("NotaFiscalId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
        
        var navigation = builder.Metadata.FindNavigation(nameof (NotaFiscal.Itens));

        if(navigation is not null)
            navigation.SetPropertyAccessMode(PropertyAccessMode.Field);
    }
}
