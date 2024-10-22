import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { SharedModule } from "../shared/shared.module";
import { ForgotPasswordComponent } from "./forgot-password.component";
import { ForgotPasswordRoutingModule } from "./forgot-password.routing";

@NgModule({
  declarations: [ForgotPasswordComponent],
  imports: [
    ForgotPasswordRoutingModule,
    CommonModule,
    SharedModule
  ]
})
export class ForgotPasswordModule { }