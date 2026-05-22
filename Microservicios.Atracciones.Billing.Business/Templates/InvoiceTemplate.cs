using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Microservicios.Atracciones.Billing.DataAccess.Entities;

namespace Microservicios.Atracciones.Billing.Business.Templates;

public class InvoiceTemplate : IDocument
{
    private readonly Invoice _invoice;

    public InvoiceTemplate(Invoice invoice)
    {
        _invoice = invoice;
    }

    public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

    public void Compose(IDocumentContainer container)
    {
        container.Page(page =>
        {
            page.Margin(50);
            page.Size(PageSizes.A4);
            page.PageColor(Colors.White);
            page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Verdana));

            page.Header().Element(ComposeHeader);
            page.Content().Element(ComposeContent);
            page.Footer().AlignCenter().Text(x =>
            {
                x.Span("Página ");
                x.CurrentPageNumber();
            });
        });
    }

    private void ComposeHeader(IContainer container)
    {
        var titleStyle = TextStyle.Default.FontSize(20).SemiBold().FontColor("#1E88E5");

        container.Row(row =>
        {
            row.RelativeItem().Column(col =>
            {
                col.Item().Text("SistemaAtracciones").Style(titleStyle);
                col.Item().Text("Tu aventura comienza aquí").FontSize(9).Italic().FontColor(Colors.Grey.Medium);
            });

            row.RelativeItem().Column(col =>
            {
                col.Item().AlignRight().Text($"FACTURA: {_invoice.InvoiceNumber}").FontSize(14).SemiBold();
                col.Item().AlignRight().Text($"Fecha: {_invoice.CreatedAt:dd/MM/yyyy HH:mm}");
            });
        });
    }

    private void ComposeContent(IContainer container)
    {
        container.PaddingVertical(40).Column(column =>
        {
            column.Spacing(20);

            // Información del Cliente
            column.Item().Row(row =>
            {
                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("CLIENTE").SemiBold().FontColor("#0D47A1");
                    c.Item().Text(_invoice.CustomerName);
                    c.Item().Text($"RUC/CI: {_invoice.TaxId}");
                });

                row.RelativeItem().Column(c =>
                {
                    c.Item().Text("DIRECCIÓN").SemiBold().FontColor("#0D47A1");
                    c.Item().Text(_invoice.Address ?? "S/N");
                    if (!string.IsNullOrEmpty(_invoice.Email))
                        c.Item().Text(_invoice.Email);
                });
            });

            // Tabla de Detalles
            column.Item().Element(ComposeTable);

            // Totales
            column.Item().AlignRight().Column(c =>
            {
                c.Spacing(5);
                c.Item().Text($"Subtotal: {_invoice.Subtotal:C}");
                c.Item().Text($"IVA (15%): {_invoice.TaxAmount:C}");
                c.Item().Text($"TOTAL: {_invoice.Total:C}").FontSize(14).SemiBold().FontColor("#1E88E5");
            });
        });
    }

    private void ComposeTable(IContainer container)
    {
        container.Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn(3);
                columns.ConstantColumn(50);
                columns.ConstantColumn(80);
                columns.ConstantColumn(80);
            });

            table.Header(header =>
            {
                header.Cell().Element(CellStyle).Text("Descripción");
                header.Cell().Element(CellStyle).AlignCenter().Text("Cant.");
                header.Cell().Element(CellStyle).AlignRight().Text("P. Unit");
                header.Cell().Element(CellStyle).AlignRight().Text("Total");

                static IContainer CellStyle(IContainer container)
                {
                    return container.DefaultTextStyle(x => x.SemiBold()).PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Black);
                }
            });

            foreach (var item in _invoice.Details)
            {
                table.Cell().Element(CellStyle).Text(item.Description);
                table.Cell().Element(CellStyle).AlignCenter().Text(item.Quantity.ToString());
                table.Cell().Element(CellStyle).AlignRight().Text(item.UnitPrice.ToString("C"));
                table.Cell().Element(CellStyle).AlignRight().Text(item.TotalItem.ToString("C"));

                static IContainer CellStyle(IContainer container)
                {
                    return container.PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten2);
                }
            }
        });
    }
}
