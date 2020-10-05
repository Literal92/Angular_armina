import { HttpClient, HttpErrorResponse, HttpHeaders } from "@angular/common/http";
import { Inject, Injectable } from "@angular/core";
import { APP_CONFIG, IAppConfig } from "src/app/core";
import { Observable, throwError } from "rxjs";
import { catchError, map } from "rxjs/operators";
import { Headers } from '@angular/http';
import { Router } from '@angular/router';
import { sortProvider } from 'src/app/shared/enum/orderProvider';
import { iImageDateSearch } from '../interfaces/iImageDateSearch';
import { iImageDate } from '../interfaces/iImageDate';
import { NotifierService } from 'angular-notifier';

const headers = new HttpHeaders({ "Content-Type": "application/json" });

@Injectable({
  providedIn: 'root'
})
export class ImageDateService {
  public readonly notifier: NotifierService;

  constructor(private http: HttpClient, private router: Router, notifierService: NotifierService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
    this.notifier = notifierService;

  }

  get(model: iImageDateSearch) {
    let url = `${this.appConfig.apiEndpointProvider}/ImageDate/Get?`;

    if (model.id !== undefined)
      url += "id=" + encodeURIComponent("" + model.id) + "&";
    if (model.date !== undefined)
      url += "date=" + encodeURIComponent("" + model.date) + "&";
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

  post(model: iImageDate) {
    let track = new FormData();
    track.append('date', model.date);
    for (var i = 0; i < model.images.length; i++) {
      track.append('images', model.images[i]);
    }
    let url = `${this.appConfig.apiEndpointProvider}/ImageDate/Create`;

    return this.http.post(url, track, {
      reportProgress: true,
      observe: 'events'
    }).pipe(
      catchError(this.errorMgmt)
    );
  }

  delete(id?: number, date?: string, fileName?: string) {
    let url = `${this.appConfig.apiEndpointProvider}/ImageDate/delete?id=${id}&date=${date}&fileName=${fileName}`;
    return this.http
      .delete(url, { headers: headers })
      .pipe(
        map(response => response || {}),
        catchError((error: HttpErrorResponse) => throwError(error))
      );
  }

  errorMgmt(error: HttpErrorResponse) {
    let errorMessage = '';
    if (error.error instanceof ErrorEvent) {
      // Get client-side error
      errorMessage = error.error.message;
      this.notifier.show({
        type: 'error',
        message: error.message
      });

    } else {
      // Get server-side error
      errorMessage = `Error Code: ${error.status}\nMessage: ${error.message}`;
      this.notifier.show({
        type: 'error',
        message: error.message
      });
    }
    console.log(errorMessage);
    return throwError(errorMessage);
  }


}
