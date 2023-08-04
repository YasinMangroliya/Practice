import { Component, OnDestroy } from '@angular/core';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnDestroy {
  constructor() { }
  ngOnDestroy(): void {
    sessionStorage.clear();
  }
  title = 'User-Management-UI';
}
