import { Component, HostListener, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { Subscription } from 'rxjs';
import { CommonService } from '../shared/components/services/common.service';
import { ContactUsService, ContactFormData } from './services/contactUs.service';
@Component({
  selector: 'app-contact-us',
  templateUrl: './contact-us.component.html',
  styleUrls: ['./contact-us.component.scss'],
})
export class ContactUsComponent implements OnInit {
  contactForm!: FormGroup;
  disabledSubmitButton: boolean = true;
  orgId!: number;
  subscriptions: Subscription[] = [];

  @HostListener('input') oninput() {
    if (this.contactForm.valid) {
      this.disabledSubmitButton = false;
    }
  }

  subjects = [
    { label: 'Feedback', value: 'Feedback' },
    { label: 'Report a Bug', value: 'Report a bug' },
    { label: 'Feature Request', value: 'Feature request' },
    { label: 'Enquiry', value: 'Enquiry' },
  ];

  constructor(
    private fb: FormBuilder,
    private contactUsService: ContactUsService,
    private commonService: CommonService,
    private router: Router
  ) {}
  ngOnInit(): void {
    this.contactForm = this.fb.group({
      contactFormName: [
        '',
        [
          Validators.required,
          this.commonService.lettersOnlyValidator,
          this.commonService.noSpaceAtStartValidator(),
          this.commonService.valueRequiredAfterEntryValidator(),
          Validators.minLength(2),
          Validators.maxLength(50),
        ],
      ],
      contactFormEmail: [
        '',
        [
          Validators.required,
          Validators.email,
          Validators.maxLength(100),
          this.commonService.valueRequiredAfterEntryValidator(),
        ],
      ],
      contactFormSubjects: ['', Validators.required],
      contactFormMessage: [
        '',
        [
          Validators.required,
          this.commonService.descriptionvalidators(),
          this.commonService.valueRequiredAfterEntryValidator(),
          Validators.minLength(10),
        ],
      ],
    });

    this.commonService.getUserData().subscribe({
      next: userData => {
        this.orgId = userData.orgId;
      },
      error: () => {},
    });
  }
  goBack() {
    this.router.navigate(['/']);
  }
  onSubmit() {
    if (this.contactForm.valid) {
      const formData: ContactFormData = {
        ...this.contactForm.value,
        orgId: this.orgId,
      };

      this.contactUsService.sendMessage(formData).subscribe({
        next: () => {
          this.contactForm.reset();
          this.commonService.showSuccess('Thank you for Contacting us');
          setTimeout(() => {
            this.router.navigate(['/']);
          }, 2000);
        },
        error: () => {
          this.commonService.showError();
        },
      });
    }
  }
}
