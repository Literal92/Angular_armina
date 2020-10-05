
import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { APP_CONFIG, IAppConfig } from "src/app/core";
import { Observable, throwError } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { Headers } from '@angular/http';
import { Router } from '@angular/router';
import { iFieldSearch } from '../interfaces/iFieldSearch';
import { iField } from '../interfaces/iField';

const headers = new HttpHeaders({ "Content-Type": "application/json" });

@Injectable({
  providedIn:'root'
})
export class FieldService {

  constructor(private http: HttpClient, private router: Router,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) { }

  get(model: iFieldSearch) {

    let url = `${this.appConfig.apiEndpointProvider}/Field/Get?`;
    if (model.id !== undefined)
      url += "id=" + encodeURIComponent("" + model.id) + "&";
    if (model.title !== undefined)
      url += "title=" + encodeURIComponent("" + model.title) + "&";
    if (model.pageIndex !== undefined)
      url += "pageIndex=" + encodeURIComponent("" + model.pageIndex) + "&";
    if (model.pageSize !== undefined)
      url += "pageSize=" + encodeURIComponent("" + model.pageSize) + "&";
    url = url.replace(/[?&]$/, "");
    return this.http
      .get(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }

  post(model: iField) {
    let url = `${this.appConfig.apiEndpointProvider}/Field/Create`;
    
    return this.http
      .post(url,model, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
   put(model: iField) {
     const url = `${this.appConfig.apiEndpointProvider}/Field/Update/${model.id}`;
     return this.http
       .put(url, model, { headers: headers })
       .pipe(
         map(response => response || {}),
         catchError((error: HttpErrorResponse) => throwError(error))
       );
   }
   Delete(id: number) {
     const url = `${this.appConfig.apiEndpointProvider}/Field/Delete/${id}`;
     return this.http
       .delete(url,{ headers: headers })
       .pipe(
         map(response => response || {}),
         catchError((error: HttpErrorResponse) => throwError(error))
       );
   }

}
