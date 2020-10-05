import { CommonModule } from "@angular/common";
import { NgModule } from "@angular/core";
import { SharedModule } from '../shared/shared.module';
//import { AccessDeniedComponent } from "./access-denied/access-denied.component";
import { AuthenticationRoutingModule } from "./authentication-routing.module";
import { LoginComponent} from "./login/login.component";
import { ChangePasswordService } from './services/change-password.service';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { AuthInterceptor } from '../core/services/auth.interceptor';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    AuthenticationRoutingModule
    
  ],
  declarations: [
    //LoginComponent,
   // AccessDeniedComponent,

  ],

  providers: [ChangePasswordService]
})
export class AuthenticationModule { }
