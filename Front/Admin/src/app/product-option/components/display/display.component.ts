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
import { ProductOptionService } from '../../services/product-option.service';
import { iProductOptionSearch } from '../../interfaces/iProductOptionSearch';
import { ProductService } from 'src/app/product/services/product.service';
import { iproductSearch } from 'src/app/product/interfaces/iproductSearch';

@Component({
  selector: 'product-option-display',
  templateUrl: './display.component.html',
  styles: [`.btn-day{width:80px}`]
})
export class DisplayComponent implements OnInit, AfterViewInit {
  //#region Fields
  private readonly notifier: NotifierService;
  pageSize: number = 10; pageIndex: number = 1;// and set in paging config
  totalPage: number = 0; pages: number[] = []; row: number = 1;
  producFields:any[]=[];
  public productId:number=0;
  public product:any;

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
    private productService: ProductService,
    private service: ProductOptionService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
    this.notifier = notifierService;
  }
  //#endregion
  //#region metods
  ngOnInit() {

    // Seo
    this.SeoConfig();
    this.route.params.subscribe(params => {
      if (params['id'] != undefined) {
        this.productId = +params['id'];
        this.getProduct(this.productId);
          this.getPage(this.pageIndex, this.pageSize);
      }
    });

  }
  ngAfterViewInit() {
  }
  ngOnDestroy() { }
 
  onRemove(id: number, index:number) {
    if (!confirm("ایا از حذف مطمئن هستید ؟")) 
         return false;
        
    this.service.Delete(id).subscribe(result => {
      this.producFields.splice(index,1);
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
  getProduct(id: number){
    let model:iproductSearch={id: id};
    this.productService.get(model).subscribe(result =>{
      let output = result as any;
      if(output.count >0)
      this.product= output.pages[0];
    }, err=>{
      throw err;
    });
  }
  getPage(pageIndex: number, pageSize: number) {
    this.loading = true;
    this.pages = [];
    let model: iProductOptionSearch = { productId: this.productId, pageIndex: pageIndex, pageSize: pageSize };
    this.service.get(model).subscribe(result => {
      console.log(result);
      let output = result as any;
      if (output != undefined) {
        this.producFields = (output.pages != undefined) ? output.pages : [];
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
    this.seoService.SetTitle('مدیریت فیلد محصول');
    // seo >> set meta for page
    let meta = {
      charset: '', content: 'مدیریت فیلد محصول', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

  //#endregion

}
