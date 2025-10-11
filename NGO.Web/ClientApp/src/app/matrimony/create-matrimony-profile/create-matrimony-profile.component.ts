import { Location } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { Component } from '@angular/core';
import { FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { City, Country, State } from 'country-state-city';
import { MenuItem, MessageService } from 'primeng/api';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { UserState } from 'src/app/store/user/user.model';
import { MatrimonyService } from '../services/matrimony.service';
@Component({
  selector: 'app-create-matrimony-profile',
  templateUrl: './create-matrimony-profile.component.html',
  styleUrl: './create-matrimony-profile.component.css',
})
export class CreateMatrimonyProfileComponent {
  activeStep: number = 0; // Tracks the current step (0-based index)
  steps: MenuItem[] = [
    { label: 'Primary Contact', icon: 'pi pi-user' },
    { label: 'Bride/Groom', icon: 'pi pi-heart' },
    { label: 'Photos', icon: 'pi pi-camera' },
    { label: 'Partner', icon: 'pi pi-users' },
    { label: 'Parents', icon: 'pi pi-home' },
    { label: 'Additional', icon: 'pi pi-info' },
  ];
  userData: any;
  countries: any;
  cityName: any;
  cities: any;
  stateName: any;
  states: any;
  countryName: any;
  gender: any;

  profileForm!: FormGroup;

  // Hardcoded values
  matrimonialDataPhotosArray: any[] = [];

  orgReFName: string = '';
  orgRefPhoneno: string = '';

  userId!: number;
  orgId!: number;
  matrimonialDataPhotos: { photoType: string; photoFile: File }[] = [];

  Answer: any[] = [];
  dietPreferences: any[] = [];
  marriageStatus: any[] = [];
  selectedFile: File | null = null;
  profileFor: any[] = [];
  registeringFor: any[] = [];
  communicationMedium: any[] = [];

  constructor(
    private router: Router,
    private matrimonialService: MatrimonyService,
    private formBuilder: FormBuilder,
    private store: Store<UserState>,
    private commonService: CommonService,
    private route: ActivatedRoute,
    private location: Location,
    private http: HttpClient,
    private messageService: MessageService
  ) {}

  ngOnInit() {
    this.gender = this.commonService.getGender();

    this.Answer = this.commonService.getAnswerOptions();
    this.dietPreferences = this.commonService.getDietPreferences();
    this.marriageStatus = this.commonService.getMarriageStatus();
    this.profileFor = this.commonService.getprofileFor();
    this.registeringFor = this.commonService.getregisteringFor();
    this.communicationMedium = this.commonService.getcommunicationMedium();
    this.countries = Country.getAllCountries();

    this.commonService.getUserData().subscribe({
      next: userData => {
        this.userData = userData;
        this.userId = userData.userId;
        this.orgId = userData.orgId;
      },
      error: () => {},
    });

    // Initialize the form group and form controls
    this.profileForm = this.formBuilder.group({
      profileFor: new FormControl('', [Validators.required]),
      registeringFor: new FormControl('', [Validators.required]),
      authPrimaryPersonFName: new FormControl('', [
        Validators.required,
        this.commonService.lettersOnlyValidator,
        this.commonService.noSpaceAtStartValidator(),
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      authPrimaryPersonLName: new FormControl('', [
        this.commonService.lettersOnlyValidator,
        this.commonService.noSpaceAtStartValidator(),
      ]),
      authPrimaryPersonEmail: new FormControl('', [
        Validators.required,
        Validators.email,
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      authPrimaryPersonPhoneNo: new FormControl('', [
        Validators.required,
        this.commonService.phoneValidator(),
        this.commonService.noSpaceAtStartValidator(),
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      authPrimaryContactCanBeShared: new FormControl(''),
      authPrimaryContactPref: new FormControl(''),

      brideOrGroomFName: new FormControl('', [
        Validators.required,
        this.commonService.lettersOnlyValidator,
        this.commonService.noSpaceAtStartValidator(),
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      brideOrGroomLName: new FormControl('', [
        this.commonService.lettersOnlyValidator,
        this.commonService.noSpaceAtStartValidator(),
      ]),
      brideOrGroomEmail: new FormControl('', [
        Validators.required,
        Validators.email,
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      brideOrGroomPhoneNumber: new FormControl('', [
        Validators.required,
        this.commonService.phoneValidator(),
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      gender: new FormControl('', [Validators.required]),
      brideGroomDOB: new FormControl('', [Validators.required]),
      brideGroomTimeOfBirth: new FormControl('', [Validators.required]),
      brideGroomPlaceOfBirth: new FormControl('', [
        Validators.required,
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      brideGroomAccessToCommunication: new FormControl(''),
      brideGroomCurrentLocation: new FormControl('', [
        Validators.required,
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      brideGroomCurrentCity: new FormControl('', [Validators.required]),
      brideGroomCurrentState: new FormControl('', [Validators.required]),
      brideGroomCurrentCountry: new FormControl('', [Validators.required]),
      brideGroomHeight: new FormControl('', [
        Validators.required,
        this.commonService.heightValidator,
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      brideGroomGothra: new FormControl('', [
        Validators.required,
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      brideGroomPravara: new FormControl('', [
        Validators.required,
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      brideGroomNakshatra: new FormControl('', [
        Validators.required,
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      brideGroomRashi: new FormControl('', [
        Validators.required,
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      brideGroomEducation: new FormControl('', [
        Validators.required,
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      brideGroomEducationDegree: new FormControl('', [
        Validators.required,
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      brideGroomUniversity: new FormControl('', [
        Validators.required,
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      brideGroomEmploymentInfo: new FormControl('', [
        Validators.required,
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      brideGroomCanRelocate: new FormControl('Yes'),
      brideGroomPrevMarried: new FormControl(''),
      brideGroomNativePlace: new FormControl('', [
        Validators.required,
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      brideGroomLegalStatus: new FormControl(''),
      brideGroomGrownUpCountryAndPlace: new FormControl('', [
        Validators.required,
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      brideGroomDiet: new FormControl(''),

      partnersPref: new FormControl('', [Validators.required, this.commonService.valueRequiredAfterEntryValidator()]),
      partnersLegalStayPref: new FormControl(''),

      orgReFName: new FormControl(this.orgReFName || 'NA'),
      orgRefPhoneno: new FormControl(this.orgRefPhoneno || 123442),

      associatedWithUserId: new FormControl(this.userId || 0),
      orgId: new FormControl(this.orgId || 0),

      fatherFName: new FormControl('', [
        Validators.required,
        this.commonService.lettersOnlyValidator,
        this.commonService.noSpaceAtStartValidator(),
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      fatherEmail: new FormControl('', Validators.email),
      fatherPhoneNo: new FormControl('', [this.commonService.phoneValidator()]),
      fatherNativePlace: new FormControl(''),
      fatherOccupation: new FormControl(''),

      motherFName: new FormControl('', [
        Validators.required,
        this.commonService.lettersOnlyValidator,
        this.commonService.noSpaceAtStartValidator(),
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      motherEmail: new FormControl('', Validators.email),
      motherPhoneNo: new FormControl('', [this.commonService.phoneValidator()]),
      motherPlaceIndia: new FormControl(''),
      motherOccupation: new FormControl(''),
      parentLocation: new FormControl(''),
      siblingsInformation: new FormControl('', [
        Validators.required,
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),

      additionalInformation: new FormControl(''),
      contactPref: new FormControl(''),
    });

    this.profileForm.valueChanges.subscribe(value => {
      // Main user fields
      if (value.city && !this.cityName) {
        this.cityName = { name: value.city };
      }
      if (value.state && !this.stateName) {
        this.stateName = { name: value.state };
      }
      if (value.country && !this.countryName) {
        this.countryName = { name: value.country };
      }
    });
  }

  onStepChange(event: any) {
    this.activeStep = event.index;
  }

  // Navigation methods
  nextStep() {
    debugger;
    if (this.activeStep < this.steps.length - 1 && this.isStepValid(this.activeStep)) {
      this.activeStep++;
    }
  }

  prevStep() {
    if (this.activeStep > 0) {
      this.activeStep--;
    }
  }

  isStepValid(step: number): boolean {
    switch (step) {
      case 0:
        return (
          this.profileForm.controls['profileFor'].valid &&
          this.profileForm.controls['registeringFor'].valid &&
          this.profileForm.controls['authPrimaryPersonFName'].valid &&
          this.profileForm.controls['authPrimaryPersonLName'].valid &&
          this.profileForm.controls['authPrimaryPersonEmail'].valid &&
          this.profileForm.controls['authPrimaryPersonPhoneNo'].valid &&
          this.profileForm.controls['authPrimaryContactPref'].valid
        );
      case 1:
        return (
          this.profileForm.controls['brideOrGroomFName'].valid &&
          this.profileForm.controls['brideOrGroomLName'].valid &&
          this.profileForm.controls['brideOrGroomEmail'].valid &&
          this.profileForm.controls['brideOrGroomPhoneNumber'].valid &&
          this.profileForm.controls['gender'].valid &&
          this.profileForm.controls['brideGroomDOB'].valid &&
          this.profileForm.controls['brideGroomTimeOfBirth'].valid &&
          this.profileForm.controls['brideGroomPlaceOfBirth'].valid &&
          this.profileForm.controls['brideGroomCurrentLocation'].valid &&
          this.profileForm.controls['brideGroomCurrentCity'].valid &&
          this.profileForm.controls['brideGroomCurrentState'].valid &&
          this.profileForm.controls['brideGroomCurrentCountry'].valid &&
          this.profileForm.controls['brideGroomHeight'].valid &&
          this.profileForm.controls['brideGroomGothra'].valid &&
          this.profileForm.controls['brideGroomPravara'].valid &&
          this.profileForm.controls['brideGroomNakshatra'].valid &&
          this.profileForm.controls['brideGroomRashi'].valid &&
          this.profileForm.controls['brideGroomEducation'].valid &&
          this.profileForm.controls['brideGroomEducationDegree'].valid &&
          this.profileForm.controls['brideGroomUniversity'].valid &&
          this.profileForm.controls['brideGroomEmploymentInfo'].valid &&
          this.profileForm.controls['brideGroomNativePlace'].valid &&
          this.profileForm.controls['brideGroomGrownUpCountryAndPlace'].valid
        );
      case 2:
        return this.matrimonialDataPhotos.length > 0;
      case 3:
        return this.profileForm.controls['partnersPref'].valid;
      case 4:
        return (
          this.profileForm.controls['fatherFName'].valid &&
          this.profileForm.controls['fatherEmail'].valid &&
          this.profileForm.controls['fatherPhoneNo'].valid &&
          this.profileForm.controls['motherFName'].valid &&
          this.profileForm.controls['motherEmail'].valid &&
          this.profileForm.controls['motherPhoneNo'].valid &&
          this.profileForm.controls['siblingsInformation'].valid
        );
      default:
        return true;
    }
  }
  onChangeCountry(e: any) {
    if (e.value) {
      let states = State.getStatesOfCountry(e.value.isoCode);
      this.countryName = e.value;
      this.states = states;
      this.profileForm.patchValue({ brideGroomCurrentCountry: e.value });

      this.profileForm.markAsDirty();
    } else {
      this.states = [];
      this.cities = [];
      this.profileForm.patchValue({ brideGroomCurrentCountry: null });

      this.profileForm.markAsDirty();
    }
  }

  onChangeState(e: any) {
    if (e.value) {
      let cities = City.getCitiesOfState(e.value.countryCode, e.value.isoCode);
      this.stateName = e.value;
      this.cities = cities;
      this.profileForm.patchValue({ brideGroomCurrentState: e.value });

      this.profileForm.markAsDirty();
    } else {
      this.cities = [];
      this.profileForm.patchValue({ brideGroomCurrentState: null });

      this.profileForm.markAsDirty();
    }
  }

  onChangeCity(e: any) {
    if (e.value) {
      this.cityName = e.value;
      this.profileForm.patchValue({ brideGroomCurrentCity: e.value });
      this.profileForm.markAsDirty();
    } else {
      this.cityName = null;
      this.profileForm.patchValue({ brideGroomCurrentCity: null });
      this.profileForm.markAsDirty();
    }
  }

  onFileSelect(event: any, photoType: string): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      const selectedFile = input.files[0];
      const existingPhotoIndex = this.matrimonialDataPhotos.findIndex(photo => photo.photoType === photoType);

      if (existingPhotoIndex > -1) {
        // Replace the existing file for this photo type
        this.matrimonialDataPhotos[existingPhotoIndex].photoFile = selectedFile;
      } else {
        // Add a new entry for this photo type
        this.matrimonialDataPhotos.push({
          photoType: photoType,
          photoFile: selectedFile,
        });
      }
    }
  }

  submitForm() {
    if (this.profileForm.valid && this.matrimonialDataPhotos.length > 0) {
      const accessToCommunicationControl = this.profileForm.get('brideGroomAccessToCommunication');
      const contactCanBeSharedControl = this.profileForm.get('authPrimaryContactCanBeShared');

      if (!accessToCommunicationControl?.value || accessToCommunicationControl?.value.length === 0) {
        accessToCommunicationControl?.setValue(false);
      }
      if (!contactCanBeSharedControl?.value || contactCanBeSharedControl?.value.length === 0) {
        contactCanBeSharedControl?.setValue(false);
      }

      this.profileForm.controls['brideGroomCurrentCountry'].setValue(
        this.countryName?.name || this.profileForm.value.country || this.countryName || ''
      );
      this.profileForm.controls['brideGroomCurrentState'].setValue(
        this.stateName?.name || this.profileForm.value.state || this.stateName || ''
      );
      this.profileForm.controls['brideGroomCurrentCity'].setValue(
        this.cityName?.name || this.profileForm.value.city || this.cityName || ''
      );

      const formData = new FormData();
      for (const key in this.profileForm.value) {
        if (Object.prototype.hasOwnProperty.call(this.profileForm.value, key)) {
          formData.append(key, this.profileForm.value[key]);
        }
      }

      this.matrimonialDataPhotos.forEach((photo, index) => {
        formData.append(`matrimonialDataPhotos[${index}].photoType`, photo.photoType);
        formData.append(`matrimonialDataPhotos[${index}].photoFile`, photo.photoFile, photo.photoFile.name);
      });

      this.matrimonialService.submitMatrimonyProfile(formData).subscribe({
        next: () => {
          this.commonService.showSuccess('Profile Sent for Admin Review');
          setTimeout(() => {
            this.router.navigate(['/matrimony']);
          }, 2000);
        },
        error: () => {
          this.commonService.showError();
        },
      });
    }
  }

  goBack(): void {
    this.location.back();
  }
  isSaveButtonDisabled(): boolean {
    console.log('Form valid:', this.profileForm.valid);
    console.log('Form dirty:', this.profileForm.dirty);
    console.log(
      'Invalid controls:',
      Object.keys(this.profileForm.controls).filter(key => this.profileForm.get(key)?.invalid)
    );
    // return !this.profileForm.valid || this.profileForm.pristine;
    return !this.profileForm.valid || this.matrimonialDataPhotos.length === 0;
  }
}
