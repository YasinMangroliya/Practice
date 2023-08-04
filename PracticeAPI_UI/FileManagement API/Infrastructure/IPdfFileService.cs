using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Data;
using System.Reflection.Metadata;

namespace Infrastructure
{
    public interface IPdfFileService
    {
        byte[] ExportToPdf(DataTable data);
    }
}
