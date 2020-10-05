import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard, AuthGuardPermission } from 'src/app/core';
import { LayoutDashboardComponent } from './components/layout/layout.component';
import { DisplayDashboardComponent } from './components/display/display.component';


const routes: Routes = [
  {
    path: "", component: LayoutDashboardComponent,

    children: [
      { path: '', component: DisplayDashboardComponent  },
    ]
    , data: {
      permission: {
        permittedRoles: ["Admin", "AccountantAdmin", "OrderAdmin", "ProductAdmin","ReportAdmin"]

      } as AuthGuardPermission
    }
    , canActivate: [AuthGuard]
  }
 // ,{ path: "**", redirectTo: "" }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class DashboardRoutingModule { }
