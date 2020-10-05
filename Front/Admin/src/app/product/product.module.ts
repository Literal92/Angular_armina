import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import { AuthInterceptor } from "../core/services/auth.interceptor";
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import * as $ from 'jquery';
import { DatePipe } from '@angular/common';
import * as moment from 'jalali-moment';
import { SharedModule } from '../shared/shared.module';
import { ProductRoutingModule } from './product-routing.module';
import { LayoutComponent } from './components/layout/layout.component';
import { DisplayComponent } from './components/display/display.component';
import { CreateOrEditComponent } from './components/create-or-edit/create-or-edit.component';
import { LeafletModule } from '@asymmetrik/ngx-leaflet';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    ProductRoutingModule,
    SharedModule
  ],
  declarations: [
    LayoutComponent,
    DisplayComponent,
    CreateOrEditComponent
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
export class ProductModule { }
