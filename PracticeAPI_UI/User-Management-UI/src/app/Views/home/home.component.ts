import { Component, OnInit, Renderer2, ViewEncapsulation } from '@angular/core';
import { Router } from '@angular/router';
import { SessionEnum } from '../../Model/EndpointEnum';
import { CommonService } from '../../Services/common.service';
import { UserService } from '../../Services/user.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css'],
})
export class HomeComponent implements OnInit {
  userName = "";
  constructor(public userService: UserService, private commonService: CommonService, private router: Router, private renderer: Renderer2) { }
  ngOnInit() {
    this.userName = sessionStorage.getItem(SessionEnum.UserName)
    this.renderer.addClass(document.body, 'sidebar-mini');
    this.renderer.addClass(document.body, 'layout-fixed');
    this.renderer.addClass(document.body, 'sidebar-collapse');

    this.userService.logoutSubject.subscribe(val => {
      if (val)
        this.onLogout()
    })
  }
  navigateToOther() {
    let rToken = sessionStorage.getItem(SessionEnum.RefreshToken)
    let uId = sessionStorage.getItem(SessionEnum.UserId)

    if (uId != null && rToken != null)
      window.location.href = 'http://localhost:4100/' + uId + '/' + rToken;
    else
      alert("Token Not Found")
  }
  onLogout() {
    let userId: number = +sessionStorage.getItem(SessionEnum.UserId)
    this.userService.logoutUser(userId, response => {
      console.log(response);
      if (response > 0) {
        sessionStorage.clear()
        this.router.navigate([''])
      }
      else
        this.commonService.showError("Failed to logged out user","Internal Server Error")
    })
  }

  onMenuClick() {
    //this.renderer.addClass(document.body, 'sidebar - mini layout - fixed');
  }
}
