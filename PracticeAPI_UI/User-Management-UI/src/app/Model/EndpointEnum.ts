export enum RepositoryEnum {
  UserManagement = "UserManagement",
  FileManagement = "FileManagement",
}
export enum EndpointEnum {
  GetUserList = "User/GetUserList",
  SaveUserDetails = "User/SaveUserDetails",
  GetUserById = "User/GetUserById",
  DeleteUserById = "User/DeleteUserById",

  AuthenticateUser = "Home/AuthenticateUser",
  RefreshToken = "Home/RefreshToken",
  Logout = "Home/Logout",
  Login = "Home/Login",

  CountryList = "User/CountryList",
  StateByCountry = "User/StateByCountry",
  CityByState = "User/CityByState",
  CheckUserNameIsUnique = "User/CheckUserNameIsUnique",


  excelExportUsers ="Excel/ExportUsers",
  pdfExportUsers ="Pdf/ExportUsers"
}


export enum SessionEnum {
  Token = "token",
  RefreshToken = "refreshToken",
  UserId = "userId",
  UserName = "userName",
  Role ="role",
}
