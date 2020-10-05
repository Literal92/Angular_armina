import { Component, OnInit, Inject, AfterViewInit, ViewChild, ElementRef, Renderer2  } from "@angular/core";
import { HttpErrorResponse } from "@angular/common/http";
//import { DOCUMENT } from '@angular/common';
import { NgForm } from "@angular/forms";
import { ActivatedRoute, Router } from "@angular/router";
import { AuthService, Credentials, AuthGuardPermission, ShemaUrl, IAppConfig, APP_CONFIG } from 'src/app/core';
import { SeoService } from 'src/app/core/services/seo.service';
import { MetaDefinition } from '@angular/platform-browser';
declare var grecaptcha: any;
@Component({
  selector: "app-login",
  templateUrl: "./login.component.html",
  styleUrls: ["./login.component.scss"]
})
export class LoginComponent implements OnInit, AfterViewInit {

  //#region Fields
  model: Credentials = { username: "", password: "", rememberMe: false, captchaToken:""};
  error = "";
  returnUrl: string | null = null;
  //#ednregion

  //#region Ctor

  constructor(
    private authService: AuthService,
    private router: Router,
    private route: ActivatedRoute,
    private seoService: SeoService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig,
    r: Renderer2
  ) {
    r.addClass(document.body, 'login-body');
  }

  //#endregion

  // #region Methods
  ngOnInit() {
    // set script google-recaptcha v3
    //let script = document.createElement("script");
    //script.type = 'text/javascript';
    ////script.async = true;
    ////script.src = "https://www.google.com/recaptcha/api.js?render=6LeipJEUAAAAADi6FBYtuN2RtVKBw7EGrNoHBrKX";
    //script.src = `https://www.google.com/recaptcha/api.js?render=${this.sitKey}`;
    //document.body.appendChild(script);
    ////
    // reset the login status
    // this.authservice changed by kamal
    this.authService.logout(false);

    // get the return url from route parameters
    this.returnUrl = this.route.snapshot.queryParams["returnUrl"];

    // seo
    this.SeoConfig();
    
  }
  ngAfterViewInit() {
    // set script google-recaptcha for get token
    //let script = document.createElement("script");
    //script.text = `grecaptcha.ready(function () {
    //  grecaptcha.execute('${this.sitKey}',
    //  { action: 'clientAdmin/loginadmin' })
    //    .then(function(token) {
    //        // add token value to form
    //        document.getElementById('captchaTokenEl').value = token;});
    //  });`;
    //setTimeout(() => {
    //document.body.appendChild(script);
    //}, 3000)

  }

  submitForm(form: NgForm) {
    this.error = "";
    this.authService.login(this.model)
      .subscribe(isLoggedIn => {
        if (isLoggedIn) {
          // check claim dashbord
          if (this.authService.isAuthUserInRoles(["superAdmin"]) ||
            this.authService.isAuthUserInRoles(["AccountantAdmin"]) ||
            this.authService.isAuthUserInRoles(["OrderAdmin"]) ||
            this.authService.isAuthUserInRoles(["ProductAdmin"]) ||
            this.authService.isAuthUserInRoles(["ReportAdmin"]))
          {
            
            if (this.returnUrl) {
              this.router.navigate([this.returnUrl]);
            } else {
              this.router.navigate([`${this.appConfig.apiAdminPathFront}`]);
            }
          }
          else {
            this.error = "مجوز دسترسی برای شما صادر نگردیده است..";
          }


        }
      },
        (error: HttpErrorResponse) => {
          console.error("Login error", error);
          //if (error.status === 401) {
          //  this.error = "نام کاربری یا پسورد اشتباه است.";
          //}
          //else {
          let errorMsg: any = error;
          if (error.ok == false && error.status==404) {
            this.error = `NotFound: ${errorMsg.error}`;
            return;
          }
            this.error = `${error.statusText}: ${errorMsg.error}`;
          //}
        });
  }

  //#region Seo config
  SeoConfig() {
    // seo >> set title page
    this.seoService.SetTitle('ورود به مدیریت');
    // seo >> set meta for page
    let meta = {
      charset: '', content: 'description description description', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion
  //#endregion
}
