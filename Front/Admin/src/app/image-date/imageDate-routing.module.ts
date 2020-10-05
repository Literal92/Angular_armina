import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard, AuthGuardPermission } from 'src/app/core';

import { LayoutComponent } from './components/layout/layout.component';
import { AddComponent } from './components/add/add.component';
import { DisplayComponent } from './components/display/display.component';


const routes: Routes = [
  {
    path: "", component: LayoutComponent,

    children: [
      { path: '', component: DisplayComponent },
      { path: 'add', component: AddComponent },
      { path: 'edit/:id', component: AddComponent }
 
    ]
    , data: {
      permission: {
        permittedRoles:["Admin"]

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
export class ImageDateRoutingModule { }
