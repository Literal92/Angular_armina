import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { ClientLayoutComponent } from './layout/layout.component';


const routes: Routes = [
  {
    path: "", component: ClientLayoutComponent,
    children: [
      { path: "", loadChildren:() => import('../wellcome/wellcome.module').then(m => m.WellcomeModule) },
    ]
  }
  //,{ path: "**", redirectTo: "" }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TemplateRoutingModule { }
