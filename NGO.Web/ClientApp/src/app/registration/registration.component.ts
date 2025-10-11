import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { City, Country, State } from 'country-state-city';
import { MessageService } from 'primeng/api';
import { DropdownChangeEvent } from 'primeng/dropdown';
import { Observable, Subscription } from 'rxjs';
import { CommonService } from '../shared/components/services/common.service';
import { UserState } from '../store/user/user.model';
import { UserDetailModel } from '../userprofile/models/UpdateUserModel';
import { UserProfileService } from '../userprofile/services/userprofile.service';
import { RegistrationModel, SpouseDetails } from './models/UserRegistrationModel';
import { RegistrationService } from './services/registrationService';

@Component({
  selector: 'app-registration1',
  templateUrl: './registration.component.html',
  styleUrls: ['./registration.component.scss'],
})
export class RegistrationComponent implements OnInit {
  selectedPaymentMode: string = '';
  currentDate: Date = new Date();

  user$: Observable<UserState> | undefined;
  userId: any;
  orgId: any;
  currentYear: number = new Date().getFullYear();

  userData: any;
  selectedFile: File | null = null;
  signUpForm!: FormGroup;
  returnUrl: string = '';
  loggedIn: boolean = false;
  submitted: Boolean = false;
  errorMessage: string = '';
  error = '';
  currentUser: any;
  subscriptions: Subscription[] = [];
  registrationModel = {} as RegistrationModel;
  spouseDetails = {} as SpouseDetails;
  countries: any;
  states: any;
  cities: any;
  spouseCountries: any[] = [];
  spouseStates: any[] = [];
  spouseCities: any[] = [];
  childCountries: any;
  childStates: any;
  childCities: any;
  countryName!: any;
  stateName!: any;
  cityName!: any;
  childCountryName: string[] = [];
  childStateName: string[] = [];
  childCityName: string[] = [];
  spouseCountryName!: any;
  disableSpouseFlag: boolean = false;
  disableChildFlag: boolean[] = [];
  membershipPlans: any[] = [];
  spouseCityName!: any;
  selectedMembershipAmt: number | null = null;
  selectedValidityPeriod: number | null = null;
  Answer!: { name: string; code: string }[];

  hasSpouse: boolean = false;

  userDetail: UserDetailModel = new UserDetailModel();
  image: any;

  selectedCity: any[] = [];

  spouseStateName!: any;

  disabled: boolean = true;

  chartDataSetValue: number[] = [10, 20];
  yearOfBirth!: number;
  spouseYearOfBirth: any;

