import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { APP_CONFIG, IAppConfig } from "src/app/core";
import { Observable, throwError } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { Headers } from '@angular/http';
import { Router } from '@angular/router';
import { iCategory } from '../interfaces/iCategory';
import {  iCategorySearch } from '../interfaces/iCategorySearch';

const headers = new HttpHeaders({ "Content-Type": "application/json" });

@Injectable({
  providedIn: 'root'
})
export class CategoryService {

  constructor(private http: HttpClient, private router: Router,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) { }

  get(model: iCategorySearch) {
    let url = `${this.appConfig.apiEndpointProvider}/category/Get?`;
      if (model.id !== undefined)
      url += "id=" + encodeURIComponent("" + model.id) + "&";

    if (model.title !== undefined)
      url += "title=" + encodeURIComponent("" + model.title) + "&";
    if (model.isChkParent != undefined) 
      url += "isChkParent=" + encodeURIComponent("" + model.isChkParent) + "&";
    if (model.contionRoot != undefined) 
      url += "contionRoot=" + encodeURIComponent("" + model.contionRoot) + "&";
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

  getddown(model: iCategorySearch) {
      let url = `${this.appConfig.apiEndpointProvider}/category/GetDDown?`;
    if (model.id !== undefined)
      url += "id=" + encodeURIComponent("" + model.id) + "&";
    if (model.title !== undefined)
      url += "title=" + encodeURIComponent("" + model.title) + "&";
    if (model.isChkParent != undefined) 
      url += "isChkParent=" + encodeURIComponent("" + model.isChkParent) + "&";
    if (model.contionRoot != undefined) 
      url += "contionRoot=" + encodeURIComponent("" + model.contionRoot) + "&";
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
  
  post(model: iCategory) {
    const url = `${this.appConfig.apiEndpointProvider}/category/Create`;
    return this.http
      .post(url, model, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }

  postCSV(model: iCategory) {
    const url = `${this.appConfig.apiEndpointProvider}/trackingcode/Create`;
    return this.http
      .post(url, model, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  put(model: iCategory) {
    const url = `${this.appConfig.apiEndpointProvider}/category/Update/${model.id}`;
    return this.http
      .put(url, model, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  delete(id: number) {
    const url = `${this.appConfig.apiEndpointProvider}/category/Delete/${id}`;
    return this.http
      .delete(url,  { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
}
