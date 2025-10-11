import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { ButtonModule } from 'primeng/button';
import { DataViewModule } from 'primeng/dataview';
import { FileUploadModule } from 'primeng/fileupload';
import { PaginatorModule } from 'primeng/paginator';
import { StepsModule } from 'primeng/steps';
import { TagModule } from 'primeng/tag';
import { ToastModule } from 'primeng/toast';
import { SharedModule } from '../shared/shared.module';
import { CreateMatrimonyProfileComponent } from './create-matrimony-profile/create-matrimony-profile.component';
import { MatrimonyProfileDetailsComponent } from './matrimony-profile-details/matrimony-profile-details.component';
import { MatrimonyRoutingModule } from './matrimony-routing.module';
import { MatrimonyComponent } from './matrimony.component';

@NgModule({
  declarations: [MatrimonyComponent, MatrimonyProfileDetailsComponent, CreateMatrimonyProfileComponent],
  imports: [
    FormsModule,
    DataViewModule,
    ButtonModule,
    PaginatorModule,
    TagModule,
    MatrimonyRoutingModule,
    SharedModule,
    CommonModule,
    FileUploadModule,
    ToastModule,
    ReactiveFormsModule,
    StepsModule,
  ],
})
export class MatrimonyModule {}
