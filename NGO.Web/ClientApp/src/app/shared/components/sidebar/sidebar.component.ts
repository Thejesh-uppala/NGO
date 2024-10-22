import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';

declare interface RouteInfo {
  path: string;
  title: string;
  icon: string;
  class: string;
}
export const ROUTES: RouteInfo[] = [
  { path: '/home', title: 'Home', icon: 'ni-tv-2 text-primary', class: '' },
  { path: '/user-profile', title: 'User profile', icon: 'ni-single-02 text-yellow', class: '' },
  //{ path: '/register', title: 'Register', icon: 'ni-circle-08 text-pink', class: '' },
 // { path: '/contact-us', title: 'Contact Us', icon: 'ni-key-25 text-info', class: '' },
  //{ path: '/admin', title: 'Admin Page', icon: 'ni-key-25 text-info', class: '' },
  //{ path: '/forgotPassword', title: 'forgotPassword', icon: 'ni-key-25 text-info', class: '' },
  //{ path: '/changePassword', title: 'changePassword', icon: 'ni-key-25 text-info', class: '' },
];

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss']
})
export class SidebarComponent implements OnInit {

  public menuItems: any[] = [];
  public isCollapsed = true;
  currentUser: any;
  sideMenuShow: boolean=false;
  showLogin:boolean=true;

  constructor(private router: Router) {
     this.currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}'); 
     this.sideMenuShow=this.currentUser.userRole=="Administrator"?false:true;
    }

  ngOnInit() {
    this.menuItems = ROUTES.filter(menuItem => menuItem);
    this.router.events.subscribe((event) => {
      this.isCollapsed = true;
    });
  }
}
