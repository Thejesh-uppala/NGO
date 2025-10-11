import { Location } from '@angular/common';
import { Component, OnInit, ElementRef, Input } from '@angular/core';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable, Subscription } from 'rxjs';
import { AuthenticationService } from 'src/app/core/services/authentication.service';
import { UserState } from 'src/app/store/user/user.model';
import { CommonService } from '../services/common.service';
import { NavBarService } from './services/navbar.service';

@Component({
  selector: 'app-navbar',
  templateUrl: './navbar.component.html',
  styleUrls: ['./navbar.component.css'],
})
export class NavbarComponent implements OnInit {
  mobileMenuOpen: boolean = false;
  menuActive: boolean = false;

  public focus: any;
  public listTitles: any[] = [];

  @Input() currentUser: any;
  subscriptions: Subscription[] = [];
  userData!: { userId: number; orgId: number; organizations: any[]; roles: any[] };
  user$: Observable<UserState> | undefined;
  image: any;
  orgLogo: string = '';
  userId!: number;
  orgId!: number;
  orgName: string = '';
  organizations!: any[];
  isAdmin!: boolean;

  constructor(
    location: Location,
    private commonService: CommonService,
    private store: Store<UserState>,
    private auth: AuthenticationService,
    private navbarService: NavBarService,
    private element: ElementRef,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.commonService.getUserData().subscribe({
      next: userData => {
        this.userData = userData;
        this.userId = userData.userId;
        this.orgId = userData.orgId;
        this.organizations = userData.organizations;
      },
      error: () => {},
    });

    if (this.userId && this.organizations.length > 0) {
      this.orgLogo = this.organizations[0].BrandLogo;

      this.orgName = this.organizations[0].OrgName;
      this.isAdmin = this.userData.roles.some(role => role.RoleName === 'Admin');
    }
  }

  // Toggle mobile menu visibility

  toggleMobileMenu() {
    this.mobileMenuOpen = !this.mobileMenuOpen;
  }

  toggleMenu() {
    if (this.menuActive) {
      this.menuActive = !this.menuActive;
    } else {
      this.menuActive = !this.menuActive;
    }
  }

  logout() {
    this.auth.logout();
  }
}
