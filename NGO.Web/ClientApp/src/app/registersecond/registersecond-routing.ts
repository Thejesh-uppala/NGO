import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { RegistersecondComponent } from './registersecond.component';


const routes: Routes = [
  { path: '', component: RegistersecondComponent },
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RegistersecondRoutingModule { }
