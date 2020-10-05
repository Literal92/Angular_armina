
import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { APP_CONFIG, IAppConfig } from "src/app/core";
import { Observable, throwError } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { Headers } from '@angular/http';
import { Router } from '@angular/router';
import { iProductOptionSearch } from '../interfaces/iProductOptionSearch';

const headers = new HttpHeaders({ "Content-Type": "application/json" });

@Injectable({
  providedIn:'root'
})
export class ProductOptionService {

  constructor(private http: HttpClient, private router: Router,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) { }

  get(model: iProductOptionSearch) {

    let url = `${this.appConfig.apiEndpointProvider}/ProductOption/Get?`;
    if (model.id !== undefined)
      url += "id=" + encodeURIComponent("" + model.id) + "&";
      if (model.productId !== undefined)
      url += "productId=" + encodeURIComponent("" + model.productId) + "&";
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

  post(model: any) {
    let url = `${this.appConfig.apiEndpointProvider}/ProductOption/Create`;
    
    return this.http
      .post(url,model, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
   put(model: any) {
     const url = `${this.appConfig.apiEndpointProvider}/ProductOption/Update/${model.id}`;
     return this.http
       .put(url, model, { headers: headers })
       .pipe(
         map(response => response || {}),
         catchError((error: HttpErrorResponse) => throwError(error))
       );
  }
  UpdateColor(model: any) {
    const url = `${this.appConfig.apiEndpointProvider}/ProductOption/UpdateColor/${model.id}`;
    return this.http
      .put(url, model, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
   Delete(id: number) {
     const url = `${this.appConfig.apiEndpointProvider}/ProductOption/Delete/${id}`;
     return this.http
       .delete(url,{ headers: headers })
       .pipe(
         map(response => response || {}),
         catchError((error: HttpErrorResponse) => throwError(error))
       );
   }

}
