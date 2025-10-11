import { Location } from '@angular/common';
import { Component, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { Table } from 'primeng/table';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { PaymentType } from 'src/app/shared/enums/enums';
import * as XLSX from 'xlsx';
import { EventService } from '../services/event.service';

interface GuestUser {
  GuestUserName: string;
  GuestUserPhone: string;
  GuestUserType: string;
}

interface EventPayment {
  paymentMethod: string;
  transactionId?: string;
  paymentDate?: Date;
  checkNumber?: string;
  bankName?: string;
  checkDate?: Date;
  totalCost: number;
}

interface RegisteredUser {
  userID: number;
  registeredUserName: string;
  registeredUserEmail: string;
  registeredUserPhone: string;
  registeredUserType: { [key: string]: number };
  guestUsers: GuestUser[];
  paymentInfo?: EventPayment; // Added payment info
}

@Component({
  selector: 'app-registered-users',
  templateUrl: './registered-users.component.html',
  styleUrl: './registered-users.component.css',
})
export class RegisteredUsersComponent {
  expandedRows: { [userID: number]: boolean } = {}; // Track expanded rows
  expandedPaymentRows: { [userID: number]: boolean } = {}; // New property for payment details expansion
  @ViewChild('dt') dt!: Table;
  constructor(
    private location: Location,
    private route: ActivatedRoute,
    private eventService: EventService,
    private messageService: MessageService,
    private router: Router,
    private commonService: CommonService
  ) {}
  userData!: { userId: number; orgId: number; organizations: any[]; roles: any[] };
  userId!: any;
  eventId: number | undefined;
  eventCount: number = 0;
  first: number = 0;
  rows: number = 6;
  loading: boolean = true;
  response: any; // Define a variable to hold the response data
  ngOnInit(): void {
    this.commonService.getUserData().subscribe({
      next: userData => {
        this.userData = userData;
        this.userId = userData.userId;
      },
      error: () => {},
    });

    if (this.userData && this.userId) {
      const isAdmin = this.userData.roles.some(role => role.RoleName === 'Admin');

      if (!isAdmin) {
        this.router.navigate(['/home']);
      }
    }

    this.eventId = +this.route.snapshot.paramMap.get('eventId')!;
    const page = 1;
    this.eventService.getRegisteredUserPaginatedEvents(this.eventId, page, this.rows).subscribe({
      next: res => {
        this.response = res; // Assign the response data to a variable
        this.eventCount = res.totalRecords;
        this.loading = false; // Set loading to false after the data is received
      },
      error: () => {
        this.loading = false; // Set loading to false in case of error
        this.commonService.showError();
      },
    });
  }
  goBack(): void {
    this.location.back();
  }
  exportExcel(): void {
    if (this.response?.registeredUsers) {
      // Filter out any empty/falsy user objects
      const users: RegisteredUser[] = this.response.registeredUsers.filter(
        (user: RegisteredUser) => user && Object.keys(user).length > 0
      );

      const exportData: any[] = [];

      users.forEach((user: RegisteredUser) => {
        // Filter out empty guest objects if any
        const validGuests: any[] = (user.guestUsers || []).filter(guest => guest && Object.keys(guest).length > 0);

        // Base user data
        const userData = {
          'User Name': user.registeredUserName,
          'User Email': user.registeredUserEmail,
          'User Phone': user.registeredUserPhone,
          Tickets: this.getRegisteredUserTypes(user.registeredUserType),
          'Payment Status': user.paymentInfo ? 'Paid' : 'Unpaid',
          'Payment Method': user.paymentInfo?.paymentMethod || 'N/A',
          'Payment Amount': user.paymentInfo ? this.formatAmount(user.paymentInfo.totalCost) : 'N/A',
          'Payment Date': user.paymentInfo ? this.formatPaymentDate(user.paymentInfo.paymentDate) : 'N/A',
          'Transaction ID': user.paymentInfo?.transactionId || 'N/A',
          'Check Number': user.paymentInfo?.checkNumber || 'N/A',
          'Bank Name': user.paymentInfo?.bankName || 'N/A',
          'Check Date': user.paymentInfo?.checkDate ? this.formatPaymentDate(user.paymentInfo.checkDate) : 'N/A',
          'Guest Name': '',
          'Guest Phone': '',
          'Guest Type': '',
        };

        if (validGuests.length === 0) {
          // No guests, just push the user data once
          exportData.push(userData);
        } else {
          // For the first guest, include full user data
          exportData.push({
            ...userData,
            'Guest Name': validGuests[0].guestUserName,
            'Guest Phone': validGuests[0].guestUserPhone,
            'Guest Type': validGuests[0].guestUserType,
          });

          // For subsequent guests, only populate guest columns
          for (let i = 1; i < validGuests.length; i++) {
            exportData.push({
              'User Name': '',
              'User Email': '',
              'User Phone': '',
              Tickets: '',
              'Payment Status': '',
              'Payment Method': '',
              'Payment Amount': '',
              'Payment Date': '',
              'Transaction ID': '',
              'Check Number': '',
              'Bank Name': '',
              'Check Date': '',
              'Guest Name': validGuests[i].guestUserName,
              'Guest Phone': validGuests[i].guestUserPhone,
              'Guest Type': validGuests[i].guestUserType,
            });
          }
        }
      });

      // Convert JSON to sheet
      const ws: XLSX.WorkSheet = XLSX.utils.json_to_sheet(exportData);

      // Optional: set column widths
      ws['!cols'] = [
        { wch: 20 }, // User Name
        { wch: 25 }, // User Email
        { wch: 15 }, // User Phone
        { wch: 20 }, // Tickets
        { wch: 15 }, // Payment Status
        { wch: 15 }, // Payment Method
        { wch: 15 }, // Payment Amount
        { wch: 15 }, // Payment Date
        { wch: 20 }, // Transaction ID
        { wch: 15 }, // Check Number
        { wch: 20 }, // Bank Name
        { wch: 15 }, // Check Date
        { wch: 20 }, // Guest Name
        { wch: 15 }, // Guest Phone
        { wch: 15 }, // Guest Type
      ];

      // Create workbook and append sheet
      const wb: XLSX.WorkBook = XLSX.utils.book_new();
      XLSX.utils.book_append_sheet(wb, ws, 'Registered Users');
      XLSX.writeFile(wb, 'registered-users.xlsx');
    } else {
      this.commonService.showError();
    }
  }

  applyFilters(page: number = 1, pageSize: number = this.rows): void {
    this.loading = true;

    this.eventId = +this.route.snapshot.paramMap.get('eventId')!;
    this.eventService.getRegisteredUserPaginatedEvents(this.eventId, page, this.rows).subscribe({
      next: res => {
        this.response = res; // Assign the response data to a variable
        this.eventCount = res.totalRecords;
        this.first = (page - 1) * pageSize;
        this.rows = pageSize;
        this.loading = false;
      },
      error: () => {
        this.loading = false; // Set loading to false in case of error
        this.commonService.showError();
      },
    });
  }
  onPageChange(event: any): void {
    this.first = event.first;
    this.rows = event.rows;
    const page = Math.floor(this.first / this.rows) + 1;
    this.applyFilters(page, this.rows);
  }

  getRegisteredUserTypes(types: { [key: string]: number }): string {
    debugger;
    const entries = Object.entries(types);

    if (entries.length === 1 && entries[0][0].toLowerCase() === PaymentType.Free.toString()) {
      return PaymentType.Free.toString(); // For free events, just show "Free" without count
    }
    return entries.map(([key, value]) => `${key}: ${value}`).join(', ');
  }

  formatPaymentDate(date: Date | undefined): string {
    if (!date) return 'N/A';
    return new Date(date).toLocaleDateString();
  }

  isFreeEvent(user: RegisteredUser): boolean {
    const types = Object.keys(user.registeredUserType);
    return types.length === 1 && types[0].toLowerCase() === PaymentType.Free.toString();
  }

  get hasPaidUsers(): boolean {
    return this.response?.registeredUsers.some((user: RegisteredUser) => !this.isFreeEvent(user)) || false;
  }
  formatAmount(amount: number): string {
    return amount.toLocaleString('en-US', {
      style: 'currency',
      currency: 'USD',
    });
  }

  togglePaymentDetails(user: RegisteredUser) {
    this.expandedPaymentRows[user.userID] = !this.expandedPaymentRows[user.userID];
  }

  // Helper function to get payment details display
  getPaymentDetails(payment: EventPayment | undefined): string {
    if (!payment) return 'No payment information';

    let details = `${payment.paymentMethod} - ${this.formatAmount(payment.totalCost)}`;

    if (payment.paymentMethod === 'Check') {
      details += ` | Check #${payment.checkNumber}`;
      if (payment.bankName) details += ` | ${payment.bankName}`;
      if (payment.checkDate) details += ` | Date: ${this.formatPaymentDate(payment.checkDate)}`;
    } else if (payment.transactionId) {
      details += ` | Transaction ID: ${payment.transactionId}`;
      if (payment.paymentDate) details += ` | Date: ${this.formatPaymentDate(payment.paymentDate)}`;
    }

    return details;
  }

  toggleRow(user: RegisteredUser) {
    this.expandedRows[user.userID] = !this.expandedRows[user.userID];
  }

  expandAll() {
    this.response?.registeredUsers?.forEach((user: RegisteredUser) => {
      this.expandedRows[user.userID] = true;
    });
  }

  // Collapse all rows
  collapseAll() {
    this.expandedRows = {};
  }
}
