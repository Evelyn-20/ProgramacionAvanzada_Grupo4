using Pasteleria.Abstracciones.ModeloUI;
using Pasteleria.Controllers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using System.Collections.Generic;

namespace Pasteleria.Reportes.PDF
{
    public class ReportePedidosPDF : IDocument
    {
        private readonly List<ReportePedidoDetalle> _data;
        private readonly string _fechaInicio;
        private readonly string _fechaFin;

        public ReportePedidosPDF(
            List<ReportePedidoDetalle> data,
            string fechaInicio,
            string fechaFin)
        {
            _data = data;
            _fechaInicio = fechaInicio;
            _fechaFin = fechaFin;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(20);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                // HEADER
                page.Header().Column(col =>
                {
                    // Título principal
                    col.Item().AlignCenter().Text("REPORTE DE PEDIDOS")
                        .FontSize(20)
                        .Bold()
                        .FontColor("#e67e73");

                    col.Item().PaddingVertical(5);

                    // Fecha de generación
                    col.Item().AlignCenter().Text($"Generado: {DateTime.Now:dd/MM/yyyy HH:mm:ss}")
                        .FontSize(10)
                        .FontColor("#666666");

                    col.Item().PaddingVertical(5);

                    // Línea separadora
                    col.Item().LineHorizontal(2).LineColor("#e67e73");
                });

                // CONTENT
                page.Content().Column(col =>
                {
                    col.Spacing(10);

                    // Información del período
                    col.Item().Background("#f9f9f9")
                        .Border(1).BorderColor("#ddd")
                        .BorderLeft(4).BorderColor("#e67e73")
                        .Padding(12)
                        .Column(infoCol =>
                        {
                            infoCol.Item().Row(row =>
                            {
                                row.RelativeItem().Text(txt =>
                                {
                                    txt.Span("Período: ").Bold().FontColor("#555555");
                                    txt.Span($"{_fechaInicio} - {_fechaFin}").FontColor("#333333");
                                });
                            });
                        });

                    col.Item().PaddingVertical(5);

                    // Resumen estadístico
                    if (_data != null && _data.Count > 0)
                    {
                        var totalPedidos = _data.Count;
                        var totalGeneral = _data.Sum(r => r.Pedido.Total);
                        var totalProductos = _data.Sum(r => r.Detalles.Sum(d => d.Cantidad));

                        col.Item().Row(row =>
                        {
                            row.Spacing(10);

                            // Card 1: Total Pedidos
                            row.RelativeItem().Background("#e67e73")
                                .Padding(15)
                                .Column(cardCol =>
                                {
                                    cardCol.Item().AlignCenter().Text(totalPedidos.ToString())
                                        .FontSize(22)
                                        .Bold()
                                        .FontColor("#ffffff");

                                    cardCol.Item().AlignCenter().Text("TOTAL PEDIDOS")
                                        .FontSize(8)
                                        .FontColor("#ffffff");
                                });

                            // Card 2: Total Facturado
                            row.RelativeItem().Background("#5d3a2a")
                                .Padding(15)
                                .Column(cardCol =>
                                {
                                    cardCol.Item().AlignCenter().Text($"₡{totalGeneral:N2}")
                                        .FontSize(18)
                                        .Bold()
                                        .FontColor("#ffffff");

                                    cardCol.Item().AlignCenter().Text("TOTAL FACTURADO")
                                        .FontSize(8)
                                        .FontColor("#ffffff");
                                });

                            // Card 3: Productos Vendidos
                            row.RelativeItem().Background("#f5e6d3")
                                .Padding(15)
                                .Column(cardCol =>
                                {
                                    cardCol.Item().AlignCenter().Text(totalProductos.ToString())
                                        .FontSize(22)
                                        .Bold()
                                        .FontColor("#5d3a2a");

                                    cardCol.Item().AlignCenter().Text("PRODUCTOS VENDIDOS")
                                        .FontSize(8)
                                        .FontColor("#5d3a2a");
                                });
                        });

                        col.Item().PaddingVertical(10);
                    }

                    // Listado de pedidos
                    if (_data != null && _data.Count > 0)
                    {
                        foreach (var item in _data)
                        {
                            col.Item().Border(1).BorderColor("#ddd").Column(pedidoCol =>
                            {
                                // HEADER DEL PEDIDO
                                pedidoCol.Item().Background("#f5e6d3")
                                    .BorderBottom(2).BorderColor("#e67e73")
                                    .Padding(12)
                                    .Row(headerRow =>
                                    {
                                        // Información izquierda
                                        headerRow.RelativeItem().Column(leftCol =>
                                        {
                                            leftCol.Item().Text($"Pedido #{item.Pedido.IdPedido}")
                                                .FontSize(12)
                                                .Bold()
                                                .FontColor("#333333");

                                            leftCol.Item().PaddingTop(5).Row(metaRow =>
                                            {
                                                metaRow.AutoItem().Text($"Cliente: {item.Pedido.NombreCliente}")
                                                    .FontSize(9)
                                                    .FontColor("#666666");

                                                metaRow.AutoItem().PaddingLeft(10).Text($"Fecha: {item.Pedido.Fecha:dd/MM/yyyy}")
                                                    .FontSize(9)
                                                    .FontColor("#666666");
                                            });

                                            if (!string.IsNullOrWhiteSpace(item.Pedido.NombreUsuario))
                                            {
                                                leftCol.Item().PaddingTop(3).Text($"Procesado por: {item.Pedido.NombreUsuario}")
                                                    .FontSize(9)
                                                    .FontColor("#666666");
                                            }

                                            // Badge de estado
                                            leftCol.Item().PaddingTop(5).Row(badgeRow =>
                                            {
                                                var estadoColor = ObtenerColorEstado(item.Pedido.Estado);

                                                badgeRow.AutoItem()
                                                    .Background(estadoColor)
                                                    .PaddingVertical(3)
                                                    .PaddingHorizontal(8)
                                                    .Text(item.Pedido.Estado)
                                                    .FontSize(8)
                                                    .Bold()
                                                    .FontColor("#ffffff");
                                            });
                                        });

                                        // Total derecha
                                        headerRow.AutoItem().AlignRight().Column(rightCol =>
                                        {
                                            rightCol.Item().Text("Total")
                                                .FontSize(8)
                                                .FontColor("#666666");

                                            rightCol.Item().Text($"₡{item.Pedido.Total:N2}")
                                                .FontSize(16)
                                                .Bold()
                                                .FontColor("#e67e73");
                                        });
                                    });

                                // BODY DEL PEDIDO
                                pedidoCol.Item().Padding(12).Column(bodyCol =>
                                {
                                    bodyCol.Item().Text("Productos del Pedido")
                                        .FontSize(10)
                                        .Bold()
                                        .FontColor("#333333");

                                    bodyCol.Item().PaddingVertical(5);

                                    // Tabla de productos
                                    bodyCol.Item().Table(table =>
                                    {
                                        table.ColumnsDefinition(columns =>
                                        {
                                            columns.RelativeColumn(3); // Producto
                                            columns.RelativeColumn(1); // Cantidad
                                            columns.RelativeColumn(1.5f); // Precio Unit
                                            columns.RelativeColumn(1.5f); // Descuento
                                            columns.RelativeColumn(1.5f); // Subtotal
                                        });

                                        // Header de tabla
                                        table.Header(header =>
                                        {
                                            header.Cell().Background("#f5f5f5")
                                                .Padding(5)
                                                .Text("Producto")
                                                .FontSize(9)
                                                .Bold();

                                            header.Cell().Background("#f5f5f5")
                                                .Padding(5)
                                                .AlignCenter()
                                                .Text("Cant.")
                                                .FontSize(9)
                                                .Bold();

                                            header.Cell().Background("#f5f5f5")
                                                .Padding(5)
                                                .AlignRight()
                                                .Text("Precio Unit.")
                                                .FontSize(9)
                                                .Bold();

                                            header.Cell().Background("#f5f5f5")
                                                .Padding(5)
                                                .AlignRight()
                                                .Text("Descuento")
                                                .FontSize(9)
                                                .Bold();

                                            header.Cell().Background("#f5f5f5")
                                                .Padding(5)
                                                .AlignRight()
                                                .Text("Subtotal")
                                                .FontSize(9)
                                                .Bold();
                                        });

                                        // Filas de productos
                                        foreach (var detalle in item.Detalles)
                                        {
                                            table.Cell().BorderBottom(1).BorderColor("#eeeeee")
                                                .Padding(5)
                                                .Text(detalle.NombreProducto)
                                                .FontSize(9);

                                            table.Cell().BorderBottom(1).BorderColor("#eeeeee")
                                                .Padding(5)
                                                .AlignCenter()
                                                .Text(detalle.Cantidad.ToString())
                                                .FontSize(9);

                                            table.Cell().BorderBottom(1).BorderColor("#eeeeee")
                                                .Padding(5)
                                                .AlignRight()
                                                .Text($"₡{detalle.Precio:N2}")
                                                .FontSize(9);

                                            table.Cell().BorderBottom(1).BorderColor("#eeeeee")
                                                .Padding(5)
                                                .AlignRight()
                                                .Text(detalle.Descuento > 0 ? $"-₡{detalle.Descuento:N2}" : "-")
                                                .FontSize(9)
                                                .FontColor("#27ae60");

                                            table.Cell().BorderBottom(1).BorderColor("#eeeeee")
                                                .Padding(5)
                                                .AlignRight()
                                                .Text($"₡{detalle.Subtotal:N2}")
                                                .FontSize(9)
                                                .Bold();
                                        }
                                    });

                                    // Resumen de totales
                                    bodyCol.Item().PaddingTop(10).AlignRight().Width(250).Column(totalesCol =>
                                    {
                                        totalesCol.Item().Background("#f9f9f9")
                                            .Border(1).BorderColor("#ddd")
                                            .Padding(10)
                                            .Column(resumenCol =>
                                            {
                                                resumenCol.Item().Row(row =>
                                                {
                                                    row.RelativeItem().Text("Subtotal:")
                                                        .FontSize(9);
                                                    row.AutoItem().Text($"₡{item.Pedido.Subtotal:N2}")
                                                        .FontSize(9)
                                                        .Bold();
                                                });

                                                if (item.Pedido.Descuento.HasValue && item.Pedido.Descuento > 0)
                                                {
                                                    resumenCol.Item().PaddingTop(3).Row(row =>
                                                    {
                                                        row.RelativeItem().Text("Descuento:")
                                                            .FontSize(9);
                                                        row.AutoItem().Text($"-₡{item.Pedido.Descuento:N2}")
                                                            .FontSize(9)
                                                            .Bold()
                                                            .FontColor("#27ae60");
                                                    });
                                                }

                                                if (item.Pedido.Impuesto.HasValue && item.Pedido.Impuesto > 0)
                                                {
                                                    resumenCol.Item().PaddingTop(3).Row(row =>
                                                    {
                                                        row.RelativeItem().Text("Impuesto (13%):")
                                                            .FontSize(9);
                                                        row.AutoItem().Text($"₡{item.Pedido.Impuesto:N2}")
                                                            .FontSize(9)
                                                            .Bold();
                                                    });
                                                }

                                                resumenCol.Item().PaddingTop(5)
                                                    .BorderTop(2).BorderColor("#e67e73")
                                                    .PaddingTop(5)
                                                    .Row(row =>
                                                    {
                                                        row.RelativeItem().Text("TOTAL:")
                                                            .FontSize(10)
                                                            .Bold();
                                                        row.AutoItem().Text($"₡{item.Pedido.Total:N2}")
                                                            .FontSize(11)
                                                            .Bold()
                                                            .FontColor("#e67e73");
                                                    });
                                            });
                                    });
                                });
                            });
                        }
                    }
                    else
                    {
                        col.Item().AlignCenter().Padding(40).Text("No se encontraron pedidos que coincidan con los filtros seleccionados.")
                            .FontSize(11)
                            .FontColor("#666666");
                    }
                });

                // FOOTER
                page.Footer().Column(footer =>
                {
                    footer.Item().LineHorizontal(1).LineColor("#ddd");
                    footer.Item().PaddingTop(10).AlignCenter().Column(footerCol =>
                    {
                        footerCol.Item().Text("Pastelería - Sistema de Gestión de Pedidos")
                            .FontSize(9)
                            .Bold()
                            .FontColor("#666666");

                        footerCol.Item().Text("Documento generado automáticamente - Confidencial")
                            .FontSize(8)
                            .FontColor("#999999");
                    });
                });
            });
        }

        private string ObtenerColorEstado(string estado)
        {
            return estado switch
            {
                "Pendiente" => "#f39c12",
                "En Proceso" => "#3498db",
                "Completado" => "#27ae60",
                "Cancelado" => "#e74c3c",
                "Entregado" => "#16a085",
                _ => "#95a5a6"
            };
        }
    }
}