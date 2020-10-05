import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { APP_CONFIG, IAppConfig } from "src/app/core";
import { Observable, throwError } from "rxjs";
import { catchError, map } from "rxjs/operators";
import {Headers } from '@angular/http';
const headers = new Headers({ "Content-Type": "application/json" });

@Injectable()
export class SidebarService {

  //#region Ctor
  constructor(private http: HttpClient/*,protected http: HttpResponseInterceptor*/,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) { }
  //#endregion
  GetAreaAccess() {
    //const headers = new HttpHeaders({ "Content-Type": "application/json" });
    //const url = `${this.appConfig.apiEndpointArea}/Area/GetAreaForCurrentUser`;
    //return this.http
    //  .get(url, { headers: headers })
    //  .pipe(
    //    map(response => response || {}),
    //    catchError((error: HttpErrorResponse) => throwError(error))
    //  );
  }
  
}
