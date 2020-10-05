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
import * as moment from 'jalali-moment';
import { SharedService } from 'src/app/shared/services/shared.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { FieldService } from '../../services/field.service';
import { iFieldSearch } from '../../interfaces/iFieldSearch';

@Component({
  selector: 'field-display',
  templateUrl: './display.component.html',
  styles: [`.btn-day{width:80px}`]
})
export class DisplayComponent implements OnInit, AfterViewInit {
  //#region Fields
  public searchForm: FormGroup | undefined;
  private readonly notifier: NotifierService;
  pageSize: number = 10; pageIndex: number = 1;// and set in paging config
  totalPage: number = 0; pages: number[] = []; row: number = 1;
  fields: any[] = [];
  field: any = null;
  ////#endregion


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
    private service: FieldService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
    this.notifier = notifierService;
  }
  //#endregion
  //#region metods
  ngOnInit() {

    // Seo
    this.SeoConfig();
    this.route.params.subscribe(params => {
          this.getPage(this.pageIndex, this.pageSize);
    });

  }
  ngAfterViewInit() {
  }
  ngOnDestroy() { }
 
  onDelete(id: number, index:number) {
    if (!confirm("ایا از حذف مطمئن هستید ؟")) 
         return false;
         
    this.service.Delete(id).subscribe(result => {
      this.fields.splice(index,1);
      this.notifier.show({
        type: 'success',
        message:'با موفقیت اعمال شد'
      });
    }, err => {
      this.notifier.show({
        type: 'error',
        message: err.error.text
      });
      throw err;
    });
  }
  onChSize(value: any): void {
    this.pageSize = Number(value);
    this.getPage(this.pageIndex, this.pageSize);
  }
  onPageChange(i: number) {
    this.getPage(i, this.pageSize);
    this.row = ((i - 1) * this.pageSize) + 1;
    this.config.currentPage = i;
  }
  getPage(pageIndex: number, pageSize: number) {
    this.loading = true;
    this.pages = [];
    let model: iFieldSearch = { pageIndex: pageIndex, pageSize: pageSize };
    this.service.get(model).subscribe(result => {
      console.log(result);
      let output = result as any;
      if (output != undefined) {
        this.fields = (output.pages != undefined) ? output.pages : [];
        this.totalPage = output.totalPage;
        let rows = output.totalPage;
        for (var i = 1; i <= this.totalPage; i++) {
          this.pages.push(i);
        }
      }
      this.loading = false;

    }, err => {
      this.loading = false;

      throw err;
    });
  }
  //#region Seo config
  SeoConfig() {
    // seo >> set title page
    this.seoService.SetTitle('مدیریت فیلدها');
    // seo >> set meta for page
    let meta = {
      charset: '', content: 'مدیریت فیلدها', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

  //#endregion

}
