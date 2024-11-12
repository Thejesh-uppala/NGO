import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ButtonModule } from 'primeng/button';
import { RegistersecondRoutingModule } from './registersecond-routing';
import { RegistersecondComponent } from './registersecond.component';
import { DashboardIconComponent } from '../Icons/dashboardicon';
import { NotificationIconComponent } from '../Icons/notificationicon';
import { MatrimonyIconComponent } from '../Icons/matrimonyicon';
import { EventsIconComponent } from '../Icons/eventsicon';
import { MembershipIconComponent } from '../Icons/membershipicon';
import { TableModule } from 'primeng/table';
import { ProgressBarModule } from 'primeng/progressbar';
import { TagModule } from 'primeng/tag';
import { DropdownModule } from 'primeng/dropdown';
import { MultiSelectModule } from 'primeng/multiselect';
import { ToolbarModule } from 'primeng/toolbar';
import { ToastModule } from 'primeng/toast';
import { DialogModule } from 'primeng/dialog';
import { FileUploadModule } from 'primeng/fileupload';
import { InputTextModule } from 'primeng/inputtext';
import { RatingModule } from 'primeng/rating';
import { RadioButtonModule } from 'primeng/radiobutton';
import { InputNumberModule } from 'primeng/inputnumber';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { FormsModule } from '@angular/forms'; // For ngModel binding

@NgModule({
  declarations: [
    RegistersecondComponent,
    DashboardIconComponent,
    NotificationIconComponent,
    MatrimonyIconComponent,
    EventsIconComponent,
    MembershipIconComponent
  ],
  imports: [
    CommonModule,
    RegistersecondRoutingModule,
    ButtonModule,
    TableModule,
    ProgressBarModule,
    TagModule,
    DropdownModule,
    MultiSelectModule,
    ToolbarModule,
    ToastModule,
    DialogModule,
    FileUploadModule,
    InputTextModule,
    RatingModule,
    RadioButtonModule,
    InputNumberModule,
    ConfirmDialogModule,
    FormsModule // For ngModel binding
  ]
})
export class RegistersecondModule { }
