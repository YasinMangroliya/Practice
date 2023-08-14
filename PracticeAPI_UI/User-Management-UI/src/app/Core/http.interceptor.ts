import { Injectable } from '@angular/core';
import {
  HttpInterceptor,
  HttpHandler,
  HttpRequest,
  HttpErrorResponse,
} from '@angular/common/http';
import { catchError, switchMap } from 'rxjs/operators';
import { throwError, Observable, BehaviorSubject } from 'rxjs';
import { UserService } from '../Services/user.service';
import { SessionEnum } from '../Model/EndpointEnum';

@Injectable()
export class HttpCustomInterceptor implements HttpInterceptor {
  private refreshTokenSubject: BehaviorSubject<any> = new BehaviorSubject<any>(null);

  constructor(private userService: UserService) { }

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<any> {
    const authReq = this.addAccessToken(req);

    return next.handle(authReq).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401 && sessionStorage.getItem(SessionEnum.Token) /*&& this.isRefreshing*/) {
          return this.userService.refreshToken().pipe(
            switchMap((res: any) => {
              this.userService.setLoginSession(res);
              this.refreshTokenSubject.next(res);
              return next.handle(this.addAccessToken(req));  //switch map used to return inner observable
            }),
            catchError((refreshError: any) => {
              if (!this.userService.logoutSubject.closed) {
                this.userService.logoutSubject.next(true)
                this.userService.logoutSubject.unsubscribe();
              }
              return throwError(() => refreshError);
            }),
          )
        }
        else {
          if (!this.userService.logoutSubject.closed) {
            //this.userService.logoutSubject.next(false)
            this.userService.logoutSubject.unsubscribe();
          }
          return throwError(() => error);
        }
      }),
    );
  }

  private addAccessToken(req: HttpRequest<any>): HttpRequest<any> {
    const accessToken = sessionStorage.getItem(SessionEnum.Token);
    if (accessToken) {
      return req.clone({
        setHeaders: {
          Authorization: `Bearer ${accessToken}`,
        },
      });
    }
    return req;
  }
}
