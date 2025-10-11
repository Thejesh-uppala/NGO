import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { FileUploadModule } from 'primeng/fileupload';
import { InputNumberModule } from 'primeng/inputnumber';
import { InputTextModule } from 'primeng/inputtext';
import { MultiSelectModule } from 'primeng/multiselect';
import { RadioButtonModule } from 'primeng/radiobutton';
import { RatingModule } from 'primeng/rating';
import { TableModule } from 'primeng/table';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { ToolbarModule } from 'primeng/toolbar';
import { SharedModule } from '../shared/shared.module'; // For ngModel binding
import { AdminEventsComponent } from './admin-events/admin-events.component';
import { AdminMatrimonyComponent } from './admin-matrimony/admin-matrimony.component';
import { AdminRoutingModule } from './admin-routing';
import { AdminComponent } from './admin.component';

@NgModule({
  declarations: [AdminComponent, AdminMatrimonyComponent, AdminEventsComponent],
  imports: [
    CommonModule,
    AdminRoutingModule,
    ButtonModule,
    TableModule,
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
    FormsModule,
    SharedModule,
  ],
})
export class AdminModule {}
