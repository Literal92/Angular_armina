import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard, AuthGuardPermission } from 'src/app/core';
import { LayoutComponent } from './components/layout/layout.component';
import { AddComponent } from './components/add/add.component';
import { EditComponent } from './components/edit/edit.component';
import { DisplayComponent } from './components/display/display.component';


const routes: Routes = [
  { 
    path: "", component: LayoutComponent,

    children: [
      { path: '', component: DisplayComponent },
      { path: 'add', component: AddComponent },
      { path: 'edit/:id', component: EditComponent }

    ]
    , data: {
      permission: {
        permittedRoles: ["Admin","ProductAdmin"]

      } as AuthGuardPermission
    }
    , canActivate: [AuthGuard]
  }
  ];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TagRoutingModule { }
