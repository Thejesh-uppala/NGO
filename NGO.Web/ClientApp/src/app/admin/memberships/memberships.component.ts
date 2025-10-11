import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Store } from '@ngrx/store';
import { MessageService, ConfirmationService } from 'primeng/api';

//admin.component.ts
import { Dropdown } from 'primeng/dropdown';
import { Table } from 'primeng/table';
import { Subscription } from 'rxjs';
import { Observable } from 'rxjs';
import { UserState } from 'src/app/store/user/user.model';
import * as XLSX from 'xlsx';
import { CommonService } from '../../shared/components/services/common.service';
import { SharedModule } from '../../shared/shared.module';
import { UserDetailModel } from '../../userprofile/models/UpdateUserModel';
import { AdminService } from '../services/admin.service';

export interface User {
  userId?: number;
  firstName?: string;
  lastName?: string;
  emailAddress?: string;
  membershipStatus?: string;
  phoneNumber?: string;
  addInWhatsAppOrTelegram?: string;
  photo?: string;
  yearOfBirth?: number;
  city?: string;
  state?: string;
  country?: string;
  postalCode?: string;
  homeTown?: string;
  spouseDetails?: any;
  childrensDetails?: any[];
  membershipValidityDate?: string;
}

interface DropDownData {
  name: string;
  code: string;
}
interface ChartValue {
  name: string;
  count: number;
}

@Component({
  selector: 'app-memberships',
  standalone: true,
  imports: [SharedModule],
  templateUrl: './memberships.component.html',
  styleUrl: './memberships.component.css',
})
export class MembershipsComponent implements OnInit {
  userDialog: boolean = false;
  users!: any[];
  user!: any[];
  userIds: number[] = [];
  selectedUsers: any[] = [];
  submitted: boolean = false;
  statuses: any[] | undefined;
  @ViewChild('dt') dt: Table | undefined; // Reference to the p-table component
  subscriptions: Subscription[] = [];
  userDetail: UserDetailModel[] = [];
  copyUserDetails!: UserDetailModel[];
  filteredChartDetails: UserDetailModel[] = [];
  filteredExpiryDetails: UserDetailModel[] = [];
  chartBool: boolean = false;
  loading: boolean = true;
  displayModal: boolean = false;
  displayBasic: boolean = false;
  displayBasic2: boolean = false;
  displayMaximizable: boolean = false;
  displayPosition: boolean = false;
  position: string = '';
  memberId: string = '';
  chapterId: string = '';
  customerId: number = 0;
  paymentData: string[] = [];
  data: any;
  userDetail1!: UserDetailModel[];
  chapterData!: DropDownData[];
  selectedChapterData!: DropDownData;
  organizationData!: DropDownData[];
  selectedOrganizationData!: DropDownData;
  @ViewChild('dropDownThing')
  dropDownThing!: Dropdown;
  chartDataSetValue: number[] = [];
  chartLableData: string[] = [];
  multiAxisData: any;
  multiAxisOptions: any;
  representatives: any;
  expiresIn: any;
  newData: ChartValue[] = [];

  user$!: Observable<UserState | null>;
  currentUser: any;
  orgId!: number;
  userId!: number;
  userData!: {
    userId: number;
    orgId: number;
    organizations: any[];
    roles: any[];
  };

  constructor(
    private router: Router,
    private modalService: NgbModal,
    private messageService: MessageService,
    private adminService: AdminService,
    private store: Store<UserState>,
    private commonService: CommonService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit(): void {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
    if (this.currentUser?.userRoles[0]?.roleName !== 'Admin') {
      this.router.navigateByUrl('/home');
    }
    this.commonService.getUserData().subscribe({
      next: userData => {
        this.userData = userData;
        this.userId = userData.userId;
        this.orgId = userData.orgId;
      },
      error: () => {},
    });
    if (this.userData && this.userId) {
      const isAdmin = this.userData.roles.some(role => role.RoleName === 'Admin');

      if (!isAdmin) {
        this.router.navigate(['/home']);
      }
      this.GetAllMembers(this.orgId);
    }
  }

  showInfo() {
    this.commonService.showInfo('Mail sending');
  }

  exportExcel() {
    if (this.dt && this.dt.el) {
      this.commonService.showInfo('Exporting...');
      // Ensure the table is available
      const ws: XLSX.WorkSheet = XLSX.utils.table_to_sheet(this.dt.el.nativeElement);
      const wb: XLSX.WorkBook = XLSX.utils.book_new();
      XLSX.utils.book_append_sheet(wb, ws, 'Users');
      XLSX.writeFile(wb, 'Members.xlsx');
    }
  }

  remindUser() {
    this.confirmationService.confirm({
      message: 'Membership Validity reminder will be sent to selected users',
      header: 'Send',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        if (!this.selectedUsers || this.selectedUsers.length === 0) {
          this.commonService.showInfo('No users selected');
          return;
        }

        const userEmails = this.selectedUsers
          ?.map(user => user.emailAddress)
          .filter((email): email is any => email !== undefined);

        this.adminService.remindUsers(userEmails, this.userId, this.orgId).subscribe(
          () => {
            this.selectedUsers = [];

            this.commonService.showSuccess('Users Reminded successfully');
          },
          () => {
            // Handle errors

            this.commonService.showError();
          }
        );
      },
    });
  }

  GetAllMembers(orgId: number) {
    this.subscriptions.push(
      this.adminService.getAllMembers(orgId).subscribe({
        next: res => {
          this.users = res.userslist;
          this.copyUserDetails = res.userslist;
          this.loading = false;
        },
        error: () => {},
      })
    );
  }

  extractUserIds(): number[] {
    var Ids = this.selectedUsers.map(user => user.userId);
    return Ids;
  }

  viewMemberProfile(userId: number): void {
    // Navigate to the details page and pass id and orgId as query params
    this.router.navigate(['/verify-member'], { queryParams: { userId } });
  }

  acceptMembership(userId: number) {
    this.confirmationService.confirm({
      message: 'Are you sure you want to Accept this Membership?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        if (!userId || isNaN(userId)) {
          return;
        }
        this.userIds = this.extractUserIds();
        if (!this.userIds.includes(userId)) {
          this.userIds.push(userId);
        }
        this.adminService.acceptMembership(this.orgId, this.userIds, this.userId).subscribe({
          next: () => {
            this.userIds.forEach(userId => {
              const updatedUser = this.users.find(user => user.userId === userId);
              if (updatedUser) {
                updatedUser.isApproved = 'Approved'; // Update the status directly
              }
              this.commonService.showSuccess('Membership Approved Successfully');
            });
          },
          error: () => {
            this.commonService.showError();
            return;
          },
        });
      },
    });
  }

  rejectMembership(userId: number) {
    this.confirmationService.confirm({
      message: 'Are you sure you want to Reject this Membership?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        if (!userId || isNaN(userId)) {
          return;
        }
        this.userIds = this.extractUserIds();
        if (!this.userIds.includes(userId)) {
          this.userIds.push(userId);
        }
        this.adminService.rejectMembership(this.orgId, this.userIds, this.userId).subscribe({
          next: () => {
            this.userIds.forEach(userId => {
              const updatedUser = this.users.find(user => user.userId === userId);
              if (updatedUser) {
                updatedUser.isApproved = 'Rejected'; // Update the status directly
              }
            });
            this.commonService.showSuccess('Membership Rejected Successfully');
          },
          error: () => {
            this.commonService.showError();

            return;
          },
        });
      },
    });
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

  filterGlobal(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    const value = inputElement?.value || '';
    if (this.dt) {
      this.dt.filterGlobal(value, 'contains');
    }
  }
}
