import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthenticationService } from '../core/services/authentication.service';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { first } from 'rxjs';
import { MessageService } from 'primeng/api';
@Component({
  selector: 'app-auth',
  templateUrl: './auth.component.html',
  styleUrls: ['./auth.component.scss']
})
export class AuthComponent implements OnInit {
  loginForm!: FormGroup;
  returnUrl: string = '';
  loggedIn: boolean = false;
  submitted: Boolean = false;
  errorMessage: string = ""
  error = "";
  currentUser: any;

  constructor(private router: Router,
    private authenticationService: AuthenticationService, private messageService: MessageService,
    private formBuilder: FormBuilder,
  ) {
    // if (this.authenticationService.currentUserValue) {
    //   this.router.navigate(["/"]);
    // }
    this.currentUser = JSON.parse(localStorage.getItem('currentUser') || '{}');
  }

  ngOnInit(): void {
    this.returnUrl = "/home";

    this.loginForm = this.formBuilder.group({
      username: ["", Validators.required],
      password: ["", Validators.required],
    });
  }

  // convenience getter for easy access to form fields
  get f(): { [key: string]: AbstractControl } {
    return this.loginForm.controls;
  }
  forgotPassword() {
    this.router.navigateByUrl("/forgotPassword");
  }
  showSuccess() {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Login Successfull' });
  }
  showError() {
    this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Login Failed!Please check credentials' });
  }
  onConfirm() {
    this.messageService.clear('c');
  }

  onReject() {
    this.messageService.clear('c');
  }

  login() {
    this.submitted = true;
    if (this.loginForm.invalid)
      return;
    const userName = this.loginForm.value.username;
    const passWord = this.loginForm.value.password;
    this.authenticationService
      .login(userName, passWord)
      .pipe(first())
      .subscribe(
        (data) => {
          if (data.error == null) {
            this.loggedIn = true;
            this.authenticationService.setUserContext(data);
            this.showSuccess();
            if(data.userRole=="Administrator"){
              this.router.navigate(["/admin"]);
            }else{
                this.router.navigate(["/home"]);
            }
          }
          this.errorMessage = data.error;
          this.submitted = false;
          this.loginForm.reset();

          if (this.errorMessage != null) {
            this.error = this.errorMessage;
          }
        },
        (error) => {
          this.showError();
          this.error = error;
          this.submitted = false;
          this.loginForm.reset();
        }
      );
  }
}