  constructor(
    private userprofileService: UserProfileService,
    private formBuilder: FormBuilder,
    private router: Router,
    private store: Store<UserState>,
    private commonService: CommonService,
    private http: HttpClient,
    private registrationService: RegistrationService,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    this.Answer = this.commonService.getAnswerOptions();

    this.commonService.getUserData().subscribe({
      next: userData => {
        this.userData = userData;
        this.userId = userData.userId;
        this.orgId = userData.orgId;
      },
      error: () => {},
    });

    this.fetchMembershipPlans();

    this.signUpForm = this.formBuilder.group({
      firstName: [
        '',
        [
          Validators.required,
          this.commonService.lettersOnlyValidator,
          this.commonService.noSpaceAtStartValidator(),
          this.commonService.valueRequiredAfterEntryValidator(),
        ],
      ],
      emailAddress: ['', Validators.email],
      lastName: ['', [this.commonService.lettersOnlyValidator, this.commonService.noSpaceAtStartValidator()]],
      yearOfBirth: ['', [this.commonService.numbersOnlyValidator, this.commonService.validYearValidator.bind(this)]],

      city: [''],
      country: [''],
      state: [''],
      postalCode: [''],
      addInWhatsAppOrTelegram: [''],

      memberShipId: ['', Validators.required],
      phoneNumber: ['', this.commonService.phoneValidator()],
      homeTown: ['', [this.commonService.noSpaceAtStartValidator()]],
      familyName: ['', [this.commonService.noSpaceAtStartValidator(), this.commonService.lettersOnlyValidator]],
      preferredBy: ['', [this.commonService.noSpaceAtStartValidator(), this.commonService.lettersOnlyValidator]],
      Photo: [''],

      reason: [''],
      spouseFirstName: [null],
      spouseEmail: [''],
      spouseLastName: [''],
      spousePhoneNumber: [''],
      spouseYearOfBirth: ['', this.commonService.validYearValidator.bind(this)],
      spouseState: [null],
      spouseCity: [null],
      spouseCountry: [null],
      spouseHometown: [null],

      childrensDetails: this.formBuilder.array([]),

      paymentMode: ['', Validators.required],
      // Optional payment fields - will be required based on selection
      transactionId: [''],
      paymentDate: [null],
      checkNumber: [''],
      bankName: [''],
      checkDate: [null],
    });
    // Sync autofill or external changes
    this.signUpForm.valueChanges.subscribe(value => {
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

      // Spouse fields
      if (value.spouseCity && !this.spouseCityName) {
        this.spouseCityName = { name: value.spouseCity };
      }
      if (value.spouseState && !this.spouseStateName) {
        this.spouseStateName = { name: value.spouseState };
      }
      if (value.spouseCountry && !this.spouseCountryName) {
        this.spouseCountryName = { name: value.spouseCountry };
      }
      // Child fields (loop through FormArray)
      value.childrensDetails?.forEach((child: any, i: number) => {
        if (child.childCity && !this.childCityName[i]) {
          this.childCityName[i] = child.childCity;
        }
        if (child.childState && !this.childStateName[i]) {
          this.childStateName[i] = child.childState;
        }
        if (child.childCountry && !this.childCountryName[i]) {
          this.childCountryName[i] = child.childCountry;
        }
      });
    });

    this.getUserDetails();
    this.GetAllCoutries();
  }

  onPaymentModeChange(mode: string) {
    // Reset all payment-related fields
    this.signUpForm.patchValue({
      transactionId: '',
      paymentDate: null,
      checkNumber: '',
      bankName: '',
      checkDate: null,
    });

    // Add validators based on selected payment mode
    if (mode === 'zelle' || mode === 'paypal') {
      this.signUpForm.get('transactionId')?.setValidators([Validators.required]);
      this.signUpForm.get('paymentDate')?.setValidators([Validators.required]);
      this.signUpForm.get('checkNumber')?.clearValidators();
      this.signUpForm.get('bankName')?.clearValidators();
      this.signUpForm.get('checkDate')?.clearValidators();
    } else if (mode === 'check') {
      this.signUpForm.get('checkNumber')?.setValidators([Validators.required]);
      this.signUpForm.get('bankName')?.setValidators([Validators.required]);
      this.signUpForm.get('checkDate')?.setValidators([Validators.required]);
      this.signUpForm.get('transactionId')?.clearValidators();
      this.signUpForm.get('paymentDate')?.clearValidators();
    }

    // Update validation status
    Object.keys(this.signUpForm.controls).forEach(key => {
      this.signUpForm.get(key)?.updateValueAndValidity();
    });
  }

  onMembershipChange(event: DropdownChangeEvent) {
    const selectedCode = event.value;

    const selectedMembership = this.membershipPlans.find(m => m.code === selectedCode);

    if (selectedMembership) {
      this.selectedMembershipAmt = selectedMembership.membershipamt;
      this.selectedValidityPeriod = selectedMembership.validityperiod;
    } else {
      this.selectedMembershipAmt = null;
      this.selectedValidityPeriod = null;
    }
  }
  get f() {
    return this.signUpForm.controls;
  }
  get hasDropDownError() {
    return (
      this.signUpForm.controls['spouseCountry'].get('spouseCountry')?.touched &&
      this.signUpForm.controls['spouseCountry'].get('spouseCountry')?.errors &&
      this.signUpForm.controls['spouseCountry'].get('spouseCountry')?.invalid
    );
  }

