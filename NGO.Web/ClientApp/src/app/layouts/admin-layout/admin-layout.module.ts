import { NgModule } from '@angular/core';
import { HttpClientModule } from '@angular/common/http';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';

import { ClipboardModule } from 'ngx-clipboard';
import { AdminLayoutRoutes } from './admin-layout.routing';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { DashBoardComponent } from 'src/app/dashboard/dashboard.component';
import { UserprofileComponent } from 'src/app/userprofile/userprofile.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { ContactUsComponent } from 'src/app/contact-us/contact-us.component';
import { AdminComponent } from 'src/app/admin/admin.component';
import { ChangePasswordComponent } from 'src/app/change-password/change-password.component';
import { ForgotPasswordComponent } from 'src/app/forgot-password/forgot-password.component';

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
  ],
  declarations: [
    DashBoardComponent,
    UserprofileComponent,
    ContactUsComponent,
    AdminComponent,
    ChangePasswordComponent
  ]
})

export class AdminLayoutModule {}
