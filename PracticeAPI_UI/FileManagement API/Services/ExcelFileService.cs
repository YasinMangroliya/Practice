using ClosedXML.Excel;
using Infrastructure;

namespace Services
{
    public class ExcelFileService : IExcelFileService
    {
        public MemoryStream ExportToExcel<T>(List<T> data)
        {
            var fileStream = new MemoryStream();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Sheet1");

                //// Write data to the worksheet
                //for (int row = 1; row <= data.Count; row++)
                //{
                //    var rowData = data[row - 1];

                //    worksheet.Cell(row, 1).Value = rowData.UserName;
                //    worksheet.Cell(row, 2).Value = rowData.Password;
                //    // Add more properties as needed
                //}
                // Add header row with column names
                int headerRow = 1;
                int column = 1;
                var properties = typeof(T).GetProperties();
                foreach (var property in properties)
                {
                    var headerCell = worksheet.Cell(headerRow, column);
                    headerCell.Value = property.Name;

                    // Apply header cell style
                    var headerStyle = headerCell.Style;
                    headerStyle.Fill.SetBackgroundColor(XLColor.FromHtml("#007bff"));
                    headerStyle.Font.Bold = true;
                    headerStyle.Font.FontColor = XLColor.White;
                    headerStyle.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                    headerStyle.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                    column++;
                }

                // Write data to the worksheet
                int row = headerRow + 1;
                foreach (var item in data)
                {
                    column = 1;
                    foreach (var property in properties)
                    {
                        var value = property.GetValue(item);
                        //// Handle null values here
                        //var cellValue = value != null ? value.ToString() : string.Empty;
                        //worksheet.Cell(row, column).Value = cellValue;

                        // Handle null values here
                        if (value != null)
                        {
                            var cell = worksheet.Cell(row, column);

                            // Determine the data type and set it accordingly
                            if (IsNumericType(property.PropertyType))
                            {
                                cell.Value = Convert.ToDouble(value);
                            }
                            else
                            {
                                cell.Value = value.ToString();
                            }
                            cell.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
                            cell.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;
                        }

                        column++;
                    }
                    row++;
                }

                // Auto-fit columns after inserting data
                worksheet.Columns().AdjustToContents();

                workbook.SaveAs(fileStream);
            }

            // Set the position to the beginning of the stream before returning
            fileStream.Seek(0, SeekOrigin.Begin);
            return fileStream;
        }

        private bool IsNumericType(Type type)
        {
            return type == typeof(int) || type == typeof(double) || type == typeof(float) || type == typeof(decimal) || type == typeof(long) || type == typeof(Int64);
        }


       

    }
}