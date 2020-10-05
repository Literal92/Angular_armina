import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { APP_CONFIG, IAppConfig } from "src/app/core";
import { Observable, throwError } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { Headers } from '@angular/http';
import { Router } from '@angular/router';
import { iSpecialSearch } from '../interfaces/iSpecialSearch';
import { iSpecial } from '../interfaces/iSpecial';

const headers = new HttpHeaders({ "Content-Type": "application/json" });

@Injectable({
  providedIn:'root'
})
export class SpecialService {

  constructor(private http: HttpClient, private router: Router,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) { }
  Get(model: iSpecialSearch) {
    const url = `${this.appConfig.apiEndpointProvider}/ProviderSpecial/Get?id=${model.id}&state=${model.state}&mobile=${model.mobile}&pageIndex=${model.pageIndex}&pageSize=${model.pageSize}`;
    return this.http
      .get(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  Put(model: iSpecial) {
    const url = `${this.appConfig.apiEndpointProvider}/ProviderSpecial/Update/${model.id}`;
    return this.http
      .put(url, model, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
 
  
}
