export class ModalParams<T>{
  constructor(type = "", heading = "", message = "", id = 0, content = null) {
    this.Type = type
    this.Heading = heading
    this.Message = message
    this.Id = id;
    this.Content = content;
  }

  public Type: string;
  public Heading: string;
  public Message: string;
  public Id: number;
  public Content: T;
}


export enum ModalTypeEnum {
  Delete = "Delete",
  Register = "Register",
}
export enum RoleEnum {
  Admin = "Admin",
  Customer = "Customer",
  User = "User",
  Restricted = "Restricted"
}
export enum SortingClassEnum {
  DefaultSort = "fas fa-sort",
  SortUp = "fas fa-sort-up text-danger",
  SortDown = "fas fa-sort-down text-danger"
}
export enum LocationEnum {
  Country = "Country",
  State = "State",
  City = "City",
}
