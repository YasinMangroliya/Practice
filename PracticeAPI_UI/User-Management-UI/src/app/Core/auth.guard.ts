import { inject, Injectable } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivate, CanActivateChild, CanActivateChildFn, CanActivateFn, Router, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { RoleEnum } from '../Model/CommonModel';
import { SessionEnum } from '../Model/EndpointEnum';
import { CommonService } from '../Services/common.service';


export const AuthGuard: CanActivateFn = (route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean => {
  const commonService = inject(CommonService);
  const router = inject(Router);
  let token = sessionStorage.getItem(SessionEnum.Token)
  if (token != null) {
    return true;
  }
  else {
    commonService.previousPath = state.url;
    router.navigate([""]);
    return false;
  }
};

export const AuthorizeGuard: CanActivateChildFn = (route: ActivatedRouteSnapshot,
  state: RouterStateSnapshot): Observable<boolean> | Promise<boolean> | boolean => {
  const commonService = inject(CommonService);
  const router = inject(Router);
  let token = sessionStorage.getItem(SessionEnum.Token)

  let role = sessionStorage.getItem(SessionEnum.Role)
  let alloRoute = (route.data.role && route.data.role.indexOf(role) != -1) ? true : false;

  if (token == null) {
    commonService.previousPath = alloRoute ? state.url : "Home";
    router.navigate([""]);
    return false;
  }

  if (alloRoute) {
    return true;
  }
  else {
    commonService.showError(`${role} Not Allow to navigate this page`, "Access Denied")
    return false;
  }
};
