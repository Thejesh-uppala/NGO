import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { SharedModule } from "../shared/shared.module";
import { ChangePasswordComponent } from "./change-password.component";
import { ChangePasswordRoutingModule } from "./changePassword-routing-module";

@NgModule({
  declarations: [ChangePasswordComponent],
  imports: [
    ChangePasswordRoutingModule,
    CommonModule,
    SharedModule
  ]
})
export class ChangePasswordModule { }