import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { APP_CONFIG, IAppConfig } from "src/app/core";
import { Observable, throwError } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { Headers } from '@angular/http';

const headers = new HttpHeaders({ "Content-Type": "application/json" });

@Injectable({
  providedIn: 'root'
})
export class SharedService {

  constructor(private http: HttpClient,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) { }

  GetStates() {
    const url = `${this.appConfig.apiEndpointCommon}/city/GetStates`;
    return this.http
      .get(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  GetCities() {
    const url = `${this.appConfig.apiEndpointCommon}/city/GetCities`;
    return this.http
      .get(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  GetCitiesByStateId(StateId: number) {
    const url = `${this.appConfig.apiEndpointCommon}/city/GetCities/${StateId}`;
    return this.http
      .get(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  GetAreas(cityId: number) {
    const url = `${this.appConfig.apiEndpointCommon}/city/GetAreas/${cityId}`;
    return this.http
      .get(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }
  GetAreasByCity(city: string) {
    const url = `${this.appConfig.apiEndpointCommon}/city/GetAreas/${city}`;
    return this.http
      .get(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }

}
