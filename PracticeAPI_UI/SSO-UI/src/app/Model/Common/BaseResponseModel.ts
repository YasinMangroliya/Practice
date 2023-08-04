export class BaseResponseModel {
  Id: number;
  StatusCode: number;
  StatusMessage: string;
  ResponseContent: any;
}
export class ErrorDetailsModel {
  Exception: any;
  ExceptionType: string;
  Message: string;
  StatusCode: number;
  StatusMessage: string;
  EndPoint: string;
  Path: string;
}
export class AuthenticateResponse {
  userId: number;
  userName: string;
  token: string;
  refreshToken: string;
}
