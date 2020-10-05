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
import { RollsService } from '../../services/rolls.service';
import { iUserSearch } from '../../interfaces/iUserSearch';
import { modal } from 'src/app/shared/js/modal';
import { first } from 'rxjs/operators';
@Component({
  selector: 'user-display',
  templateUrl: './display.component.html',
  styles: [`.btn-day{width:80px}`]
})
export class DisplayComponent implements OnInit, AfterViewInit {
  //#region Fields
  public searchForm: FormGroup | undefined;
  private readonly notifier: NotifierService;
  pageSize: number = 10; pageIndex: number = 1;// and set in paging config
  totalPage: number = 0; pages: number[] = []; row: number = 1;
  userName: string = null; displayName: string = null; mobile: string = null;
  rolls: any[] = []; id: number = 0;
  showParent: boolean = true;
  showChPass: boolean = false;
  permissions: any[] = [];
  claimExist: any[] = [];
    roleId: number = 0;
    reserves: any[] = []; services: any[] = [];

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
    private service: RollsService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
    this.notifier = notifierService;
  }
  //#endregion
  //#region metods
  ngOnInit() {

    // Seo
    this.SeoConfig();
    this.getPage(this.pageIndex, this.pageSize);
    this.searchForm = this.formBuilder.group({
      displayName: [''],
      userName: [''],
      mobile: [''],
    });
  }
  ngAfterViewInit() {
    this.service.GetPermission().subscribe(result => {
      let output = result as any;
      if (output != undefined) {
        this.permissions = output;
        console.log(this.permissions);
      }
    }, err => {
      throw err;
    });
  }
  ngOnDestroy() { }

  onPermit(id: number, index: number) {
    this.roleId = id;
    this.claimExist = [];
    this.loading = true;
    this.service.GetRoleClaim(id).subscribe(result => {
      this.loading = false;
        modal('#modal', true);

      for (var i = 0; i <= this.permissions.length; i++) {
        $(`input[name="claim-${i}"]`).prop('checked', false);
      }
      let output = result as any;
      if (output != undefined) {
        this.claimExist = output;
      }
    }, err => {
        this.loading = false;
      throw err;
    });
  }
  onChPermit(chk: boolean, item: any, index: number) {
    if (chk == true) {
      if (this.claimExist != undefined) {
        if (this.claimExist.length > 0) {
          let indexItem = this.claimExist.findIndex(c => c.roleId == this.roleId && c.claimType == item.groupName && c.claimValue == item.shortName);
          if (indexItem == -1) {
            this.claimExist.push({ roleId: this.roleId, claimType: item.groupName, claimValue: item.shortName });
          }
        }
        else {
          this.claimExist.push({ roleId: this.roleId, claimType: item.groupName, claimValue: item.shortName });
        }
      }
      else {
        this.claimExist.push({ roleId: this.roleId, claimType: item.groupName, claimValue: item.shortName });
      }
    }
    else {
      if (this.claimExist != undefined) {
        if (this.claimExist.length > 0) {
          let indexItem = this.claimExist.findIndex(c => c.roleId == this.roleId && c.claimType == item.groupName && c.claimValue == item.shortName);
          if (indexItem > -1) {
            this.claimExist.splice(indexItem, 1);
          }
        }
      }
    }
  }
  onSavePermit() {
    console.log(this.claimExist);
    this.loading = true;
    this.service.SetRoleClaims(this.claimExist).subscribe(result => {
      this.notifier.show({
        type: 'success',
        message: 'با موفقیت ثبت شد.'
      });
      this.loading = false;
    }, err => {
        this.loading = false;

      this.notifier.show({
        type: 'error',
        message: err.error
      });
      throw err;
    });
  }

  onSubmit(items: iUserSearch) {
    this.displayName = items.displayName;
    this.mobile = items.mobile;
    this.userName = items.userName;
    this.getPage(this.pageIndex, this.pageSize);
  }
  // onChSize(value: any): void {
  //   this.pageSize = Number(value);
  //   this.getPage(this.pageIndex, this.pageSize);
  // }
  // onPageChange(i: number) {
  //   this.getPage(i, this.pageSize);
  //   this.row = ((i - 1) * this.pageSize) + 1;
  //   this.config.currentPage = i;
  // }
  getPage(pageIndex: number, pageSize: number) {
    this.loading = true;
    this.pages = [];
    this.service.Get().subscribe(result => {
      // debugger;
      console.log(result);
      let output = result as any;
      if (output != undefined) {
          this.rolls = (output.item1 != undefined) ? output.item1 : [];
          let count = output.item2;
          // this.totalPage = Math.round(count / 10) + 1;


          // let rows = output.totalPage;
          // for (var i = 1; i <= this.totalPage; i++) {
          //   this.pages.push(i);
          // }
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
    this.seoService.SetTitle('مدیریت نقش ها');
    // seo >> set meta for page
    let meta = {
      charset: '', content: 'مدیریت نقش ها', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

  //#endregion

}
