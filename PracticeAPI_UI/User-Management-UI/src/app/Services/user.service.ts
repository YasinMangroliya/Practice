import { HttpErrorResponse } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, catchError, map, Observable, Subject, take } from 'rxjs';
import { CoredataService } from '../Core/coredata.service';
import { AuthenticateResponse } from '../Model/BaseResponseModel';
import { DataTableParams } from '../Model/DataTableModel';
import { EndpointEnum, RepositoryEnum, SessionEnum } from '../Model/EndpointEnum';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  constructor(public coreDataService: CoredataService) {
  }

  public logoutSubject = new Subject<boolean>();
  public registerSubject = new Subject<boolean>();

  authenticateUser(objModel, success) {
    this.coreDataService.Get(RepositoryEnum.UserManagement, EndpointEnum.AuthenticateUser, objModel,
      (response) => {
        if (response != null) {
          return success(response);
        }
        return null;
      });
  }
  refreshToken(){
    let authModel: AuthenticateResponse = new AuthenticateResponse();
    authModel.userId = +sessionStorage.getItem(SessionEnum.UserId);
    authModel.userName = sessionStorage.getItem(SessionEnum.UserName);
    authModel.role = sessionStorage.getItem(SessionEnum.Role);
    authModel.token = sessionStorage.getItem(SessionEnum.Token);
    authModel.refreshToken = sessionStorage.getItem(SessionEnum.RefreshToken);

    return this.coreDataService.GetWithObservable(RepositoryEnum.UserManagement, EndpointEnum.RefreshToken, authModel)
      
  }

  logoutUser(userId, success) {
    this.coreDataService.Get(RepositoryEnum.UserManagement, EndpointEnum.Logout + '/' + userId, null,
      (response) => {
        if (response != null) {
          return success(response);
        }
        return null;
      });
  }
  setLoginSession(authResponse: AuthenticateResponse) {
    sessionStorage.setItem(SessionEnum.Token, authResponse.token)
    sessionStorage.setItem(SessionEnum.Role, authResponse.role)
    sessionStorage.setItem(SessionEnum.RefreshToken, authResponse.refreshToken)
    sessionStorage.setItem(SessionEnum.UserId, authResponse.userId.toString())
    sessionStorage.setItem(SessionEnum.UserName, authResponse.userName.toString())
  }
   getUserList(datatableParams: DataTableParams, fromDate: string, toDate: string, locationIds: string, locationBy: string, success) {

    //here null values are considerd as a "null" string so due to this facing issues on backend api, so we can avoid this as follows
    let params = null;
    params = JSON.parse(JSON.stringify(datatableParams));// to avoid two way binding
    if (fromDate) params['fromDate'] = fromDate;
    if (toDate) params['toDate'] = toDate;
    if (locationIds) params['locationIds'] = locationIds;
    if (locationBy) params['locationBy'] = locationBy;

    this.coreDataService.Get(RepositoryEnum.UserManagement, EndpointEnum.GetUserList,
      params,
      (response) => {
        if (response != null) {
          return success(response);
        }
        return null;
      });
  }

  async getUserById(userId: number) {
    return await this.coreDataService.GetWithPromise(RepositoryEnum.UserManagement, EndpointEnum.GetUserById + '/' + userId, null)
  }

  deleteUserById(userId: number, success) {
    this.coreDataService.Get(RepositoryEnum.UserManagement, EndpointEnum.DeleteUserById + '/' + userId, null,
      (response) => {
        if (response != null) {
          return success(response);
        }
        return null;
      });
  }



  getCountry(success) {
    this.coreDataService.Get(RepositoryEnum.UserManagement, EndpointEnum.CountryList, null,
      (response) => {
        if (response != null) {
          return success(response);
        }
        return null;
      });
  }

  async getStateByCountryId(countryId: number) {
    return await this.coreDataService.GetWithPromise(RepositoryEnum.UserManagement, EndpointEnum.StateByCountry + '/' + countryId, null)
  }
  async getCityByStateId(stateId: number) {
    return await this.coreDataService.GetWithPromise(RepositoryEnum.UserManagement, EndpointEnum.CityByState + '/' + stateId, null)
  }

  checkUniqueUserName(userId: number, username: string, success) {
    this.coreDataService.GetWithbackend(RepositoryEnum.UserManagement, EndpointEnum.CheckUserNameIsUnique + '/' + userId + '/' + username, null,
      (response) => {
        if (response != null) {
          return success(response);
        }
        return null;
      });
  }


  saveUser(formdata: FormData, success) {
    this.coreDataService.postUpload(RepositoryEnum.UserManagement, EndpointEnum.SaveUserDetails, formdata,
      (response) => {
        if (response != null) {
          return success(response);
        }
        return null;
      });
  }

  exportAllUserToExcel() {
    return this.coreDataService.GetBlob(RepositoryEnum.FileManagement, EndpointEnum.excelExportUsers, null)
  }
  exportAllUserToPdf() {
    return this.coreDataService.GetBlob(RepositoryEnum.FileManagement, EndpointEnum.pdfExportUsers, null)
  }

}
