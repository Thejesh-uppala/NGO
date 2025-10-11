import { Component, OnInit, ViewChild } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { Subscription } from 'rxjs';
import { EventService } from 'src/app/event/services/event.service';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { UserState } from 'src/app/store/user/user.model';
import { AdminService } from '../services/admin.service';

@Component({
  selector: 'app-admin-events',
  templateUrl: './admin-events.component.html',
  styleUrls: ['./admin-events.component.css'],
})
export class AdminEventsComponent implements OnInit {
  selectedEvents: any[] = [];
  loading: boolean = true;
  @ViewChild('dt') dt: Table | undefined;
  searchValue: string = '';
  representatives: any[] = [];
  statuses: any[] = [];

  currentUser: any;
  orgId!: number;
  userId!: number;
  userData!: { userId: number; orgId: number; organizations: any[]; roles: any[] };

  subscriptions: Subscription[] = [];
  eventslist: any;
  filteredEventsList: any[] = [];
  currentEventId: number | null = null;

  constructor(
    private router: Router,
    private messageService: MessageService,
    private formBuilder: FormBuilder,
    private store: Store<UserState>,
    private confirmationService: ConfirmationService,
    private commonService: CommonService,
    private adminService: AdminService,
    private eventService: EventService
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
      this.GetAllEvent(this.orgId);
    }

    const isAdmin = this.userData.roles.some(role => role.RoleName === 'Admin');

    if (!isAdmin) {
      this.router.navigate(['/events']);
    }

    this.loading = false;

    this.statuses = [
      { label: 'Active', value: 'active' },
      { label: 'Inactive', value: 'inactive' },
    ];
  }

  filterGlobal(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    const value = inputElement?.value || '';
    if (this.dt) {
      this.dt.filterGlobal(value, 'contains');
    }
  }

  deleteEvent(eventId: number): void {
    this.currentEventId = eventId;

    this.confirmationService.confirm({
      key: 'confirmDelete',

      header: 'Confirm',
      accept: () => {
        // Ensure selectedUsers is not null or empty

        this.adminService.deleteEvent(eventId).subscribe({
          next: () => {
            this.filteredEventsList = this.filteredEventsList.filter(event => event.id !== eventId);
            this.commonService.showSuccess('Successfully deleted');
          },
          error: () => {
            this.commonService.showError('Successfully deleted');
          },
        });
      },
    });
  }

  navigateToEvent(eventId?: number): void {
    const id = eventId || this.currentEventId;

    if (id) {
      this.confirmationService.close();
      this.router.navigate(['/events/registered-users/', id]);
      this.currentEventId = null;
    }
  }

  clear(table: any) {
    this.searchValue = '';
    table.clear();
  }

  GetAllEvent(orgId: number) {
    this.subscriptions.push(
      this.eventService.getAllEvent(orgId).subscribe({
        next: res => {
          this.eventslist = res.eventslist || [];
          this.filteredEventsList = this.eventslist;
          this.loading = false;
        },

        error: () => {},
      })
    );
  }

  createEvent() {
    this.router.navigate(['/events/create-event']);
  }

  navigateToEditEvent(eventId: string | number): void {
    this.router.navigate(['/events/edit-event', eventId]);
  }

  getSeverity(status: string) {
    switch (status) {
      case 'Approved':
        return 'success';
      case 'Pending':
        return 'danger';
      default:
        return 'info';
    }
  }
}
