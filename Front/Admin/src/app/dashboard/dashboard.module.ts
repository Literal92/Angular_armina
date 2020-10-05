import { CommonModule, DatePipe } from "@angular/common";
import { RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import { AuthInterceptor } from "../core/services/auth.interceptor";
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import * as $ from 'jquery';
import { SharedModule } from '../shared/shared.module';
import { LayoutDashboardComponent } from './components/layout/layout.component';
import { DashboardRoutingModule } from './dashboard-routing.module';
import { DisplayDashboardComponent } from './components/display/display.component';
import { DashboardService } from './services/dashboard.service';


@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    DashboardRoutingModule,
    SharedModule,
  ],
  declarations: [
    LayoutDashboardComponent,
    DisplayDashboardComponent  

  ],
  providers: [
  {
    provide: HTTP_INTERCEPTORS,
    useClass: AuthInterceptor,
    multi: true
    },
    DashboardService
  ]
})
export class DashboardModule { }
