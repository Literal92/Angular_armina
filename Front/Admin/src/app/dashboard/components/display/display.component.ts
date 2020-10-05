import { Component, OnInit, AfterViewInit, ViewChild, Inject, TemplateRef } from '@angular/core';
import { Subject } from 'rxjs';
import { NotifierService } from 'angular-notifier';
import * as $ from 'jquery';
import { Router, ActivatedRoute } from '@angular/router';
import { IAppConfig, APP_CONFIG } from 'src/app/core';
import { SeoService } from 'src/app/core/services/seo.service';
import { MetaDefinition, DomSanitizer } from '@angular/platform-browser';
import { DashboardService } from '../../services/dashboard.service';

@Component({
  selector: 'dashboard-display',
  templateUrl: './display.component.html'
})
export class DisplayDashboardComponent implements OnInit, AfterViewInit {
  //#region Fields

  //#endrigion

  //#region Ctor
  constructor(
    private router: Router,
    protected route: ActivatedRoute,
    private service: DashboardService,
    private seoService: SeoService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
  }
  //#endregion

  //#region metods
  ngOnInit() {
    // Seo
    this.SeoConfig();
  }
  ngAfterViewInit(): void {
  }

  //#region Seo config
  SeoConfig() {
    // seo >> set title page
    this.seoService.SetTitle('داشبورد');
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
