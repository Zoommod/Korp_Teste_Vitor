using System;
using System.Data.Common;
using FaturamentoService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FaturamentoService.Infrastructure.Data;

public class FaturamentoDbContext : DbContext
{
    public FaturamentoDbContext(DbContextOptions<FaturamentoDbContext> options) : base (options){}

    public DbSet<NotaFiscal> NotaFiscais => Set<NotaFiscal>();
    public DbSet<ItemNotaFiscal> ItensNotaFiscal => Set<ItemNotaFiscal>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(FaturamentoDbContext).Assembly);
    }
}
