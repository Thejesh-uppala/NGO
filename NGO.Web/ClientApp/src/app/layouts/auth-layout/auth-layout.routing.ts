import { Routes } from '@angular/router';
import { ForgotPasswordComponent } from 'src/app/forgot-password/forgot-password.component';
import { AuthComponent } from 'src/app/login/auth.component';
import { SignUpComponent } from 'src/app/sign-up/sign-up.component';

export const AuthLayoutRoutes: Routes = [
  { path: 'login', component: AuthComponent },
  { path: 'signup', component: SignUpComponent },
  { path: 'forgotPassword', component: ForgotPasswordComponent },
];
