import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { first } from 'rxjs';
import { AuthenticationService } from '../core/services/authentication.service';
import { ToastService } from '../shared/components/toast/toast.service';
import { ChangePasswordModel } from './models/changePassword';
import { ChangePasswordService } from './services/changepasswordService';

@Component({
  selector: 'app-change-password',
  templateUrl: './change-password.component.html',
  styleUrls: ['./change-password.component.scss']
})
export class ChangePasswordComponent implements OnInit {
  resetPasswordForm!: FormGroup;
  returnUrl: string = '';
  loggedIn: boolean = false;
  submitted: Boolean = false;
  errorMessage: string = ""
  error = "";
  currentUser: any
  resetPasswordModel = {} as ChangePasswordModel;
  constructor(private changePasswordService: ChangePasswordService,
    private authenticationService: AuthenticationService,
    private messageService: MessageService, private router: Router,
    private formBuilder: FormBuilder) {
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
  }

  ngOnInit(): void {
    this.resetPasswordForm = this.formBuilder.group({
      oldPassword: ["", Validators.required],
      newPassword: ["", Validators.required],
    });
  }
  showSuccess() {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Password Changed' });
  }
  showError() {
    this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Something Went Wrong!' });
  }
  onConfirm() {
    this.messageService.clear('c');
  }

  onReject() {
    this.messageService.clear('c');
  }
  resetCreadentials() {
    debugger
    this.submitted = true;
    if (this.resetPasswordForm.invalid)
      return;
    const oldPassword = this.resetPasswordForm.value.oldPassword;
    const newPassword = this.resetPasswordForm.value.newPassword;
    const userName = this.currentUser.email;
    this.resetPasswordModel.userName = userName;
    this.resetPasswordModel.password = newPassword;
    this.resetPasswordModel.oldPassword = oldPassword;
    this.changePasswordService
      .resetPassword(this.resetPasswordModel)
      .subscribe(
        {
          next: (data) => {
            if (data.statusCode == 200) {
              this.showSuccess();
            } else if (data != 200) {
              if (data.reasonPhrase == "Old Password is Incorrect!") {
                this.errorMessage = data.reasonPhrase;
              }
            }
            this.resetPasswordForm.reset();
            this.errorMessage = data.error;
            this.submitted = false;
            this.resetPasswordForm.reset();

            if (this.errorMessage != null) {
              this.error = this.errorMessage;
            }
          },
          error: () => {
            this.showError();
            this.resetPasswordForm.reset();
          }
        },
      )
  }
}
