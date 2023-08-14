import { Component, OnDestroy, OnInit, Renderer2, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { AuthenticateResponse} from '../../Model/BaseResponseModel';
import { SessionEnum } from '../../Model/EndpointEnum';
import { LoginModel } from '../../Model/UserDetailsModel';
import { CommonService } from '../../Services/common.service';
import { UserService } from '../../Services/user.service';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css'],
  encapsulation: ViewEncapsulation.None
})
export class LoginComponent implements OnInit, OnDestroy {
  userName: string = "";
  password: string = "";

  constructor(public userService: UserService, private commonService: CommonService, private router: Router, private renderer: Renderer2) { }

  ngOnInit() {
    sessionStorage.clear();
    this.renderer.addClass(document.body, 'loginBody');
  }

  login() {
    let loginModel = new LoginModel();
    loginModel.UserName = this.userName;
    loginModel.Password = this.password;
    this.userService.authenticateUser(loginModel, (response: AuthenticateResponse) => {
      //console.log(JSON.parse(atob(response.token.split('.')[1])));
      this.userService.setLoginSession(response)

      if (response.userId > 0) {
        this.renderer.removeClass(document.body, 'loginBody');
        if (this.commonService.previousPath != null)
          this.router.navigate([this.commonService.previousPath]);
        else
          this.router.navigate(['/Home'])
      }
    });


  }

  ngOnDestroy(): void {

  }
}
