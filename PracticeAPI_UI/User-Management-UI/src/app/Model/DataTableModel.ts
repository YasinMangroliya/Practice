import { environment } from "../../environments/environment";

export class DataTableParams {
  CurrentPage: number = 1;
  PageSize: number = environment.DataTablePageSize;
  SortColumn: string = "";
  SortColumnDir: string = OrderByEnum.Ascending;
  SearchIn: string = "";
  SearchValue: string = "";

}

export class PaginationParams {
  currentPage: number;
  pageSize: number;
  totalCount: number;
  totalPages: number;
}

export enum OrderByEnum {
  Ascending = "ascending",
  Descending = "descending"
}
export enum SortingEnum {
  Sorting = "sorting",
  Asc = "sorting_asc",
  Desc = "sorting_desc"
}
