import { Component, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { Store } from '@ngrx/store';
import { MessageService, ConfirmationService } from 'primeng/api';
//admin.component.ts
import { Dropdown } from 'primeng/dropdown';
import { Table } from 'primeng/table';
import { Subscription } from 'rxjs';
import { UserState } from 'src/app/store/user/user.model';
import * as XLSX from 'xlsx';
import { CommonService } from '../shared/components/services/common.service';
import { UserDetailModel } from '../userprofile/models/UpdateUserModel';
import { AdminService } from './services/admin.service';

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
  selector: 'app-admin',
  templateUrl: './admin.component.html',
  styleUrls: ['./admin.component.scss'],
  providers: [MessageService, ConfirmationService],
})
export class AdminComponent implements OnInit {
  userDialog: boolean = false;
  users!: User[];
  user!: User;
  selectedUsers!: User[] | null;
  submitted: boolean = false;
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
  barchartColors: any;
  orgId!: number;
  userId!: number;

  currentUser: any;
  userData!: { userId: number; orgId: number; organizations: any[]; roles: any[] };
  summaryCounts: any;

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
      this.GetAllUsers(this.orgId);
      const isAdmin = this.userData.roles.some(role => role.RoleName === 'Admin');

      if (!isAdmin) {
        this.router.navigate(['/home']);
      }
    }
    this.loadSummaryCounts();
    // this.getChapters();
    this.getOrganizations();
  }

  exportExcel() {
    if (this.dt && this.dt.el) {
      this.commonService.showInfo('Exporting...');
      // Ensure the table is available
      const ws: XLSX.WorkSheet = XLSX.utils.table_to_sheet(this.dt.el.nativeElement);
      const wb: XLSX.WorkBook = XLSX.utils.book_new();
      XLSX.utils.book_append_sheet(wb, ws, 'Users');
      XLSX.writeFile(wb, 'users.xlsx');
    }
  }

  loadSummaryCounts(): void {
    this.adminService.getSummaryCounts(this.orgId).subscribe({
      next: (response: any) => {
        this.summaryCounts = response; // Store the response in the component variable
      },
      error: () => {},
    });
  }

  getOrganizations() {
    this.subscriptions.push(
      this.adminService.getOrganizations().subscribe({
        next: res => {
          this.organizationData = res;
        },
        error: () => {},
      })
    );
  }

  viewUserProfile(userId: number): void {
    // Navigate to the details page and pass id and orgId as query params
    this.router.navigate(['/user-profile-details'], { queryParams: { userId } });
  }
  getChapters() {
    this.subscriptions.push(
      this.adminService.getChapters().subscribe({
        next: res => {
          this.chapterData = res;
        },
        error: () => {},
      })
    );
  }

  setOrgChapter(id: any) {
    this.displayBasic = true;
    this.customerId = id;
    var res = this.userDetail.find(x => x.userId.toString() == id);
    this.memberId = res ? res.memberId : '';
  }

  showModalDialog(content: any) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
    this.displayModal = true;
  }

  showMaximizableDialog() {
    this.displayMaximizable = true;
  }

  GetAllUsers(orgId: number) {
    this.subscriptions.push(
      this.adminService.getAllUsers(orgId).subscribe({
        next: res => {
          this.users = res.userslist;
          this.copyUserDetails = res.userslist;
          this.loading = false;
        },
        error: () => {
          this.commonService.showError();
        },
      })
    );
  }

  openModal(content: any) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  getSeverity(status: any) {
    switch (status) {
      case 'Active':
        return 'success';
      case 'Inactive':
        return 'warning';
    }
    return 'danger';
  }

  deleteSelectedUsers() {
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete the selected user?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        // Ensure selectedUsers is not null or empty
        if (!this.selectedUsers || this.selectedUsers.length === 0) {
          this.commonService.showError('No users selected');
          return;
        }

        const userIds = this.selectedUsers.map(user => user.userId).filter((id): id is number => id !== undefined);

        this.adminService.deleteUsers(userIds, this.userId, this.orgId).subscribe(
          () => {
            this.users = this.users.filter(val => !this.selectedUsers?.includes(val));
            this.selectedUsers = null;

            // Show success message
            this.commonService.showSuccess('Deleted Successfully');
          },
          () => {
            // Handle errors

            this.commonService.showError('Failed to delete, try again');
          }
        );
      },
    });
  }

  deleteUser(user: User) {
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete this user?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        if (this.orgId && this.userId) {
          this.adminService.deleteUsers([user.userId!], this.userId, this.orgId).subscribe({
            next: () => {
              this.users = this.users.filter(val => val.userId !== user.userId);
              this.commonService.showSuccess('User Deleted successfully');
            },
            error: () => {
              this.commonService.showError('Failed to delete user');
            },
          });
        } else {
          this.commonService.showError('Missing organization or user details');
        }
      },
    });
  }

  filterGlobal(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    const value = inputElement?.value || '';
    if (this.dt) {
      this.dt.filterGlobal(value, 'contains');
    }
  }
}
