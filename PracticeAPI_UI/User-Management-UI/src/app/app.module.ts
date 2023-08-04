import { DatePipe } from '@angular/common';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { BrowserModule } from '@angular/platform-browser';
import { NgHttpLoaderModule } from 'ng-http-loader';
import { ToastrModule } from 'ngx-toastr';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HomeComponent } from './Views/home/home.component';
import { LoginComponent } from './Views/login/login.component';
import { RegisterComponent } from './Views/register/register.component';
import { UserListComponent } from './Views/user-list/user-list.component';
import { ValidationSummaryComponent } from './Views/Shared/validation-summary/validation-summary.component';
import { CommonModalComponent } from './Views/Shared/common-modal/common-modal.component';
import { PaginationModule } from 'ngx-bootstrap/pagination';
import { ModalModule } from 'ngx-bootstrap/modal';
import { TimeagoModule } from 'ngx-timeago';
import { NotificationComponent } from './Views/notification/notification.component';
import { NgxSelectModule } from 'ngx-select-ex';
import { HttpCustomInterceptor } from './Core/http.interceptor';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    HomeComponent,
    RegisterComponent,
    UserListComponent,
    ValidationSummaryComponent,
    CommonModalComponent,
    NotificationComponent
  ],
  imports: [
    BrowserModule,
    HttpClientModule,
    AppRoutingModule,
    ReactiveFormsModule,
    FormsModule,
    NgHttpLoaderModule.forRoot(),
    BrowserAnimationsModule, // required animations module
    PaginationModule.forRoot(),
    ToastrModule.forRoot(),
    TimeagoModule.forRoot(),
    ModalModule.forRoot(),
    BsDropdownModule.forRoot(),
    NgxSelectModule
  ],
  providers: [
    DatePipe,
    {
      provide: HTTP_INTERCEPTORS, useClass: HttpCustomInterceptor, multi: true
    },
  ],

  bootstrap: [AppComponent],
})
export class AppModule { }
