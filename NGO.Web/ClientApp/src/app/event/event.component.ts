import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { MessageService } from 'primeng/api';

import { Observable, Subscription } from 'rxjs';
import { CommonService } from '../shared/components/services/common.service';
import { UserState } from '../store/user/user.model';
import { EventService } from './services/event.service';

@Component({
  selector: 'app-event',
  templateUrl: './event.component.html',
  styleUrls: ['./event.component.css'],
})
export class EventComponent implements OnInit {
  filterOptions = [
    { label: 'Free', value: 'free' },
    { label: 'Paid', value: 'paid' },
  ];
  eventslist: any[] = [];
  currentUser: any;

  layout: 'list' | 'grid' = 'grid';
  searchQuery: string = '';
  subscriptions: Subscription[] = [];
  endTime: Date | undefined;
  loading: boolean = true;
  eventCount: number = 0;
  first: number = 0;
  rows: number = 6;

  user$: Observable<UserState> | undefined;
  userId: any;
  orgId: any;
  userData: any;
  filters = {
    startdate: '',
    enddate: '',
    searchValue: '',
    eventtype: '',
  };
  defaultfilters = {
    startdate: '',
    enddate: '',
    searchValue: '',
    eventtype: '',
  };
  constructor(
    private router: Router,
    private formBuilder: FormBuilder,
    private store: Store<UserState>,
    private commonService: CommonService,
    private messageService: MessageService,
    private eventService: EventService
  ) {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
  }
  position: 'center' | 'top' | 'bottom' | 'left' | 'right' | 'topleft' | 'topright' | 'bottomleft' | 'bottomright' =
    'left';
  visible: boolean = false;
  showDialog(
    position:
      | 'center'
      | 'top'
      | 'bottom'
      | 'left'
      | 'right'
      | 'topleft'
      | 'topright'
      | 'bottomleft'
      | 'bottomright' = 'left'
  ) {
    this.position = position;
    this.visible = true;
  }
  private searchDebounceTimer: any;
  onPageChange(event: any): void {
    this.first = event.first;
    this.rows = event.rows;
    const page = Math.floor(this.first / this.rows) + 1;
    this.applyFilters(page, this.rows);
  }
  applyFilters(page: number = 1, pageSize: number = this.rows): void {
    this.loading = true;

    this.eventService.getPaginatedEvents(this.filters, this.orgId, page, pageSize).subscribe({
      next: res => {
        this.eventslist =
          res.eventslist?.map((event: { registrationLastDate: string | number | Date }) => ({
            ...event,
            isPassed: event.registrationLastDate ? new Date(event.registrationLastDate) < this.currentDate : false,
          })) || [];
        this.eventCount = res.totalRecords;
        this.first = (page - 1) * pageSize;
        this.rows = pageSize;
        this.visible = false;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.commonService.showError();
      },
    });
  }
  clearFilters(): void {
    this.loading = true;
    this.filters = { ...this.defaultfilters };

    const page = 1;
    this.eventService.getPaginatedEvents(this.filters, this.orgId, page, this.rows).subscribe({
      next: res => {
        this.eventslist =
          res.eventslist?.map((event: { registrationLastDate: string | number | Date }) => ({
            ...event,
            isPassed: event.registrationLastDate ? new Date(event.registrationLastDate) < this.currentDate : false,
          })) || [];
        this.eventCount = res.totalRecords;
        this.first = (page - 1) * this.rows;
        this.visible = false;
        this.loading = false;
      },
      error: () => {
        this.loading = false;
        this.commonService.showError();
      },
    });
  }
  onSearch() {
    // Clear any existing timeout
    if (this.searchDebounceTimer) {
      clearTimeout(this.searchDebounceTimer);
    }

    // Set a new timeout
    this.searchDebounceTimer = setTimeout(() => {
      this.loading = true;
      const page = 1;

      // Update filters regardless of whether searchQuery has value
      this.filters.searchValue = this.searchQuery;

      if (this.searchQuery && this.searchQuery.trim()) {
        this.eventService.getPaginatedEvents(this.filters, this.orgId, page, this.rows).subscribe({
          next: res => {
            this.eventslist = res.eventslist || [];
            this.eventCount = res.totalRecords;

            this.first = (page - 1) * this.rows;
            this.visible = false;
            this.loading = false;
          },
          error: () => {
            this.loading = false;
            this.commonService.showError();
          },
        });
      } else {
        const page = Math.floor(this.first / this.rows) + 1;
        this.applyFilters(page, this.rows);
      }
    }, 300); // 300ms debounce
  }
  onSearchInput(event: any) {
    this.searchQuery = event.target.value;
    if (this.searchDebounceTimer) {
      clearTimeout(this.searchDebounceTimer);
    }

    this.searchDebounceTimer = setTimeout(() => {
      this.first = 0; // Reset to first page when search changes
      this.onSearch();
    }, 300);
  }
  currentDate = new Date();
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
      const page = Math.floor(this.first / this.rows) + 1;

      this.eventService.getPaginatedEvents(this.filters, this.orgId, page, this.rows).subscribe({
        next: res => {
          this.eventslist =
            res.eventslist?.map((event: { registrationLastDate: string | number | Date }) => ({
              ...event,
              isPassed: event.registrationLastDate ? new Date(event.registrationLastDate) < this.currentDate : false,
            })) || [];
          this.eventCount = res.totalRecords;

          this.first = (page - 1) * this.rows;
          this.visible = false;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
          this.commonService.showError();
        },
      });
    }
  }

  createProfile() {
    this.router.navigate(['/create-event-profile']);
  }

  toggleLayout() {
    this.layout = this.layout === 'list' ? 'grid' : 'list';
  }

  GetAllEvent(orgId: number) {
    this.subscriptions.push(
      this.eventService.getAllEvent(orgId).subscribe({
        next: res => {
          this.eventslist = res.eventslist || [];
          this.loading = false;
        },

        error: () => {},
      })
    );
  }

  onConfirm() {
    this.messageService.clear('c');
  }
  trimText(text: string, maxLength: number): string {
    if (!text) return ''; // Handle null or undefined
    return text.length > maxLength ? text.substring(0, maxLength) + '...' : text;
  }

  onReject() {
    this.messageService.clear('c');
  }
  onFilterChange(event: any) {
    this.filters.eventtype = event.value; // 'paid' or 'free'
    this.first = 0; // Reset to first page when search changes
    this.onSearch();
  }
  onFilterClear() {
    this.filters.eventtype = ''; // 'paid' or 'free'
    this.first = 0; // Reset to first page when search changes
    this.onSearch();
  }
}
