import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Router } from "@angular/router";
import { Observable, of, throwError } from "rxjs";
import { catchError, delay, mergeMap, retryWhen, take } from "rxjs/operators";

import { AuthTokenType } from "./../models/auth-token-type";
import { TokenStoreService } from "./token-store.service";
import { HelperError } from './helper.error.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {

  private delayBetweenRetriesMs = 1000; 
  private numberOfRetries = 1; // when status !=200 >> 3 consecutive tries
  private authorizationHeader = "Authorization";

  constructor(
    private tokenStoreService: TokenStoreService
    , private help: HelperError
    , private router: Router) { }

  intercept(request: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    const accessToken = this.tokenStoreService.getRawAuthToken(AuthTokenType.AccessToken);
   // if (accessToken) {
      request = request.clone({
        headers: request.headers.set(this.authorizationHeader, `Bearer ${accessToken}`)
      });
      return next.handle(request).pipe(
        retryWhen(errors => errors.pipe(
          mergeMap((error: HttpErrorResponse, retryAttempt: number) => {
            if (retryAttempt === this.numberOfRetries - 1) {
              console.log(`HTTP call '${request.method} ${request.url}' failed after ${this.numberOfRetries} retries.`);
              return throwError(error); // no retry
            }
            let errorCustome: any = error;
            let errorBody: any = null;
            debugger;
            if (errorCustome.error != undefined) {
              if (errorCustome.error != null) {
                errorBody = errorCustome.error != undefined ? errorCustome.error.description != undefined ? errorCustome.error.description :
                  errorCustome.error.message != undefined ? errorCustome.error.message.description != undefined ?
                    errorCustome.error.message.description : errorCustome.error.message :
                    errorCustome.error.text != undefined ? errorCustome.error.text :
                      errorCustome.error : errorCustome.error;
              }
            }
            switch (error.status) {
             
              case 400:
                {
                  this.help.error(400, errorBody);
                  break;
                }
              case 401:
                {
                  this.help.error(401, errorBody);
                  break;
                }
              case 403:
                {
                  this.help.error(403, errorBody == "" ? "عدم دسترسی" : errorBody);
                  break;

                }
              case 404:
                {
                  if (error.statusText == "OK") {
                    let errorCustome: any = error;
                    if (errorCustome.error != undefined) {
                      if (errorCustome.error != null) {
                        errorBody = errorCustome.error;
                        this.help.error(404, errorBody, 'NotFound')
                      }
                    }
                  }
                    else {
                      this.help.error(404, errorBody);
                    }
                  break;
                }
              case 409:
                {
                  this.help.error(409, errorBody);
                  break;

                }
              case 422:
                {
                  this.help.error(422, errorBody);
                  break;
                }
              case 500:
                {
                  this.help.error(500, errorBody);
                  break;
                }
              case 502:
                {
                  this.help.error(502, "Get bad a way");
                  break;
                }
              default:
                {
                  return throwError(error); // no retry
                  this.help.error(0, error);
                  break;
                }
            }

            return of(error); // retry
          }),
          delay(this.delayBetweenRetriesMs),
          take(this.numberOfRetries)
        )),
        catchError((error: any, caught: Observable<HttpEvent<any>>) => {
          console.error({ error, caught });
          if (error.status === 401 || error.status === 403) {
            const newRequest = this.getNewAuthRequest(request);
            if (newRequest) {
              console.log("Try new AuthRequest ...");
              return next.handle(newRequest);
            }
            //this.router.navigate(["/accessDenied"]);
            this.router.navigate(["/login"]);

          }
          return throwError(error);
        })
      );
   // } else {
      // login page
    //  return next.handle(request);
   // }
  }

  getNewAuthRequest(request: HttpRequest<any>): HttpRequest<any> | null {
    const newStoredToken = this.tokenStoreService.getRawAuthToken(AuthTokenType.AccessToken);
    const requestAccessTokenHeader = request.headers.get(this.authorizationHeader);
    if (!newStoredToken || !requestAccessTokenHeader) {
      console.log("There is no new AccessToken.", { requestAccessTokenHeader: requestAccessTokenHeader, newStoredToken: newStoredToken });
      return null;
    }
    const newAccessTokenHeader = `Bearer ${newStoredToken}`;
    if (requestAccessTokenHeader === newAccessTokenHeader) {
      console.log("There is no new AccessToken.", { requestAccessTokenHeader: requestAccessTokenHeader, newAccessTokenHeader: newAccessTokenHeader });
      return null;
    }
    return request.clone({ headers: request.headers.set(this.authorizationHeader, newAccessTokenHeader) });
  }
}
