import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { Table } from 'primeng/table';
import { Subscription } from 'rxjs';
import { MatrimonyService } from 'src/app/matrimony/services/matrimony.service';
import { CommonService } from 'src/app/shared/components/services/common.service';

@Component({
  selector: 'app-admin-matrimony',
  templateUrl: './admin-matrimony.component.html',
  styleUrls: ['./admin-matrimony.component.css'],
})
export class AdminMatrimonyComponent implements OnInit {
  matrimonialuserslist: any[] = []; // Replace `customers` with `matrimonialuserslist`
  selectedUsers: any[] = [];
  userIds: number[] = [];
  loading: boolean = true;
  @ViewChild('dt') dt: Table | undefined;
  searchValue: string = '';
  representatives: any[] = [];
  statuses: any[] = [];
  activityValues: number[] = [0, 100];
  selectedRepresentatives: any[] = []; // This will store selected representatives

  matrimonialUsersList: any[] = [];
  currentUser: any;
  orgId!: number;
  userId!: number;
  userData!: { userId: number; orgId: number; organizations: any[]; roles: any[] };
  defaultStatus = 'Approved';

  subscriptions: Subscription[] = [];
  filteredMatrimonialUsersList: any[] = [];

  constructor(
    private router: Router,
    private commonService: CommonService,
    private matrimonyService: MatrimonyService
  ) {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
  }

  ngOnInit() {
    this.commonService.getUserData().subscribe({
      next: userData => {
        this.userData = userData;
        this.userId = userData.userId;
        this.orgId = userData.orgId;
      },
      error: () => {},
    });

    if (this.userData && this.userId) {
      this.GetAllMatrimonialUsers(this.orgId);
    }
    const isAdmin = this.userData.roles.some(role => role.RoleName === 'Admin');

    if (!isAdmin) {
      this.router.navigate(['/home']);
    }

    this.loading = false;
  }
  ngAfterViewInit() {
    this.applyDefaultFilter();
  }

  approveProfile(userId: number): void {
    if (!userId || isNaN(userId)) {
      return;
    }

    // Extract the user IDs
    this.userIds = this.extractUserIds();

    // Check if the userId is already included
    if (!this.userIds.includes(userId)) {
      this.userIds.push(userId);
    }

    // Call the service with orgId as a query string
    this.matrimonyService.approveProfiles(this.orgId, this.userIds, this.userId).subscribe({
      next: () => {
        this.userIds.forEach(userId => {
          this.commonService.showSuccess('Profile Approved Successfully');
          const updatedUser = this.filteredMatrimonialUsersList.find(user => user.id === userId);
          if (updatedUser) {
            updatedUser.status = 'Approved'; // Update the status directly
          }
        });
      },
      error: () => {
        // Implement retry or alert mechanisms if required
      },
    });
  }

  rejectProfile(userId: number): void {
    if (!userId || isNaN(userId)) {
      return;
    }

    // Extract the user IDs
    this.userIds = this.extractUserIds();

    // Check if the userId is already included
    if (!this.userIds.includes(userId)) {
      this.userIds.push(userId);
    }

    // Call the service
    this.matrimonyService.rejectProfiles(this.orgId, this.userIds, this.userId).subscribe({
      next: () => {
        this.userIds.forEach(userId => {
          this.commonService.showSuccess('Profile Rejected Successfully');
          const updatedUser = this.filteredMatrimonialUsersList.find(user => user.id === userId);
          if (updatedUser) {
            updatedUser.status = 'Rejected'; // Update the status directly
          }
        });
      },
      error: () => {},
    });
  }

  clear(table: any) {
    this.searchValue = '';
    table.clear();
  }

  extractUserIds(): number[] {
    var Ids = this.selectedUsers.map(user => user.id);
    return Ids;
  }
  viewProfile(id: number, orgId: number): void {
    // Navigate to the details page and pass id and orgId as query params
    this.router.navigate(['/matrimony/matrimony-profile-details'], { queryParams: { id, orgId } });
  }

  applyDefaultFilter() {
    if (this.dt) {
      this.dt.filter(this.defaultStatus, 'status', 'equals');
    }
  }

  filterGlobal(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    const value = inputElement?.value || '';
    if (this.dt) {
      this.dt.filterGlobal(value, 'contains');
    }
  }

  GetAllMatrimonialUsers(orgId: number) {
    this.subscriptions.push(
      this.matrimonyService.getAllMatrimonialUsers(orgId).subscribe({
        next: res => {
          this.matrimonialUsersList = res.matrimonialuserslist || [];
          this.filteredMatrimonialUsersList = this.matrimonialUsersList;
          this.loading = false;
        },

        error: () => {},
      })
    );
  }
  getSeverity(status: string) {
    switch (status) {
      case 'Approved':
        return 'success';
      case 'Pending':
        return 'warning';
      case 'Rejected':
        return 'danger';
      default:
        return 'info';
    }
  }
}
