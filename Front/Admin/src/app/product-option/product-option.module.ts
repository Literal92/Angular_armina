import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import { AuthInterceptor } from "../core/services/auth.interceptor";
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import * as $ from 'jquery';
import { DatePipe } from '@angular/common';

import { SharedModule } from '../shared/shared.module';
import { LayoutComponent } from './components/layout/layout.component';
import { EditorModule } from '@tinymce/tinymce-angular';
import { ProductOptionRoutingModule } from './product-Option-routing.module';
import { AddComponent } from './components/add/add.component';
import { DisplayComponent } from './components/display/display.component';
import { EditComponent } from './components/edit/edit.component';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    SharedModule,
    EditorModule,
    ProductOptionRoutingModule
  ],
  declarations: [
    LayoutComponent,
    DisplayComponent,
    AddComponent,
    EditComponent
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
export class ProductOptionModule { }