  fetchMembershipPlans() {
    this.registrationService.getMembershipPlans(this.orgId).subscribe(
      response => {
        this.membershipPlans = response;
      },
      () => {}
    );
  }

  goBack() {
    this.router.navigate(['/']);
  }

  GetAllCoutries() {
    this.countries = Country.getAllCountries();
    this.spouseCountries = Country.getAllCountries();
    this.childCountries = Country.getAllCountries();
  }

  onChangeCountry(e: any) {
    if (e.value) {
      let states = State.getStatesOfCountry(e.value.isoCode);
      this.countryName = e.value;
      this.states = states;
      this.signUpForm.patchValue({ country: e.value });
      this.signUpForm.markAsDirty();
    } else {
      this.states = [];
      this.cities = [];
      this.signUpForm.patchValue({ country: null });
      this.signUpForm.markAsDirty();
    }
  }

  onChangeState(e: any) {
    this.cities = [];
    if (e.value) {
      let cities = City.getCitiesOfState(e.value.countryCode, e.value.isoCode);
      this.stateName = e.value;
      this.signUpForm.patchValue({ state: e.value });
      this.cities = cities;
      this.signUpForm.markAsDirty();
    } else {
      this.cities = [];
      this.signUpForm.patchValue({ state: null });
      this.signUpForm.markAsDirty();
    }
  }

  onChangeCity(e: any) {
    if (e.value) {
      this.cityName = e.value;
      this.signUpForm.patchValue({ city: e.value });
      this.signUpForm.markAsDirty();
    } else {
      this.cityName = null;
      this.signUpForm.patchValue({ city: null });
      this.signUpForm.markAsDirty();
    }
  }

  onChangeSpouseCountry(e: any) {
    this.spouseStates = [];
    if (e.value) {
      let states = State.getStatesOfCountry(e.value.isoCode);
      this.spouseCountryName = e.value;
      this.spouseStates = states;
      this.signUpForm.patchValue({ spouseCountry: e.value });
      this.signUpForm.markAsDirty();
    } else {
      this.spouseStates = [];
      this.spouseCities = [];
      this.signUpForm.patchValue({ spouseCountry: null });
      this.signUpForm.markAsDirty();
    }
  }

  onChangeSpouseState(e: any) {
    this.spouseCities = [];
    if (e.value) {
      let cities = City.getCitiesOfState(e.value.countryCode, e.value.isoCode);
      this.spouseCities = cities;
      this.spouseStateName = e.value;
      this.signUpForm.patchValue({ spouseState: e.value });
      this.signUpForm.markAsDirty();
    } else {
      this.spouseCities = [];
      this.spouseStateName = null;
      this.signUpForm.patchValue({ spouseState: null });
      this.signUpForm.markAsDirty();
    }
  }

  onChangeSpouseCity(e: any) {
    if (e.value) {
      this.spouseCityName = e.value;
      this.signUpForm.patchValue({ spouseCity: e.value });
      this.signUpForm.markAsDirty();
    } else {
      this.spouseCityName = null;
      this.signUpForm.patchValue({ spouseCity: null });
      this.signUpForm.markAsDirty();
    }
    this.signUpForm.get('spouseCity')?.updateValueAndValidity();
  }

  onChangeChildCountry(e: any, i: number) {
    const childFormGroup = this.childrensDetails.at(i) as FormGroup;
    if (e.value) {
      let states = State.getStatesOfCountry(e.value.isoCode);
      this.childCountryName[i] = e.value.name; // Store as string for simplicity
      this.childStates = states;
      childFormGroup.patchValue({ childCountry: e.value });
      this.signUpForm.markAsDirty();
    } else {
      this.childStates = [];
      this.childCities = [];
      this.childCountryName[i] = '';
      childFormGroup.patchValue({ childCountry: null });
      this.signUpForm.markAsDirty();
    }
  }
  onChangeChildState(e: any, i: number) {
    const childFormGroup = this.childrensDetails.at(i) as FormGroup;
    if (e.value) {
      let cities = City.getCitiesOfState(e.value.countryCode, e.value.isoCode);
      this.childStateName[i] = e.value.name;
      this.childCities = cities;
      childFormGroup.patchValue({ childState: e.value });
      this.signUpForm.markAsDirty();
    } else {
      this.childCities = [];
      this.childStateName[i] = '';
      childFormGroup.patchValue({ childState: null });
      this.signUpForm.markAsDirty();
    }
  }

