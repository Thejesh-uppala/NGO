import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { ToastService } from '../shared/components/toast/toast.service';
import { ForgotPasswordPasswordService } from './services/forgot-password.service';

@Component({
  selector: 'app-forgot-password',
  templateUrl: './forgot-password.component.html',
  styleUrls: ['./forgot-password.component.scss']
})
export class ForgotPasswordComponent implements OnInit {
  sendOtpForm!: FormGroup;
  forgotPasswordForm!: FormGroup;
  emailId: string = "";
  Otp: string = "";
  detailText:string="";
  subscriptions: Subscription[] = [];
  returnUrl: string = '';
  loggedIn: boolean = false;
  displayNewPassword: Boolean = false;
  errorMessage: string = ""
  error:boolean = false;
  currentUser: any;

  constructor(private forgotPasswordService: ForgotPasswordPasswordService,
    private toastService: ToastService,
    private router: Router,
    private formBuilder: FormBuilder,
    private messageService:MessageService) { }

  ngOnInit(): void {
    this.forgotPasswordForm = this.formBuilder.group({
      newPassword: ["", Validators.required],
      otp: ["", Validators.required],
    });
    this.sendOtpForm = this.formBuilder.group({
      emailId: ["", Validators.required],
      otp: ["", Validators.required],
    });
  }
  showSuccess() {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: this.detailText });
  }
  showError() {
    this.messageService.add({ severity: 'error', summary: 'Error', detail: this.detailText });
  }
  showInfo() {
    this.messageService.add({ severity: 'info', summary: 'Info', detail:this.detailText });
  }

  onConfirm() {
    this.messageService.clear('c');
  }

  onReject() {
    this.messageService.clear('c');
  }
  forgotPassword() {
    debugger
    if(this.Otp!==this.forgotPasswordForm.value.otp)
    {
      this.error=true;
      this.errorMessage="Invalid OTP";
      return
    }
      
    this.subscriptions.push(
      this.forgotPasswordService
        .changePassWord(this.emailId,this.forgotPasswordForm.value.newPassword).subscribe(
          {
            next: (res) => {
              this.onReject();
              this.detailText="Password Changed Successfully"
              this.showSuccess();
              this.displayNewPassword = true;
              this.error=false;
              this.forgotPasswordForm.reset();
              this.router.navigateByUrl("login");
            },
            error: (err) => {
              this.showError();
              this.forgotPasswordForm.reset();
            }
          },
        )
    );
 
  }
  sendOtp() {
    debugger
    if (this.sendOtpForm.value.emailId == null)
      return;
    this.detailText="OTP Sending in Progress"  
    this.showInfo();
    this.emailId = this.sendOtpForm.value.emailId;
    this.subscriptions.push(
      this.forgotPasswordService
        .sendOTP(this.sendOtpForm.value.emailId).subscribe(
          {
            next: (res) => {
              this.onReject();
             if(res.isSuccessStatusCode&&res.statusCode==200){
              this.detailText="Otp has sent to "+this.sendOtpForm.value.emailId;
              this.showSuccess();
              this.displayNewPassword = true;
              this.Otp=res.reasonPhrase;
             }else if(res.statusCode==500){
              this.detailText="Error in sending OTP";
              this.showError();
              this.displayNewPassword = false;
             }
            },
            error: (err) => {
              this.onReject();
              this.showError();
              this.sendOtpForm.reset();
            },
          },
        )
    );
    
  }
}

