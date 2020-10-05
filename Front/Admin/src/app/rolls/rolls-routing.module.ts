import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard, AuthGuardPermission } from 'src/app/core';
import { LayoutComponent } from './components/layout/layout.component';
import { DisplayComponent } from './components/display/display.component';


const routes: Routes = [
  {
    path: "", component: LayoutComponent,

    children: [
      {
        path: '', component: DisplayComponent
        , data: {
          permission: {
          //  permittedUserClaims: [{ controller: "user", action: "get" }]
                permittedRoles: ["Admin"]


          } as AuthGuardPermission
        }
        , canActivate: [AuthGuard]
      },
    ]
    , data: {
      permission: {
        permittedRoles: ["Admin"]

      } as AuthGuardPermission
    }
    , canActivate: [AuthGuard]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class RollsRoutingModule { }