  onChangeChildCity(e: any, i: number) {
    const childFormGroup = this.childrensDetails.at(i) as FormGroup;
    if (e.value) {
      this.childCityName[i] = e.value.name;
      childFormGroup.patchValue({ childCity: e.value });
      this.signUpForm.markAsDirty();
    } else {
      this.childCityName[i] = '';
      childFormGroup.patchValue({ childCity: null });
      this.signUpForm.markAsDirty();
    }
    childFormGroup.get('childCity')?.updateValueAndValidity();
  }
  spouseSameAddress(e: any) {
    const isChecked = e.currentTarget.checked;
    this.disableSpouseFlag = isChecked;

    if (isChecked) {
      this.spouseStates = this.states;
      this.spouseCities = this.cities;
      this.spouseCityName = this.signUpForm.value.city;
      this.spouseStateName = this.signUpForm.value.state;
      this.spouseCountryName = this.signUpForm.value.country;
      this.signUpForm.patchValue({
        spouseCountry: this.signUpForm.value.country,
        spouseState: this.signUpForm.value.state,
        spouseCity: this.signUpForm.value.city,
      });
    } else {
      this.disableSpouseFlag = e.currentTarget.checked;
      this.spouseStates = [];
      this.spouseCities = [];
      this.signUpForm.patchValue({
        spouseCountry: null,
        spouseState: null,
        spouseCity: null,
      });
    }
    this.signUpForm.markAsDirty();
  }

  isSaveButtonDisabled(): boolean {
    console.log('Form valid:', this.signUpForm.valid);
    console.log('Form dirty:', this.signUpForm.dirty);
    console.log(
      'Invalid controls:',
      Object.keys(this.signUpForm.controls).filter(key => this.signUpForm.get(key)?.invalid)
    );
    return !this.signUpForm.valid || this.signUpForm.pristine || this.isRegisterButtonDisabled();
  }
  processFile(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (input.files && input.files.length > 0) {
      this.selectedFile = input.files[0];
    }
  }
  childSameAddress(e: any, i: number) {
    const childFormGroup = this.childrensDetails.at(i) as FormGroup;
    const isChecked = e.currentTarget.checked;
    this.disableChildFlag[i] = isChecked;

    if (isChecked) {
      this.disableChildFlag[i] = e.currentTarget.checked;
      this.childStates = this.states;
      this.childCities = this.cities;
      this.childCityName[i] = this.signUpForm.value.city;
      this.childStateName[i] = this.signUpForm.value.state;
      this.childCountryName[i] = this.signUpForm.value.country;
      childFormGroup.patchValue({
        childCountry: this.signUpForm.value.country,
        childState: this.signUpForm.value.state,
        childCity: this.signUpForm.value.city,
      });
    } else {
      this.disableChildFlag[i] = e.currentTarget.checked;
      this.childStates = [];
      this.childCities = [];
      childFormGroup.patchValue({
        childCountry: null,
        childState: null,
        childCity: null,
      });
    }

    childFormGroup.markAsDirty();
  }

