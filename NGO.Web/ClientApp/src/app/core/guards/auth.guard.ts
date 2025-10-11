import { Injectable } from '@angular/core';
import { Router, CanActivate } from '@angular/router';
import { CommonService } from 'src/app/shared/components/services/common.service';

@Injectable({ providedIn: 'root' })
export class AuthGuard implements CanActivate {
  orgId!: number;
  userData!: { userId: number; orgId: number; organizations: any[]; roles: any[] };
  userId!: number;
  constructor(
    private commonService: CommonService,
    private router: Router
  ) {}
  canActivate() {
    this.commonService.getUserData().subscribe({
      next: userData => {
        this.userData = userData;
        this.userId = userData.userId;
        this.orgId = userData.orgId;
      },
      error: () => {},
    });
    if (this.userData && this.userId != null) {
      // logged in so return true
      return true;
    }

    this.orgId = parseInt(localStorage.getItem('orgId') || '0', 10);
    this.router.navigate([`/login/${this.orgId}`]);
    return false;
  }
}
