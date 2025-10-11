import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { CalendarModule } from 'primeng/calendar';
import { RadioButtonModule } from 'primeng/radiobutton';
import { SharedModule } from '../shared/shared.module';
import { RegisterRoutingModule } from './registration-routing';
import { RegistrationComponent } from './registration.component';

@NgModule({
  declarations: [RegistrationComponent],
  imports: [RegisterRoutingModule, CommonModule, SharedModule, RadioButtonModule, CalendarModule],
  providers: [],
})
export class RegisterModule {}
