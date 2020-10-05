import { Component, OnInit, AfterViewInit, ViewChild, Inject, TemplateRef } from '@angular/core';
import { Subject } from 'rxjs';
import { NotifierService } from 'angular-notifier';
import * as $ from 'jquery';
import { Router, ActivatedRoute } from '@angular/router';
import { IAppConfig, APP_CONFIG } from 'src/app/core';
import { SeoService } from 'src/app/core/services/seo.service';
import { MetaDefinition, DomSanitizer } from '@angular/platform-browser';
import { PaginationInstance } from 'ngx-pagination';
import { NgxLoadingComponent, ngxLoadingAnimationTypes } from 'ngx-loading';
import { SpecialService } from '../../services/special.service';
import { iSpecialSearch } from '../../interfaces/iSpecialSearch';
import { iSpecial } from '../../interfaces/iSpecial';
import { SpecialState } from 'src/app/shared/enum/specialState';
import { FormGroup, FormBuilder } from '@angular/forms';
@Component({
  selector: 'special-display',
  templateUrl: './display.component.html'
})
export class DisplayComponent implements OnInit, AfterViewInit {
  //#region Fields
  private readonly notifier: NotifierService;
  public searchForm: FormGroup | undefined;

  rows: number = 0;
  public Id: number;
  pages: any[] = [];
  public list: any[] = []; public TotalRows: number = 0;
  public pagesize: number = 10; public pageIndex: number = 1; public row: number = 1;
  public model: iSpecialSearch = { pageIndex: 1, pageSize: 10 };
  public frenchiseSpecial: number = 0;
  status: any[] = [
    { id: -1, display:'همه'},
    { id: SpecialState.pendding, display: 'معلق' },
    { id: SpecialState.cancel, display: 'کنسل' },
    { id: SpecialState.accept, display: 'تایید' }
  ];
  //#endregion

  //#region paging config
  public filter: string = '';
  public maxSize: number = 7;
  public directionLinks: boolean = true;
  public autoHide: boolean = false;
  public responsive: boolean = false;
  public config: PaginationInstance = {
    id: 'customer_paging',
    itemsPerPage: 1,
    currentPage: 1
  };
  public labels: any = {
    previousLabel: 'قبلی',
    nextLabel: 'بعدی',
    screenReaderPaginationLabel: 'Pagination',
    screenReaderPageLabel: 'page',
    screenReaderCurrentLabel: `صفحه`
  };

  //#endregion

  //#region loading

  public ngxLoadingAnimationTypes = ngxLoadingAnimationTypes;
  public loading = false;
  public coloursEnabled = false;
  public loadingTemplate: TemplateRef<any>;
  public configLoading = {
    animationType: ngxLoadingAnimationTypes.circle,
    primaryColour: '#dd0031', secondaryColour: '#006ddd',
    tertiaryColour: '#ffffff', backdropBorderRadius: '3px',
    backdropBackgroundColour: 'rgba(0,0,0,0.1)',
  };

  //#endregion
  //#endregion

  //#region Ctor
  constructor(
    private router: Router,
    protected route: ActivatedRoute,
    private formBuilder: FormBuilder,
    // loading
    private sanitizer: DomSanitizer,
    private notifierService: NotifierService,
    private seoService: SeoService,
    private service: SpecialService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
    this.notifier = notifierService;
  }
  //#endregion
  //#region metods
  ngOnInit() {
    // Seo
    this.SeoConfig();
    this.getPage(this.model);
    this.searchForm = this.formBuilder.group({
      id: [''],
      mobile: [''],
      state: [this.status[0].id],
    });
  }
  ngAfterViewInit() {
  }
  ngOnDestroy() {
  }
  onSearch(items: iSpecialSearch) {
    let model: iSpecialSearch = { mobile: items.mobile, state: items.state };
    this.getPage(model);
    //this.service.Get(model).subscribe(result => {
    //  let output = result as any;
    //  if (output != undefined) {
    //    this.list = output.pages;
    //  }
    //}, err => {
    //  throw err;
    //});
  }
  onChSize(value: any): void {
    this.pagesize = Number(value);

    this.model.pageSize = this.pagesize;

    this.getPage(this.model);
  }
  onPageChange(i: number) {
    this.row = ((i - 1) * this.pagesize) + 1;
    this.model.pageSize = this.pagesize;
    this.model.pageIndex = i;
    this.getPage(this.model);
    this.config.currentPage = i;
  }
  getPage(model: iSpecialSearch) {
    this.loading = true;
    this.pages = [];
    this.list = [];
    let rows = 0;
    this.service.Get(model).subscribe(result => {
          console.log(result);
          this.list = (result as any).pages;
          let rows = (result as any).count;
          this.TotalRows = (result as any).totalPage;
      this.frenchiseSpecial = (result as any).frenchiseSpecial
          for (var i = 1; i <= this.TotalRows; i++) {
            this.pages.push(i);
          }
          this.loading = false;
        }, err => {
      this.loading = false;
      throw err;
    });
  }
  onAccept(id:number, index:number) {
    let find = this.list[index].providerId;
    if (find == undefined) {
      this.notifier.show({ type: 'error', message:'ایتم یافت نشد !' })
      return;
    }
    let model: iSpecial = { id:id, providerId: find, state: SpecialState.accept };
    this.service.Put(model).subscribe(result => {
      let output = result as any;
      if (output != undefined) {

        this.list[index].state = output.state;
        this.list[index].start = output.start;
        this.list[index].end = output.end;

        this.notifier.show({ type: 'success', message: 'با موفقیت ثبت شد' })
      }
    }, err => {
      throw err;
    });
  }
  onReject(id:number , index: number) {
    let find = this.list[index].providerId;
    if (find == undefined) {
      this.notifier.show({ type: 'error', message: 'ایتم یافت نشد !' })
      return;
    }
    let model: iSpecial = { id: id, providerId: find, state: SpecialState.cancel };
    this.service.Put(model).subscribe(result => {
      let output = result as any;
      if (output != undefined) {
        this.list[index].state = output.state;
        this.notifier.show({ type: 'success', message: 'با موفقیت ثبت شد' });
      }
    }, err => {
      throw err;
    });
  }
  //#region Seo config
  SeoConfig() {
    // seo >> set title page
    this.seoService.SetTitle('ویژه شدن');
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
