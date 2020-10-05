
import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { APP_CONFIG, IAppConfig } from "src/app/core";
import { Observable, throwError } from "rxjs";
import { catchError, map } from "rxjs/operators";

import { Router } from '@angular/router';
import { iUser } from '../interfaces/iUser';
import { iUserSearch } from '../interfaces/iUserSearch';

const headers = new HttpHeaders({ "Content-Type": "application/json" });

@Injectable({
  providedIn:'root'
})
export class UserManagerService {

  constructor(private http: HttpClient, private router: Router,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) { }

  get(model: iUserSearch) {
    let url = `${this.appConfig.apiEndpointProvider}/User/Get?`;
      if (model.id !== undefined && model.id!=null)
      url += "id=" + encodeURIComponent("" + model.id) + "&";
      if (model.userName !== undefined && model.userName != null)
      url += "userName=" + encodeURIComponent("" + model.userName) + "&";
      if (model.displayName !== undefined && model.displayName != null)
      url += "displayName=" + encodeURIComponent("" + model.displayName) + "&";
      if (model.mobile !== undefined && model.mobile != null)
      url += "mobile=" + encodeURIComponent("" + model.mobile) + "&";
      if (model.pageIndex !== undefined && model.pageIndex != null)
      url += "pageIndex=" + encodeURIComponent("" + model.pageIndex) + "&";
      if (model.pageSize !== undefined && model.pageSize != null)
      url += "pageSize=" + encodeURIComponent("" + model.pageSize) + "&";
    url = url.replace(/[?&]$/, "");
    return this.http
      .get(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  GetSimple() {
    let url = `${this.appConfig.apiEndpointProvider}/User/GetSimple`;
    return this.http
      .get(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  post(model: iUser) {
    const url = `${this.appConfig.apiEndpointProvider}/User/Create`;
    return this.http
      .post(url,model, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  put(model: iUser) {
    const url = `${this.appConfig.apiEndpointProvider}/User/Update`;
    return this.http
      .put(url, model, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  Active(id: number) {
    const url = `${this.appConfig.apiEndpointProvider}/User/ActiveUser/${id}`;
    return this.http
      .patch(url,{ headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  
  ChangeType(id: number) {
    const url = `${this.appConfig.apiEndpointProvider}/User/ChangetypeUser/${id}`;
    return this.http
      .patch(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  ChPassUser(id: number, pass: string) {
    const url = `${this.appConfig.apiEndpointProvider}/User/ChangePass`;
    return this.http
      .post(url, { item1: id, item2: pass }, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  GetPermission() {
    const url = `${this.appConfig.apiEndpointProvider}/Permission/Get`;
    return this.http
      .get(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  GetUserClaim(id:number) {
    const url = `${this.appConfig.apiEndpointProvider}/Permission/GetUserClaim/${id}`;
    return this.http
      .get(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  SetUserClaims(userId: number, model:any[]) {
    const url = `${this.appConfig.apiEndpointProvider}/Permission/SetUserClaims?userId=${userId}`;
    return this.http
      .post(url, model, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
    }
  getReserve(userId: number) {
        const url = `${this.appConfig.apiEndpointProvider}/Reserve/Get?userId=${userId}`;
        return this.http
            .get(url, { headers: headers })
            .pipe(
                map(response => response || {}),
                catchError((error: HttpErrorResponse) => throwError(error))
            );

  }
  PutRole(id: number, roles: any[]) {
    const url = `${this.appConfig.apiEndpointProvider}/User/PutRole/${id}`;
    return this.http
      .put(url, roles, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }  
}
