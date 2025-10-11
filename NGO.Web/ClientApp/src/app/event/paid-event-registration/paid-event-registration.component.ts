import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { Observable, Subscribable, Subscription } from 'rxjs';

import { Child, Guest, Spouse, User } from 'src/app/shared/models/user-model';
import { addRegisteredEvent } from 'src/app/store/events/event.actions';
import { selectRegisteredEvents } from 'src/app/store/events/event.selectors';
import { UserProfileService } from 'src/app/userprofile/services/userprofile.service';
import { CommonService } from '../../shared/components/services/common.service';
import { EventRegistrationRequest, UserPaidDetails } from '../models/EventModel';
import { EventService } from '../services/event.service';

@Component({
  selector: 'app-paid-event-registration',
  templateUrl: './paid-event-registration.component.html',
  styleUrl: './paid-event-registration.component.css',
})
export class PaidEventRegistrationComponent implements OnInit {
  paymentModes: string[] = ['zelle', 'paypal', 'check'];
  user!: User;
  spouse!: Spouse;
  children: Child[] = [];
  paymentDetailsForm!: FormGroup;
  selectedPaymentMode: string = '';
  currentDate: Date = new Date();
  guests: Guest[] = [];
  guestForm!: FormGroup;
  display = false;
  currentYear: number = new Date().getFullYear();
  selectedPerson: Spouse | Child | null = null;
  selectedCategory: string | null = null;
  categorySelectionDialog = false;
  currentUser: any;
  userId: any;
  subscriptions: Subscription[] = [];
  orgId: any;
  eventId!: number;
  eventData: any;

  calculatedCosts: { type: string; count: number; total: number }[] = [];

  grandTotal: number = 0;

  costArray: number[] = [];
  isRegistered: boolean = false;
  registeredEvents$!: Observable<unknown> | Subscribable<unknown> | Promise<unknown>;
  userData!: { userId: number; orgId: number; organizations: any[]; roles: any[] };
  organizations!: any[];
  isSpouse!: boolean;
  costTypes!: { name: string; code: number }[];

  constructor(
    private router: Router,
    private commonService: CommonService,
    private route: ActivatedRoute,
    private eventService: EventService,
    private formBuilder: FormBuilder,
    private eventStore: Store,
    private userprofileService: UserProfileService
  ) {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
  }
  categories = {
    adult: { minAge: 18, maxAge: Infinity, count: 0, cost: 0 },
    child: { minAge: 6, maxAge: 17, count: 0, cost: 0 },
    infant: { minAge: 0, maxAge: 5, count: 0, cost: 0 },
  };

