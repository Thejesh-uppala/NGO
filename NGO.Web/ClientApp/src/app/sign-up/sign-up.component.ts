import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { SigUpService } from './services/signupServices';

@Component({
  selector: 'app-sign-up',
  templateUrl: './sign-up.component.html',
  styleUrls: ['./sign-up.component.scss']
})
export class SignUpComponent implements OnInit {
  registrationForm!: FormGroup;
  subscriptions: Subscription[] = [];
  userMail: string = "";
  showRegistrationForm: boolean = false
  userName: string = ""
  phNumber: string = ""
  password: string = ""
  email: string = ""
  emailPattern:string = "^[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$";
  constructor(private formBuilder: FormBuilder,
    private router: Router,
    private route: ActivatedRoute,
    private sigupService: SigUpService,
    private messageService: MessageService) { }

  ngOnInit(): void {
    this.registrationForm = this.formBuilder.group({
      username: ["", Validators.required],
      password: ["", Validators.required],
      phNumber: ["", Validators.required],
      email: ["", [Validators.required,Validators.pattern(this.emailPattern)]],
    })
    this.route.queryParamMap.subscribe(params => console.log(params));
  }
  showSuccess() {
    this.messageService.add({ severity: 'success', summary: 'Success', detail: 'Registered Successfully' });
  }
  showError() {
    this.messageService.add({ severity: 'error', summary: 'Error', detail: 'Something Went Wrong!' });
  }
  showInfo() {
    this.messageService.add({ severity: 'info', summary: 'Info', detail: 'Registration in progress' });
  }
  onReject() {
    this.messageService.clear('c');
  }
  onConfirm() {
    this.messageService.clear('c');
  }
  signUp() {
    debugger
    this.showRegistrationForm = true
    this.userMail = this.registrationForm.value.username;
    this.password = this.registrationForm.value.password;
    this.phNumber = this.registrationForm.value.phNumber;
    this.email = this.registrationForm.value.email;
    //   if (this.registrationForm.invalid)
    //     return;
    //   this.showInfo();
    //   const userName = this.registrationForm.value.username;
    //   const password = this.registrationForm.value.password;
    //   const phNumber = this.registrationForm.value.phNumber;
    //   const email = this.registrationForm.value.email;
    //   this.subscriptions.push(
    //     this.sigupService
    //       .register(userName,password,phNumber,email).subscribe(
    //         {
    //           next: (res) => {
    //             if (res) {
    //               this.onReject();
    //               this.showSuccess();
    //               this.router.navigate(
    //                 ['/register'],
    //                 { queryParams: { order: res.email} }
    //               );
    //             }
    //           },
    //           error: () => {
    //             this.onReject();
    //             this.showError();
    //           }
    //         },
    //       )
    //   );
  }
}
