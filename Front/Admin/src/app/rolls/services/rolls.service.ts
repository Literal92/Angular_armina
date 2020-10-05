
import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { APP_CONFIG, IAppConfig } from "src/app/core";
import { Observable, throwError } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { Headers } from '@angular/http';
import { Router } from '@angular/router';
import { iUser } from '../interfaces/iUser';
import { iUserSearch } from '../interfaces/iUserSearch';

const headers = new HttpHeaders({ "Content-Type": "application/json" });

@Injectable({
  providedIn:'root'
})
export class RollsService {

  constructor(private http: HttpClient, private router: Router,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) { }

  Get() {
    let url = `${this.appConfig.apiEndpointProvider}/Role/GetNew`;
    return this.http
      .get(url, { headers: headers })
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
  GetRoleClaim(id: number) {
    const url = `${this.appConfig.apiEndpointProvider}/Permission/GetRoleClaim/${id}`;
    return this.http
      .get(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  SetRoleClaims(model:any[]) {
    const url = `${this.appConfig.apiEndpointProvider}/Permission/SetRoleClaims`;
    return this.http
      .post(url, model, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  getUserRole(id: number) {
    const url = `${this.appConfig.apiEndpointProvider}/Role/GetRoleByUserId/${id}`;
    return this.http
      .get(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
}
