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
import { OrderService } from '../../services/order.service';
import { iorderSearch } from '../../interfaces/iorderSearch';
import { SharedService } from 'src/app/shared/services/shared.service';
import { FormBuilder, FormGroup } from '@angular/forms';
import { modal } from 'src/app/shared/js/modal';
import { DatePickerDirective, DatePickerComponent } from 'ng2-jalali-date-picker';


@Component({
  selector: 'address-display',
  templateUrl: './address.component.html',
  styles: []
})
export class AddressComponent implements OnInit, AfterViewInit {
  //#region Fields
  public list: any[] = [];
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
    private seoService: SeoService,
    private service: OrderService,
    private sharedService: SharedService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
    document.body.className = "body-print";
  }
  //#endregion
  //#region metods
  ngOnInit() {
    this.getPage();
    // Seo
    this.SeoConfig();
  }
  ngAfterViewInit() {
  }
  ngOnDestroy() { }

  
  getPage() {
    this.loading = true;
    this.service.getOrdersByAddress().subscribe(result => {
      console.log(result);
      let output = result as any;
      if (output != undefined) {
        this.list = (output != undefined) ? output : [];
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
    this.seoService.SetTitle('لیست ادرس سفارشات بسته بندی نشده');
    // seo >> set meta for page
    let meta = {
      charset: '', content: 'لیست ادرس سفارشات بسته بندی نشده', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

  //#endregion

}
