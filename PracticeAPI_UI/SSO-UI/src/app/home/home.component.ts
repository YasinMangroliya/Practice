import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { AuthenticateResponse } from '../Model/Common/BaseResponseModel';
import { LoginModel } from '../Model/LoginModel';
import { UserService } from '../Service/user.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  constructor(public userService: UserService, private route: ActivatedRoute) { }
  ngOnInit() {
    let userId = this.route.snapshot.paramMap.get('uId');
    let refreshToken = this.route.snapshot.paramMap.get('rToken');
    let ssoRequestModel = { uId: userId, rToken: refreshToken }
    console.log(ssoRequestModel);
    if (userId != null && refreshToken != null) {
      this.ssoAuthenticate(ssoRequestModel)
    }
    else if (sessionStorage.getItem('userId') == null) {
      alert("User Not LoggedIn")
    }
  }


  getCall() {
    this.userService.getUser(response => {
      console.log(response);
    })
  }
  postCall() {
    let loginModel = new LoginModel();
    loginModel.UserName = "Test User"
    loginModel.Password = "Test Password"
    this.userService.postUser(loginModel, response => {
      console.log(response);
    })
  }

  putCall() {
    this.userService.putUser(2, response => {
      console.log(response);
    })
  }
  deleteCall() {
    this.userService.deleteUser(2, response => {
      console.log(response);
    })
  }

  ssoAuthenticate(ssoRequestModel: any) {
    this.userService.sSOAuthentication(ssoRequestModel, (response: AuthenticateResponse) => {
      sessionStorage.setItem('token', response.token)
      sessionStorage.setItem('refreshToken', response.refreshToken)
      sessionStorage.setItem('userId', response.userId.toString())
      sessionStorage.setItem('userName', response.userName.toString())
      alert("LoggedIn Succeed With UserId-" + response.userId.toString())
    })
  }
}
