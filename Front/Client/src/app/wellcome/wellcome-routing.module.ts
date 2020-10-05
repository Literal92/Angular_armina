import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { WellcomeComponent } from './components/wellcome.component';
import { WellLayoutComponent } from './components/layout/layout.component';

const routes: Routes = [
  {

    path: "", component: WellLayoutComponent,
    children: [
      { path: "", component: WellcomeComponent },
    ]
  }
];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class WellcomeRoutingModule { }
