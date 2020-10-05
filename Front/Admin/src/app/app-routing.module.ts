import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PageNotFoundComponent } from './page-not-found/page-not-found.component';
import { WelcomeComponent } from './welcome/welcome.component';
import { AuthGuardPermission, AuthGuard } from './core';
import { AccessDeniedComponent } from './authentication/access-denied/access-denied.component';
import { LoginComponent } from './authentication/login/login.component';
// import { AreaModule } from './template/area/area.module';

const routes: Routes = [
  // { path: "welcome", component: WelcomeComponent },
  {
    path: '', // dashboard
    loadChildren: () => import('./template/template.module').then(m => m.TemplateModule)
    ,
    data: {
      permission: {
        permittedRoles: ['Admin', 'Provider', 'AccountantAdmin', 'OrderAdmin', 'ProductAdmin','ReportAdmin']
      } as AuthGuardPermission
    }
    , canActivate: [AuthGuard]
  },
  {
    path: 'login',
    redirectTo: 'login', pathMatch: 'full'
  },
  { path: 'accessDenied', component: AccessDeniedComponent },
  { path: 'login', component: LoginComponent },

  { path: '**', component: PageNotFoundComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
