import { Component, OnInit } from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { Router } from '@angular/router';

import { Store } from '@ngrx/store';
import { City, Country, State } from 'country-state-city';
import { MessageService } from 'primeng/api';

import { Observable, Subscription } from 'rxjs';
import { CommonService } from '../shared/components/services/common.service';
import { UserState } from '../store/user/user.model';
import { MatrimonyService } from './services/matrimony.service';

export interface Product {
  id?: string;
  code?: string;
  name?: string;
  description?: string;
  price?: number;
  quantity?: number;
  inventoryStatus?: string;
  category?: string;
  image?: string;
  rating?: number;
}

@Component({
  selector: 'app-matrimony',
  templateUrl: './matrimony.component.html',
  styleUrls: ['./matrimony.component.css'],
})
export class MatrimonyComponent implements OnInit {
  matrimonialUsersList: any[] = [];
  paginatedMatrimonialUsersList: any[] = [];
  totalRecords: number = 0;
  first: number = 0;
  rows: number = 8;

  currentUser: any;
  layout: 'list' | 'grid' = 'list';
  isGridLayout: boolean = true;

  visible: boolean = false;
  searchQuery: string = '';
  position: 'center' | 'top' | 'bottom' | 'left' | 'right' | 'topleft' | 'topright' | 'bottomleft' | 'bottomright' =
    'left';

  subscriptions: Subscription[] = [];

  loading: boolean = true;
  private searchDebounceTimer: any;

  user$: Observable<UserState> | undefined;
  userId: any;
  orgId: any;
  userData: any;

  filteredMatrimonialUsersList: any[] = [];

  defaultFilters = {
    gender: '',
    minAge: null,
    maxAge: null,
    minHeight: '',
    maxHeight: '',
    state: null,
    country: null,
    education: '',
    profession: '',
    prevMarried: '',
    searchValue: '',
  };

  filters = {
    gender: '',
    minAge: null,
    maxAge: null,
    minHeight: '',
    maxHeight: '',
    state: null,
    country: null,
    education: '',
    profession: '',
    prevMarried: '',
    searchValue: '',
  };

  marriageStatus!: { name: string; code: string }[];
  countries: any[] = [];
  states: any[] = [];
  cities: any[] = [];
  city: any;
  cityName: any;
  countryName: any;
  stateName: any;
  gender!: { name: string; code: string }[];

  constructor(
    private router: Router,
    private formBuilder: FormBuilder,
    private store: Store<UserState>,
    private commonService: CommonService,
    private messageService: MessageService,
    private matrimonyService: MatrimonyService
  ) {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
  }

  ngOnInit(): void {
    this.initializeComponent();
  }

  ngOnDestroy(): void {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }

  private initializeComponent(): void {
    this.gender = this.commonService.getGender();
    this.marriageStatus = this.commonService.getMarriageStatus();
    this.countries = Country.getAllCountries();

    this.subscriptions.push(
      this.commonService.getUserData().subscribe({
        next: userData => {
          this.userData = userData;
          this.userId = userData.userId;
          this.orgId = userData.orgId;
          if (this.orgId) {
            this.loadInitialData();
          }
        },
        error: () => {
          this.commonService.showError();
        },
      })
    );
  }

  private loadInitialData(): void {
    const page = Math.floor(this.first / this.rows) + 1;
    this.GetAllMatrimonialUsers(this.orgId, page, this.rows);
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
  onChangeCountry(e: any) {
    if (e) {
      let states = State.getStatesOfCountry(e.isoCode);
debugger
      this.countryName = e.name || e;
      this.states = states;
      this.filters.country = e.name || e;
    } else {
      this.states = [];
      this.cities = [];
    }
  }

  onChangeState(e: any) {
    if (e) {
      let cities = City.getCitiesOfState(e.countryCode, e.isoCode);
      this.stateName = e.name || e;
      this.filters.state = e.name || e;
      this.cities = cities;
    } else {
      this.cities = [];
    }
  }
  onChangeCity(e: any) {
    if (e) {
      this.city = e.name;
    } else {
      this.city = this.cityName;
    }
  }

  onClearCountry() {
    this.filters.country = null;
    this.states = [];
    this.cities = [];
    this.countryName = null;
  }

  onClearState() {
    this.filters.state = null;
    this.cities = [];
    this.stateName = null;
  }

  clearEducation() {
    this.filters.education = '';
  }

  clearProfession() {
    this.filters.profession = '';
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
        this.matrimonyService.getFilteredMatrimonialUsers(this.filters, this.orgId, page, this.rows).subscribe({
          next: res => {
            this.filteredMatrimonialUsersList = res.matrimonialuserslist || [];
            this.totalRecords = res.totalRecords;
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

  createProfile() {
    this.router.navigate(['/matrimony/create-matrimony-profile']);
  }

  getSeverity(product: any): 'success' | 'secondary' | 'info' | 'warning' | 'danger' | 'contrast' | undefined {
    switch (product) {
      case 'active':
        return 'success';

      case 'expired':
        return 'danger';

      default:
        return undefined; // Return undefined instead of null
    }
  }

  toggleLayout() {
    this.isGridLayout = !this.isGridLayout;
    this.layout = this.layout === 'list' ? 'grid' : 'list';
  }

  scrollToPartners() {
    const element = document.getElementById('search-filter-header');
    if (element) {
      element.scrollIntoView({ behavior: 'smooth' });
    }
  }

  viewProfile(id: number, orgId: number): void {
    // Navigate to the details page and pass id and orgId as query params
    this.router.navigate(['/matrimony/matrimony-profile-details'], { queryParams: { id, orgId } });
  }
  clearFilters(): void {
    this.loading = true;
    this.filters = { ...this.defaultFilters };

    const page = 1;
    this.matrimonyService.getFilteredMatrimonialUsers(this.filters, this.orgId, page, this.rows).subscribe({
      next: res => {
        this.filteredMatrimonialUsersList = res.matrimonialuserslist || [];
        this.totalRecords = res.totalRecords;
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

  applyFilters(page: number = 1, pageSize: number = this.rows): void {
    this.loading = true;
    this.matrimonyService.getFilteredMatrimonialUsers(this.filters, this.orgId, page, pageSize).subscribe({
      next: res => {
        this.filteredMatrimonialUsersList = res.matrimonialuserslist || [];
        this.totalRecords = res.totalRecords;
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

  GetAllMatrimonialUsers(orgId: number, page: number, pageSize: number): void {
    this.loading = true;
    this.subscriptions.push(
      this.matrimonyService.getAllApprovedMatrimonialUsers(orgId, page, pageSize).subscribe({
        next: res => {
          this.matrimonialUsersList = res.matrimonialuserslist.filter(
            (user: { status: string }) => user.status === 'Approved'
          );
          this.filteredMatrimonialUsersList = this.matrimonialUsersList;
          this.totalRecords = res.totalRecords;
          this.loading = false;
        },
        error: () => {
          this.loading = false;
          this.commonService.showError();
        },
      })
    );
  }

  onPageChange(event: any): void {
    this.first = event.first;
    this.rows = event.rows;
    const page = Math.floor(this.first / this.rows) + 1;
    this.applyFilters(page, this.rows);
  }

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

  onConfirm() {
    this.messageService.clear('c');
  }

  onReject() {
    this.messageService.clear('c');
  }
}
