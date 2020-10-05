import { AuthUser } from './../../core/models/auth-user';
import { Component, Inject, OnInit, AfterViewInit, ChangeDetectorRef, ElementRef } from '@angular/core';
import { Subscription } from "rxjs";
import { AuthService } from 'src/app/core';
import { ActivatedRoute, Router } from '@angular/router';


@Component({
  selector: 'client-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.scss']
})
export class HeaderComponent implements OnInit, AfterViewInit {

  isLoggedIn = false;
  subscription: Subscription | null = null;
  displayName = "";
  user: AuthUser;

  constructor(private elem: ElementRef, private authService: AuthService, private cdRef: ChangeDetectorRef,
    private router: Router,
    private route: ActivatedRoute,

  ) {
  }

  hasAccess(rolename: string) {
    this.user = this.authService.getAuthUser();
    if (this.user && this.user.roles) {
      const rec = this.user.roles.filter(a => a.toLowerCase() == rolename.toLowerCase().trim())
      if (rec && rec.length > 0) {
        return true;
      }
    }
    return false;
  }
  ngOnInit() {

    this.subscription = this.authService.authStatus$.subscribe(status => {
      this.isLoggedIn = status;
      if (status) {


        // this.displayName = authUser ? authUser.displayName : "";
      }
    });

  }

  ngAfterViewInit() {
    //this.subscription = this.authService.authStatus$.subscribe(status => {
    //  this.isLoggedIn = status;
    //  if (status) {
    //    const authUser = this.authService.getAuthUser();
    //    this.displayName = authUser ? authUser.displayName : "";
    //    this.cdRef.detectChanges();
    //  }
    //});
  }

  ngOnDestroy() {
    // prevent memory leak when component is destroyed
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  logout() {
    this.authService.logout(true);
  }

  headerMenuTrigger() {
    var offcanvas = document.querySelector('#offcanvas-menu');
    offcanvas.classList.toggle('active');

    var bodywrapper = document.querySelector('.body-wrapper');
    bodywrapper.classList.toggle('active-overlay');

    var body = document.querySelector('body');
    body.classList.toggle('overflow-hidden');
  }

  searchFocus() {

    var searchkeywordbox = document.querySelector('#search-keyword-box');
    searchkeywordbox.classList.remove("search-keyword-area");
  }
  searchFocusout() {
    var searchkeywordbox = document.querySelector('#search-keyword-box');

    searchkeywordbox.classList.add("search-keyword-area");

  }

  search(value) {
    
    this.router.navigate([''], { queryParams: { title: value } });


  }
}
