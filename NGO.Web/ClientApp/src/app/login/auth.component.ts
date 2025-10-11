import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { MessageService } from 'primeng/api';
import { first } from 'rxjs';
import { AuthenticationService } from '../core/services/authentication.service';
import { CommonService } from '../shared/components/services/common.service';
import { loginSuccess } from '../store/user/user.actions';
import { UserState } from '../store/user/user.model';

@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss'],
})
export class AuthComponent implements OnInit {
  loginForm!: FormGroup;
  returnUrl: string = '';
  loggedIn: boolean = false;
  submitted: Boolean = false;
  errorMessage: string = '';
  error = '';
  currentUser: any;
  orgId: string = '';
  passwordVisible: boolean = false;

  brandImage: string = '';
  brandLogo: string = '';
  brandColor: string = '';
  orgName: string = '';
  orgWelMsg: string = '';

  constructor(
    private router: Router,
    private authenticationService: AuthenticationService,
    private messageService: MessageService,
    private formBuilder: FormBuilder,
    private store: Store,
    private commonService: CommonService,
    private route: ActivatedRoute
  ) {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
  }

  ngOnInit(): void {
    this.returnUrl = '/home';

    this.orgId = this.route.snapshot.paramMap.get('orgId') || '';
    // Store orgId in localStorage
    if (this.orgId) {
      localStorage.setItem('orgId', this.orgId);
      this.loadBrandDetails(this.orgId);
    }

    this.loginForm = this.formBuilder.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  // convenience getter for easy access to form fields
  get f(): { [key: string]: AbstractControl } {
    return this.loginForm.controls;
  }
  forgotPassword() {
    this.router.navigateByUrl('/forgotPassword');
  }

  onConfirm() {
    this.messageService.clear('c');
  }

  onReject() {
    this.messageService.clear('c');
  }
  loadBrandDetails(orgId: string): void {
    this.authenticationService.getBrandDetails(orgId).subscribe(
      data => {
        this.brandImage = data.brandImage || ''; // Fallback to empty string if undefined
        this.brandColor = data.brandColor || ''; // Fallback to empty string if undefined
        this.orgName = data.orgName || '';
        this.orgWelMsg = data.orgWelMsg || '';
        this.brandLogo = data.brandLogo || ''; // Optional welcome message
      },
      () => {}
    );
  }
  show() {
    this.commonService.showError('Invalid Link! Please check the URL & retry');
  }

  login(): void {
    if (this.orgId === '0') {
      this.show();
    }
    if (this.submitted) return; // Prevent double submission

    this.submitted = true;
    this.errorMessage = ''; // Clear any previous error messages

    if (this.loginForm.invalid) {
      this.submitted = false;
      return;
    }

    const storedOrgId = localStorage.getItem('orgId') || this.orgId;
    const { username, password } = this.loginForm.value;

    this.authenticationService
      .login(username.toLowerCase(), password, storedOrgId)
      .pipe(first())
      .subscribe({
        next: data => {
          if (data.error) {
            this.handleLoginError(data.error);
            return;
          }
          this.handleLoginSuccess(data);
        },
        error: error => {
          this.handleLoginError(error);
        },
      });
  }

  private handleLoginSuccess(data: any): void {
    this.loggedIn = true;
    this.authenticationService.setUserContext(data);

    const userState: UserState = {
      userId: data.userId,
      name: data.name,

      token: data.token,
      tokenExpiryDate: data.tokenExpiryDate,
      userRoles: data.userRoles,
      organizations: data.organizations,
      error: null,
    };

    this.store.dispatch(loginSuccess({ user: userState }));
    this.commonService.showSuccess('Login Successful!');

    const isAdmin = data.userRoles.some((role: { roleName: string }) => role.roleName === 'Admin');
    this.router.navigate([isAdmin ? '/admin' : '/home']);
  }

  togglePasswordVisibility() {
    this.passwordVisible = !this.passwordVisible;
  }

  private handleLoginError(error: any): void {
    this.submitted = false;
    this.errorMessage = typeof error === 'string' ? error : 'Login failed. Please try again.';
    this.commonService.showError(this.errorMessage);

    // Ensure orgId is included in the URL after login failure
    this.router.navigate([`/login/${this.orgId}`]);

    this.loginForm.patchValue({ password: '' });
  }
}
