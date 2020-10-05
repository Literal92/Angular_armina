import { Component,OnInit, OnDestroy, Inject, HostBinding } from '@angular/core';
import { DOCUMENT } from '@angular/common';
//import { DOCUMENT } from "@angular/platform-browser";
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';

@Component({
  selector: 'admin-layout',
  templateUrl: './layout.component.html'
})
export class AdminLayoutComponent implements OnInit, OnDestroy {
  public sidebarMinimized = true;
  private changes: MutationObserver;
  public element: HTMLElement;
  // someField: boolean = false;
  @HostBinding('class.fixed') fixed: boolean = false;

  constructor(
    private router: Router,
    private route: ActivatedRoute,
    @Inject(DOCUMENT) public _document?: any
  ) {
    this.changes = new MutationObserver((mutations) => {
      this.sidebarMinimized = _document.body.classList.contains('sidebar-minimized');
    });
    this.element = _document.body;
    this.changes.observe(<Element>this.element, {
      attributes: true,
      attributeFilter: ['class']
    });
    _document.body.classList.remove("login-body");
  }
  ngOnInit() {
    this.fixed = true;
    this.router.events.subscribe((evt) => {
      if (!(evt instanceof NavigationEnd)) {
        return;
      }
      window.scrollTo(0, 0);
    });
  }
  ngOnDestroy(): void {
    this.changes.disconnect();
  }
}
