import { Component } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { EventService } from '../services/event.service';

@Component({
  selector: 'app-create-event',
  templateUrl: './create-event.component.html',
  styleUrl: './create-event.component.css',
})
export class CreateEventComponent {
  orgId!: number;
  userId!: number;

  eventAccessTypes!: { name: string; code: string }[];

  eventForm!: FormGroup;

  costTypes: any[] = [];
  paymentModes: any[] = [];
  selectedCostTypes: number[] = []; // Changed to number[] for participantTypeId

  selectedFile: File | null = null;
  userData!: { userId: number; orgId: number; organizations: any[]; roles: any[] };
  eventAccess: string = 'free';

  constructor(
    private router: Router,
    private formBuilder: FormBuilder,
    private commonService: CommonService,
    private eventService: EventService,
    private messageService: MessageService
  ) {}

  ngOnInit() {
    this.costTypes = this.commonService.getCostTypes();
    this.paymentModes = this.commonService.getPaymentTypes();
    this.eventAccessTypes = this.commonService.getAccessTypes();

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
    }

    this.eventForm = this.formBuilder.group({
      orgId: [this.orgId, Validators.required],
      eventTitle: [
        '',
        [
          Validators.required,
          this.commonService.noSpaceAtStartValidator(),
          this.commonService.valueRequiredAfterEntryValidator(),
        ],
      ],
      eventDesc: [
        '',
        [
          Validators.required,
          this.commonService.noSpaceAtStartValidator(),
          this.commonService.descriptionvalidators(),
          this.commonService.valueRequiredAfterEntryValidator(),
        ],
      ],
      eventRequirements: [
        '',
        [
          Validators.required, // Allow alphanumeric and punctuation (same as eventLocation),
          this.commonService.noSpaceAtStartValidator(),
          this.commonService.descriptionvalidators(),
          this.commonService.valueRequiredAfterEntryValidator(),
        ],
      ],
      eventPhoto: [null, Validators.required],
      eventAttendenceTerms: [
        '',
        [
          Validators.required,
          this.commonService.valueRequiredAfterEntryValidator(),
          this.commonService.noSpaceAtStartValidator(), // Required
        ],
      ],
      eventContactEmail: [
        '',
        [Validators.required, Validators.email, this.commonService.valueRequiredAfterEntryValidator()],
      ],
      eventContactPhone: [
        '',
        [
          Validators.required,
          this.commonService.noSpaceAtStartValidator(),
          this.commonService.phoneValidator(),
          this.commonService.valueRequiredAfterEntryValidator(),
        ],
      ],
      additionalInfo: ['', [this.commonService.noSpaceAtStartValidator()]],
      eventDate: ['', Validators.required],
      registrationLastDate: ['', Validators.required], // New field
      eventStartTime: ['', Validators.required], // Event start time
      eventEndTime: ['', Validators.required],
      eventLocation: [
        '',
        [
          Validators.required,
          this.commonService.noSpaceAtStartValidator(),
          this.commonService.valueRequiredAfterEntryValidator(),
        ],
      ], // New field
      eventAccess: ['Free', Validators.required],
      eventCosts: this.formBuilder.array([this.createCost()]),
    });
    this.eventForm.get('eventAccess')?.valueChanges.subscribe(accessType => {
      this.updateCostValidation(accessType);
    });
  }

  get eventCosts(): FormArray {
    return this.eventForm.controls['eventCosts'] as FormArray;
  }

  createCost(costData?: any): FormGroup {
    return this.formBuilder.group({
      participantTypeId: [costData?.participantTypeId || ''], // Changed to participantTypeId
      cost: [costData?.cost || ''],
    });
  }

  updateCostValidation(eventAccess: string) {
    const eventCosts = this.eventForm.get('eventCosts') as FormArray;
    const accessType = eventAccess.toLowerCase();

    if (accessType === 'free') {
      // If the event is free, clear the required validation on cost fields
      eventCosts.controls.forEach(control => {
        control.get('type')?.clearValidators();
        control.get('cost')?.clearValidators();
      });
    } else if (accessType === 'paid') {
      // If the event is paid, set the required validation on cost fields
      eventCosts.controls.forEach(control => {
        control.get('type')?.setValidators(Validators.required);
        control.get('cost')?.setValidators([Validators.required, Validators.pattern('^[0-9]*$')]);
      });
    }
    eventCosts.controls.forEach(control => control.updateValueAndValidity());
    eventCosts.updateValueAndValidity();
    this.eventForm.updateValueAndValidity();
  }
  goBack() {
    this.router.navigate(['/admin-events']);
  }

  isSubmitDisabled() {
    console.log('Form valid:', this.eventForm.valid);
    console.log('Form dirty:', this.eventForm.dirty);
    console.log(
      'Invalid controls:',
      Object.keys(this.eventForm.controls).filter(key => this.eventForm.get(key)?.invalid)
    );

    const eventAccess = this.eventForm.get('eventAccess')?.value || this.eventAccess.toLowerCase() || 'free';
    const accessType = eventAccess.toLowerCase();

    if (accessType === 'free') {
      return this.eventForm.invalid || !this.eventForm.dirty;
    }

    const isCostTypeEmpty = this.eventCosts.controls.some(control => !control.get('participantTypeId')?.value);
    const isCostEmpty = this.eventCosts.controls.some(control => !control.get('cost')?.value);

    return (
      this.eventForm.invalid ||
      !this.eventForm.dirty ||
      (accessType === 'paid' && (this.eventCosts.length === 0 || isCostTypeEmpty || isCostEmpty))
    );
  }

  addCost() {
    this.eventCosts.push(this.createCost());
  }

  onFileChange(event: any) {
    const file = event.files[0];
    this.eventForm.patchValue({ eventPhoto: file });
  }

  onSubmit() {
    if (this.eventForm.valid) {
      const formData = this.eventForm.value;

      const eventDate = formData.eventDate
        ? formData.eventDate // Send the local date as is
        : null;

      const registrationLastDate = formData.registrationLastDate
        ? formData.registrationLastDate // Send the local date as is
        : null;
      const eventStartTime = formData.eventStartTime ? formData.eventStartTime.toTimeString().split(' ')[0] : null;

      const eventEndTime = formData.eventEndTime ? formData.eventEndTime.toTimeString().split(' ')[0] : null;

      // Convert `eventCosts` to a JSON string

      const formDataToSend = new FormData();
      formDataToSend.append('orgId', formData.orgId);
      formDataToSend.append('eventTitle', formData.eventTitle);
      formDataToSend.append('eventDesc', formData.eventDesc);
      formDataToSend.append('eventRequirements', formData.eventRequirements);
      formDataToSend.append('eventAttendenceTerms', formData.eventAttendenceTerms);
      formDataToSend.append('eventContactEmail', formData.eventContactEmail);
      formDataToSend.append('eventContactPhone', formData.eventContactPhone);
      formDataToSend.append('additionalInfo', formData.additionalInfo);
      formDataToSend.append('eventAccess', formData.eventAccess);
      formDataToSend.append('eventDate', eventDate || '');
      formDataToSend.append('registrationLastDate', registrationLastDate || '');
      formDataToSend.append('eventStartTime', eventStartTime || '');
      formDataToSend.append('eventEndTime', eventEndTime || '');
      formDataToSend.append('eventLocation', formData.eventLocation);

      const eventCostsJson = JSON.stringify(
        formData.eventAccess.toLowerCase() === 'paid'
          ? formData.eventCosts.map((cost: any) => ({
              participantTypeId: cost.participantTypeId,
              cost: cost.cost,
            }))
          : []
      );
      // Append the JSON string of event costs
      formDataToSend.append('eventCosts', eventCostsJson);

      // Append the file separately
      if (formData.eventPhoto) {
        formDataToSend.append('EventPhoto', formData.eventPhoto);
      }

      this.eventService.sendEventData(formDataToSend).subscribe(
        () => {
          this.commonService.showSuccess('Event Created Successfully');
          this.router.navigate(['/admin-events']);
        },
        () => {}
      );
    }
  }

  removeCost(index: number): void {
    const eventCosts = this.eventForm.get('eventCosts') as FormArray;
    const removedTypeId = eventCosts.at(index).value.participantTypeId; // Changed to participantTypeId
    eventCosts.removeAt(index);

    if (removedTypeId) {
      this.selectedCostTypes = this.selectedCostTypes.filter(id => id !== removedTypeId);
    }
  }

  updateSelectedCosts(): void {
    this.selectedCostTypes = this.eventCosts.controls
      .map(cost => cost.value.participantTypeId) // Changed to participantTypeId
      .filter(id => id); // Exclude empty values
  }

  getFilteredCostTypes(index: number): any[] {
    const selectedIds = this.eventCosts.controls
      .map((cost, i) => (i !== index ? cost.value.participantTypeId : null)) // Changed to participantTypeId
      .filter(id => id); // Exclude null values

    return this.costTypes.filter(costType => !selectedIds.includes(costType.code));
  }
}
