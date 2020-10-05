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
import { FormBuilder, FormGroup } from '@angular/forms';
import { DatePickerDirective, DatePickerComponent } from 'ng2-jalali-date-picker';
import { ImageDateService } from '../../services/imageDate.service';
import { iImageDateSearch } from '../../interfaces/iImageDateSearch';


@Component({
  selector: 'image-date-display',
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
  public searchForm: FormGroup | undefined;
  private readonly notifier: NotifierService;
  pageSize: number = 10; pageIndex: number = 1;// and set in paging config
  totalPage: number = 0; pages: number[] = []; row: number = 1;
  list: any[] = [];
  date: any = null;
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
    private service: ImageDateService,
    private sharedService: SharedService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
    this.notifier = notifierService;
  }
  //#endregion
  //#region metods
  ngOnInit() {
    this.searchForm = this.formBuilder.group({
      date: [''],
    });
    this.getPage(this.date, this.pageIndex, this.pageSize);
    // Seo
    this.SeoConfig();
  }
  ngAfterViewInit() {
  }
  ngOnDestroy() { }

  onDelete(id: number, index: number) {
    if (!confirm("ایا از حذف مطمئن هستید ؟"))
      return false;

    this.service.delete(id).subscribe(result => {
      this.notifier.show({
        type: 'success',
        message: 'با موفقیت حذف شد شد.'
      });
      this.list.splice(index, 1);
    }, err => {
      this.notifier.show({
        type: 'error',
        message: err.error.message
      });
      throw err;
    });
  }
  onSubmit(items: any) {
    this.getPage(items.date, this.pageIndex, this.pageSize);
  }
  onChSize(value: any): void {
    this.pageSize = Number(value);
    this.getPage(this.date, this.pageIndex, this.pageSize);
  }
  onPageChange(i: number) {
    this.getPage(this.date, i, this.pageSize);
    this.row = ((i - 1) * this.pageSize) + 1;
    this.config.currentPage = i;
  }
  getPage(date:any=null, pageIndex: number, pageSize: number) {
    this.loading = true;
    this.pages = [];
    let model: iImageDateSearch = {
      date: date, pageIndex: pageIndex, pageSize: pageSize,
    };
    this.service.get(model).subscribe(result => {
      console.log(result);
      let output = result as any;
      if (output != undefined) {
        this.list = (output.pages != undefined) ? output.pages : [];
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
    this.seoService.SetTitle('عکس های روز');
    // seo >> set meta for page
    let meta = {
      charset: '', content: 'عکس های روز', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

  //#endregion

}
