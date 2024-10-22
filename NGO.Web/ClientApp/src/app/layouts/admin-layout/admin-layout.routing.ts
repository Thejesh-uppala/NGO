import { Routes } from '@angular/router';
import { AdminComponent } from 'src/app/admin/admin.component';
import { ChangePasswordComponent } from 'src/app/change-password/change-password.component';
import { ContactUsComponent } from 'src/app/contact-us/contact-us.component';
import { AuthGuard } from 'src/app/core/guards/auth.guard';
import { DashBoardComponent } from 'src/app/dashboard/dashboard.component';
import { UserprofileComponent } from 'src/app/userprofile/userprofile.component';


export const AdminLayoutRoutes: Routes = [
    {
        path: 'home',
        canActivate: [AuthGuard],
        component: DashBoardComponent
    },
    {
        path: '',
        component: DashBoardComponent
    },
    {
        path: 'user-profile',
        canActivate: [AuthGuard],
        component: UserprofileComponent
    },
    {
        path: 'admin',
        canActivate: [AuthGuard],
        component: AdminComponent
    },
    {
        path: 'contact-us',
        canActivate: [AuthGuard],
        component: ContactUsComponent
    },
    {
        path: 'changePassword',
        canActivate: [AuthGuard],
        component: ChangePasswordComponent
    },
];
