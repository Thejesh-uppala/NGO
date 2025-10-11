import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { ButtonModule } from 'primeng/button';
import { CarouselModule } from 'primeng/carousel';
import { TagModule } from 'primeng/tag';
import { SharedModule } from '../shared/shared.module';
import { DashBoardRoutingModule } from './dashboard-routing.module';
import { DashBoardComponent } from './dashboard.component';

@NgModule({
  declarations: [DashBoardComponent],
  imports: [ButtonModule, CarouselModule, DashBoardRoutingModule, CommonModule, SharedModule, TagModule],
})
export class DashBoardModule {}
