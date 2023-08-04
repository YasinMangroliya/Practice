using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Data;
using Infrastructure;
using iText.Layout.Properties;
using iText.Kernel.Colors;

namespace Services
{
    public class PdfFileService : IPdfFileService
    {

        public byte[] ExportToPdf(DataTable data)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var writer = new PdfWriter(memoryStream))
                {
                    using (var pdf = new PdfDocument(writer))
                    {
                        using (var document = new Document(pdf))
                        {
                            // Calculate the width of each column based on the number of columns
                            float columnWidth = 100f / data.Columns.Count;

                            // Calculate the desired font size adjustment ratio based on the data length and column width
                            float desiredFontSizeRatio = 0.7f;

                            // Calculate the maximum font size based on the data length and column width
                            float maxFontSize = columnWidth * desiredFontSizeRatio;

                            // Calculate the minimum font size for all cells
                            float minFontSize = 6f;

                            // Create a table with the number of columns based on the DataTable
                            var table = new Table(data.Columns.Count)
                                .UseAllAvailableWidth()
                                .SetWidth(UnitValue.CreatePercentValue(100));

                            // Set default font size for the table
                            float defaultFontSize = 8f;

                            int columnsCount= data.Columns.Count;

                            // Add table headers
                            foreach (DataColumn column in data.Columns)
                            {
                                var headerCell = new Cell().Add(new Paragraph(column.ColumnName));
                       
                                // Calculate the font size based on the data length and column width
                                float fontSize = columnsCount<6? defaultFontSize:CalculateFontSize(column.ColumnName.Length, columnWidth, minFontSize, maxFontSize);
                                headerCell.SetFontSize(fontSize);

                                headerCell.SetBackgroundColor(ColorConstants.BLUE);
                                headerCell.SetFontColor(ColorConstants.WHITE);
                                headerCell.SetBold();
                                headerCell.SetTextAlignment(TextAlignment.CENTER);

                                table.AddHeaderCell(headerCell);

                            }

                            // Add data rows to the table
                            foreach (DataRow row in data.Rows)
                            {
                                foreach (DataColumn column in data.Columns)
                                {
                                    var cellValue = row[column].ToString();
                                    var cell = new Cell().Add(new Paragraph(cellValue));

                                    // Calculate the font size based on the data length and column width
                                    float fontSize = columnsCount < 6 ? defaultFontSize : CalculateFontSize(cellValue.Length, columnWidth, minFontSize, maxFontSize);
                                    cell.SetFontSize(fontSize);

                                    cell.SetTextAlignment(TextAlignment.CENTER);
                                    table.AddCell(cell);
                                }
                            }

                            // Add the table to the document
                            document.Add(table);
                        }
                    }
                }

                // Return the PDF content as a byte array
                return memoryStream.ToArray();
            }
        }
        // Helper method to calculate font size based on data length and column width
        private float CalculateFontSize(int dataLength, float columnWidth, float minFontSize, float maxFontSize)
        {
            // Calculate the target font size based on the data length and column width
            float targetFontSize = columnWidth * dataLength;

            // Ensure that the target font size is within the specified range
            targetFontSize = Math.Min(Math.Max(targetFontSize, minFontSize), maxFontSize);

            return targetFontSize;
        }
    }
}
