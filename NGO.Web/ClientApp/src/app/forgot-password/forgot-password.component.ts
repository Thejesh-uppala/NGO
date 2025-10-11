import { Location } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, ValidationErrors, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { CommonService } from '../shared/components/services/common.service';
import { ForgotPasswordPasswordService } from './services/forgot-password.service';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss'],
})
export class ForgotPasswordComponent implements OnInit {
  sendOtpForm!: FormGroup;
  otpValidationForm!: FormGroup;
  forgotPasswordForm!: FormGroup;
  emailId: string = '';
  orgId: any;
  Otp: string = '';
  detailText: string = '';
  subscriptions: Subscription[] = [];
  returnUrl: string = '';
  loggedIn: boolean = false;
  displayOTPForm: Boolean = false;
  displayNewPassword: Boolean = false;
  displayEmailInput: Boolean = true;
  errorMessage: string = '';
  error: boolean = false;
  currentUser: any;
  passwordVisible: boolean = false;
  confirmPasswordVisible: boolean = false;

  constructor(
    private forgotPasswordService: ForgotPasswordPasswordService,
    private router: Router,
    private formBuilder: FormBuilder,
    private location: Location,
    private messageService: MessageService,
    private commonService: CommonService // Inject CommonService
  ) {}

  ngOnInit(): void {
    this.orgId = parseInt(localStorage.getItem('orgId') || '0', 10);

    this.forgotPasswordForm = this.formBuilder.group(
      {
        newPassword: ['', [Validators.required, this.commonService.passwordValidator()]],
        confirmPassword: ['', [Validators.required]],
      },
      { validators: this.passwordMatchValidator }
    );

    this.sendOtpForm = this.formBuilder.group({
      emailId: ['', [Validators.required, Validators.email, this.commonService.noSpaceAtStartValidator()]],
    });

    this.otpValidationForm = this.formBuilder.group({
      otp: ['', [Validators.required, Validators.pattern('^[0-9]{6}$'), this.commonService.noSpaceAtStartValidator()]],
    });
  }

  goBack() {
    this.location.back();
  }

  togglePasswordVisibility(type: string) {
    if (type === 'password') {
      this.passwordVisible = !this.passwordVisible;
    } else {
      this.confirmPasswordVisible = !this.confirmPasswordVisible;
    }
  }

  passwordMatchValidator(form: FormGroup): ValidationErrors | null {
    const newPassword = form.get('newPassword')?.value;
    const confirmPassword = form.get('confirmPassword')?.value;
    return newPassword === confirmPassword ? null : { passwordMismatch: true };
  }
  otpValidate() {
    if (this.otpValidationForm.invalid) {
      this.error = true;
      this.errorMessage = 'Please enter a valid 6-digit OTP';
      return;
    }

    if (this.Otp !== this.otpValidationForm.value.otp) {
      this.error = true;
      this.errorMessage = 'Invalid OTP';
      return;
    }

    const otp = this.otpValidationForm.value.otp;
    this.subscriptions.push(
      this.forgotPasswordService.validateOTP(this.emailId, this.orgId.toString(), otp).subscribe({
        next: res => {
          if (res.message.toLowerCase() === 'otp matched') {
            this.error = false;
            this.displayOTPForm = false;
            this.displayNewPassword = true;
          } else {
            this.error = true;
            this.errorMessage = res.Error || 'Invalid OTP';
          }
        },
        error: err => {
          this.error = true;
          this.errorMessage = err.error?.Error || 'Error validating OTP';
        },
      })
    );
  }

  navigateToLogin(): void {
    this.router.navigate([`/login/${this.orgId}`]);
  }

  forgotPassword() {
    if (this.forgotPasswordForm.value.newPassword !== this.forgotPasswordForm.value.confirmPassword) {
      this.error = true;
      this.errorMessage = 'Confirm password is not matching new password';
      return;
    }
    this.subscriptions.push(
      this.forgotPasswordService
        .changePassWord(this.emailId, this.orgId, this.forgotPasswordForm.value.newPassword)
        .subscribe({
          next: () => {
            this.commonService.showSuccess('Password Changed Successfully');
            this.error = false;
            this.router.navigate([`/login/${this.orgId}`]);
          },
          error: () => {
            this.commonService.showError();
            this.forgotPasswordForm.reset();
          },
        })
    );
  }

  sendOtp() {
    if (this.sendOtpForm.invalid) {
      this.error = true;
      this.errorMessage = 'Please enter a valid email address';
      return;
    }

    this.commonService.showInfo('OTP Sending in Progress');
    const storedOrgId = localStorage.getItem('orgId');
    if (!storedOrgId) {
      this.commonService.showError('Organization ID not found. Please log in again.');
      return;
    }

    this.orgId = storedOrgId;
    this.emailId = this.sendOtpForm.value.emailId;
    this.subscriptions.push(
      this.forgotPasswordService.sendOTP(this.sendOtpForm.value.emailId, this.orgId).subscribe({
        next: res => {
          if (res.isSuccessStatusCode && res.statusCode == 200) {
            this.commonService.showSuccess('Otp has sent to ' + this.sendOtpForm.value.emailId);
            this.displayEmailInput = false;
            this.displayOTPForm = true;
            this.Otp = res.reasonPhrase;
          } else if (res.statusCode == 500) {
            this.commonService.showError('Error in sending OTP');
            this.displayNewPassword = false;
          } else {
            this.commonService.showError(res.reasonPhrase);
            this.sendOtpForm.reset();
          }
        },
        error: () => {
          this.commonService.showError();
          this.sendOtpForm.reset();
        },
      })
    );
  }
}
