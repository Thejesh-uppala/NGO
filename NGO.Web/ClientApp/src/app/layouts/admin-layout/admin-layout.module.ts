import { CommonModule } from '@angular/common';
import { HttpClientModule } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';

import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ClipboardModule } from 'ngx-clipboard';
import { AdminModule } from 'src/app/admin/admin-module';
import { ChangePasswordComponent } from 'src/app/change-password/change-password.component';
import { ContactUsComponent } from 'src/app/contact-us/contact-us.component';
import { DashBoardComponent } from 'src/app/dashboard/dashboard.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { UserprofileComponent } from 'src/app/userprofile/userprofile.component';
import { AdminLayoutRoutes } from './admin-layout.routing';

@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(AdminLayoutRoutes),
    FormsModule,
    HttpClientModule,
    NgbModule,
    ClipboardModule,
    CommonModule,
    SharedModule,
    ReactiveFormsModule,
    AdminModule,
  ],
  declarations: [DashBoardComponent, UserprofileComponent, ContactUsComponent, ChangePasswordComponent],
})
export class AdminLayoutModule {}
