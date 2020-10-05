import { CommonModule } from "@angular/common";
import { HttpClientModule } from "@angular/common/http";
import { ModuleWithProviders, NgModule } from "@angular/core";
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { EqualValidatorDirective } from "./directives/equal-validator.directive";
import { HasAuthUserViewPermissionDirective } from "./directives/has-auth-user-view-permission.directive";
import { IsVisibleForAuthUserDirective } from "./directives/is-visible-for-auth-user.directive";
import { NotifierModule, NotifierOptions } from 'angular-notifier';
import { IsVisibleForClaimUserDirective } from './directives/is-visible-for-claim-user.directive';

import { DpDatePickerModule } from 'ng2-jalali-date-picker';
import { DatePipe } from '@angular/common';

// pagination
// ReadMore(paging):https://www.npmjs.com/package/ngx-pagination &&  http://michaelbromley.github.io/ngx-pagination/#/advanced
 import { NgxPaginationModule } from 'ngx-pagination';
// loading
// more info => https://www.npmjs.com/package/ngx-loading && https://stackblitz.com/github/Zak-C/ngx-loading?file=src%2Fapp%2Fapp.component.html
import { NgxLoadingModule, ngxLoadingAnimationTypes } from 'ngx-loading';

import { DateGtTFt } from './pipe/dateGtTFt.pipe';
import { DateGTF } from './pipe/dateGtF.pipe';
import { SubServicePipe } from './pipe/subServiceSelected.pipe';
import { MomentJalaaliPipe } from './pipe/momentJalaali.pipe';
import { FormatPricePipe } from './pipe/price.pipe';
import { TimeViewPipe } from './pipe/time-view.pipe';
import { CheckClaim } from './pipe/checkClaim';
import { CheckRoleClaim } from './pipe/checkRoleClaim';
import { CheckRole } from './pipe/checkRole';
import { IsVisibleForRoleClaimDirective } from './directives/is-visible-for-role-claims.directive';
import { IsVisibleForUserClaimDirective } from './directives/is-visible-for-user-claim.directive';
import { CheckPaymentWay } from './pipe/checkPaymentWay';
import { CheckFieldPipe } from './pipe/chkField.pipe';
import { ReportPipe } from './pipe/checkReport.pipe';
//#region notify-optional
const customNotifierOptions: NotifierOptions = {
  position: {
    horizontal: {
      position: 'right',
      distance: 12
    },
    vertical: {
      position: 'top',
      distance: 12,
      gap: 10
    }
  },
  theme: 'material',
  behaviour: {
    autoHide: 3000,
    onClick: false,
    onMouseover: 'pauseAutoHide',
    showDismissButton: true,
    stacking: 4
  },
  animations: {
    enabled: true,
    show: {
      preset: 'slide',
      speed: 300,
      easing: 'ease'
    },
    hide: {
      preset: 'fade',
      speed: 300,
      easing: 'ease',
      offset: 50
    },
    shift: {
      speed: 300,
      easing: 'ease'
    },
    overlap: 150
  }
};
//#endregion

@NgModule({
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    HttpClientModule,
    DpDatePickerModule,
    NgxPaginationModule,
    NotifierModule.withConfig(customNotifierOptions),

    // loading config
    NgxLoadingModule.forRoot({
      //animationType: ngxLoadingAnimationTypes.circle,//ngxLoadingAnimationTypes.wanderingCubes,
      //backdropBackgroundColour: 'rgba(0,0,0,0.1)',
      //backdropBorderRadius: '4px',
      //primaryColour: '#ffffff',
      //secondaryColour: '#ffffff',
      //tertiaryColour: '#ffffff'  
    })

  ],
  entryComponents: [
    // All components about to be loaded "dynamically" need to be declared in the entryComponents section.
  ],
  declarations: [
    // common and shared components/directives/pipes between more than one module and components will be listed here.
    //IsVisibleForAuthUserDirective,
    //IsVisibleForClaimUserDirective,
    //HasAuthUserViewPermissionDirective,
    HasAuthUserViewPermissionDirective,
    IsVisibleForAuthUserDirective,
    IsVisibleForClaimUserDirective,
    IsVisibleForRoleClaimDirective,
    IsVisibleForUserClaimDirective,
    //
    EqualValidatorDirective,
    DateGtTFt,
    DateGTF,
    MomentJalaaliPipe,
    SubServicePipe,
    FormatPricePipe,
    TimeViewPipe,
    CheckClaim,
    CheckRoleClaim,
    CheckRole,
    CheckPaymentWay,
    CheckFieldPipe,
    ReportPipe
  ],
  exports: [
    // common and shared components/directives/pipes between more than one module and components will be listed here.
    CommonModule,
    HttpClientModule,
    FormsModule,
    ReactiveFormsModule,
    //IsVisibleForAuthUserDirective,
    //IsVisibleForClaimUserDirective,
    //HasAuthUserViewPermissionDirective,
    HasAuthUserViewPermissionDirective,
    IsVisibleForAuthUserDirective,
    IsVisibleForClaimUserDirective,
    IsVisibleForRoleClaimDirective,
    IsVisibleForUserClaimDirective,
    //
    EqualValidatorDirective,
    NotifierModule,
    DpDatePickerModule,
    NgxPaginationModule,
    NgxLoadingModule,
    DateGtTFt,
    DateGTF,
    MomentJalaaliPipe,
    SubServicePipe,
    FormatPricePipe,
    TimeViewPipe,
    CheckClaim,
    CheckRoleClaim,
    CheckRole,
    CheckPaymentWay,
    CheckFieldPipe,
    ReportPipe
  ]
  /* No providers here! Since they’ll be already provided in AppModule. */
})
export class SharedModule {
  static forRoot(): ModuleWithProviders {
    // Forcing the whole app to use the returned providers from the AppModule only.
    return {
      ngModule: SharedModule,
      providers: [ /* All of your services here. It will hold the services needed by `itself`. */]
    };
  }
}