  toggleSpouseDetails(event: any) {
    this.hasSpouse = event.target.checked;

    if (this.hasSpouse) {
      // Add required validation for spouseFirstName only
      this.signUpForm
        .get('spouseFirstName')
        ?.setValidators([
          Validators.required,
          this.commonService.lettersOnlyValidator,
          this.commonService.noSpaceAtStartValidator(),
        ]);
      this.signUpForm
        .get('spouseYearOfBirth')
        ?.setValidators([this.commonService.numbersOnlyValidator, this.commonService.validYearValidator.bind(this)]);
      const optionalFields = [
        'spouseEmail',
        'spouseLastName',
        'spousePhoneNumber',
        'spouseState',
        'spouseCity',
        'spouseCountry',
        'spouseHometown',
      ];

      optionalFields.forEach(field => {
        const control = this.signUpForm.get(field);
        if (control?.value) {
          if (field === 'spouseEmail') {
            control.setValidators([Validators.email]);
          } else if (field === 'spousePhoneNumber') {
            control.setValidators([this.commonService.phoneValidator()]);
          } else {
            control.setValidators([this.commonService.noSpaceAtStartValidator()]);
          }
        } else {
          control?.clearValidators();
        }
        control?.updateValueAndValidity();
      });
    } else {
      // Remove all spouse validators and clear values when unchecked
      [
        'spouseFirstName',
        'spouseEmail',
        'spouseLastName',
        'spousePhoneNumber',
        'spouseState',
        'spouseCity',
        'spouseCountry',
        'spouseHometown',
        'spouseYearOfBirth',
      ].forEach(field => {
        const control = this.signUpForm.get(field);
        control?.clearValidators();
        control?.setValue(null); // Clear the value
        control?.updateValueAndValidity();
      });
    }

    this.signUpForm.get('spouseFirstName')?.updateValueAndValidity();
    this.signUpForm.get('spouseYearOfBirth')?.updateValueAndValidity();
    this.signUpForm.updateValueAndValidity();
  }

  getUserDetails() {
    if (this.orgId) {
      this.subscriptions.push(
        this.userprofileService.getCurrentUserProfile(this.orgId, this.userId).subscribe({
          next: res => {
            this.signUpForm.patchValue(res.userData);

            this.signUpForm.patchValue({
              spouseFirstName: res.userData.spouseDetails.spouseFirstName,
              spouseCountry: res.userData.spouseDetails.spouseCountry,
              spouseCity: res.userData.spouseDetails.spouseCity,
              spouseYearOfBirth: res.userData.spouseDetails.spouseYearOfBirth,
              spouseEmail: res.userData.spouseDetails.spouseEmail,
              spouseHometown: res.userData.spouseDetails.spouseHometown,
              spouseLastName: res.userData.spouseDetails.spouseLastName,
              spousePhoneNumber: res.userData.spouseDetails.spousePhoneNumber,
              spouseState: res.userData.spouseDetails.spouseState,
            });

            var yob = res.userData.yearOfBirth ? res.userData.yearOfBirth : null;
            var spouseYearOfBirth = res.userData.spouseDetails.spouseYearOfBirth
              ? res.userData.spouseDetails.spouseYearOfBirth
              : null;

            this.signUpForm.setControl('childrensDetails', this.setExistingChild(res.userData.childrensDetails));
            this.disableChildFlag = new Array(res.userData.childrensDetails.length).fill(false);
            this.userDetail = res.userData;

            this.yearOfBirth = yob;
            this.spouseYearOfBirth = spouseYearOfBirth;
            if (res.userData.spouseDetails && res.userData.spouseDetails.spouseFirstName !== null) {
              this.hasSpouse = true;
              this.toggleSpouseDetails({ target: { checked: true } });
            } else {
              this.hasSpouse = false;
            }

            this.image = 'data:image/png;base64,' + res.userData.photoData;
            this.signUpForm.updateValueAndValidity(); // Ensure validity is recalculated
          },
          error: () => {},
        })
      );
    }
  }

