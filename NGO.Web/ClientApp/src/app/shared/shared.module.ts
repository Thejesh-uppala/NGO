import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { CoreModule } from '../core/core.module';
import { JwtInterceptor } from '../core/interceptors/jwt.interceptor';
import { FooterComponent } from './components/footer/footer.component';

import { NavbarComponent } from './components/navbar/navbar.component';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { ToastComponent } from './components/toast/toast.component';
import { PrimeNGModule } from './prime-ng/prime-ng.module';
import {  NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { CommonService } from './components/services/common.service';
import { NgxPayPalModule } from 'ngx-paypal';
import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
import { LoaderService } from './components/loading-spinner/loader.service';

var modules: any[] = [
  CommonModule,
  FormsModule,
  RouterModule,
  ReactiveFormsModule,
  CoreModule,
  PrimeNGModule,
  NgbModule,
  NgxPayPalModule,
]
var declaration: any[] = [
  LoadingSpinnerComponent,
  ToastComponent,
  NavbarComponent,
  SidebarComponent,
  FooterComponent,
  SidebarComponent,
  LoadingSpinnerComponent,
]
@NgModule({
  declarations: [declaration],
  imports: [modules],
  exports: [modules, declaration],
  providers: [{ provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true },LoaderService,CommonService],
  schemas: [CUSTOM_ELEMENTS_SCHEMA]

})
export class SharedModule {
}
