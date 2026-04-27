namespace iWallet.Infrastructure.Implementation
{
    public class TransferReceiptDocument : IDocument
    {
        private readonly TransferReceiptDto _data;
        private readonly string _logoPath;

        // define colors
        private readonly string PrimaryColor = "#1A1A1A"; 
        private readonly string AccentColor = "#2563EB";  
        private readonly string LightGray = "#F9FAFB";  
        private readonly string BorderColor = "#E5E7EB";  

        public TransferReceiptDocument(TransferReceiptDto data, string path)
        {
            _data = data;
            _logoPath = path;
        }

        public DocumentMetadata GetMetaData() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(s => s.FontSize(10));

         
                page.Header().Row(row =>
                {
                    row.RelativeItem().Column(col =>
                    {
                        col.Item().Text("TRANSFERS RECEIPT")
                            .FontSize(22).SemiBold().FontColor(AccentColor);

                        col.Item().Text($"Ref: {_data.TransactionReference}")
                            .FontSize(10).FontColor(Colors.Grey.Medium);
                    });

                    row.ConstantItem(100).AlignCenter().Column(col =>
                    {
               
                        col.Item().Height(100).Width(100).Image(_logoPath);
                    });
                });
 
                page.Content().PaddingVertical(20).Column(col =>
                {
          
                    col.Item().Background(LightGray).Padding(20).Row(row =>
                    {
                        row.RelativeItem().Column(c =>
                        {
                            c.Item().Text("Total Amount").FontSize(10).FontColor(Colors.Grey.Medium);
                            c.Item().Text($"{_data.Amount:N2} {_data.Currency}")
                                .FontSize(24).ExtraBold().FontColor(PrimaryColor);
                        });

                        row.RelativeItem().AlignRight().Column(c =>
                        {
                            c.Item().Text("Status").FontSize(10).FontColor(Colors.Grey.Medium);
                            c.Item().PaddingTop(4).Text(_data.Status.ToUpper())
                                .FontSize(12).SemiBold()
                                .FontColor(_data.Status.ToLower() == "completed" ? Colors.Green.Medium : AccentColor);
                        });
                    });

                    col.Item().PaddingTop(30).Text("Transaction Details").FontSize(14).SemiBold();
                    col.Item().PaddingVertical(5).LineHorizontal(1.5f).LineColor(BorderColor);

             
                    col.Item().PaddingTop(10).Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(40);
                            columns.RelativeColumn();
                        });

               
                        table.Cell().Column(c => {
                            c.Item().Text("SENDER").FontSize(8).SemiBold().FontColor(Colors.Grey.Medium);
                            c.Item().PaddingTop(5).Text(_data.SenderName).FontSize(12).SemiBold();
                            c.Item().Text(_data.SenderWalletNumber).FontColor(Colors.Grey.Darken2);
                        });

                        table.Cell().AlignCenter().AlignMiddle().Text("→").FontSize(20).FontColor(BorderColor);

                        table.Cell().AlignRight().Column(c => {
                            c.Item().Text("RECEIVER").FontSize(8).SemiBold().FontColor(Colors.Grey.Medium);
                            c.Item().PaddingTop(5).Text(_data.ReceiverName).FontSize(12).SemiBold();
                            c.Item().Text(_data.ReceiveWalletNumber).FontColor(Colors.Grey.Darken2);
                        });
                    });

           
                    col.Item().PaddingTop(40).Column(c =>
                    {
                        c.Item().Text("Payment Summary").FontSize(14).SemiBold();
                        c.Item().PaddingVertical(10).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            void AddRow(string label, string value, bool isBold = false)
                            {
                                table.Cell().BorderBottom(1).BorderColor(LightGray).PaddingVertical(8).Text(label).FontColor(Colors.Grey.Medium);
                                var cell = table.Cell().BorderBottom(1).BorderColor(LightGray).PaddingVertical(8).AlignRight().Text(value);
                                if (isBold) cell.Bold();
                            }

                            AddRow("Transfer Date", _data.Date.ToString("MMM dd, yyyy - HH:mm"));
                            AddRow("Subtotal", $"{_data.Amount:N2} {_data.Currency}");
                            AddRow("Transaction Fees", $"+ {_data.Fees:N2} {_data.Currency}");
                            AddRow("Net Amount", $"{(_data.Amount - _data.Fees):N2} {_data.Currency}", true);
                        });
                    });

               
                    col.Item().PaddingTop(50).Background(LightGray).Padding(10).AlignCenter().Text(t =>
                    {
                        t.Span("This is a computer-generated document. No signature required. Verified by ").FontSize(9).FontColor(Colors.Grey.Medium);
                        t.Span("iWallet Secure Protocol").FontSize(9).SemiBold().FontColor(AccentColor);
                    });
                });

       
                page.Footer().Row(row =>
                {
                    row.RelativeItem().Text(x =>
                    {
                        x.Span("Page ").FontSize(9);
                        x.CurrentPageNumber().FontSize(9);
                    });

                    row.RelativeItem().AlignRight().Text("iwallet.io | Support: support@iwallet.io").FontSize(9).FontColor(Colors.Grey.Medium);
                });
            });
        }
    }
}