  setExistingChild(child: any): FormArray {
    const formArray = new FormArray<FormGroup>([]);
    child.forEach((s: any) => {
      formArray.push(
        this.formBuilder.group({
          childFirstName: [
            s.childFirstName,
            [
              Validators.required,
              this.commonService.lettersOnlyValidator,
              this.commonService.noSpaceAtStartValidator(),
            ],
          ],
          childEmailAddress: [s.childEmailAddress, Validators.email],
          childLastName: [
            s.childLastName,
            [this.commonService.lettersOnlyValidator, this.commonService.noSpaceAtStartValidator()],
          ],
          childUniqueId: [s.childUniqueId],
          childPhoneNumber: [s.childPhoneNumber, this.commonService.phoneValidator()],
          childState: [s.childState],
          childCity: [s.childCity],
          childCountry: [s.childCountry],
          childYearOfBirth: [
            s.childYearOfBirth,
            [this.commonService.numbersOnlyValidator, this.commonService.validYearValidator.bind(this)],
          ],
        })
      );
    });
    return formArray;
  }

  get childrensDetails(): FormArray {
    return this.signUpForm.controls['childrensDetails'] as FormArray;
  }

  addChildrens() {
    this.childrensDetails.push(this.newChild());
    this.signUpForm.updateValueAndValidity(); // Ensure form validity is recalculated
    this.signUpForm.markAsDirty(); // Add this to ensure the form is dirty

    this.disableChildFlag.push(false); // Add flag for new child
  }

  removeChild(i: number) {
    this.childrensDetails.removeAt(i);
    this.disableChildFlag.splice(i, 1); // Remove corresponding flag
    this.signUpForm.updateValueAndValidity(); // Recalculate validity
    this.signUpForm.markAsDirty(); // Add this to ensure the form is dirty
  }

  newChild(): FormGroup {
    return this.formBuilder.group({
      childFirstName: [
        '',
        [
          Validators.required,
          this.commonService.lettersOnlyValidator,
          this.commonService.noSpaceAtStartValidator(),
          this.commonService.valueRequiredAfterEntryValidator(),
        ],
      ],
      childUniqueId: [''],

      childEmailAddress: ['', Validators.email],
      childLastName: ['', [this.commonService.lettersOnlyValidator, this.commonService.noSpaceAtStartValidator()]],
      childPhoneNumber: ['', this.commonService.phoneValidator()],

      childYearOfBirth: [
        '',
        [this.commonService.numbersOnlyValidator, this.commonService.validYearValidator.bind(this)],
      ],

      childState: [null],
      childCity: [null],
      childCountry: [null],
    });
  }

