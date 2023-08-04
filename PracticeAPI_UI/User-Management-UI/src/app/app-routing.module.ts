import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard, AuthorizeGuard } from './Core/auth.guard';
import { RouteGuard } from './Core/route.guard';
import { RoleEnum } from './Model/CommonModel';
import { HomeComponent } from './Views/home/home.component';
import { LoginComponent } from './Views/login/login.component';
import { NotificationComponent } from './Views/notification/notification.component';
import { RegisterComponent } from './Views/register/register.component';
import { UserListComponent } from './Views/user-list/user-list.component';

const routes: Routes = [{ path: '', component: LoginComponent, pathMatch: 'full' },
{ path: 'Login', component: LoginComponent, pathMatch: 'full' },
{ path: 'Register', component: RegisterComponent, pathMatch: 'full', canDeactivate: [RouteGuard], },
{ path: 'Register/:userId', component: RegisterComponent, pathMatch: 'full', canDeactivate: [RouteGuard], },
{
  path: 'Home',
  component: HomeComponent,
  //canActivate: [AuthGuard],
  canActivateChild: [AuthorizeGuard],
  children: [
    {
      path: 'UserList',
      component: UserListComponent,
      data: {
        role: [RoleEnum.Admin],
      }
    },
    {
      path: 'Notification',
      component: NotificationComponent,
      data: {
        role: [RoleEnum.Customer],
      }
    },
  ]

}];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
