import { Injectable } from '@angular/core';
import { HttpBackend, HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { environment } from '../../environments/environment';
import { RepositoryEnum } from '../Model/EndpointEnum';
import { delay, firstValueFrom, lastValueFrom, map, Observable } from 'rxjs';
@Injectable({
  providedIn: 'root'
})

export class CoredataService {
  public result: any;
  public headers: HttpHeaders;
  public baseUrl = "";
  public userManagementBaseUrl = environment.UserManagementUrl;
  public fileManagementBaseUrl = environment.FileManagement;
  constructor(private httpClient: HttpClient, private httpBackend: HttpBackend) {

  }

  getFullURL(repository, api) {
    switch (repository) {
      case RepositoryEnum.UserManagement:
        this.baseUrl = this.userManagementBaseUrl;
        break;
      case RepositoryEnum.FileManagement:
        this.baseUrl = this.fileManagementBaseUrl;
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
      'Access-Control-Allow-Methods': 'GET,PUT,POST,DELETE'
      //'Authorization': `Bearer ${sessionStorage.getItem("token")}`
    });
    return headerss;
  }
  //for file upload
  private getAcceptHeader(): HttpHeaders {
    const header = new HttpHeaders({
      'Authorization': `Bearer ${sessionStorage.getItem("token")}`,
      'Accept': '*/*'
    })
    return header;
  }

  Get(repository, path, objModel, success,) {
    let fullURL = this.getFullURL(repository, path);
    let queryParams = new HttpParams({ fromObject: objModel })
    const httpOptions = { headers: this.getHeader(), params: queryParams };
    this.httpClient
      .get(fullURL, httpOptions)
      .subscribe({
        next: (response) => { return success(response) },
        error: (err: any) => {
          console.error("Error:", err)
          if (err?.error?.message) {
            alert(err?.error?.message)
          }
          else {
            alert("Internal Server Error")
          }
        },
        //complete: () => { console.log('completed block'); }
      })
  }
  GetWithbackend(repository, path, objModel, success,) {
    let fullURL = this.getFullURL(repository, path);
    let queryParams = new HttpParams({ fromObject: objModel })
    const httpOptions = { headers: this.getHeader(), params: queryParams };
    let httpClient = new HttpClient(this.httpBackend)
    httpClient
      .get(fullURL, httpOptions)
      .subscribe({
        next: (response) => { return success(response) },
        error: (err: any) => {
          console.error("Error:", err)
          alert("Internal Server Error...")
        },
        //complete: () => { console.log('completed block'); }
      })
  }
  async GetWithPromise(repository, path, objModel) {
    let fullURL = this.getFullURL(repository, path);
    let queryParams = new HttpParams({ fromObject: objModel })
    const httpOptions = { headers: this.getHeader(), params: queryParams };

    return await firstValueFrom(this.httpClient
      .get(fullURL, httpOptions))  //alternative of promose
  }
  GetWithObservable<T>(repository, path, objModel): Observable<T> {
    let fullURL = this.getFullURL(repository, path);
    let queryParams = new HttpParams({ fromObject: objModel })
    const httpOptions = { headers: this.getHeader(), params: queryParams };
    return this.httpClient
      .get<T>(fullURL, httpOptions)
  }
  GetBlob(repository, path, objModel) {
    let fullURL = this.getFullURL(repository, path);
    return this.httpClient
      .get(fullURL,  { responseType: 'blob' })
  }

  Post(repository, path, objModel, success) {
    let fullURL = this.getFullURL(repository, path);
    const httpOptions = { headers: this.getHeader() };
    this.httpClient
      .post(fullURL, JSON.stringify(objModel), httpOptions)
      .subscribe({
        next: (response) => { return success(response) },
        error: (err: any) => {
          console.error("Error:", err)
          if (err?.error?.message) {
            alert(err?.error?.message)
          }
          else {
            alert("Internal Server Error")
          }
        },
        //complete: () => { console.log('completed block'); }
      })
  }
  postUpload(repository, path, objModel, success) {
    let fullURL = this.getFullURL(repository, path);
    const httpOptions = { headers: this.getAcceptHeader() };
    this.httpClient
      .post(fullURL, objModel, httpOptions)
      .subscribe({
        next: (response) => { return success(response) },
        error: (err: any) => {
          console.error("Error:", err)
        },
        //complete: () => { console.log('completed block'); }
      })
  }
  Put(repository, path, objModel, success) {
    let fullURL = this.getFullURL(repository, path);
    const httpOptions = { headers: this.getHeader() };
    this.httpClient
      .put(fullURL, httpOptions)
      .subscribe({
        next: (response) => { return success(response) },
        error: (err: any) => {
          console.error("Error:", err)
        },
        //complete: () => { console.log('completed block'); }
      })
  }
  Delete(repository, path, objModel, success) {
    let fullURL = this.getFullURL(repository, path);
    const httpOptions = { headers: this.getHeader() };
    this.httpClient
      .delete(fullURL, httpOptions)
      .subscribe({
        next: (response) => { return success(response) },
        error: (err: any) => {
          console.error("Error:", err)
        },
        //complete: () => { console.log('completed block'); }
      })
  }

  
}
