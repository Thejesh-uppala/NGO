import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { MessageService } from 'primeng/api';
import { Observable } from 'rxjs';
import { AuthenticationService } from '../core/services/authentication.service';
import { CommonService } from '../shared/components/services/common.service';
import { ErrorMessages } from '../shared/enums/enums';
import { UserState } from '../store/user/user.model';
import { selectCurrentUser } from '../store/user/user.selectors';
import { ChangePasswordModel } from './models/changePassword';
import { ChangePasswordService } from './services/changepasswordService';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss'],
})
export class ChangePasswordComponent implements OnInit {
  resetPasswordForm!: FormGroup;
  userData!: UserState | null;

  user$!: Observable<UserState | null>;
  returnUrl: string = '';
  loggedIn: boolean = false;
  submitted: boolean = false;
  errorMessage: string = '';
  error = '';
  currentUser: any;
  resetPasswordModel = {} as ChangePasswordModel;
  orgId: any;
  userId: any;
  passwordVisible: boolean = false;
  confirmPasswordVisible: boolean = false;
  constructor(
    private changePasswordService: ChangePasswordService,
    private authenticationService: AuthenticationService,
    private commonService: CommonService,
    private store: Store<UserState>,
    private messageService: MessageService,
    private formBuilder: FormBuilder,
    private router: Router
  ) {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
  }

  ngOnInit(): void {
    this.user$ = this.store.select(selectCurrentUser);

    // Subscribe to the observable to log the user state
    this.user$.subscribe(userState => {
      this.userData = userState;
      this.userId = userState?.userId;
    });

    if (this.userData && this.userId) {
      const decodedToken = this.commonService.decodeToken(this.userData?.token);
      this.userId = decodedToken?.UserId;
      const organizations = JSON.parse(decodedToken?.Organizations || '[]');
      if (this.userId && organizations.length > 0) {
        this.orgId = organizations[0].Id;
      }
    }

    this.resetPasswordForm = this.formBuilder.group({
      oldPassword: [
        '',
        [
          Validators.required,
          this.commonService.passwordValidator(),
          this.commonService.valueRequiredAfterEntryValidator(),
        ],
      ],
      newPassword: [
        '',
        [
          Validators.required,
          this.commonService.passwordValidator(),
          this.commonService.valueRequiredAfterEntryValidator(),
        ],
      ],
    });
  }

  goBack() {
    this.router.navigate(['/']);
  }

  resetCreadentials() {
    this.submitted = true;
    if (this.resetPasswordForm.invalid) return;
    const oldPassword = this.resetPasswordForm.value.oldPassword;
    const newPassword = this.resetPasswordForm.value.newPassword;
    this.resetPasswordModel.userId = this.userId;
    this.resetPasswordModel.password = newPassword;
    this.resetPasswordModel.oldPassword = oldPassword;

    this.changePasswordService.resetPassword(this.orgId, this.resetPasswordModel).subscribe({
      next: () => {
        this.commonService.showSuccess('Password Resetted Successfully');

        this.resetPasswordForm.reset();
        this.submitted = false;

        this.router.navigate(['/']);
      },
      error: error => {
        if (error.error.toLocaleLowerCase() == ErrorMessages.OLD_PASSWORD_INCORRECT.toString()) {
          this.commonService.showError(error.error);
        }

        this.resetPasswordForm.reset();
        this.submitted = false;
      },
    });
  }

  togglePasswordVisibility(type: string) {
    if (type === 'password') {
      this.passwordVisible = !this.passwordVisible;
    } else {
      this.confirmPasswordVisible = !this.confirmPasswordVisible;
    }
  }
  isResetButtonDisabled(): boolean {
    return this.resetPasswordForm.invalid || this.submitted;
  }
}
