import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard, AuthGuardPermission } from 'src/app/core';

import { LayoutComponent } from './components/layout/layout.component';
import { DisplayComponent } from './components/display/display.component';
import { AddressComponent } from './components/address/address.component';
import { ReportDailyComponent } from './components/report-daily/report-daily.component';


const routes: Routes = [
  {
    path: "", component: LayoutComponent,

    children: [
      {
        path: '', component: DisplayComponent
        , data: {
          permission: {
            permittedRoles: ["Admin", "AccountantAdmin", "OrderAdmin","ReportAdmin"],
          } as AuthGuardPermission
        }
      },
      {
        path: 'address', component: AddressComponent
        , data: {
          permission: {
            permittedRoles: ["Admin","OrderAdmin"]
          } as AuthGuardPermission
        }
      },
      {
        path: 'report-daily', component: ReportDailyComponent
        , data: {
          permission: {
            permittedRoles: ["Admin"],
          } as AuthGuardPermission
        }
      }
      
 
    ]
    //, data: {
    //  permission: {
    //    permittedRoles:["Admin"]

    //  } as AuthGuardPermission
    //}
    , canActivate: [AuthGuard]
  }
  ,{ path: "**", redirectTo: "" }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class OrderRoutingModule { }
