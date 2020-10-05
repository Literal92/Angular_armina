import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import { AuthInterceptor } from "../core/services/auth.interceptor";
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import * as $ from 'jquery';
import { DatePipe } from '@angular/common';
import * as moment from 'jalali-moment';
import { SharedModule } from '../shared/shared.module';
import { ImageDateRoutingModule } from './imageDate-routing.module';
import { LayoutComponent } from './components/layout/layout.component';
import { AddComponent } from './components/add/add.component';
import { DisplayComponent } from './components/display/display.component';


@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    ImageDateRoutingModule,
    SharedModule
  ],
  declarations: [
    LayoutComponent,
    AddComponent,
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
export class ImageDateModule { }
