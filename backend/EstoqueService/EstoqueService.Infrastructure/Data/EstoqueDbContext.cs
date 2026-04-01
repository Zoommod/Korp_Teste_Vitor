using System;
using EstoqueService.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace EstoqueService.Infrastructure.Data;

public sealed class EstoqueDbContext : DbContext
{
    public EstoqueDbContext(DbContextOptions<EstoqueDbContext> options) : base(options) { }


    public DbSet<Produto> Produtos => Set<Produto>();


    protected override void OnModelCreating(ModelBuilder modelBuilder)

    {

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(EstoqueDbContext).Assembly);

    }
}
