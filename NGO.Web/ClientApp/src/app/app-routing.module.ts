import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminLayoutComponent } from './layouts/admin-layout/admin-layout.component';
import { AuthComponent } from './login/auth.component';

const routes: Routes = [
  {
    path: 'login/:orgId',
    loadChildren: () => import('./login/auth.module').then(m => m.AuthModule),
  },
  {
    path: 'register',
    loadChildren: () => import('./registration/registration-module').then(m => m.RegisterModule),
  },
  {
    path: 'matrimony',
    loadChildren: () => import('./matrimony/matrimony.module').then(m => m.MatrimonyModule),
  },
  {
    path: 'signup/:orgId',
    loadChildren: () => import('./sign-up/signup-module').then(m => m.SignUpModule),
  },
  {
    path: 'forgotPassword',
    loadChildren: () => import('./forgot-password/forgot-password.module').then(m => m.ForgotPasswordModule),
  },
  {
    path: 'events',
    loadChildren: () => import('./event/event.module').then(m => m.EventModule),
  },
  {
    path: '',
    redirectTo: 'home',
    pathMatch: 'full',
  },
  {
    path: '',
    component: AdminLayoutComponent,
    children: [
      {
        path: '',
        loadChildren: () => import('src/app/layouts/admin-layout/admin-layout.module').then(m => m.AdminLayoutModule),
      },
    ],
  },
  {
    path: '',
    component: AuthComponent,
    children: [
      {
        path: '',
        loadChildren: () => import('src/app/layouts/auth-layout/auth-layout.module').then(m => m.AuthLayoutModule),
      },
    ],
  },
  {
    path: '**',
    redirectTo: 'home',
  },
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes, {
      scrollPositionRestoration: 'enabled',
      scrollOffset: [0, 0],
      anchorScrolling: 'enabled',
    }),
  ],
  exports: [RouterModule],
})
export class AppRoutingModule {}
