import { NgModule } from "@angular/core";
import { BrowserModule } from "@angular/platform-browser";
import { HttpModule, JsonpModule/*, XHRBackend*/ } from '@angular/http';
import { AppRoutingModule } from "./app-routing.module";
import { AppComponent } from "./app.component";
import { CoreModule } from "./core/core.module";
import { PageNotFoundComponent } from "./page-not-found/page-not-found.component";
import { SharedModule } from "./shared/shared.module";
import { WelcomeComponent } from "./welcome/welcome.component";
import * as $ from 'jquery';

import { AuthenticationModule } from "./authentication/authentication.module";
import { AccessDeniedComponent } from './authentication/access-denied/access-denied.component';
import { LoginComponent } from './authentication/login/login.component';


@NgModule({
  imports: [
    BrowserModule,
    HttpModule, JsonpModule,/* XHRBackend,*/
    CoreModule,
    SharedModule.forRoot(),
    AppRoutingModule,
    AuthenticationModule,

  ],
  declarations: [
    AppComponent,
    WelcomeComponent,
    LoginComponent,
    AccessDeniedComponent,
    PageNotFoundComponent,
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
