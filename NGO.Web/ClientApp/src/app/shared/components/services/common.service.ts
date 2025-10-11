import { HttpClient, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { AbstractControl, ValidationErrors, ValidatorFn } from '@angular/forms';
import { Title } from '@angular/platform-browser';
import { Store } from '@ngrx/store';
import { MessageService } from 'primeng/api';
import { Observable } from 'rxjs';
import { UserState } from 'src/app/store/user/user.model';
import { selectCurrentUser } from 'src/app/store/user/user.selectors';

import { DashBoardURLConstants } from '../../constants/url-constants';

@Injectable()
export class CommonService {
  userData!: UserState | null;
  user$!: Observable<UserState | null>;
  subscriptions: any;

  constructor(
    private store: Store<UserState>,
    private http: HttpClient,
    private messageService: MessageService,
    private titleService: Title
  ) {}
  getHttpParams(mapParams: Map<string, string>): HttpParams {
    let params = new HttpParams();
    for (let param of mapParams.entries()) {
      if (param[1] && param[1] !== null) {
        params = params.append(param[0], param[1]);
      }
    }
    return params;
  }

  getUserData(): Observable<{
    userId: number;
    orgId: number;
    organizations: any[];
    roles: any[];
  }> {
    this.user$ = this.store.select(selectCurrentUser);

    return new Observable<{
      userId: number;
      orgId: number;
      organizations: any[];
      roles: any[];
    }>(observer => {
      this.user$.subscribe(userState => {
        if (userState) {
          const decodedToken = this.decodeToken(userState.token);
          const organizations = JSON.parse(decodedToken?.Organizations || '[]');
          const userRoles = JSON.parse(decodedToken?.UserRoles || '[]');
          const userId = decodedToken?.UserId;
          const orgId = organizations.length > 0 ? organizations[0].Id : null;

          observer.next({ userId, orgId, organizations, roles: userRoles });
          observer.complete();
        } else {
          observer.error('User state is not available');
        }
      });
    });
  }
  numbersOnlyValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;
    if (!value || value === '') {
      return null;
    }
    return /^\d+$/.test(value) ? null : { numbersOnly: true };
  }

  lettersOnlyValidator: ValidatorFn = (control: AbstractControl): ValidationErrors | null => {
    const value = control.value;
    if (!value) return null; // Allow empty values (handled by required validator).

    const lettersOnlyRegex = /^[a-zA-Z\s]*$/;
    return lettersOnlyRegex.test(value) ? null : { lettersOnly: true };
  };
  validYearValidator(control: AbstractControl): { [key: string]: boolean } | null {
    const value = control.value;
    const currentYear: number = new Date().getFullYear();

    // Allow null or empty string since the field is not required
    if (value === null || value === '') {
      return null;
    }

    // Check if the value is a valid number
    const year = Number(value);
    if (isNaN(year)) {
      return { invalidYear: true }; // Non-numeric input (e.g., "abc", "#$%")
    }

    // Check if the year is within the valid range
    if (year < 1900 || year > currentYear) {
      return { invalidYear: true };
    }

    return null; // Valid year
  }

  noSpaceAtStartValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      if (control.value === null || control.value === undefined) {
        return null; // No validation error if empty
      }

      // Ensure value is a string
      const value = String(control.value);

      // Compare trimmed and original value to detect leading space
      if (value !== value.trim() && value.startsWith(' ')) {
        return { noSpaceAtStart: true };
      }

      return null;
    };
  }

  // Validator that remembers if field had a value
  valueRequiredAfterEntryValidator(): ValidatorFn {
    let hadValue = false;

    return (control: AbstractControl): ValidationErrors | null => {
      // If there's a value, mark that it had a value
      if (control.value) {
        hadValue = true;
        return null;
      }

      // If it previously had a value but is now empty
      if (hadValue && !control.value) {
        return { valueRequired: true };
      }

      return null;
    };
  }

  /**
   * Password validator that checks for:
   * - At least one lowercase letter
   * - At least one uppercase letter
   * - At least one digit
   * - At least one special character
   * - Minimum length of 8 characters
   */
  passwordValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;

      if (!value) {
        return null; // Allow empty values (handled by required validator).
      }

      const hasUpperCase = /[A-Z]/.test(value);
      const hasLowerCase = /[a-z]/.test(value);
      const hasDigits = /\d/.test(value);
      const hasSpecialChar = /[!@#$%^&*(),.?":{}|<>]/.test(value);
      const isLengthValid = value.length >= 8;

      if (!hasUpperCase) {
        return { noUpperCase: true };
      }
      if (!hasLowerCase) {
        return { noLowerCase: true };
      }
      if (!hasDigits) {
        return { noDigits: true };
      }
      if (!hasSpecialChar) {
        return { noSpecialChar: true };
      }
      if (!isLengthValid) {
        return { minLength: true };
      }

      return null; // Valid password
    };
  }

  descriptionvalidators(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value;

      if (value && value.length > 500) {
        return { maxlength: true };
      }

      return null;
    };
  }
  phoneValidator(): ValidatorFn {
    return (control: AbstractControl): ValidationErrors | null => {
      const value = control.value?.trim();
      if (!value) return null;

      // Basic format check: should contain only numbers, spaces, and optional '+'
      const pattern = /^\+?[0-9 ]+$/;
      if (!pattern.test(value)) {
        return { invalidCharacters: true };
      }

      // If starts with '+', validate country code format
      if (value.startsWith('+')) {
        const parts = value.split(/\s+/);

        // Check if there's a proper country code (not just '+')
        if (parts[0] === '+') {
          return { invalidCountryCode: 'Enter valid country code' };
        }

        // Must have space after country code
        if (parts.length === 1) {
          return { invalidFormat: 'Country code must be followed by a space' };
        }

        const countryCode = parts[0];
        const phoneNumber = parts.slice(1).join('');

        // Check country code format (1-4 digits after '+')
        const countryDigits = countryCode.slice(1);
        if (!/^\d{1,4}$/.test(countryDigits)) {
          return { invalidCountryCode: 'Enter valid country code' };
        }

        // Check phone number length
        if (!/^\d{10}$/.test(phoneNumber)) {
          return { invalidLength: 'Phone number must be 10 digits' };
        }
      }
      // No country code case
      else {
        // Remove all spaces and check length
        const cleanNumber = value.replace(/\s+/g, '');
        if (cleanNumber.length !== 10) {
          return { invalidLength: 'Phone number must be 10 digits' };
        }
      }

      return null;
    };
  }

  heightValidator(control: AbstractControl): ValidationErrors | null {
    const value = control.value;

    // Ensure it's a number and between 1 and 10
    if (value === null || value === undefined || value === '') {
      return null; // No validation error if the field is empty (optional)
    }

    const numValue = Number(value);
    if (isNaN(numValue) || numValue < 2 || numValue > 7) {
      return { invalidHeight: true }; // Validation error
    }

    return null; // Valid input
  }

  calculateAge(yearOfBirth: number): number {
    const today = new Date();
    let age = today.getFullYear() - yearOfBirth;
    return age;
  }

  determineTypeByAge(yearOfBirth: number): string {
    const age = this.calculateAge(yearOfBirth);

    if (age >= 18) {
      return 'adult';
    } else if (age >= 6) {
      return 'child';
    } else {
      return 'infant';
    }
  }

  decodeToken(token: string): any {
    try {
      const base64Url = token.split('.')[1];
      const base64 = base64Url.replace(/-/g, '+').replace(/_/g, '/');
      const jsonPayload = decodeURIComponent(
        atob(base64)
          .split('')
          .map(c => `%${('00' + c.charCodeAt(0).toString(16)).slice(-2)}`)
          .join('')
      );
      return JSON.parse(jsonPayload);
    } catch {
      return null;
    }
  }

  getAnswerOptions() {
    return [
      { name: 'Yes', code: 'Yes' },
      { name: 'No', code: 'No' },
    ];
  }

  getprofileFor() {
    return [
      { name: 'Bride', code: 'Bride' },
      { name: 'Groom', code: 'Groom' },
    ];
  }

  getregisteringFor() {
    return [
      { name: 'Self', code: 'Self' },
      { name: 'Son', code: 'Son' },
      { name: 'Daughter', code: 'Daughter' },
      { name: 'Sister', code: 'Sister' },
      { name: 'Brother', code: 'Brother' },
      { name: 'Cousin', code: 'Cousin' },
      { name: 'Relative', code: 'Relative' },
    ];
  }

  getcommunicationMedium() {
    return [
      { name: 'Phone Call', code: 'Phone Call' },
      { name: 'Email', code: 'Email' },
      { name: 'WhatsApp', code: 'WhatsApp' },
      { name: 'Telegram', code: 'Telegram' },
      { name: 'Others', code: 'Others' },
    ];
  }

  // Define diet preferences options
  getDietPreferences() {
    return [
      { name: 'Vegetarian', code: 'vegetarian' },
      { name: 'Non-Vegetarian', code: 'non-vegetarian' },
      { name: 'Vegan', code: 'vegan' },
      { name: 'Gluten-Free', code: 'gluten-free' },
    ];
  }

  // Define marriage status options
  getMarriageStatus() {
    return [
      { name: 'Never Married', code: 'never-married' },
      { name: 'Married', code: 'married' },
      { name: 'Divorced', code: 'divorced' },
      { name: 'Widowed', code: 'widowed' },
    ];
  }

  getGender() {
    return [
      { name: 'Male', code: 'Male' },
      { name: 'Female', code: 'Female' },
      { name: 'Others', code: 'Others' },
    ];
  }
  getAccessTypes() {
    return [
      { name: 'Free', code: 'Free' },
      { name: 'Paid', code: 'Paid' },
    ];
  }

  getCostTypes() {
    return [
      { name: 'Adult', code: 1 },
      { name: 'Child', code: 2 },
      { name: 'Infant', code: 3 },
    ];
  }

  getPaymentTypes() {
    return [
      { name: 'Credit', code: 'credit' },
      { name: 'Debit', code: 'debit' },
      { name: 'Paypal', code: 'Paypal' },
      { name: 'None', code: 'Free' },
    ];
  }

  showSuccess(msg: string) {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: msg });
  }

  showError(errorMsg?: string): void {
    this.messageService.add({
      severity: 'error',
      summary: 'Error',
      detail: errorMsg || 'Something Went Wrong',
    });
  }

  showInfo(info: string) {
    this.messageService.add({ severity: 'info', summary: 'Info', detail: info });
  }

  formatDate(date: any) {
    const d = new Date(date);
    let month = '' + (d.getMonth() + 1);
    let day = '' + d.getDate();
    const year = d.getFullYear();
    if (month.length < 2) month = '0' + month;
    if (day.length < 2) day = '0' + day;
    return [year, month, day].join('-');
  }

  getCurrentUserCustomData(storedOrgId: number, userId: string) {
    return this.http.get<any>(DashBoardURLConstants.GET_CURRENT_USER_CUSTOM_DATA(storedOrgId), {
      params: { userId: userId },
    });
  }
  parseFormattedText(text: string): string {
    if (!text) return '';

    // Normalize line breaks
    let formattedText = text.replace(/\r\n/g, '\n').replace(/\r/g, '\n');

    // Convert markdown-like syntax to HTML
    formattedText = this.parseMarkdown(formattedText);

    // Handle paragraphs and spacing
    formattedText = this.parseParagraphsAndSpacing(formattedText);

    return formattedText;
  }

  private parseMarkdown(text: string): string {
    // Bold (**text** or __text__)
    text = text.replace(/(?:\*\*|__)(.*?)(?:\*\*|__)/g, '<strong>$1</strong>');

    // Italic (*text* or _text_)
    text = text.replace(/(?:\*|_)(.*?)(?:\*|_)/g, '<em>$1</em>');

    // Bullet points (-, *, • followed by space)
    text = text.replace(/^([-\*•])\s+(.+)$/gm, '<li>$2</li>');

    // Convert consecutive bullet points into ul
    text = text.replace(/(<li>.*<\/li>\n?)+/g, match => `<ul>${match}</ul>`);

    return text;
  }

  private parseParagraphsAndSpacing(text: string): string {
    // Split by double newlines for paragraphs
    const paragraphs = text.split(/\n\s*\n/).filter(p => p.trim());

    // Process each paragraph
    const formattedParagraphs = paragraphs.map(paragraph => {
      // Check if it's already a list
      if (paragraph.startsWith('<ul>')) {
        return paragraph;
      }

      // Preserve single line breaks within paragraphs
      const lines = paragraph.split('\n').filter(line => line.trim());
      if (lines.length > 1) {
        return `<p>${lines.join('<br>')}</p>`;
      }

      return `<p>${paragraph.trim()}</p>`;
    });

    return formattedParagraphs.join('');
  }

  formatText(text: string): string {
    return this.parseFormattedText(text);
  }
}
