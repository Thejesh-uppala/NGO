import { Component, HostListener, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Subscription } from 'rxjs';
import { ContactUsService } from './services/contactUs.service';

@Component({
  selector: 'app-contact-us',
  templateUrl: './contact-us.component.html',
  styleUrls: ['./contact-us.component.scss']
})
export class ContactUsComponent implements OnInit {
  contactForm!: FormGroup;
  disabledSubmitButton: boolean = true;

  subscriptions: Subscription[] = [];

  @HostListener('input') oninput() {

    if (this.contactForm.valid) {
      this.disabledSubmitButton = false;
    }
  }

  constructor(private fb: FormBuilder,private contactUsService: ContactUsService) {

    
  }
  ngOnInit(): void {
    this.contactForm = this.fb.group({
      'contactFormName': ['', Validators.required],
      'contactFormEmail': ['', Validators.compose([Validators.required, Validators.email])],
      'contactFormSubjects': ['', Validators.required],
      'contactFormMessage': ['', Validators.required],
      'contactFormCopy': [''],
    });
  }

  onSubmit() :void{
    debugger
    if (this.contactForm.invalid)
    return;
    const userName = this.contactForm.value.username;
  this.subscriptions.push(
    this.contactUsService
      .sendMessage(this.contactForm.value).subscribe(
        {
          next: ()=>{
           // localStorage.removeItem('currentUser');
            //this.router.navigate(["/login"]);
          },
          error: ()=>{

          }
        },
      )
    );
  }
}
