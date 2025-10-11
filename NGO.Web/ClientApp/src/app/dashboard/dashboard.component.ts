import { Component, ElementRef, OnInit, ViewChild } from '@angular/core';
import { Router } from '@angular/router';

import { Store } from '@ngrx/store';
import { MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { Subscription } from 'rxjs';
import { Observable } from 'rxjs';
import { UserState } from 'src/app/store/user/user.model';
import { CommonService } from '../shared/components/services/common.service';
import { loadRegisteredEventsSuccess } from '../store/events/event.actions';
import { UserDetailModel } from '../userprofile/models/UpdateUserModel';
import { DashBoardService } from './services/dashBoardService';

interface DropDownData {
  name: string;
  code: string;
}

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss'],
})
export class DashBoardComponent implements OnInit {
  public datasets: any;
  public salesChart: any;
  public chartOrders: any;
  public chartSales: any;
  public userId: any;
  public clicked: boolean = true;
  public clicked1: boolean = false;
  public image: any;
  subscriptions: Subscription[] = [];
  leads!: UserDetailModel[];
  userDetail: UserDetailModel = new UserDetailModel();
  filteredChartDetails: UserDetailModel[] = [];
  chartBool: boolean = false;
  currentUser: any;
  totalRecords: number = 0;
  multiAxisData: any;
  basicData: any;
  multiAxisOptions: any;
  bgImageUrl: string =
    'https://res.cloudinary.com/dzwihcqk2/image/upload/v1721201312/thumbnail_contactus_bg_252f109f35.jpg'; // Replace with your image path

  gradientStyle = {
    background: `linear-gradient(180deg, rgba(0, 0, 0, 0.60) 0%, rgba(0, 0, 0, 0.60) 66.95%, rgba(0, 0, 0, 0.00) 152.46%), url(${this.bgImageUrl}) lightgray 50% / cover no-repeat`,
  };

  userDetail1!: UserDetailModel[];
  copyUserDetails!: UserDetailModel[];

  statuses!: any[];
  loading: boolean = true;
  activityValues: number[] = [0, 100];
  displayBasic: boolean = false;
  displayBasic2: boolean = false;
  data: any;
  chartDataSetValue: number[] = [];
  chartLableData: string[] = [];
  organizationData!: DropDownData[];
  warningColor: string = '';

  barchartColors: any;
  payemetDate!: string;
  paymentId!: string;
  payerId!: string;

  @ViewChild('paypalRef', { static: true })
  private paypalRef!: ElementRef;
  user$!: Observable<UserState | null>;

  responsiveOptions = [
    {
      breakpoint: '1400px',
      numVisible: 2,
      numScroll: 1,
    },
    {
      breakpoint: '1199px',
      numVisible: 3,
      numScroll: 1,
    },
    {
      breakpoint: '767px',
      numVisible: 2,
      numScroll: 1,
    },
    {
      breakpoint: '625px',
      numVisible: 1,
      numScroll: 1,
    },
  ];

  userData: any;
  orgId!: number;
  eventslist: any;
  registeredEventsList: any[] = [];
  upcomingEventslist!: any[];

  constructor(
    private router: Router,
    private store: Store<UserState>,
    private eventStore: Store,
    private messageService: MessageService,
    private commonService: CommonService,
    private dashBoradService: DashBoardService
  ) {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
    if (this.currentUser.userRoles[0].roleName == 'Admin') {
      this.router.navigateByUrl('/admin');
    }
  }

  ngOnInit(): void {
    this.commonService.getUserData().subscribe({
      next: userData => {
        this.userData = userData;
        this.userId = userData.userId;
        this.orgId = userData.orgId;
      },
      error: () => {},
    });

    if (this.userData && this.userId) {
      this.getUsersDetails(this.userId, this.orgId);
      this.GetUpcomingEvent(this.orgId);
      this.GetAllUserRegisteredEvents(this.orgId, this.userId);
    }
  }
  GetAllUserRegisteredEvents(orgId: number, userId: any) {
    this.subscriptions.push(
      this.dashBoradService.getAllUserRegisteredEvents(orgId, userId).subscribe({
        next: res => {
          const eventIds = (res.registeredEventsList || []).map((event: { id: any }) => event.id);

          this.eventStore.dispatch(loadRegisteredEventsSuccess({ registeredEventsList: eventIds || [] }));
          this.registeredEventsList = res.registeredEventsList || [];
          this.loading = false;
        },
        error: () => {},
      })
    );
  }

  navigateToRegisterPage(): void {
    this.router.navigate(['/register']); // Assuming you are using Angular Router for navigation
  }

  trimText(text: string, maxLength: number): string {
    if (!text) return ''; // Handle null or undefined
    return text.length > maxLength ? text.substring(0, maxLength) + '...' : text;
  }

  isExpired(validityDate: string | Date | undefined): boolean {
    if (!validityDate) {
      return true; // Treat as expired if no validity date is provided
    }

    // If the validityDate is a string, parse it into a Date object
    const dateObject = typeof validityDate === 'string' ? new Date(validityDate) : validityDate;

    // If the validityDate is still invalid after parsing, treat it as expired
    if (!(dateObject instanceof Date) || isNaN(dateObject.getTime())) {
      return true;
    }

    const today = new Date();
    return dateObject < today; // Compare if the date is in the past
  }

  daysLeftToExpiry(validityDate: Date | string | undefined): number {
    if (!validityDate) {
      return 0; // Default to 0 days if no validity date is provided
    }
    // Parse the date if it's a string
    const date = validityDate instanceof Date ? validityDate : new Date(validityDate);

    // Validate the parsed date
    if (isNaN(date.getTime())) {
      throw new Error('Invalid date provided');
    }

    const today = new Date();
    const diffInTime = date.getTime() - today.getTime();
    return Math.ceil(diffInTime / (1000 * 3600 * 24)); // Convert milliseconds to days
  }

  getSeverity(status: string): 'success' | 'warning' | 'danger' | undefined {
    switch (status) {
      case 'valid':
        return 'success'; // Green color for valid membership
      case 'invalid':
        return 'danger'; // Red color for expired or invalid membership
      case 'low':
        return 'warning'; // Yellow color for near-expiry membership (optional)
      default:
        return undefined; // No severity if status is unknown
    }
  }

  clear(table: Table) {
    table.clear();
    this.chartBool = false;
    this.filteredChartDetails = [];
    this.userDetail1 = [];
    this.userDetail1 = this.copyUserDetails;
  }

  selectData(e: any) {
    this.chartBool = true;
    this.filteredChartDetails = this.userDetail1.filter(x => x.country == this.chartLableData[e.element.index]);
  }

  onConfirm() {
    this.messageService.clear('c');
  }

  onReject() {
    this.messageService.clear('c');
  }
  edit() {
    this.router.navigate(['/user-profile']);
  }
  renew() {
    this.displayBasic = true;
  }

  GetUpcomingEvent(orgId: number) {
    this.subscriptions.push(
      this.dashBoradService.getUpcomingEvent(orgId).subscribe({
        next: res => {
          this.upcomingEventslist = res.eventslist || [];

          this.loading = false;
        },

        error: () => {},
      })
    );
  }

  getUsersDetails(userId: number, orgId: number) {
    this.subscriptions.push(
      this.dashBoradService.getUserDetails(userId, orgId).subscribe({
        next: res => {
          this.userDetail = res.userData;

          this.image = res.userData.photo;
        },
        error: () => {},
      })
    );
  }
}
