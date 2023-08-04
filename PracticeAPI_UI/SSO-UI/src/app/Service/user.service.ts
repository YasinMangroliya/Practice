import { Injectable } from '@angular/core';
import { CoreDataService } from '../Core/core-data.service';
import { EndpointEnum, RepositoryEnum } from '../Model/Common/EndpointEnum';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor(public coreDataService: CoreDataService) { }

  getUser(success) {
    this.coreDataService.Get(RepositoryEnum.UserManagement, EndpointEnum.GetUsers, 
      (response) => {
        if (response != null) {
          return success(response);
        }
        return null;
      });
  }
  postUser<T>(objModel: T, success) {
    this.coreDataService.Post(RepositoryEnum.UserManagement, EndpointEnum.PostUser, objModel,
      (response) => {
        if (response != null) {
          return success(response);
        }
        return null;
      });
  } 
  putUser(id: number, success) {
    this.coreDataService.Put(RepositoryEnum.UserManagement, EndpointEnum.UpdateUser + "/" + id, null,
      (response) => {
        if (response != null) {
          return success(response);
        }
        return null;
      });
  }
  deleteUser(id: number, success) {
    this.coreDataService.Delete(RepositoryEnum.UserManagement, EndpointEnum.DeleteUser + "/" + id,
      (response) => {
        if (response != null) {
          return success(response);
        }
        return null;
      });
  }

  sSOAuthentication<T>(objModel: T, success) {
    this.coreDataService.Post(RepositoryEnum.UserManagement, EndpointEnum.SSOAuthentication, objModel,
      (response) => {
        if (response != null) {
          return success(response);
        }
        return null;
      });
  }
}
