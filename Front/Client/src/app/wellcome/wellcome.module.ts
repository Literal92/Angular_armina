import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import { AuthInterceptor } from "../core/services/auth.interceptor";
import { HTTP_INTERCEPTORS } from "@angular/common/http";
import { SharedModule } from '../shared/shared.module';
import { WellcomeComponent } from './components/wellcome.component';
import { WellcomeRoutingModule } from './wellcome-routing.module';
import { WellLayoutComponent } from './components/layout/layout.component';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    SharedModule,
    WellcomeRoutingModule
  ],
  declarations: [
    WellcomeComponent,
    WellLayoutComponent
  ],
  providers: [
  {
    provide: HTTP_INTERCEPTORS,
    useClass: AuthInterceptor,
    multi: true
    }
  ]
})
export class WellcomeModule { }
