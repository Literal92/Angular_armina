import { HeaderComponent } from './template/header/header.component';
import { FooterComponent } from './template/footer/footer.component';

import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { HttpModule, JsonpModule/*, XHRBackend*/ } from '@angular/http';
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { CoreModule } from "./core/core.module";
import { PageNotFoundComponent } from "./page-not-found/page-not-found.component";
import { SharedModule } from "./shared/shared.module";
import * as $ from 'jquery';

import { BrowserAnimationsModule } from '@angular/platform-browser/animations';

@NgModule({
  imports: [
    BrowserModule,
    BrowserAnimationsModule,

    HttpModule,
    JsonpModule,/* XHRBackend,*/
    CoreModule,
    SharedModule.forRoot(),
    AppRoutingModule
  ],
  declarations: [
    AppComponent,
    HeaderComponent,
    FooterComponent,
    PageNotFoundComponent,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
