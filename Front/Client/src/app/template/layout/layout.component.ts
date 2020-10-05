import { Component,OnInit, OnDestroy, Inject, HostBinding } from '@angular/core';
import { DOCUMENT } from '@angular/common';
//import { DOCUMENT } from "@angular/platform-browser";
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';

@Component({
  selector: 'client-layout',
  templateUrl: './layout.component.html'
})
export class ClientLayoutComponent implements OnInit, OnDestroy {

  ngOnInit() {
  }
  ngOnDestroy(): void {
  }
}
