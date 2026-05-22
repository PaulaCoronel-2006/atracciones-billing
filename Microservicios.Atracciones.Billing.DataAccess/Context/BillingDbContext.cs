using Microsoft.EntityFrameworkCore;
using Microservicios.Atracciones.Billing.DataAccess.Common;
using Microservicios.Atracciones.Billing.DataAccess.Entities;
using System.Text.Json;

namespace Microservicios.Atracciones.Billing.DataAccess.Context;

public class BillingDbContext : DbContext
{
    public BillingDbContext(DbContextOptions<BillingDbContext> options)
        : base(options)
    {
    }

    // ══════════════════════════════════════════════════
    // 9. PAGOS Y FACTURACIÓN
    // ══════════════════════════════════════════════════
    public DbSet<PaymentMethodType> PaymentMethodTypes { get; set; }
    public DbSet<PaymentStatusType> PaymentStatusTypes { get; set; }
    public DbSet<Payment> Payments { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<InvoiceDetail> InvoiceDetails { get; set; }

    // ══════════════════════════════════════════════════
    // MODEL CREATING
    // ══════════════════════════════════════════════════
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BillingDbContext).Assembly);

        // Mapeo manual para resolver la inconsistencia de nombres en tu BD
        var tableMapping = new Dictionary<string, string>
        {
            { nameof(Payment), "payment" },
            { nameof(PaymentMethodType), "payment_method_type" },
            { nameof(PaymentStatusType), "payment_status_type" },
            { nameof(Invoice), "invoice" },
            { nameof(InvoiceDetail), "invoice_detail" }
        };

        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var entityName = entity.ClrType.Name;
            if (tableMapping.TryGetValue(entityName, out var tableName))
            {
                entity.SetTableName(tableName);
            }

            // Mantenemos snake_case para las columnas porque esas sí parecen consistentes
            foreach (var property in entity.GetProperties())
            {
                var propName = ToSnakeCase(property.Name);
                property.SetColumnName(propName);
            }
        }
    }

    private string ToSnakeCase(string input)
    {
        if (string.IsNullOrEmpty(input)) return input;
        return System.Text.RegularExpressions.Regex.Replace(input, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
    }

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampAuditFields();
        return await base.SaveChangesAsync(cancellationToken);
    }

    private void StampAuditFields()
    {
        var now = DateTime.UtcNow;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    entry.Entity.UpdatedAt = now;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    break;
            }
        }
    }
}
