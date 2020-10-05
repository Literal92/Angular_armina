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
import { iEditAddress } from '../../interfaces/iEditAddress';


@Component({
  selector: 'product-display',
  templateUrl: './display.component.html',
  styles: [`.btn-day{width:80px}
  .fabIcon{
    position: fixed;
    left: 51px;
    bottom: 51px;
    padding: 20px;
    border-radius: 50%;
    width: 60px;
    height: 60px;
    box-shadow: 0 3px 7px -1px #333;
  }
.ordersc{margin-left:-30px;margin-right:-30px}
`]
})
export class DisplayComponent implements OnInit, AfterViewInit {
  //#region Fields
  datePickerConfig = {
    drops: 'down',
    opens: 'right',
    format: 'YYYY/MM/DD'
  };

  orderType: any[] = [{ title: "همه", value: -1 },
  { title: "در انتظار پرداخت", value: 0/*, id: 0*/ },
  { title: "پرداخت شده", value: 1/*, id: 1*/ },
  { title: "لغو شده", value: 2 /*, id: 2*/ },
  ];

  orderSendType: any[] = [{ title: "همه", value: -1 },
  { title: "پاکت نشده", value: 0/*, id: 0*/ },
  { title: "پاکت شده", value: 1/*, id: 1*/ },
  { title: "ارسال شده", value: 2 /*, id: 2*/ },
  ];



  public searchForm: FormGroup | undefined;
  public addressForm: FormGroup | undefined; 
  private readonly notifier: NotifierService;
  pageSize: number = 10; pageIndex: number = 1;// and set in paging config
  totalPage: number = 0; pages: number[] = []; row: number = 1;
  products: any[] = [];
  orderProducts: any[] = [];
  username: string = null; access: boolean = undefined;
  provider: any = undefined;
  order: sortProvider = sortProvider.createdDesc;
  organ: string = null;
  model: iorderSearch = null;
  dateFrom: any = null;
  dateTo: any = null;
  orderTypeId: any = null;
  orderSendTypeId: any = null;
  id: number;
  productName: string = null;
  editAddressId: number;
  reciverName: string = '';
  reciverMobile: string = '';
  senderName: string = '';
  senderMobile: string = '';
  address: string = '';