  ngOnInit(): void {
    this.eventStore.select(selectRegisteredEvents).subscribe(registeredEventsList => {
      if (registeredEventsList) {
        const eventId = this.eventId;
        this.isRegistered = registeredEventsList.includes(eventId);
      }
    });

    this.paymentDetailsForm = this.formBuilder.group({
      paymentMode: ['', Validators.required],
      // Optional payment fields - will be required based on selection
      transactionId: new FormControl('', [
        Validators.required,
        this.commonService.noSpaceAtStartValidator(),
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      paymentDate: [null],
      checkNumber: new FormControl('', [
        Validators.required,
        this.commonService.lettersOnlyValidator,
        this.commonService.noSpaceAtStartValidator(),
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      bankName: new FormControl('', [
        Validators.required,
        this.commonService.noSpaceAtStartValidator(),
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      checkDate: [null],
    });

    this.guestForm = this.formBuilder.group({
      firstName: new FormControl('', [
        Validators.required,
        this.commonService.lettersOnlyValidator,
        this.commonService.noSpaceAtStartValidator(),
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      lastName: new FormControl('', [
        this.commonService.lettersOnlyValidator,
        this.commonService.noSpaceAtStartValidator(),
      ]),
      yearOfBirth: [
        '',
        [
          this.commonService.numbersOnlyValidator,
          Validators.required,
          this.commonService.validYearValidator.bind(this),
        ],
      ],
      email: new FormControl('', [
        Validators.required,
        Validators.email,
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      phone: new FormControl('', [
        Validators.required,
        this.commonService.phoneValidator(),
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
    });

    this.eventId = +this.route.snapshot.paramMap.get('eventId')!;

    // Subscribe to the observable to log the user state
    this.commonService.getUserData().subscribe({
      next: userData => {
        this.userData = userData;
        this.userId = userData.userId;
        this.orgId = userData.orgId;
        this.organizations = userData.organizations;
      },
      error: () => {},
    });

    this.eventService.getEventById(this.orgId, this.eventId).subscribe(data => {
      this.eventData = data;
    });
    debugger;

    this.costTypes = this.commonService.getCostTypes();
    this.getUserDetaisl();

    this.updateTotalCost();
  }
  onPersonSelection(person: Spouse | Child | User) {
    if (!person.yearOfBirth) {
      this.selectedPerson = person;
      this.categorySelectionDialog = true;
      this.selectedCategory = null; // Reset selection each time
      person.selected = false; // Prevent immediate selection
    } else {
      this.updateTotalCost();
    }
  }

  setPersonCategory() {
    if (this.selectedPerson && this.selectedCategory) {
      switch (this.selectedCategory) {
        case 'Adult':
          this.selectedPerson.yearOfBirth = this.currentYear - 25; // Approximate age 25
          break;
        case 'Child':
          this.selectedPerson.yearOfBirth = this.currentYear - 10; // Approximate age 10
          break;
        case 'Infant':
          this.selectedPerson.yearOfBirth = this.currentYear - 2; // Approximate age 2
          break;
      }

      this.selectedPerson.selected = true;
      this.categorySelectionDialog = false;
      this.updateTotalCost();
    } else {
      this.commonService.showError('Please select a category before confirming');
    }
  }
  onPaymentModeChange(mode: string) {
    this.selectedPaymentMode = mode;

    // Reset the form while preserving the selected payment mode
    this.paymentDetailsForm.patchValue({
      paymentMode: mode,
      transactionId: '',
      paymentDate: null,
      checkNumber: '',
      bankName: '',
      checkDate: null,
    });

    // Clear all validators first
    Object.keys(this.paymentDetailsForm.controls).forEach(key => {
      this.paymentDetailsForm.get(key)?.clearValidators();
      this.paymentDetailsForm.get(key)?.updateValueAndValidity();
    });

    // Add validators dynamically based on selected payment mode
    if (mode === 'zelle' || mode === 'paypal') {
      this.paymentDetailsForm.get('transactionId')?.setValidators([Validators.required]);
      this.paymentDetailsForm.get('paymentDate')?.setValidators([Validators.required]);
    } else if (mode === 'check') {
      this.paymentDetailsForm.get('checkNumber')?.setValidators([Validators.required]);
      this.paymentDetailsForm.get('bankName')?.setValidators([Validators.required]);
      this.paymentDetailsForm.get('checkDate')?.setValidators([Validators.required]);
    }

    // Update validity after setting new validators
    Object.keys(this.paymentDetailsForm.controls).forEach(key => {
      this.paymentDetailsForm.get(key)?.updateValueAndValidity();
    });
  }

  getUserDetaisl() {
    if (this.orgId) {
      this.subscriptions.push(
        this.userprofileService.getCurrentUserProfile(this.orgId, this.userId).subscribe({
          next: res => {
            this.user = {
              firstName: res.userData.firstName,
              lastName: res.userData.lastName,
              email: res.userData.emailAddress,
              yearOfBirth: res.userData.yearOfBirth,
              selected: false,
            };

            // Assigning spouse details to the spouse array
            this.spouse = {
              firstName: res.userData.spouseDetails.spouseFirstName,
              lastName: res.userData.spouseDetails.spouseLastName,
              email: res.userData.spouseDetails.spouseEmail,
              yearOfBirth: res.userData.spouseDetails.spouseYearOfBirth,
              selected: false,
            };

            if (!this.spouse.firstName && !this.spouse.email && !this.spouse.yearOfBirth) {
              this.isSpouse = false; // Mark is_spouse as false
            } else {
              this.isSpouse = true; // Mark is_spouse as true if any field is non-null
            }
            // Assigning children details to the children array
            this.children = res.userData.childrensDetails.map((child: any) => ({
              firstName: child.childFirstName,
              lastName: child.childLastName,
              yearOfBirth: child.childYearOfBirth,
              selected: false,
            }));

            var userYearOfBirth = res.userData.yearOfBirth ? res.userData.yearOfBirth : null;
            var spouseYearOfBirth = res.userData.spouseDetails.spouseYearOfBirth
              ? res.userData.spouseDetails.spouseYearOfBirth
              : null;

            if (userYearOfBirth) this.user.yearOfBirth = userYearOfBirth;
            if (spouseYearOfBirth) this.spouse.yearOfBirth = spouseYearOfBirth;
          },
          error: () => {},
        })
      );
    }
  }

  openAddGuestDialog() {
    this.display = true;
  }

  addGuest() {
    if (this.guestForm.valid) {
      const newGuest: Guest = {
        ...this.guestForm.value,
        type: this.commonService.determineTypeByAge(this.guestForm.value.yearOfBirth),
        selected: true, // Set default as selected
      };
      debugger;
      this.guests.push(newGuest); // Add guest to attendees array
      this.display = false; // Close the modal
      this.guestForm.reset(); // Reset the form
      this.updateTotalCost();
      this.commonService.showSuccess('Guest details added');
    } else {
      this.commonService.showError();
    }
  }

  getSeverity(status: string) {
    switch (status) {
      case 'Free':
        return 'success';
      case 'Paid':
        return 'warning';
      default:
        return 'info';
    }
  }

  registerEvent() {
    if (this.isRegistered || this.isRegisterButtonDisabled()) {
      if (!this.paymentDetailsForm.valid) {
        this.commonService.showError('Please enter valid payment details');
      }
      return;
    }

    const typeToIdMap = new Map<string, number>(this.costTypes.map(type => [type.name.toLowerCase(), type.code]));

    const userPaidDetails: UserPaidDetails[] = this.calculatedCosts
      .filter(attendee => attendee.count > 0) // Exclude records with count = 0
      .flatMap(attendee =>
        Array(attendee.count)
          .fill(null)
          .map(() => ({
            participantTypeId: typeToIdMap.get(attendee.type.toLowerCase()) ?? 1,
            cost: this.getCostByType(attendee.type).toString(),
          }))
      );

    const selectedGuests = (this.guests || [])
      .filter(guest => guest && guest.selected)
      .map(guest => ({
        firstName: guest.firstName,
        lastName: guest.lastName,
        yearOfBirth: guest.yearOfBirth,
        email: guest.email,
        phoneNo: guest.phone,
        participantTypeId: typeToIdMap.get(guest.type.toLowerCase()) ?? 1,
        cost: this.getCostByType(guest.type)?.toString() || '0',
      }));

    const paymentDetails = {
      paymentMethod: this.paymentDetailsForm.value.paymentMode,
      transactionId: this.paymentDetailsForm.value.transactionId || '',
      paymentDate: this.paymentDetailsForm.value.paymentDate
        ? new Date(this.paymentDetailsForm.value.paymentDate).toISOString()
        : null,
      checkNumber: this.paymentDetailsForm.value.checkNumber || '',
      bankName: this.paymentDetailsForm.value.bankName || '',
      checkDate: this.paymentDetailsForm.value.checkDate
        ? new Date(this.paymentDetailsForm.value.checkDate).toISOString()
        : null,
      amount: this.grandTotal, // Assign the total amount
    };
    const registrationRequest: EventRegistrationRequest = {
      eventId: this.eventId,
      userId: this.userId,
      access: 'Paid', // or 'Free' based on the event type
      userPaidDetails: userPaidDetails,
      guests: selectedGuests,
      paymentDetails: paymentDetails,
    };

    this.eventService.registerEvent(this.orgId, registrationRequest).subscribe(
      () => {
        const eventId = this.eventId;
        this.eventStore.dispatch(addRegisteredEvent({ eventId }));
        this.commonService.showSuccess('Event Registeration Success');
        this.router.navigate(['/events']);
      },
      () => {
        this.commonService.showError();
      }
    );
  }

  getCostByType(type: string): number {
    switch (type) {
      case 'Adult':
        return this.costArray[0]; // adult cost from the array
      case 'Child':
        return this.costArray[1]; // child cost from the array
      case 'Infant':
        return this.costArray[2]; // infant cost from the array
      default:
        return 0;
    }
  }

  updateTotalCost() {
    // Reset counts for each category
    this.categories.adult.count = 0;
    this.categories.child.count = 0;
    this.categories.infant.count = 0;

    // Gather all selected attendees
    const attendees = [
      ...(this.user.selected ? [{ yearOfBirth: this.user.yearOfBirth }] : []),
      ...(this.spouse?.selected ? [{ yearOfBirth: this.spouse.yearOfBirth }] : []),
      ...this.children.filter(child => child.selected),
      ...this.guests.filter(guest => guest.selected),
    ];

    // Calculate counts based on age categories
    attendees.forEach(attendee => {
      const age = this.commonService.calculateAge(attendee.yearOfBirth);
      if (age >= this.categories.adult.minAge) {
        this.categories.adult.count++;
      } else if (age >= this.categories.child.minAge && age <= this.categories.child.maxAge) {
        this.categories.child.count++;
      } else if (age >= this.categories.infant.minAge && age <= this.categories.infant.maxAge) {
        this.categories.infant.count++;
      }
    });

    // Assign costs dynamically (these values could be retrieved from `eventData`)
    this.categories.adult.cost =
      this.eventData?.eventCosts.find((cost: { participantTypeId: number }) => cost.participantTypeId === 1)?.cost || 0;
    this.categories.child.cost =
      this.eventData?.eventCosts.find((cost: { participantTypeId: number }) => cost.participantTypeId === 2)?.cost || 0;
    this.categories.infant.cost =
      this.eventData?.eventCosts.find((cost: { participantTypeId: number }) => cost.participantTypeId === 3)?.cost || 0;

    this.costArray = [this.categories.adult.cost, this.categories.child.cost, this.categories.infant.cost];
    // Prepare summary
    this.calculatedCosts = Object.entries(this.categories).map(([key, value]) => ({
      type: key.charAt(0).toUpperCase() + key.slice(1),
      count: value.count,
      total: value.cost * value.count,
    }));

    // Calculate grand total
    this.grandTotal = this.calculatedCosts.reduce((acc, item) => acc + item.total, 0);
  }

  getAgeCategory(yearOfBirth: number | null): string {
    if (yearOfBirth === null) {
      return 'Not Available';
    }

    const currentYear = new Date().getFullYear();
    const age = currentYear - yearOfBirth;

    if (age >= this.categories.adult.minAge) {
      return 'Adult';
    } else if (age >= this.categories.child.minAge && age <= this.categories.child.maxAge) {
      return 'Child';
    } else if (age >= this.categories.infant.minAge && age <= this.categories.infant.maxAge) {
      return 'Infant';
    } else {
      return 'Unknown';
    }
  }
  isRegisterButtonDisabled(): boolean {
    const hasAttendees = this.calculatedCosts.some(attendee => attendee.count > 0);
    const hasPaymentMethod = !!this.paymentDetailsForm.value.paymentMode;

    if (!hasAttendees || !hasPaymentMethod) {
      return true;
    }

    // Validate required payment fields based on the selected payment mode
    if (this.selectedPaymentMode === 'zelle' || this.selectedPaymentMode === 'paypal') {
      return !this.paymentDetailsForm.value.transactionId || !this.paymentDetailsForm.value.paymentDate;
    }

    if (this.selectedPaymentMode === 'check') {
      return (
        !this.paymentDetailsForm.value.checkNumber ||
        !this.paymentDetailsForm.value.bankName ||
        !this.paymentDetailsForm.value.checkDate
      );
    }

    return !this.paymentDetailsForm.valid;
  }
  formatInputText(inputText: string) {
    return this.commonService.formatText(inputText);
  }
}
