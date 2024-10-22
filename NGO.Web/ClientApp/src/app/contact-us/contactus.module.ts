import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { SharedModule } from "../shared/shared.module";
import { ContactUsComponent } from "./contact-us.component";
import { ContactUsRoutingModule } from "./contact-us.routing";

@NgModule({
    declarations:[ContactUsComponent],
    imports:[
        ContactUsRoutingModule,
        CommonModule,
        SharedModule
    ],
    providers:[]
})
export class ContactUsModule{
    
}