import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { City, Country, State } from 'country-state-city';
import { MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { AuthenticationService } from '../core/services/authentication.service';
import { CommonService } from '../shared/components/services/common.service';
import { loginSuccess } from '../store/user/user.actions';
import { UserState } from '../store/user/user.model';
import { SigUpService } from './services/signupServices';
@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss'],
})
export class SignUpComponent implements OnInit {
  signupForm!: FormGroup;
  subscriptions: Subscription[] = [];
  userMail: string = '';
  showsignupForm: boolean = false;
  userName: string = '';
  phNumber: string = '';
  password: string = '';
  email: string = '';
  firstName: string = '';
  lastName: string = '';
  address: string = '';
  city: string = '';
  country: string = '';
  state: string = '';
  orgId: string = '';
  chapterId: number = 0;
  chapters: any[] = [];
  countries: any;
  countryName: { name: string } | null = null; // Simplified to match profile component
  stateName: { name: string } | null = null;
  cityName: { name: string } | null = null;
  states: any;
  cities: any;

  passwordVisible: boolean = false;

  constructor(
    private formBuilder: FormBuilder,
    private router: Router,
    private authenticationService: AuthenticationService,
    private store: Store,
    private location: Location,
    private route: ActivatedRoute,
    private commonService: CommonService,
    private sigupService: SigUpService,
    private messageService: MessageService
  ) {}

  ngOnInit(): void {
    let pathOrgId = this.route.snapshot.paramMap.get('orgId');

    if (pathOrgId) {
      localStorage.setItem('orgId', pathOrgId);
      this.orgId = pathOrgId;
    } else {
      this.orgId = localStorage.getItem('orgId') || '';
    }
    this.countries = Country.getAllCountries();
    this.signupForm = this.formBuilder.group({
      password: [
        '',
        [Validators.required, this.commonService.noSpaceAtStartValidator(), this.commonService.passwordValidator()],
      ],
      phNumber: ['', [this.commonService.phoneValidator()]],
      email: ['', [Validators.required, Validators.email]],
      firstName: [
        '',
        [Validators.required, this.commonService.lettersOnlyValidator, this.commonService.noSpaceAtStartValidator()],
      ],
      lastName: ['', [this.commonService.lettersOnlyValidator]],
      address: [''],
      city: [null, Validators.required],
      country: [null, Validators.required],
      state: [null, Validators.required],
      chapterId: ['', Validators.required],
    });

    this.signupForm.valueChanges.subscribe(value => {
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
    // Fetch chapters from the API
    this.fetchChapters();
  }

  togglePasswordVisibility() {
    this.passwordVisible = !this.passwordVisible;
  }
  fetchChapters() {
    const storedOrgId = localStorage.getItem('orgId') || '0'; // Default to '1' if not found
    this.sigupService.getChapters(storedOrgId).subscribe(
      response => {
        this.chapters = response;
      },
      () => {}
    );
  }

  goBack() {
    this.location.back();
  }

  onChangeCountry(e: any) {
    if (e.value) {
      let states = State.getStatesOfCountry(e.value.isoCode);
      this.countryName = e.value;
      this.states = states;

      // Reset state and city when country changes
      this.signupForm.patchValue({
        country: e.value,
        state: null,
        city: null,
      });

      this.cities = []; // Reset city list
    } else {
      this.countryName = null;
      this.states = [];
      this.cities = [];

      this.signupForm.patchValue({
        country: null,
        state: null,
        city: null,
      });
    }

    this.signupForm.markAsDirty();
  }

  onChangeState(e: any) {
    if (e.value) {
      let cities = City.getCitiesOfState(e.value.countryCode, e.value.isoCode);
      this.stateName = e.value;
      this.cities = cities;

      // Reset city when state changes
      this.signupForm.patchValue({
        state: e.value,
        city: null,
      });
    } else {
      this.stateName = null;
      this.cities = [];

      this.signupForm.patchValue({
        state: null,
        city: null,
      });
    }

    this.signupForm.markAsDirty();
  }

  onChangeCity(e: any) {
    if (e.value) {
      this.cityName = e.value;
      this.signupForm.patchValue({ city: e.value });
      this.signupForm.markAsDirty();
    } else {
      this.cityName = null;
      this.signupForm.patchValue({ city: null });
      this.signupForm.markAsDirty();
    }
  }

  signUp() {
    const storedOrgId = localStorage.getItem('orgId');

    // Display the form and initialize the signup data
    this.userMail = this.signupForm.value.username;
    this.password = this.signupForm.value.password;
    this.phNumber = this.signupForm.value.phNumber;
    this.email = this.signupForm.value.email.toLowerCase();
    this.firstName = this.signupForm.value.firstName;
    this.lastName = this.signupForm.value.lastName;
    this.address = this.signupForm.value.address;
    (this.city = this.cityName?.name || this.signupForm.value.city || ''),
      (this.country = this.countryName?.name || this.signupForm.value.country || ''),
      (this.state = this.stateName?.name || this.signupForm.value.state || ''),
      (this.chapterId = this.signupForm.value.chapterId); // Get chapterId from the form
    if (storedOrgId) {
      this.orgId = storedOrgId;
    }
    this.commonService.showInfo('Creating an Account...');
    // Call the sign-up service with individual parameters
    this.sigupService
      .signup(
        this.password,
        this.phNumber,
        this.email,
        this.firstName,
        this.lastName,
        this.address,
        this.city,
        this.country,
        this.state,
        this.orgId,
        this.chapterId
      )
      .subscribe(
        response => {
          this.commonService.showSuccess('Welcome!');
          localStorage.setItem('currentUser', JSON.stringify(response));
          this.authenticationService.setUserContext(response);

          const userState: UserState = {
            userId: response.userId,
            name: response.name, // `userName` in AuthModel mapped to `name` in UserState
            token: response.token,
            tokenExpiryDate: response.tokenExpiryDate,
            userRoles: response.userRoles,
            organizations: response.organizations,
            error: null, // Error is handled separately
          };

          this.store.dispatch(loginSuccess({ user: userState }));
          this.router.navigate(['/home']);
        },
        err => {
          this.commonService.showError(err.Message);

          this.signupForm.reset();
        }
      );
  }
}
