import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';

import { RegistersecondRoutingModule } from './registersecond-routing';
import { RegistersecondComponent } from './registersecond.component';


@NgModule({
  declarations: [RegistersecondComponent],
  imports: 
  [CommonModule,
   RegistersecondRoutingModule],
})
export class RegistersecondModule { }
