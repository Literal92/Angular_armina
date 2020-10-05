import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import { AuthInterceptor } from "../core/services/auth.interceptor";
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import * as $ from 'jquery';
import { DatePipe } from '@angular/common';

import { SharedModule } from '../shared/shared.module';
import { LayoutComponent } from './components/layout/layout.component';
import { SpecialRoutingModule } from './special-routing.module';
import { DisplayComponent } from './components/display/display.component';
@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    SpecialRoutingModule,
    SharedModule
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
export class SpecialModule { }
