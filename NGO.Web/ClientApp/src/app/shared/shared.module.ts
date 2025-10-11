import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { CUSTOM_ELEMENTS_SCHEMA, NgModule } from '@angular/core';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { RouterModule } from '@angular/router';
import { NgbModule } from '@ng-bootstrap/ng-bootstrap';
import { ButtonModule } from 'primeng/button';
import { CardModule } from 'primeng/card';
import { CarouselModule } from 'primeng/carousel';
import { FileUploadModule } from 'primeng/fileupload';
import { TagModule } from 'primeng/tag';
import { ToolbarModule } from 'primeng/toolbar';
import { CoreModule } from '../core/core.module';
import { JwtInterceptor } from '../core/interceptors/jwt.interceptor';
import { FooterComponent } from './components/footer/footer.component';
import { LoaderService } from './components/loading-spinner/loader.service';
import { LoadingSpinnerComponent } from './components/loading-spinner/loading-spinner.component';
import { NavbarComponent } from './components/navbar/navbar.component';
import { CommonService } from './components/services/common.service';
import { SidebarComponent } from './components/sidebar/sidebar.component';
import { PrimeNGModule } from './prime-ng/prime-ng.module';

var modules: any[] = [
  CommonModule,
  FormsModule,
  RouterModule,
  ReactiveFormsModule,
  CoreModule,
  ToolbarModule,
  PrimeNGModule,
  NgbModule,
  TagModule,
  CardModule,
  ButtonModule,
  FileUploadModule,
  CarouselModule,
];
var declaration: any[] = [
  LoadingSpinnerComponent,
  NavbarComponent,
  SidebarComponent,
  FooterComponent,
  SidebarComponent,
];
@NgModule({
  declarations: [declaration],
  imports: [modules],
  exports: [modules, declaration],
  providers: [{ provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true }, LoaderService, CommonService],
  schemas: [CUSTOM_ELEMENTS_SCHEMA],
})
export class SharedModule {}
