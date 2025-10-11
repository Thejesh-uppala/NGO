import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '../core/guards/auth.guard';
import { ContactUsComponent } from './contact-us.component';

const routes: Routes = [
  {
    path: 'contact-us',
    canActivate: [AuthGuard],
    component: ContactUsComponent,
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class ContactUsRoutingModule {}
