import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { SharedModule } from "../shared/shared.module";
import { DashBoardRoutingModule } from "./dashboard-routing.module";
import { DashBoardComponent } from "./dashboard.component";

@NgModule({
  declarations: [DashBoardComponent],
  imports: [
    DashBoardRoutingModule,
    CommonModule,
    SharedModule
  ]
})
export class DashBoardModule { }