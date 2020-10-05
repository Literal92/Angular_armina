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
import { sortProvider } from 'src/app/shared/enum/orderProvider';
import { iCategory } from '../../interfaces/iCategory';
import { SharedService } from 'src/app/shared/services/shared.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CategoryService } from '../../services/category.service';
import { iCategorySearch } from '../../interfaces/iCategorySearch';

@Component({
  selector: 'category-display',
  templateUrl: './display.component.html',
  styles: [`.btn-day{width:80px}`]
})
export class DisplayComponent implements OnInit, AfterViewInit {
  //#region Fields
  public searchForm: FormGroup | undefined;
  private readonly notifier: NotifierService;
  pageSize: number = 10; pageIndex: number = 1;// and set in paging config
  totalPage: number = 0; pages: number[] = []; row: number = 1;
  categories: any[] = []; isChkParent: boolean = false;
  title: string = null;
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
    private service: CategoryService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
    this.notifier = notifierService;
  }
  //#endregion
  //#region metods
  ngOnInit() {
 // Seo
    this.SeoConfig();
    this.getPage(this.pageIndex, this.pageSize);

   
  }
  ngAfterViewInit() {
  }
  ngOnDestroy() { }
  onDelete(id: number, index: number) {
    if (!confirm("ایا از حذف مطمئن هستید ؟")) {
      return false;
    }
    this.service.delete(id).subscribe(result => {
      this.notifier.show({
        type: 'success',
        message: 'با موفقیت حذف شد.'
      });
      this.categories.splice(index, 1);
    }, err => {
      this.notifier.show({
        type: 'error',
        message:'خطا در حذف'
      });
      throw err;
    });
  }
 
  onChSize(value: any): void {
    this.pageSize = Number(value);
    this.getPage(this.pageIndex, this.pageSize,this.title);
  }
  onPageChange(i: number) {
    this.getPage(i, this.pageSize, this.title);
    this.row = ((i - 1) * this.pageSize) + 1;
    this.config.currentPage = i;
  }
  onSearch(title: string) {
    this.title = title;
    this.getPage(this.pageIndex = 1, this.pageSize, this.title);
    this.isChkParent = false;
  }
  onSearchChild(parentTitle: string) {
    this.title = parentTitle;
    this.isChkParent = true;
    this.getPage(this.pageIndex = 0, this.pageSize, this.title);
  }
  getPage(pageIndex: number, pageSize: number, term?:string) {
    this.loading = true;
    this.pages = [];
    let model: iCategorySearch = { pageIndex: pageIndex, pageSize: pageSize, title: term, isChkParent: this.isChkParent };
    this.service.get(model).subscribe(result => {
      console.log(result);
      let output = result as any;
      if (output != undefined) {
        this.categories = (output.pages != undefined) ? output.pages : [];
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
    this.seoService.SetTitle('لیست دسته بندی ها');
    // seo >> set meta for page
    let meta = {
      charset: '', content: 'لیست دسته بندی ها', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

  //#endregion

}
