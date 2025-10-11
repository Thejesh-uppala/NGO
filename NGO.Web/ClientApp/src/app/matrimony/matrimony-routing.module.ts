import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AuthGuard } from '../core/guards/auth.guard';
import { CreateMatrimonyProfileComponent } from './create-matrimony-profile/create-matrimony-profile.component';
import { MatrimonyProfileDetailsComponent } from './matrimony-profile-details/matrimony-profile-details.component';
import { MatrimonyComponent } from './matrimony.component';

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    component: MatrimonyComponent,
  },
  {
    path: 'matrimony-profile-details',
    canActivate: [AuthGuard],
    component: MatrimonyProfileDetailsComponent,
  },
  {
    path: 'create-matrimony-profile',
    canActivate: [AuthGuard],
    component: CreateMatrimonyProfileComponent,
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class MatrimonyRoutingModule {}
