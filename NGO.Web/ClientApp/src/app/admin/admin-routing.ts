import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '../core/guards/auth.guard';
import { AdminEventsComponent } from './admin-events/admin-events.component';
import { AdminMatrimonyComponent } from './admin-matrimony/admin-matrimony.component';
import { AdminComponent } from './admin.component';
import { MembershipsComponent } from './memberships/memberships.component';
import { VerifyMemberComponent } from './verify-member/verify-member.component';

const routes: Routes = [
  {
    path: '',
    component: AdminComponent,
  },
  {
    path: 'admin-matrimony',
    canActivate: [AuthGuard],
    component: AdminMatrimonyComponent,
  },
  {
    path: 'verify-member',
    canActivate: [AuthGuard],
    component: VerifyMemberComponent,
  },
  {
    path: 'user-profile-details',
    canActivate: [AuthGuard],
    component: VerifyMemberComponent,
  },
  {
    path: 'admin-events',
    canActivate: [AuthGuard],
    component: AdminEventsComponent,
  },
  {
    path: 'memberships',
    canActivate: [AuthGuard],
    component: MembershipsComponent,
  },
  {
    path: 'admin',
    canActivate: [AuthGuard],
    component: AdminComponent,
  },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class AdminRoutingModule {}
