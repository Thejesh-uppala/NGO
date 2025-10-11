import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '../core/guards/auth.guard';
import { DashBoardComponent } from './dashboard.component';

const routes: Routes = [
  {
    path: '',
    component: DashBoardComponent,
  },
  {
    path: 'home',
    canActivate: [AuthGuard],
    component: DashBoardComponent,
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class DashBoardRoutingModule {}