  selectedAddress: string = '';
  ordershot: string = '';

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
    private service: OrderService,
    private sharedService: SharedService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
    this.notifier = notifierService;
  }
  //#endregion
  //#region metods
  ngOnInit() {
    this.searchForm = this.formBuilder.group({
      id:[''],
      username: [''],
      reciverName: [''],
      reciverMobile: [''],
      senderName: [''],
      senderMobile: [''],
      address:[''],
      orderType: [this.orderType[0].value],
      orderSendType: [this.orderSendType[0].value],
      from: [''],
      to: [''],
      productName:['']
    });
    this.addressForm = this.formBuilder.group({
      senderMobile:[''],
      senderName: [''],
      reciverMobile: [''],
      reciverName: [''],
      reciverAddress: ['']
    });
    this.getPage(this.pageIndex, this.pageSize);
    // Seo
    this.SeoConfig();
  }
  ngAfterViewInit() {
  }
  ngOnDestroy() { }

  onDel(id: number, index: number) {
    if (!confirm("ایا از حذف مطمئن هستید ؟"))
      return false;

    this.service.delete(id).subscribe(result => {
      this.notifier.show({
        type: 'success',
        message: 'با موفقیت حذف شد شد.'
      });
      this.products.splice(index, 1);
    }, err => {
      this.notifier.show({
        type: 'error',
        message: err.error.message
      });
      throw err;
    });
  }
  onAcceptPayment(id: number, index: number) {
    this.service.AcceptPayment(id, true).subscribe(result => {
      this.notifier.show({
        type: 'success',
        message: 'با موفقیت ثبت شد.'
      });
      this.products[index].acceptPayment = true;
    }, err => {
        this.notifier.show({
          type: 'error',
          message: err.error.text
        });
        throw err;
    });
  }
  onDisacceptPayment(id: number, index: number) {
    this.service.AcceptPayment(id, false).subscribe(result => {
      this.notifier.show({
        type: 'success',
        message: 'با موفقیت ثبت شد.'
      });
      this.products[index].acceptPayment = false;

    }, err => {
      this.notifier.show({
        type: 'error',
        message: err.error.text
      });
      throw err;
    });
  }
  onSubmit(items: any) {
    console.log(items);

    let dateFromFa = items.from;
    let datefrom = undefined;
    if (dateFromFa != undefined) {
      if (dateFromFa != "") {
        this.dateFrom = moment(dateFromFa, 'jYYYY/jMM/jDD').locale('en').format('YYYY/MM/DD');
      }
    }
    let dateToFa = items.to;
    let dateTo = undefined;
    if (dateToFa != undefined) {
      if (dateToFa != "") {
        this.dateTo = moment(dateToFa, 'jYYYY/jMM/jDD').locale('en').format('YYYY/MM/DD');
      }
    }
    this.orderTypeId = items.orderType == -1 ? null : items.orderType;
    this.orderSendTypeId = items.orderSendType == -1 ? null : items.orderSendType;
    this.productName = items.productName;
    this.address = items.address;
    this.senderMobile = items.senderMobile;
    this.senderName = items.senderName;
    this.reciverName = items.reciverName;
    this.reciverMobile = items.reciverMobile;
    this.id = items.id;

      this.username = items.username;
    this.getPage(this.pageIndex, this.pageSize);
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
    this.model = {
      id: this.id,
      productName: this.productName,
      username: this.username, pageIndex: pageIndex, pageSize: pageSize,
      orderType: this.orderTypeId,
      orderSendType: this.orderSendTypeId,
      from: this.dateFrom,
      to: this.dateTo,
      address: this.address,
      reciverMobile: this.reciverMobile,
      reciverName: this.reciverName,
      senderMobile: this.senderMobile,
      senderName: this.senderName      

    };
    this.service.get(this.model).subscribe(result => {
      console.log(result);
      let output = result as any;
      if (output != undefined) {
        this.products = (output.pages != undefined) ? output.pages : [];
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
  onShowProduct(item: any) {
    this.selectedAddress = "";
    this.selectedAddress = item.reciverAddress
    if (item.reciverName != undefined && item.reciverName != '.' && item.reciverName != '0') {
      this.selectedAddress += " گیرنده:" + item.reciverName;
    }

    if (item.reciverMobile != undefined && item.reciverMobile != '.' && item.reciverMobile != '0') {
      this.selectedAddress += " تماس گیرنده:" + item.reciverMobile;
    }
    if (item.senderName != undefined && item.senderName != '.' && item.senderName != '0') {
      this.selectedAddress += " فرستنده:" + item.senderName;
    }
    if (item.senderMobile != undefined && item.senderMobile != '.' && item.senderMobile != '0') {
      this.selectedAddress += " تماس فرستنده:" + item.senderMobile;
    }
    this.orderProducts = [];

    this.service.getOrderProduct(item.id).subscribe(result => {

      let output = result as any;
      if (output != undefined) {
        this.orderProducts = (output.pages != undefined) ? output.pages : [];

        console.log(this.orderProducts);

      }
      this.loading = false;

    }, err => {
      this.loading = false;

      throw err;
    });


    modal('#modal', true);

  }

  onShowShot(item: any) {
    this.ordershot = item;
    modal('#modalshot', true);
  }
  chSendType(item) {
    if (!confirm("ایا نسبت به تغییر وضعیت مطمئن هستید ؟"))
      return false;
    this.loading = true;
    this.service.senderType(item).subscribe(result => {
      this.loading = false;
      if (item.orderSendType==0) {
        item.orderSendType = 1;

      }
      else if (item.orderSendType == 1) {
        item.orderSendType =0 ;

      }
      this.notifier.show({
        type: 'success',
        message: 'با موفقیت ثبت شد'
      });
    }, err => {
    //  item.isActive = isActive;
      this.loading = false;
      this.notifier.show({
        type: 'error',
        message: err.error.text
      });
      throw err;
    });
  }
  pay(item) {
    if (!confirm("ایا تکمیل خرید مطمئن هستید ؟"))
      return false;
    this.loading = true;

    this.service.complete(item).subscribe(result => {
      // بعدا این خط کامنت شود  و به درگاه منتقل شود

      let output = result as any;
      //this.notifier.show({
      //  type: 'success',
      //  message: 'با موفقیت ثبت شد'
      //});
      this.loading = false;


    }, err => {
      this.loading = false;

      throw err;
    })

  }
  onChAddress(id: number) {
    let index=  this.products.findIndex(c => c.id == id);
    if (index > -1) {
      let model = this.products[index];
      this.editAddressId = id;
      this.addressForm.controls['senderMobile'].setValue(model.senderMobile);
      this.addressForm.controls['senderName'].setValue(model.senderName);
      this.addressForm.controls['reciverMobile'].setValue(model.reciverMobile);
      this.addressForm.controls['reciverName'].setValue(model.reciverName);
      this.addressForm.controls['reciverAddress'].setValue(model.reciverAddress);
      modal('#modalAddress', true);
    }
    else {
      this.notifier.show({
        type: 'error',
        message:'ایتمی یافت نشد !'
      });
    }
  }
  onDeletePrd(id: number, index: number, totalPrice: number, orderId: number) {

    if (!confirm("نسبت به حذف هستید ؟"))
      return false;

    this.loading = true;
    this.service.deleteByOrderProductId(id).subscribe(result => {
      this.loading = false;

      this.notifier.show({
        type: 'success',
        message:'با موفقیت حذف شد'
      });
      this.orderProducts.splice(index, 1);
      let findIndex = this.products.findIndex(c => c.id == orderId);
      if (findIndex > -1) {
        this.products[findIndex].totalPrice -= totalPrice;
        this.products[findIndex].totalWithDiscountPrice -= totalPrice;
      }
    }, err => {
        this.loading = false;

        this.notifier.show({
          type: 'error',
          message: err.error
        });
        throw err;
    })
  }
  onEditAdress(items: iEditAddress) {
    this.loading = true;
    items.id = this.editAddressId;
    this.service.editAddress(items).subscribe(result => {
      modal('#modalAddress', false);
      this.loading = false;
      this.notifier.show({
        type: 'success',
        message:'با موفقیت ثبت شد.'
      });
      let index = this.products.findIndex(c => c.id == this.editAddressId);
      if (index > -1) {
        this.products[index].reciverAddress= items.reciverAddress;
        this.products[index].reciverMobile= items.reciverMobile;
        this.products[index].reciverName= items.reciverName;
        this.products[index].senderMobile= items.senderMobile;
        this.products[index].senderName= items.senderName;

      }

    }, err => {
        modal('#modalAddress', false);
        this.loading = false;
        this.notifier.show({
          type: 'error',
          message:err.error
        });
      throw err;
    });
  }
  //#region Seo config
  SeoConfig() {
    // seo >> set title page
    this.seoService.SetTitle('لیست سفارشات');
    // seo >> set meta for page
    let meta = {
      charset: '', content: 'سفارشات', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

  //#endregion

}
