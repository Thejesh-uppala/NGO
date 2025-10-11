import { Component, OnInit } from '@angular/core';
import { FormArray, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { Country, State, City, ICity } from 'country-state-city';
import { MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { AuthenticationService } from '../core/services/authentication.service';
import { SpouseDetails } from '../registration/models/UserRegistrationModel';
import { CommonService } from '../shared/components/services/common.service';
import { UserDetailModel } from './models/UpdateUserModel';
import { UserProfileService } from './services/userprofile.service';

@Component({
  selector: 'app-userprofile',
  templateUrl: './userprofile.component.html',
  styleUrls: ['./userprofile.component.scss'],
})
export class UserprofileComponent implements OnInit {
  profileForm!: FormGroup;
  returnUrl: string = '';
  hasSpouse: boolean = false;
  subscriptions: Subscription[] = [];
  selectedFile!: File;
  updateUserModel = {} as UserDetailModel;
  spouseDetails = {} as SpouseDetails;
  userDetail: UserDetailModel = new UserDetailModel();
  image: any;
  currentYear: number = new Date().getFullYear();

  countries: any;
  states: any;
  cities: any;
  spouseCountries: any;
  spouseStates: any;
  selectedCity: any[] = [];
  spouseCities: ICity[] = [];
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

  spouseStateName!: any;
  spouseCityName!: any;
  disabled: boolean = true;
  //childstateName:string="";
  disableSpouseFlag: boolean = false;
  disableChildFlag: boolean[] = [];
  chartDataSetValue: number[] = [10, 20];
  yearOfBirth!: number;
  spouseYearOfBirth: any;
  isEditClicked: boolean = false;
  userData!: { userId: number; orgId: number; organizations: any[]; roles: any[] };
  userId!: any;
  orgId!: any;
  Answer!: { name: string; code: string }[];
  constructor(
    private router: Router,
    private authenticationService: AuthenticationService,
    private store: Store,
    private commonService: CommonService,
    private route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private messageService: MessageService,
    private userprofileService: UserProfileService
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

    this.profileForm = this.formBuilder.group({
      firstName: new FormControl('', [
        Validators.required,
        this.commonService.lettersOnlyValidator,
        this.commonService.noSpaceAtStartValidator(),
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      emailAddress: new FormControl('', Validators.email),
      lastName: ['', [this.commonService.lettersOnlyValidator, this.commonService.noSpaceAtStartValidator()]],
      yearOfBirth: ['', [this.commonService.numbersOnlyValidator, this.commonService.validYearValidator.bind(this)]],
      city: [''],
      country: [''],
      state: [''],
      postalCode: [''],
      addInWhatsAppOrTelegram: [''],
      phoneNumber: new FormControl('', [this.commonService.phoneValidator()]),
      homeTown: ['', [this.commonService.noSpaceAtStartValidator()]],
      familyName: ['', [this.commonService.noSpaceAtStartValidator(), this.commonService.lettersOnlyValidator]],
      preferredBy: ['', [this.commonService.noSpaceAtStartValidator(), this.commonService.lettersOnlyValidator]],
      Photo: [''],
      reason: [''],
      spouseFirstName: new FormControl('', [
        this.commonService.lettersOnlyValidator,
        this.commonService.noSpaceAtStartValidator(),
        this.commonService.valueRequiredAfterEntryValidator(),
      ]),
      spouseEmail: [''],
      spouseLastName: [''],
      spousePhoneNumber: [''],
      spouseYearOfBirth: new FormControl('', this.commonService.validYearValidator.bind(this)),
      spouseState: [null],
      spouseCity: [null],
      spouseCountry: [null],
      spouseHometown: [null],

      childrensDetails: this.formBuilder.array([]),
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

  spouseSameAddress(e: any) {
    const isChecked = e.currentTarget.checked;
    this.disableSpouseFlag = isChecked;

    if (isChecked) {
      this.spouseStates = this.states;
      this.spouseCities = this.cities;
      this.spouseCityName = this.profileForm.value.city;
      this.spouseStateName = this.profileForm.value.state;
      this.spouseCountryName = this.profileForm.value.country;
      this.profileForm.patchValue({
        spouseCountry: this.profileForm.value.country,
        spouseState: this.profileForm.value.state,
        spouseCity: this.profileForm.value.city,
      });
    } else {
      this.disableSpouseFlag = e.currentTarget.checked;
      this.spouseStates = [];
      this.spouseCities = [];
      this.profileForm.patchValue({
        spouseCountry: null,
        spouseState: null,
        spouseCity: null,
      });
    }
    this.profileForm.markAsDirty();
  }

  childSameAddress(e: any, i: number) {
    const childFormGroup = this.childrensDetails.at(i) as FormGroup;
    const isChecked = e.currentTarget.checked;
    this.disableChildFlag[i] = isChecked;

    if (isChecked) {
      this.disableChildFlag[i] = e.currentTarget.checked;
      this.childStates = this.states;
      this.childCities = this.cities;
      this.childCityName[i] = this.profileForm.value.city;
      this.childStateName[i] = this.profileForm.value.state;
      this.childCountryName[i] = this.profileForm.value.country;
      childFormGroup.patchValue({
        childCountry: this.profileForm.value.country,
        childState: this.profileForm.value.state,
        childCity: this.profileForm.value.city,
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

  get f() {
    return this.profileForm.controls;
  }

  get childrensDetails(): FormArray {
    return this.profileForm.controls['childrensDetails'] as FormArray;
  }

  addChildrens() {
    this.childrensDetails.push(this.newChild());
    this.profileForm.updateValueAndValidity(); // Ensure form validity is recalculated
    this.profileForm.markAsDirty(); // Add this to ensure the form is dirty
    this.disableChildFlag.push(false); // Add flag for new child
  }

  removeChild(i: number) {
    this.childrensDetails.removeAt(i);
    this.disableChildFlag.splice(i, 1); // Remove corresponding flag
    this.profileForm.updateValueAndValidity(); // Recalculate validity
    this.profileForm.markAsDirty(); // Add this to ensure the form is dirty
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
      childState: [null],
      childCity: [null],
      childCountry: [null],
      childYearOfBirth: [
        '',
        [this.commonService.numbersOnlyValidator, this.commonService.validYearValidator.bind(this)],
      ],
    });
  }

  onConfirm() {
    this.messageService.clear('c');
  }

  onReject() {
    this.messageService.clear('c');
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
              this.commonService.valueRequiredAfterEntryValidator(),
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

  getUserDetails() {
    if (this.orgId) {
      this.subscriptions.push(
        this.userprofileService.getCurrentUserProfile(this.orgId, this.userId).subscribe({
          next: res => {
            this.profileForm.patchValue(res.userData);

            this.profileForm.patchValue({
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

            this.profileForm.setControl('childrensDetails', this.setExistingChild(res.userData.childrensDetails));
            this.disableChildFlag = new Array(res.userData.childrensDetails.length).fill(true);
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
            this.profileForm.updateValueAndValidity(); // Ensure validity is recalculated
          },
          error: () => {},
        })
      );
    }
  }

  toggleSpouseDetails(event: any) {
    this.hasSpouse = event.target.checked;
    if (this.hasSpouse) {
      this.profileForm
        .get('spouseFirstName')
        ?.setValidators([
          Validators.required,
          this.commonService.lettersOnlyValidator,
          this.commonService.noSpaceAtStartValidator(),
        ]);
      this.profileForm
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
        const control = this.profileForm.get(field);
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
        const control = this.profileForm.get(field);
        control?.clearValidators();
        control?.setValue(null); // Clear the value
        control?.updateValueAndValidity();
      });
    }
    this.profileForm.get('spouseFirstName')?.updateValueAndValidity();
    this.profileForm.get('spouseYearOfBirth')?.updateValueAndValidity();
    this.profileForm.updateValueAndValidity();
  }

  composeModel(): void {
    this.updateUserModel.userId = this.userId;
    this.updateUserModel.firstName = this.profileForm.value.firstName;
    this.updateUserModel.lastName = this.profileForm.value.lastName;
    this.updateUserModel.city =
      this.cityName?.name || this.profileForm.value.city?.name || this.profileForm.value.city || '';
    this.updateUserModel.state =
      this.stateName?.name || this.profileForm.value.state?.name || this.profileForm.value.state || '';
    this.updateUserModel.country =
      this.countryName?.name || this.profileForm.value.country?.name || this.profileForm.value.country || '';
    this.updateUserModel.yearOfBirth = this.profileForm.value.yearOfBirth;
    this.updateUserModel.homeTown = this.profileForm.value.homeTown;
    this.updateUserModel.addInWhatsAppOrTelegram = this.profileForm.value.addInWhatsAppOrTelegram;
    this.updateUserModel.phoneNumber = this.profileForm.value.phoneNumber;
    this.updateUserModel.reason = this.profileForm.value.reason;
    this.updateUserModel.email = this.profileForm.value.emailAddress;
    this.updateUserModel.userId = this.userId;
    this.updateUserModel.preferredBy = this.profileForm.value.preferredBy;
    this.updateUserModel.postalCode = this.profileForm.value.postalCode;
    this.updateUserModel.emailAddress = this.profileForm.value.emailAddress;
    this.updateUserModel.familyName = this.profileForm?.value.familyName;

    //Spouse Details
    if (this.hasSpouse) {
      this.spouseDetails.spouseFirstName = this.profileForm.value.spouseFirstName;

      this.spouseDetails.spouseCountry =
        this.spouseCountryName?.name ||
        this.profileForm.value.spouseCountry?.name ||
        this.profileForm.value.spouseCountry ||
        '';
      this.spouseDetails.spouseState =
        this.spouseStateName?.name ||
        this.profileForm.value.spouseState?.name ||
        this.profileForm.value.spouseState ||
        '';
      this.spouseDetails.spouseCity =
        this.spouseCityName?.name || this.profileForm.value.spouseCity?.name || this.profileForm.value.spouseCity || '';

      this.spouseDetails.spouseYearOfBirth = this.profileForm.value.spouseYearOfBirth;
      this.spouseDetails.spouseEmail = this.profileForm.value.spouseEmail;
      this.spouseDetails.spouseHometown = this.profileForm.value.spouseHometown;
      this.spouseDetails.spouseLastName = this.profileForm.value.spouseLastName;
      this.spouseDetails.spousePhoneNumber = this.profileForm.value.spousePhoneNumber;
    } else {
      this.spouseDetails = {} as SpouseDetails;
    }
    this.updateUserModel.spouseDetails = this.spouseDetails;

    for (let index = 0; index < this.profileForm?.value?.childrensDetails?.length; index++) {
      this.profileForm.value.childrensDetails[index].childCountry =
        this.childCountryName[index] ||
        this.profileForm.value.childrensDetails[index]?.childCountry?.name ||
        this.profileForm.value.childrensDetails[index]?.childCountry ||
        '';

      this.profileForm.value.childrensDetails[index].childCity =
        this.childCityName[index] ||
        this.profileForm.value.childrensDetails[index]?.childCity?.name ||
        this.profileForm.value.childrensDetails[index]?.childCity ||
        '';

      this.profileForm.value.childrensDetails[index].childState =
        this.childStateName[index] ||
        this.profileForm.value.childrensDetails[index]?.childState?.name ||
        this.profileForm.value.childrensDetails[index]?.childState ||
        '';

      if (!this.profileForm.value.childrensDetails[index].childUniqueId) {
        this.profileForm.value.childrensDetails[index].childUniqueId = this.orgId + '-' + this.userId + '-' + index;
      }
    }
    this.updateUserModel.childrensDetails = this.profileForm?.value?.childrensDetails;
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
      this.profileForm.patchValue({ country: e.value });

      this.profileForm.markAsDirty();
    } else {
      this.states = [];
      this.cities = [];
      this.profileForm.patchValue({ country: null });

      this.profileForm.markAsDirty();
    }
  }

  onChangeState(e: any) {
    if (e.value) {
      let cities = City.getCitiesOfState(e.value.countryCode, e.value.isoCode);
      this.stateName = e.value;
      this.cities = cities;
      this.profileForm.patchValue({ state: e.value });

      this.profileForm.markAsDirty();
    } else {
      this.cities = [];
      this.profileForm.patchValue({ state: null });

      this.profileForm.markAsDirty();
    }
  }

  onChangeCity(e: any) {
    if (e.value) {
      this.cityName = e.value;
      this.profileForm.patchValue({ city: e.value });
      this.profileForm.markAsDirty();
    } else {
      this.cityName = null;
      this.profileForm.patchValue({ city: null });
      this.profileForm.markAsDirty();
    }
  }

  onChangeSpouseCountry(e: any) {
    this.spouseStates = [];
    if (e.value) {
      let states = State.getStatesOfCountry(e.value.isoCode);
      this.spouseCountryName = e.value;
      this.spouseStates = states;
      this.profileForm.patchValue({ spouseCountry: e.value });
      this.profileForm.markAsDirty();
    } else {
      this.spouseStates = [];
      this.spouseCities = [];
      this.profileForm.patchValue({ spouseCountry: null });
      this.profileForm.markAsDirty();
    }
  }
  onChangeSpouseState(e: any) {
    this.spouseCities = [];
    if (e.value) {
      let cities = City.getCitiesOfState(e.value.countryCode, e.value.isoCode);
      this.spouseCities = cities;
      this.spouseStateName = e.value;
      this.profileForm.patchValue({ spouseState: e.value });
      this.profileForm.markAsDirty();
    } else {
      this.spouseCities = [];
      this.spouseStateName = null;
      this.profileForm.patchValue({ spouseState: null });
      this.profileForm.markAsDirty();
    }
  }
  onChangeSpouseCity(e: any) {
    if (e.value) {
      this.spouseCityName = e.value;
      this.profileForm.patchValue({ spouseCity: e.value });
      this.profileForm.markAsDirty();
    } else {
      this.spouseCityName = null;
      this.profileForm.patchValue({ spouseCity: null });
      this.profileForm.markAsDirty();
    }
    this.profileForm.get('spouseCity')?.updateValueAndValidity();
  }

  onChangeChildCountry(e: any, i: number) {
    const childFormGroup = this.childrensDetails.at(i) as FormGroup;
    if (e.value) {
      let states = State.getStatesOfCountry(e.value.isoCode);
      this.childCountryName[i] = e.value.name; // Store as string for simplicity
      this.childStates = states;
      childFormGroup.patchValue({ childCountry: e.value });
      this.profileForm.markAsDirty();
    } else {
      this.childStates = [];
      this.childCities = [];
      this.childCountryName[i] = '';
      childFormGroup.patchValue({ childCountry: null });
      this.profileForm.markAsDirty();
    }
  }

  onChangeChildState(e: any, i: number) {
    const childFormGroup = this.childrensDetails.at(i) as FormGroup;
    if (e.value) {
      let cities = City.getCitiesOfState(e.value.countryCode, e.value.isoCode);
      this.childStateName[i] = e.value.name;
      this.childCities = cities;
      childFormGroup.patchValue({ childState: e.value });
      this.profileForm.markAsDirty();
    } else {
      this.childCities = [];
      this.childStateName[i] = '';
      childFormGroup.patchValue({ childState: null });
      this.profileForm.markAsDirty();
    }
  }

  onChangeChildCity(e: any, i: number) {
    const childFormGroup = this.childrensDetails.at(i) as FormGroup;
    if (e.value) {
      this.childCityName[i] = e.value.name;
      childFormGroup.patchValue({ childCity: e.value });
      this.profileForm.markAsDirty();
    } else {
      this.childCityName[i] = '';
      childFormGroup.patchValue({ childCity: null });
      this.profileForm.markAsDirty();
    }
    childFormGroup.get('childCity')?.updateValueAndValidity();
  }

  isSaveButtonDisabled(): boolean {
    console.log('Form valid:', this.profileForm.valid);
    console.log('Form dirty:', this.profileForm.dirty);
    console.log(
      'Invalid controls:',
      Object.keys(this.profileForm.controls).filter(key => this.profileForm.get(key)?.invalid)
    );
    // Check if edit mode is active and if the form is pristine
    if (!this.isEditClicked || this.profileForm.pristine) {
      return true;
    }

    // Check if the main form is invalid
    if (this.profileForm.invalid) {
      return true;
    }

    // Explicitly check each child form group's validity
    const allChildrenValid = this.childrensDetails.controls.every((child: any) => {
      const childFirstNameControl = child.get('childFirstName');
      return childFirstNameControl?.valid && childFirstNameControl?.value.trim() !== '';
    });
    return !allChildrenValid;
  }

  processFile(_event: any): void {
    const reader = new FileReader();
    if (_event.target.files && _event.target.files.length) {
      const [file] = _event.target.files;
      reader.readAsDataURL(file);
      reader.onload = () => {
        this.profileForm.patchValue({
          Photo: reader.result,
        });
      };
    }
    this.selectedFile = <File>_event.target.files[0];
  }

  onEditClick() {
    this.isEditClicked = true;
    this.disableChildFlag = new Array(this.userDetail.childrensDetails.length).fill(false);
  }

  updateDetails() {
    if (this.profileForm.invalid) return;
    this.composeModel();
    this.commonService.showInfo('Profile Saving in Progress');

    this.subscriptions.push(
      this.userprofileService.updateUser(this.updateUserModel, this.selectedFile, this.orgId).subscribe({
        next: () => {
          this.onReject();
          this.commonService.showSuccess('Profile Updated Successfully');
        },
        error: () => {
          this.onReject();
          this.commonService.showError();
        },
      })
    );
  }
}
