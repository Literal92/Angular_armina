import { CommonModule } from "@angular/common";
import { RouterModule } from "@angular/router";
import { NgModule } from "@angular/core";
import * as $ from 'jquery';

//#region coreui setting
import { PerfectScrollbarModule, PERFECT_SCROLLBAR_CONFIG  } from 'ngx-perfect-scrollbar';
import { PerfectScrollbarConfigInterface } from 'ngx-perfect-scrollbar';
const DEFAULT_PERFECT_SCROLLBAR_CONFIG: PerfectScrollbarConfigInterface = {
  suppressScrollX: true
};
const APP_CONTAINERS = [
  AdminLayoutComponent
  //DefaultLayoutComponent
];
import {
  AppAsideModule,
  AppBreadcrumbModule,
  AppHeaderModule,
  AppFooterModule,
  AppSidebarModule,
} from '@coreui/angular';

// Import routing module

// Import 3rd party components
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { TabsModule } from 'ngx-bootstrap/tabs';
//#endregion
// #region modules
import { SharedModule } from 'src/app/shared/shared.module';
import { TemplateRoutingModule } from './template-routing.module';
// #endregion
//#region component
import { AdminHeaderComponent } from './header/header.component';
import { AdminLayoutComponent } from './layout/layout.component';
import { AdminFooterComponent } from './footer/footer.component';
import { AdminSidebarComponent } from './sidebar/sidebar.component';
import { AdminBreadCrumbComponent } from './breadcrumb/breadcrumb.component';
import { ChangePasswordAdminService } from './services/change-password.service';
import { ChangePasswordAdminComponent } from './change-password/change-password.component';
import { SidebarService } from './services/sidebar.service';
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
    //#region coreui
    AppAsideModule,
    AppBreadcrumbModule.forRoot(),
    AppFooterModule,
    AppHeaderModule,
    AppSidebarModule,
    PerfectScrollbarModule,
    BsDropdownModule.forRoot(),
    TabsModule.forRoot()
    //#endregion
  ],
  declarations: [
    ...APP_CONTAINERS,
    //#region component
    AdminHeaderComponent,
    AdminBreadCrumbComponent,
    AdminSidebarComponent,
    AdminFooterComponent,
    ChangePasswordAdminComponent,

    //#endregion
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    ChangePasswordAdminService,
    SidebarService
  ]
})
export class TemplateModule { }
