import { Routes } from '@angular/router';
import { ForgotPasswordComponent } from 'src/app/forgot-password/forgot-password.component';
import { AuthComponent } from 'src/app/login/auth.component';
import { RegistrationComponent } from 'src/app/registration/registration.component';
import { SignUpComponent } from 'src/app/sign-up/sign-up.component';

export const AuthLayoutRoutes: Routes = [
    { path: 'login',          component: AuthComponent },
    { path: 'register',       component: RegistrationComponent },
    { path: 'signup',       component: SignUpComponent },
    { path: 'forgotPassword',       component: ForgotPasswordComponent },
];
