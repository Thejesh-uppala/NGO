import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { AuthLayoutRoutes } from './auth-layout.routing';
import { AuthComponent } from 'src/app/login/auth.component';
import { RegistrationComponent } from 'src/app/registration/registration.component';
import { SharedModule } from 'src/app/shared/shared.module';
import { ForgotPasswordComponent } from 'src/app/forgot-password/forgot-password.component';
@NgModule({
  imports: [
    CommonModule,
    RouterModule.forChild(AuthLayoutRoutes),
    FormsModule,
    SharedModule,
    ReactiveFormsModule,
    // NgbModule
  ],
  declarations: [
    // AuthComponent,
    // RegistrationComponent,
    // ForgotPasswordComponent
  ]
})
export class AuthLayoutModule { }
