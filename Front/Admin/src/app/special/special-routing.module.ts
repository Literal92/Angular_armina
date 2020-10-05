import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard, AuthGuardPermission } from 'src/app/core';
import { LayoutComponent } from './components/layout/layout.component';
import { DisplayComponent } from './components/display/display.component';


const routes: Routes = [
  {
    path: "", component: LayoutComponent,

    children: [
      { path: '', component: DisplayComponent }
    ]
    , data: {
      permission: {
        permittedRoles:["provider"]

      } as AuthGuardPermission
    }
    , canActivate: [AuthGuard]
  }
  //,{ path: "**", redirectTo: "" }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class SpecialRoutingModule { }
