import { Component,OnInit, OnDestroy, Inject, HostBinding } from '@angular/core';
import { DOCUMENT } from '@angular/common';
//import { DOCUMENT } from "@angular/platform-browser";
import { Router, ActivatedRoute, NavigationEnd } from '@angular/router';

@Component({
  selector: 'wellcome-layout',
  templateUrl: './layout.component.html'
})
export class WellLayoutComponent implements OnInit, OnDestroy {

  ngOnInit() {
  }
  ngOnDestroy(): void {
  }
}
