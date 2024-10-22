import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { DashBoardComponent } from "./dashboard.component";

const routes: Routes = [
    {
        path: '',
        component: DashBoardComponent
    },
    {
        path: 'home',
        component:DashBoardComponent
    },
]
@NgModule({
    imports:[RouterModule.forChild(routes)],
    exports:[RouterModule]
})
export class DashBoardRoutingModule {}