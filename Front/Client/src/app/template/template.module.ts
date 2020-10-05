import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import * as $ from 'jquery';

// #region modules
import { SharedModule } from 'src/app/shared/shared.module';
import { TemplateRoutingModule } from './template-routing.module';
// #endregion
//#region component
import { ClientLayoutComponent } from './layout/layout.component';
import { AuthInterceptor } from 'src/app/core/services/auth.interceptor';
import { HTTP_INTERCEPTORS } from '@angular/common/http';

//#endregion

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    // #region module
    TemplateRoutingModule,
    SharedModule,
    // #endregion 

  ],
  declarations: [
    ClientLayoutComponent,
    // FooterComponent,
    // HeaderComponent,
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    }],
})
export class TemplateModule { }
