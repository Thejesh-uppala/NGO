import { Component, ElementRef, ViewChild } from '@angular/core';
import { AuthenticationService } from './core/services/authentication.service';
import { AuthModel } from './login/models/login.model';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent {
  title = 'NGO';
  currentUser!: AuthModel;
  showSideBar:boolean=false;
  /**
   *
   */
  /**
   *
   */
 
  constructor(private autheticationService: AuthenticationService) {
    this.autheticationService.currentUser.subscribe(x => {
      this.currentUser = x;
    })
  }
}
