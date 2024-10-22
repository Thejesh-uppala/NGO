import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { SharedModule } from "../shared/shared.module";
import { UserProfileRoutingModule } from "./userprofile-routing.module";
import { UserprofileComponent } from "./userprofile.component";

@NgModule({
    declarations:[UserprofileComponent],
    imports:[
        UserProfileRoutingModule,
        SharedModule,
        CommonModule
    ]
})
export class UserProfileModule{

}