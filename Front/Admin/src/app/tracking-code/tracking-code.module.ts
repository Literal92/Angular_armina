import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import { AuthInterceptor } from "../core/services/auth.interceptor";
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import * as $ from 'jquery';
import { DatePipe } from '@angular/common';

import { SharedModule } from '../shared/shared.module';
import { LayoutComponent } from './components/layout/layout.component';
import { TrackinCodeRoutingModule } from './tracking-code-routing.module';
import { AddComponent } from './components/add/add.component';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    TrackinCodeRoutingModule,
    SharedModule
  ],
  declarations: [
    LayoutComponent,
    AddComponent
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
export class TrackinCodeModule { }
