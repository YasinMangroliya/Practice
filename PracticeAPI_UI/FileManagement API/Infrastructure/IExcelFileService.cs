using Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public interface IExcelFileService
    {
        MemoryStream ExportToExcel<T>(List<T> data);
    }
}
