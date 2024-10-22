import { Component, OnInit, ElementRef, Input } from '@angular/core';
import { ROUTES } from '../sidebar/sidebar.component';
import { Location, LocationStrategy, PathLocationStrategy } from '@angular/common';
import { Router } from '@angular/router';
import { AuthenticationService } from 'src/app/core/services/authentication.service';
import { NavBarService } from './services/navbar.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.scss']
})
export class NavbarComponent implements OnInit {
  public focus: any;
  public listTitles: any[] = [];
  public location: Location;
  currentUserInfo: any;
  @Input() currentUser: any;
  subscriptions: Subscription[] = [];
  image: any;
  constructor(location: Location, private auth: AuthenticationService, private navbarService: NavBarService, private element: ElementRef, private router: Router) {
    this.location = location;
    this.currentUserInfo = JSON.parse(localStorage.getItem('currentUser') || '{}');
  }

  ngOnInit() {
    this.listTitles = ROUTES.filter((listTitle: any) => listTitle);
    console.log(this.currentUser);
    this.getUsersDetails();

  }
  getUsersDetails() {
    this.subscriptions.push(
      this.navbarService
        .getUserDetails(this.currentUserInfo.userId).subscribe(
          {
            next: (res) => {
              this.image = 'data:image/png;base64,' + res.userslist.photo;
            },
            error: () => {

            }
          },
        )
    );
  }
  getTitle() {
    var titlee = this.location.prepareExternalUrl(this.location.path());
    if (titlee.charAt(0) === '#') {
      titlee = titlee.slice(1);
    }

    for (var item = 0; item < this.listTitles.length; item++) {
      if (this.listTitles[item].path === titlee) {
        return this.listTitles[item].title;
      }
    }
    return 'Dashboard';
  }
  logout() {
    this.auth.logout();
  }
}
