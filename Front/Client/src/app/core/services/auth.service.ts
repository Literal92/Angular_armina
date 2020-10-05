import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { BehaviorSubject, Observable, throwError } from "rxjs";
import { catchError, finalize, map } from "rxjs/operators";

import { AuthTokenType } from "./../models/auth-token-type";
import { AuthUser } from "./../models/auth-user";
import { Credentials } from "./../models/credentials";
import { ApiConfigService } from "./api-config.service";
import { APP_CONFIG, IAppConfig } from "./app.config";
import { RefreshTokenService } from "./refresh-token.service";
import { TokenStoreService } from "./token-store.service";
import { ShemaUrl } from '..';
import { ShemaRoleClaimsToken } from '../models/auth-guard-permission';
import { NotifierService } from 'angular-notifier';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private authStatusSource = new BehaviorSubject<boolean>(false);
  authStatus$ = this.authStatusSource.asObservable();
  private readonly notifier: NotifierService;

  constructor(
    private http: HttpClient,
    private router: Router,
    @Inject(APP_CONFIG) private appConfig: IAppConfig,
    private apiConfigService: ApiConfigService,
    private tokenStoreService: TokenStoreService,
    private refreshTokenService: RefreshTokenService,
    private notifierService: NotifierService
  ) {
    this.updateStatusOnPageRefresh();
    this.refreshTokenService.scheduleRefreshToken(this.isAuthUserLoggedIn(), false);
    this.notifier = notifierService;
  }

  login(credentials: Credentials): Observable<boolean> {
    const headers = new HttpHeaders({ "Content-Type": "application/json" });
    return this.http
      .post(`${this.appConfig.apiEndpointCommon}/${this.apiConfigService.configuration.loginPath}`,
        //credentials
        { item1: credentials.username, item2: credentials.password, item3: credentials.captchaToken }
        , { headers: headers })
      .pipe(
        map((response: any) => {
          if (!response) {
            this.tokenStoreService.setRememberMe(credentials.rememberMe);
            console.error("There is no `{'" + this.apiConfigService.configuration.accessTokenObjectKey +
              "':'...','" + this.apiConfigService.configuration.refreshTokenObjectKey + "':'...value...'}` response after login.");
            this.authStatusSource.next(false);
            return false;
          }
          this.tokenStoreService.storeLoginSession(response);
          console.log("Logged-in user info", this.getAuthUser());
          this.refreshTokenService.scheduleRefreshToken(true, true);
          this.authStatusSource.next(true);
          return true;
        }),
        catchError((error: HttpErrorResponse) =>throwError(error))
      );
  }


  CheckToken(number: string, token: string, type: number) {
    const headers = new HttpHeaders({ 'Content-Type': 'application/json' });
    const url = `${this.appConfig.apiEndpointCommon}/Account/CheckToken`;
    return this.http.get(url + '/' + number + '/' + token, { headers })
      .pipe(
        map((response: any) => {
          if (!response) {
            this.tokenStoreService.setRememberMe(true);
            console.error("There is no `{'" + this.apiConfigService.configuration.accessTokenObjectKey +
              "':'...','" + this.apiConfigService.configuration.refreshTokenObjectKey + "':'...value...'}` response after login.");
            this.authStatusSource.next(false);
            return false;
          }
          this.tokenStoreService.storeLoginSession(response);
          console.log("Logged-in user info", this.getAuthUser());
          this.refreshTokenService.scheduleRefreshToken(true, true);
          this.authStatusSource.next(true);
          return true;
        }),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }

  getBearerAuthHeader(): HttpHeaders {
    return new HttpHeaders({
      "Content-Type": "application/json",
      "Authorization": `Bearer ${this.tokenStoreService.getRawAuthToken(AuthTokenType.AccessToken)}`
    });
  }

  logout(navigateToHome: boolean): void {
    if (navigateToHome != false) {

      const headers = new HttpHeaders({ "Content-Type": "application/json" });
      const refreshToken = encodeURIComponent(this.tokenStoreService.getRawAuthToken(AuthTokenType.RefreshToken));
      this.http
        .get(`${this.appConfig.apiEndpointCommon}/${this.apiConfigService.configuration.logoutPath}?refreshToken=${refreshToken}`,
          { headers: headers })
        .pipe(
          map(response => response || {}),
          catchError((error: HttpErrorResponse) => throwError(error)),
          finalize(() => {
            this.tokenStoreService.deleteAuthTokens();
            this.refreshTokenService.unscheduleRefreshToken(true);
            this.authStatusSource.next(false);
            if (navigateToHome) {
              this.router.navigate(["/login"]);
            }
          }))
        .subscribe(result => {
          console.log("logout", result);
        });
    }

  }

  isAuthUserLoggedIn(): boolean {
    return this.tokenStoreService.hasStoredAccessAndRefreshTokens() &&
      !this.tokenStoreService.isAccessTokenTokenExpired();
  }

  getAuthUser(): AuthUser | null {
    if (!this.isAuthUserLoggedIn()) {
      return null;
    }

    const decodedToken = this.tokenStoreService.getDecodedAccessToken();
    const roles = this.tokenStoreService.getDecodedTokenRoles();
    const roleClaims = this.tokenStoreService.getDecodeTokenRoleClaims();
    return Object.freeze({
      userId: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"],
      userName: decodedToken["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name"],
      serialNumber: decodedToken["http://schemas.microsoft.com/ws/2008/06/identity/claims/serialnumber"],
      displayName: decodedToken["DisplayName"],
      userType: decodedToken["UserType"],

      roles: roles,
      roleClaims: roleClaims
    });
  }
  // #region CheckRole
  isAuthUserInRoles(requiredRoles: string[]): boolean {
    const user = this.getAuthUser();
    if (!user || !user.roles) {
      return false;
    }
    // Check is Admin
    if (user.roles.indexOf(this.apiConfigService.configuration.adminRoleName.toLowerCase()) >= 0) {
      return true; // The `Admin` role has full access to every pages.
    }

    return requiredRoles.some(requiredRole => {
      if (user.roles) {
        return user.roles.indexOf(requiredRole.toLowerCase()) >= 0;
      } else {
        return false;
      }
    });
  }

  isAuthUserInRole(requiredRole: string): boolean {
    return this.isAuthUserInRoles([requiredRole]);
  }
  //#endregion
  // #region CheckClaimRole
  isAuthUserInRoleClaims(requiredClaim: ShemaUrl[]): boolean {
    const user = this.getAuthUser();
    if (!user || !user.roles) {
      return false;
    }
    // Check is Admin
    if (user.roles.indexOf(this.apiConfigService.configuration.adminRoleName.toLowerCase()) >= 0) {
      return true; // The `Admin` role has full access to every pages.
    }
    if (!user.roleClaims) {
      return false;
    }
    // more info about some: https://www.tutorialspoint.com/typescript/typescript_array_some.htm

     return requiredClaim.some(requiredClaim => {
      if (user.roleClaims) {
        let type = requiredClaim.area != null ?
                  `${(requiredClaim.area).toLowerCase()}.${ (requiredClaim.controller).toLowerCase()}`
                  : `${(requiredClaim.controller).toLowerCase()}`;
        let value = (requiredClaim.action).toLowerCase();
       // return user.roleClaims.indexOf({ type: type, value: value }) >= 0;
        //let output = user.roleClaims.indexOf(t) >= 0;
        let index = user.roleClaims.findIndex(x => x.type == type && x.value == value);
        return index>-1 ? true:false;

      } else {
        return false;
      }
     });
  }

  isAuthUserInRoleClaim(requiredClaim: ShemaUrl): boolean {
    return this.isAuthUserInRoleClaims([requiredClaim]);
  }

  // #endregion

  private updateStatusOnPageRefresh(): void {
    this.authStatusSource.next(this.isAuthUserLoggedIn());
  }
}
