import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { CommonService } from '../services/common.service';
@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
})
export class SidebarComponent implements OnInit {
  subscriptions: Subscription[] = [];
  orgId: any;
  userId: any;
  userData!: { userId: number; orgId: number; organizations: any[]; roles: any[] };
  firstName: any;
  lastName: any;
  profileImage: any;
  yearOfBirth!: number;
  phoneNo: any;
  emailAddress: any;
  isSidebarHidden = false;
  constructor(
    private router: Router,
    private commonService: CommonService
  ) {}

  ngOnInit() {
    this.commonService.getUserData().subscribe({
      next: userData => {
        this.userData = userData;
        this.userId = userData.userId;
        this.orgId = userData.orgId;

        this.getUserCustomData(this.orgId, this.userId);
      },
      error: () => {},
    });
  }
  collapse() {
    this.isSidebarHidden = !this.isSidebarHidden;
  }

  getUserCustomData(orgId: any, userId: any) {
    this.subscriptions.push(
      this.commonService.getCurrentUserCustomData(orgId, userId).subscribe({
        next: res => {
          this.firstName = res.userData.firstName;
          this.lastName = res.userData.lastName;
          this.profileImage = res.userData.photo;
          this.yearOfBirth = res.userData.yearOfBirth;
          this.phoneNo = res.userData.ehoneNumber;
          this.emailAddress = res.userData.emailAddress;
        },
        error: () => {},
      })
    );
  }
}
