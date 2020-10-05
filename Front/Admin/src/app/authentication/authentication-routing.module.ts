import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AuthGuard, AuthGuardPermission } from '../core';

import { AccessDeniedComponent } from "./access-denied/access-denied.component";
import { LoginComponent } from "./login/login.component";

const routes: Routes = [

  { path: "login", component: LoginComponent },

  { path: "accessDenied", component: AccessDeniedComponent }

  //{ path: "changePassword", component: ChangePasswordComponent }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class AuthenticationRoutingModule { }
