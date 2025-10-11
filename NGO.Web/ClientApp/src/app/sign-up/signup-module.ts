import { CommonModule } from '@angular/common';
import { NgModule, CUSTOM_ELEMENTS_SCHEMA } from '@angular/core';
import { SharedModule } from '../shared/shared.module';
import { SignUpComponent } from './sign-up.component';
import { SignUpRoutingModule } from './signup-routing';

@NgModule({
  declarations: [SignUpComponent],
  imports: [SignUpRoutingModule, CommonModule, SharedModule],
  providers: [],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class SignUpModule {}
