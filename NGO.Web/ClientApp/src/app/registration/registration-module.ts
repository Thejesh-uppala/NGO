import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { SharedModule } from "../shared/shared.module";
import { RegisterRoutingModule } from "./registration-routing";
import { RegistrationComponent } from "./registration.component";
import { RegistrationService } from "./services/registrationService";

@NgModule({
    declarations:[RegistrationComponent],
    imports:[
        RegisterRoutingModule,
        CommonModule,
        SharedModule
    ],
    providers:[]
})
export class RegisterModule{
    
}