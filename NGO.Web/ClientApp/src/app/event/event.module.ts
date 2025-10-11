import { CommonModule, DatePipe } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { CardModule } from 'primeng/card';
import { DataViewModule } from 'primeng/dataview';
import { FileUploadModule } from 'primeng/fileupload';
import { PaginatorModule } from 'primeng/paginator';
import { RadioButtonModule } from 'primeng/radiobutton';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { ToggleButtonModule } from 'primeng/togglebutton';
import { SharedModule } from '../shared/shared.module';
import { CreateEventComponent } from './create-event/create-event.component';
import { EditEventComponent } from './edit-event/edit-event.component';
import { EventDetailComponent } from './event-detail/event-detail.component';
import { EventRoutingModule } from './event-routing.module';
import { EventComponent } from './event.component';
import { PaidEventRegistrationComponent } from './paid-event-registration/paid-event-registration.component';
import { RegisteredUsersComponent } from './registered-users/registered-users.component';

@NgModule({
  declarations: [
    CreateEventComponent,
    EventComponent,
    PaidEventRegistrationComponent,
    EditEventComponent,

    RegisteredUsersComponent,
    EventDetailComponent,
  ],
  imports: [
    FormsModule,
    DataViewModule,
    ButtonModule,
    TagModule,
    ToggleButtonModule,
    PaginatorModule,
    EventRoutingModule,
    SharedModule,
    CommonModule,
    CardModule,
    TableModule,
    ToastModule,
    RadioButtonModule,
    CalendarModule,
    FileUploadModule,
    ReactiveFormsModule,
    FileUploadModule,
  ],
  providers: [DatePipe],
})
export class EventModule {}
