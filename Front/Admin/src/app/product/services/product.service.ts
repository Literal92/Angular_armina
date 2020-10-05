import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { APP_CONFIG, IAppConfig } from "src/app/core";
import { Observable, throwError } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { Headers } from '@angular/http';
import { Router } from '@angular/router';
import { sortProvider } from 'src/app/shared/enum/orderProvider';
import { iproductSearch } from '../interfaces/iproductSearch';

const headers = new HttpHeaders({ "Content-Type": "application/json" });

@Injectable({
  providedIn: 'root'
})
export class ProductService {

  constructor(private http: HttpClient, private router: Router,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) { }

  get(model: iproductSearch) {
    let url = `${this.appConfig.apiEndpointProvider}/product/Get?`;

    if (model.id !== undefined)
      url += "id=" + encodeURIComponent("" + model.id) + "&";
    if (model.title !== undefined)
      url += "title=" + encodeURIComponent("" + model.title) + "&";
    if (model.code !== undefined)
      url += "code=" + encodeURIComponent("" + model.code) + "&";
    if (model.categoryId != undefined) 
      url += "categoryId=" + encodeURIComponent("" + model.categoryId) + "&";
    if(model.withfield != undefined)
      url += "withfield=" + encodeURIComponent("" + model.withfield) + "&";
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



  Active(id: number) {
    const url = `${this.appConfig.apiEndpointProvider}/product/ActiveProduct/${id}`;
    return this.http
      .patch(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }


  post(data: any) {
    let url = `${this.appConfig.apiEndpointProvider}/product/create`;
    return this.http
      .post(url, data, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }


  put(id: number, data: any) {
    let url = `${this.appConfig.apiEndpointProvider}/product/update/${id}`;
    return this.http
      .put(url, data, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  delete (id:number){
    let url = `${this.appConfig.apiEndpointProvider}/product/delete/${id}`;
    return this.http
      .delete(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  deletePic(id: number) {
    let url = `${this.appConfig.apiEndpointProvider}/product/DeletePic/${id}`;
    return this.http
      .delete(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
}
