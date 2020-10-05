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
import { SharedService } from 'src/app/shared/services/shared.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { EnableLoading, DisableLoading } from '../../../core/loading/loading';
import { OrderService } from '../../services/order.service';
import { FormatPricePipe } from '../../../shared/pipe/price.pipe';


@Component({
  selector: 'report-daily',
  templateUrl: './report-daily.component.html'
})
export class ReportDailyComponent implements OnInit {
  //#region Fields
  public searchForm: FormGroup | undefined;
  public searchMonthlyForm: FormGroup | undefined;
  private readonly notifier: NotifierService;
  datePickerConfig = {
    drops: 'down',
    opens: 'right',
    format: 'YYYY/MM/DD'
  };
  show: boolean = false;
  pageSize: number = 10; pageIndex: number = 1;// and set in paging config
  totalPage: number = 0; pages: number[] = []; row: number = 1;
  list: any[] = [];

  years: any[] = [
  { id: 1398, display: '1398' },
  { id: 1399, display: '1399' },
  { id: 1400, display: '1400' }];

  months: any[] = [
  { id: 1, display: 'فروردین' },
  { id: 2, display: 'ادیبهشت' },
  { id: 3, display: 'خرداد' },
  { id: 4, display: 'تیر' },
  { id: 5, display: 'مرداد' },
  { id: 6, display: 'شهریور' },
  { id: 7, display: 'مهر' },
  { id: 8, display: 'ابان' },
  { id: 9, display: 'اذر' },
  { id: 10, display: 'دی' },
  { id: 11, display: 'بهمن' },
  { id: 12, display: 'اسفند' }];
  ////#endregion



  //#endregion

  //#region Ctor
  constructor(
    private router: Router,
    protected route: ActivatedRoute,
    private formBuilder: FormBuilder,
    // loading
    private notifierService: NotifierService,
    private seoService: SeoService,
    private service: OrderService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
    this.notifier = notifierService;
  }
  //#endregion
  //#region metods

  public barChartOptions: any = {
    scaleShowVerticalLines: false,
    responsive: true,
    scales: {
      yAxes: [{
        ticks: {
          beginAtZero: true
        }
      }]
    }
    
  };
  public barChartLabels: string[] = [/*'2006', '2007', '2008', '2009', '2010', '2011', '2012'*/];
  public barChartType = 'bar';
  public barChartLegend = true;

  public barChartData: any[] = [
    { data: [], label: '' },
    //{ data: [28, 48, 40, 19, 86, 27, 90], label: 'Series B' }
  ];



  ngOnInit() {
    // Seo
    this.SeoConfig();
    EnableLoading();

    this.searchForm = this.formBuilder.group({
      from: [''],
      to: ['']      
    });
    this.searchMonthlyForm = this.formBuilder.group({
      fromMonth: [this.months[0].id],
      toMonth: [this.months[0].id],
      year:[this.years[0].id]
    });
     this.getPage(null,null);
  }
  ngAfterViewInit() {
  }
  ngOnDestroy() { }
  onSubmit(items: any) {
    let utcTo = null;
    let utcFrom = null;

    if (items.to!= undefined) {
      if (items.to != "") {
        utcTo = moment(items.to, 'jYYYY/jMM/jDD').locale('en').utc(true).format('YYYY/MM/DD');
      }
    }
    if (items.from != undefined) {
      if (items.from != "") {
        utcFrom = moment(items.from, 'jYYYY/jMM/jDD').locale('en').utc(true).format('YYYY/MM/DD');
      }
    }
    this.getPage(utcFrom, utcTo)
  }
  onSubmitMonthly(items: any) {
    this.getPageMonthly(items.fromMonth, items.toMonth, items.year);
  }
  getPage(from:any, to:any) {

    EnableLoading();
    this.pages = [];
    this.service.GetDailyReport(from,to).subscribe(result => {
      DisableLoading();
      console.log(result);
      let output = result as any;
      if (output != undefined) {
        let data = [];
        this.barChartLabels = [];
        for (let item of output) {
          let day = item.dateFa;
          this.barChartLabels.push(day);
          data.push(item.count);
        }
        this.barChartData[0].data = data;
        this.barChartData[0].label = "گزارش درامد روزانه";
      }

    }, err => {
      DisableLoading();

      throw err;
    });
  }
  getPageMonthly(from: any, to: any, year: any) {
    debugger;
    EnableLoading();
    this.pages = [];
    this.service.GetMonthReport(from, to, year).subscribe(result => {
      DisableLoading();
      console.log(result);
      let output = result as any;
      if (output != undefined) {
        let data = [];
        this.barChartLabels = [];
        for (let item of output) {
          let month = this.transform(item.month);
          this.barChartLabels.push(month);
          data.push(item.count);
        }
        this.barChartData[0].data = data;
        this.barChartData[0].label = "گزارش درامد ماهیانه";
      }

    }, err => {
      DisableLoading();

      throw err;
    });
  }

  transform(id: number) {
    switch (id) {
      case 1: return 'فروردین';
      case 2: return 'ادیبهشت';
      case 3: return 'خرداد';
      case 4: return 'تیر';
      case 5: return 'مرداد';
      case 6: return 'شهریور';
      case 7: return 'مهر';
      case 8: return 'ابان';
      case 9: return 'اذر';
      case 10: return 'دی';
      case 11: return 'بهمن';
      case 12: return 'اسفند';
    }
  }


  //#region Seo config
  SeoConfig() {
    // seo >> set title page
    this.seoService.SetTitle('گزارش مالی');
    // seo >> set meta for page
    let meta = {
      charset: '', content: 'گزارش مالی', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

  //#endregion

}
