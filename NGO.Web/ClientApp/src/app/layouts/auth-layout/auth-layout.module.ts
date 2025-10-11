import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { SharedModule } from 'src/app/shared/shared.module';
import { AuthLayoutRoutes } from './auth-layout.routing';
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
  ],
})
export class AuthLayoutModule {}
