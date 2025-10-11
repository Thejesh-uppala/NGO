import { Component } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { EventService } from '../services/event.service';

@Component({
  selector: 'app-edit-event',
  templateUrl: './edit-event.component.html',
  styleUrl: './edit-event.component.css',
})
export class EditEventComponent {
  originalEventPhoto: string | null = null;
  eventData: any;
  userData!: { userId: number; orgId: number; organizations: any[]; roles: any[] };
  orgId!: number;
  userId!: number;
  eventAccessTypes!: { name: string; code: string }[];
  eventForm!: FormGroup;
  selectedCostTypes: string[] = [];
  costTypes: any[] = [];
  paymentModes: any[] = [];

  selectedFile: File | null = null;
  eventPhotoFileName: string = '';
  eventAccess: any;
  constructor(
    private router: Router,
    private formBuilder: FormBuilder,
    private commonService: CommonService,
    private route: ActivatedRoute,
    private eventService: EventService,
    private messageService: MessageService
  ) {}
  convertTimeToDate(time: string): Date {
    const today = new Date();
    const [hours, minutes] = time.split(':');

    // Set the hours and minutes on the current date
    today.setHours(+hours, +minutes, 0, 0);

    return today;
  }

  ngOnInit() {
    const eventId = +this.route.snapshot.paramMap.get('eventId')!;

    // Get user data
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
      eventPhoto: [null],
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
      registrationLastDate: ['', Validators.required],
      eventStartTime: ['', Validators.required],
      eventEndTime: ['', Validators.required],
      eventLocation: [
        '',
        [
          Validators.required,
          this.commonService.noSpaceAtStartValidator(),
          this.commonService.valueRequiredAfterEntryValidator(),
        ],
      ], // New field
      eventAccess: ['', Validators.required],
      eventCosts: this.formBuilder.array([this.createCost()]),
    });

    // Initialize static values
    this.costTypes = this.commonService.getCostTypes();

    this.paymentModes = this.commonService.getPaymentTypes();
    this.eventAccessTypes = this.commonService.getAccessTypes();

    // Get event data by ID
    this.getEventData(eventId);

    // Listen for changes in eventAccess and update validation accordingly
    this.eventForm.get('eventAccess')?.valueChanges.subscribe(accessType => {
      this.updateCostValidation(accessType);
      this.eventAccess = accessType;
    });
  }

  getEventData(eventId: number) {
    this.eventService.getEventById(this.orgId, eventId).subscribe({
      next: data => {
        this.eventData = data;

        // Ensure the data is not null and is properly structured
        if (this.eventData) {
          const eventDate = new Date(this.eventData.eventDate);
          const registrationLastDate = new Date(this.eventData.registrationLastDate);

          if (this.eventData.eventTimeUtc) {
            const [startTime, endTime] = this.eventData.eventTimeUtc.split('-');

            // Create Date objects for start and end times using today's date
            const startDate = this.convertTimeToDate(startTime);
            const endDate = this.convertTimeToDate(endTime);
            this.originalEventPhoto = this.eventData.eventPhoto;
            this.eventForm.patchValue({
              eventId: this.eventData.id,
              eventTitle: this.eventData.eventTitle,
              eventDesc: this.eventData.eventDesc,
              eventRequirements: this.eventData.eventRequirements,
              eventAttendenceTerms: this.eventData.eventAttendenceTerms,
              eventContactEmail: this.eventData.eventContactEmail,
              eventContactPhone: this.eventData.eventContactPhone,
              additionalInfo: this.eventData.additionalInfo,
              eventPhoto: this.eventData.eventPhoto,
              eventDate: eventDate,
              registrationLastDate: registrationLastDate,
              eventStartTime: startDate,
              eventEndTime: endDate,
              eventLocation: this.eventData.eventLocation,
              eventAccess: this.eventData.eventAccess,
            });
            if (this.eventData.eventAccess === 'Paid' && this.eventData.eventCosts) {
              const eventCostsFormArray = this.eventForm.get('eventCosts') as FormArray;
              eventCostsFormArray.clear();

              debugger;

              this.eventData.eventCosts.forEach((cost: any) => {
                const matchedCostTypeDropdown = this.costTypes.find(
                  type => type.name.toLowerCase() === cost.type.toLowerCase()
                );

                const costData = {
                  type: matchedCostTypeDropdown ? matchedCostTypeDropdown.name : cost.type, // Fallback to original if no match
                  cost: cost.cost,
                };
              });

              this.eventData.eventCosts.forEach((cost: any) => {
                const matchedCostType = this.costTypes.find(type => type.code === cost.participantTypeId);

                const costDataMapping = {
                  type: matchedCostType ? matchedCostType.code : cost.participantTypeId, // Fallback to original if no match
                  cost: cost.cost,
                };

                eventCostsFormArray.push(this.createCost(costDataMapping));
              });
            }
          }
        }
      },
      error: () => {},
    });
  }

  get eventCosts(): FormArray {
    return this.eventForm.controls['eventCosts'] as FormArray;
  }
  createCost(costData?: any): FormGroup {
    return this.formBuilder.group({
      participantTypeId: [costData?.type || '', Validators.required],
      cost: [costData?.cost || '', Validators.required],
    });
  }

  updateCostValidation(eventAccess: string) {
    const eventCosts = this.eventForm.get('eventCosts') as FormArray;
    const accessType = eventAccess.toLowerCase();

    if (accessType === 'free') {
      eventCosts.clear();
      eventCosts.clearValidators();
      // If the event is free, clear the required validation on cost fields
      eventCosts.controls.forEach(control => {
        control.get('participantTypeId')?.clearValidators();
        control.get('cost')?.clearValidators();
      });
    } else if (accessType === 'paid') {
      eventCosts.setValidators(Validators.required);
      // If the event is paid, set the required validation on cost fields
      eventCosts.controls.forEach(control => {
        control.get('participantTypeId')?.setValidators(Validators.required);
        control.get('cost')?.setValidators([Validators.required, Validators.pattern('^[0-9]*$')]);
      });
    }

    // Update the form state after changing validators
    eventCosts.controls.forEach(control => control.updateValueAndValidity());
    eventCosts.updateValueAndValidity();
    this.eventForm.updateValueAndValidity();
  }

  addCost() {
    this.eventCosts.push(this.createCost());
  }

  removeCost(index: number): void {
    const eventCosts = this.eventForm.get('eventCosts') as FormArray;
    const removedType = eventCosts.at(index).value.participantTypeId; // Get the removed type
    eventCosts.removeAt(index);
    debugger;
    // If the removed type exists, remove it from the selected list
    if (removedType) {
      this.selectedCostTypes = this.selectedCostTypes.filter(type => type !== removedType);
    }
  }

  isUpdateButtonDisabled(): boolean {
    debugger;
    const accessType = this.eventAccess.toLowerCase();

    if (accessType === 'free') {
      return this.eventForm.invalid || this.eventForm.pristine;
    }
    const isCostTypeEmpty = this.eventCosts.controls.some(control => !control.get('participantTypeId')?.value);
    const isCostEmpty = this.eventCosts.controls.some(control => !control.get('cost')?.value);

    return (
      this.eventForm.invalid ||
      this.eventForm.pristine ||
      (accessType === 'paid' && (this.eventCosts.length === 0 || isCostTypeEmpty || isCostEmpty))
    );
  }

  onFileChange(event: any) {
    const file = event.files[0];

    // If there is an existing event photo and the file hasn't changed, set it to null
    if (this.eventData?.eventPhoto && this.eventData.eventPhoto === file) {
      this.eventForm.patchValue({ eventPhoto: null }); // Pass null when unchanged
      return; // Exit early
    }

    // If there is an existing event photo, clear it
    if (this.eventData?.eventPhoto) {
      this.eventData.eventPhoto = null; // Remove the existing image
    }

    // Update the form value with the new file
    this.eventForm.patchValue({ eventPhoto: file });

    // Manually mark the form as dirty to enable the update button
    this.eventForm.markAsDirty();
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
      const eventCostsJson = JSON.stringify(formData.eventCosts);

      const formDataToSend = new FormData();

      formDataToSend.append('Id', this.eventData.id);
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

      // Append the JSON string of event costs
      formDataToSend.append('eventCosts', eventCostsJson);

      // Append the file separately
      if (this.originalEventPhoto != formData.eventPhoto) {
        // If the photo hasn't changed, set it to null
        formDataToSend.append('EventPhoto', formData.eventPhoto);
      }
      this.eventService.updateEventData(formDataToSend).subscribe(
        () => {
          this.commonService.showSuccess('Event Updated Successfully');
          this.router.navigate(['/admin-events']);
        },
        () => {
          this.commonService.showError();
        }
      );
    }
  }
  goBack() {
    this.router.navigate(['/admin-events']);
  }
  updateSelectedCosts(): void {
    this.selectedCostTypes = this.eventCosts.controls.map(cost => cost.value.participantTypeId).filter(type => type); // Exclude empty values
  }

  getFilteredCostTypes(index: number): any[] {
    const selectedTypes = this.eventCosts.controls
      .map((cost, i) => (i !== index ? cost.value.participantTypeId : null))
      .filter(type => type); // Exclude null values

    return this.costTypes.filter(costType => !selectedTypes.includes(costType.code));
  }
}
