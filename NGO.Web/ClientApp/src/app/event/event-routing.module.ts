import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';

import { AuthGuard } from '../core/guards/auth.guard';
import { CreateEventComponent } from './create-event/create-event.component';
import { EditEventComponent } from './edit-event/edit-event.component';
import { EventDetailComponent } from './event-detail/event-detail.component';
import { EventComponent } from './event.component';
import { PaidEventRegistrationComponent } from './paid-event-registration/paid-event-registration.component';
import { RegisteredUsersComponent } from './registered-users/registered-users.component';

const routes: Routes = [
  {
    path: '',
    canActivate: [AuthGuard],
    component: EventComponent,
  },
  {
    path: 'event-registration/:eventId',
    canActivate: [AuthGuard],
    component: PaidEventRegistrationComponent,
  },
  {
    path: 'registered-users/:eventId',
    canActivate: [AuthGuard],
    component: RegisteredUsersComponent,
  },
  {
    path: 'create-event',
    canActivate: [AuthGuard],
    component: CreateEventComponent,
  },
  {
    path: 'edit-event/:eventId',
    canActivate: [AuthGuard],
    component: EditEventComponent,
  },
  {
    path: ':orgId/:eventId',
    component: EventDetailComponent,
  },
];
@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule],
})
export class EventRoutingModule {}