  composeModel(): void {
    this.registrationModel.userId = this.userId;
    this.registrationModel.firstName = this.signUpForm.value.firstName;
    this.registrationModel.familyName = this.signUpForm.value.familyName;
    this.registrationModel.emailAddress = this.signUpForm.value.emailAddress;
    this.registrationModel.memberShipId = this.signUpForm.value.memberShipId;
    this.registrationModel.yearOfBirth = this.signUpForm.value.yearOfBirth;
    this.registrationModel.lastName = this.signUpForm.value.lastName;
    this.registrationModel.postalCode = this.signUpForm.value.postalCode;
    this.registrationModel.city =
      this.cityName?.name || this.signUpForm.value.city?.name || this.signUpForm.value.city || '';
    this.registrationModel.state =
      this.stateName?.name || this.signUpForm.value.state?.name || this.signUpForm.value.state || '';
    this.registrationModel.country =
      this.countryName?.name || this.signUpForm.value.country?.name || this.signUpForm.value.country || '';
    this.registrationModel.homeTown = this.signUpForm.value.homeTown;
    this.registrationModel.reason = this.signUpForm.value.reason;
    this.registrationModel.preferredBy = this.signUpForm.value.preferredBy;
    this.registrationModel.phoneNumber = this.signUpForm.value.phoneNumber;

    this.registrationModel.addInWhatsAppOrTelegram = this.signUpForm.value.addInWhatsAppOrTelegram;

    (this.registrationModel.paymentMethod = this.signUpForm.value.paymentMode),
      (this.registrationModel.transactionId = this.signUpForm.value.transactionId || null),
      (this.registrationModel.paymentDate = this.signUpForm.value.paymentDate || null),
      (this.registrationModel.checkNumber = this.signUpForm.value.checkNumber || null),
      (this.registrationModel.bankName = this.signUpForm.value.bankName || null),
      (this.registrationModel.checkDate = this.signUpForm.value.checkDate || null);
    (this.registrationModel.amount = this.selectedMembershipAmt ?? 0),
      (this.registrationModel.request_type = 'Register');

    //Spouse Details
    if (this.hasSpouse) {
      this.spouseDetails.spouseFirstName = this.signUpForm.value.spouseFirstName;
      this.spouseDetails.spouseCountry =
        this.spouseCountryName?.name ||
        this.signUpForm.value.spouseCountry?.name ||
        this.signUpForm.value.spouseCountry ||
        '';
      this.spouseDetails.spouseState =
        this.spouseStateName?.name ||
        this.signUpForm.value.spouseState?.name ||
        this.signUpForm.value.spouseState ||
        '';
      this.spouseDetails.spouseCity =
        this.spouseCityName?.name || this.signUpForm.value.spouseCity?.name || this.signUpForm.value.spouseCity || '';

      this.spouseDetails.spouseYearOfBirth = this.signUpForm.value.spouseYearOfBirth;
      this.spouseDetails.spouseEmail = this.signUpForm.value.spouseEmail;
      this.spouseDetails.spouseHometown = this.signUpForm.value.spouseHometown;
      this.spouseDetails.spouseLastName = this.signUpForm.value.spouseLastName;
      this.spouseDetails.spousePhoneNumber = this.signUpForm.value.spousePhoneNumber;
    } else {
      this.spouseDetails = {} as SpouseDetails;
    }
    this.registrationModel.spouseDetails = this.spouseDetails;

    for (let index = 0; index < this.signUpForm?.value?.childrensDetails?.length; index++) {
      this.signUpForm.value.childrensDetails[index].childCountry =
        this.childCountryName[index] ||
        this.signUpForm.value.childrensDetails[index]?.childCountry?.name ||
        this.signUpForm.value.childrensDetails[index]?.childCountry ||
        '';
      this.signUpForm.value.childrensDetails[index].childState =
        this.childStateName[index] ||
        this.signUpForm.value.childrensDetails[index]?.childState?.name ||
        this.signUpForm.value.childrensDetails[index]?.childState ||
        '';
      this.signUpForm.value.childrensDetails[index].childCity =
        this.childCityName[index] ||
        this.signUpForm.value.childrensDetails[index]?.childCity?.name ||
        this.signUpForm.value.childrensDetails[index]?.childCity ||
        '';

      if (!this.signUpForm.value.childrensDetails[index].childUniqueId) {
        this.signUpForm.value.childrensDetails[index].childUniqueId = this.orgId + '-' + this.userId + '-' + index;
      }
    }
    this.registrationModel.childrensDetails = this.signUpForm?.value?.childrensDetails;
  }

  signUp(): void {
    if (this.signUpForm.invalid) return;
    this.commonService.showInfo('Registration in progress');
    this.composeModel();

    this.subscriptions.push(
      this.registrationService.register(this.registrationModel, this.selectedFile, this.orgId).subscribe({
        next: (res: any) => {
          if (res) {
            this.commonService.showSuccess('Registration Request Sent');
            this.router.navigate(['/home']);
          }
        },
        error: () => {
          this.commonService.showError();
        },
      })
    );
  }
  isRegisterButtonDisabled(): boolean {
    // Validate required payment fields based on the selected payment mode
    if (this.selectedPaymentMode === 'zelle' || this.selectedPaymentMode === 'paypal') {
      return !this.signUpForm.value.transactionId || !this.signUpForm.value.paymentDate;
    }

    if (this.selectedPaymentMode === 'check') {
      return !this.signUpForm.value.checkNumber || !this.signUpForm.value.bankName || !this.signUpForm.value.checkDate;
    }

    return !this.signUpForm.valid;
  }
}
