import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { RegistersecondRoutingModule } from './registersecond-routing';
import { RegistersecondComponent } from './registersecond.component';
import {DashboardIconComponent} from '../Icons/dashboardicon'
import {NotificationIconComponent} from '../Icons/notificationicon'
import {MatrimonyIconComponent} from '../Icons/matrimonyicon'
import {EventsIconComponent} from '../Icons/eventsicon'
import {MembershipIconComponent} from '../Icons/membershipicon'
import { TableModule } from 'primeng/table';
import { ProgressBarModule } from 'primeng/progressbar';
import { TagModule } from 'primeng/tag';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { InputTextModule } from 'primeng/inputtext';
@NgModule({
  declarations: [RegistersecondComponent,DashboardIconComponent,NotificationIconComponent,MatrimonyIconComponent,EventsIconComponent,MembershipIconComponent
  ],
  imports: 
  [CommonModule,
   RegistersecondRoutingModule,
  ButtonModule,
  TableModule,CommonModule,ButtonModule,ProgressBarModule,TagModule,DropdownModule,MultiSelectModule,InputTextModule],
})

export class RegistersecondModule { }