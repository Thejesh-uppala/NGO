import { NgModule } from "@angular/core";
import { Router, RouterModule, Routes } from "@angular/router";
import { UsersComponent } from "../users/users.component";
import { UserprofileComponent } from "./userprofile.component";
const routes: Routes = [
    {
        path: "",
        component: UserprofileComponent
    },
    {
        path:"user-profile",
        component:UserprofileComponent
    }
]
@NgModule({
    imports: [RouterModule.forChild(routes)],
    exports:[RouterModule]

})
export class UserProfileRoutingModule{

}
