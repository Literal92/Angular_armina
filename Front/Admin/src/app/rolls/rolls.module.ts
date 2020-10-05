import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import { AuthInterceptor } from "../core/services/auth.interceptor";
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import * as $ from 'jquery';
import { DatePipe } from '@angular/common';

import { SharedModule } from '../shared/shared.module';
import { LayoutComponent } from './components/layout/layout.component';
import { DisplayComponent } from './components/display/display.component';
import { RollsRoutingModule } from './Rolls-routing.module';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    SharedModule,
    RollsRoutingModule
  ],
  declarations: [
    LayoutComponent,
    DisplayComponent
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
export class RollsModule { }
