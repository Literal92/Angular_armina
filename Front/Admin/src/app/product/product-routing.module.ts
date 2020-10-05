import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard, AuthGuardPermission } from 'src/app/core';
import { CreateOrEditComponent } from './components/create-or-edit/create-or-edit.component';
import { LayoutComponent } from './components/layout/layout.component';
import { DisplayComponent } from './components/display/display.component';


const routes: Routes = [
  {
    path: "", component: LayoutComponent,

    children: [
      { path: '', component: DisplayComponent },
      { path: 'edit/:id', component: CreateOrEditComponent },
      { path: 'add', component: CreateOrEditComponent },
    ]
    , data: {
      permission: {
        permittedRoles: ["Admin","ProductAdmin"]

      } as AuthGuardPermission
    }
    , canActivate: [AuthGuard]
  }
  ,{ path: "**", redirectTo: "" }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class ProductRoutingModule { }
