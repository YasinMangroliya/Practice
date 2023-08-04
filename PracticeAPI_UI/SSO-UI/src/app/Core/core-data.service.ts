import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../Environment/environment';
import { RepositoryEnum } from '../Model/Common/EndpointEnum';

@Injectable({
  providedIn: 'root'
})
export class CoreDataService {

  public headers: HttpHeaders;
  public baseUrl = "";
  public userManagementBaseUrl = environment.UserManagementUrl;
  constructor(private http: HttpClient, private httpService: HttpClient) {

  }

  getFullURL(repository, api) {
    switch (repository) {
      case RepositoryEnum.UserManagement:
        this.baseUrl = this.userManagementBaseUrl;
        break;
    }
    return this.baseUrl + api;
  }

  private getHeader(): HttpHeaders {
    const headerss = new HttpHeaders({
      'Content-Type': 'application/json',
      'Access-Control-Allow-Origin': '*',
      'Access-Control-Allow-Credentials': 'true',
      'Access-Control-Allow-Headers': 'Content-Type,Authorization',
      'Access-Control-Allow-Methods': 'GET,PUT,POST,DELETE',
      //'Authorization': `Bearer ${sessionStorage.getItem("token")}`
    });

    return headerss;
  }

  Get(repository, path, success) {
    let fullURL = this.getFullURL(repository, path);
    const httpOptions = { headers: this.getHeader() };
    this.httpService
      .get(fullURL, httpOptions)
      .subscribe({
        next: (response) => { return success(response) },
        error: (err: any) => {
          console.error("Error:", err)
        },
        complete: () => { console.log('completed block'); }
      })
  }
  Post<T>(repository, path, objModel:T, success) {
    let fullURL = this.getFullURL(repository, path);
    const httpOptions = { headers: this.getHeader() };
    this.httpService
      .post(fullURL, JSON.stringify(objModel), httpOptions)
      .subscribe({
        next: (response) => { return success(response) },
        error: (err: any) => {
          console.error("Error:", err)
        },
        complete: () => { console.log('completed block'); }
      })
  }
  Put(repository, path, objModel, success) {
    let fullURL = this.getFullURL(repository, path);
    const httpOptions = { headers: this.getHeader() };
    this.httpService
      .put(fullURL, JSON.stringify(objModel), httpOptions)
      .subscribe({
        next: (response) => { return success(response) },
        error: (err: any) => {
          console.error("Error:", err)
        },
        complete: () => { console.log('completed block'); }
      })
  }
  Delete(repository, path, success) {
    let fullURL = this.getFullURL(repository, path);
    const httpOptions = { headers: this.getHeader() };
    this.httpService
      .delete(fullURL, httpOptions)
      .subscribe({
        next: (response) => { return success(response) },
        error: (err: any) => {
          console.error("Error:", err)
        },
        complete: () => { console.log('completed block'); }
      })
  }
}
