import { Routes } from '@angular/router';
import { AdminComponent } from 'src/app/admin/admin.component';
import { ChangePasswordComponent } from 'src/app/change-password/change-password.component';
import { ContactUsComponent } from 'src/app/contact-us/contact-us.component';
import { AuthGuard } from 'src/app/core/guards/auth.guard';
import { DashBoardComponent } from 'src/app/dashboard/dashboard.component';
import { MatrimonyComponent } from 'src/app/matrimony/matrimony.component';
import { RegistrationComponent } from 'src/app/registration/registration.component';
import { UserprofileComponent } from 'src/app/userprofile/userprofile.component';

export const AdminLayoutRoutes: Routes = [
  {
    path: 'home',
    canActivate: [AuthGuard],
    component: DashBoardComponent,
  },
  {
    path: '',
    canActivate: [AuthGuard],
    component: DashBoardComponent,
  },
  { path: 'register', canActivate: [AuthGuard], component: RegistrationComponent },
  {
    path: 'user-profile',
    canActivate: [AuthGuard],
    component: UserprofileComponent,
  },
  {
    path: 'admin',
    canActivate: [AuthGuard],
    component: AdminComponent,
  },
  {
    path: 'matrimony',
    canActivate: [AuthGuard],
    component: MatrimonyComponent,
  },
  {
    path: 'contact-us',
    canActivate: [AuthGuard],
    component: ContactUsComponent,
  },
  {
    path: 'change-password',
    canActivate: [AuthGuard],
    component: ChangePasswordComponent,
  },
];
