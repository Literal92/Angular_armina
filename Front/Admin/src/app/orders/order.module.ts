import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import { AuthInterceptor } from "../core/services/auth.interceptor";
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import * as $ from 'jquery';
import { DatePipe } from '@angular/common';
import * as moment from 'jalali-moment';
import { SharedModule } from '../shared/shared.module';
import { OrderRoutingModule } from './order-routing.module';
import { LayoutComponent } from './components/layout/layout.component';
import { DisplayComponent } from './components/display/display.component';
import { AddressComponent } from './components/address/address.component';
import { ChartsModule } from 'ng2-charts';
import { ReportDailyComponent } from './components/report-daily/report-daily.component';


@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    OrderRoutingModule,
    SharedModule,
    ChartsModule
  ],
  declarations: [
    LayoutComponent,
    DisplayComponent,
    AddressComponent,
    ReportDailyComponent
  ],
  providers: [
  {
    provide: HTTP_INTERCEPTORS,
    useClass: AuthInterceptor,
    multi: true
    },
    DatePipe
  ]
})
export class OrderModule { }
