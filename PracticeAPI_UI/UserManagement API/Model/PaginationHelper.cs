using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Model
{
    public class PaginationHelper<T>
    {
        public static async Task<PaginationModel<T>> CreateList(IQueryable<T> source, int currentPage,
         int pageSize)
        {
            PaginationModel<T> paginationModel = new PaginationModel<T>();
            paginationModel.TotalCount = await source.CountAsync();
            paginationModel.DataList = await source.Skip((currentPage - 1) * pageSize).Take(pageSize).ToListAsync();
            paginationModel.CurrentPage = currentPage;
            paginationModel.TotalPages = (int)Math.Ceiling(paginationModel.TotalCount / (double)pageSize);
            paginationModel.PageSize = pageSize;
            return paginationModel;
        }
    }
    public class PaginationModel<T>
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public List<T>? DataList { get; set; }
    }

    public class PaginationParams
    {
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    public class DataTableParams
    {

        public int CurrentPage { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public string? sortColumn { get; set; }
        public string? sortColumnDir { get; set; }=OrderByEnum.Ascending;
        public string? searchIn { get; set; }
        public string? searchValue { get; set; }

    }

    public class OrderByEnum
    {
        public const string Ascending = "ascending";
        public const string Descending = "descending";
    }
}
