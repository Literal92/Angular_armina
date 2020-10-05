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
import { UserManagerService } from '../../services/user-manager.service';
import { iUserSearch } from '../../interfaces/iUserSearch';
import { modal } from 'src/app/shared/js/modal';
import { first } from 'rxjs/operators';
import { RollsService } from '../../../rolls/services/rolls.service';
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
  users: any[] = []; id: number = 0;
  showParent: boolean = true;
  showChPass: boolean = false;
  permissions: any[] = [];
  claimExist: any[] = [];
  userId: number = 0;
  reserves: any[] = []; services: any[] = [];
  roles: any[] = [];
  userRole: any[] = [];
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
    private service: UserManagerService,
    private serviceRole: RollsService,
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
    this.serviceRole.Get().subscribe(result => {
      console.log(result);

      let output = result as any;
      if (output != undefined) {
        this.roles = (output.item1 != undefined) ? output.item1 : [];
      }
    }, err => {
      throw err;
    });
  }
  ngOnDestroy() { }
  chActive(isActive: boolean, item) {
    this.loading = true;
    this.service.Active(item.id).subscribe(result => {
      this.loading = false;
      item.isActive = !isActive;
      this.notifier.show({
        type: 'success',
        message: 'با موفقیت ثبت شد'
      });
    }, err => {
      item.isActive = isActive;
      this.loading = false;
      this.notifier.show({
        type: 'error',
        message: err.error.text
      });
      throw err;
    });
  }



  chType(userType: number, item) {
    this.loading = true;
    this.service.ChangeType(item.id).subscribe(result => {
      this.loading = false;
      if (userType==0) {
        item.userType = 1;
      }
      else {
        item.userType = 0;
      }
      this.notifier.show({
        type: 'success',
        message: 'با موفقیت ثبت شد'
      });
    }, err => {
        item.userType = userType;
      this.loading = false;
      this.notifier.show({
        type: 'error',
        message: err.error.text
      });
      throw err;
    });
  }


  onRole(id: number, index: number) {
    this.userId = id;
    this.userRole = [];
    this.loading = true;
    this.serviceRole.getUserRole(id).subscribe(result => {
      this.loading = false;
      modal('#modalRole', true);
      console.log(result);
      let output = result as any;
      this.userRole = output != undefined ? output : [];
      debugger;
    }, err => {
        throw err;
        this.loading = false;
    });

  }

  onChRole(chk: boolean, item:any, index: number) {
    if (chk == true) {
      if (this.userRole != undefined) {
        if (this.userRole.length > 0) {
          let indexItem = this.userRole.findIndex(c => c.id == item.id);
          if (indexItem == -1) {
            this.userRole.push({ id: item.id, name: item.name, description: item.description });
          }
        }
        else {
          this.userRole.push({ id: item.id, name: item.name, description: item.description });
        }
      }
      else {
        this.userRole.push({ id: item.id, name: item.name, description: item.description });
      }
    }
    else {
      if (this.userRole != undefined) {
        if (this.userRole.length > 0) {
          let indexItem = this.userRole.findIndex(c => c.id == item.id);
          if (indexItem > -1) {
            this.userRole.splice(indexItem, 1);
          }
        }
      }
    }

  }
  onSaveRole() {
    this.loading = true;

    console.log(this.userRole);
    this.service.PutRole(this.userId, this.userRole).subscribe(result => {
      this.loading = false;
      this.notifier.show({
        type: 'success',
        message: 'با موفقیت ثبت شد.'
      });

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
  onChSize(value: any): void {
    this.pageSize = Number(value);
    this.getPage(this.pageIndex, this.pageSize);
  }
  onPageChange(i: number) {
    console.log(i);
    this.getPage(i, this.pageSize);
    this.row = ((i - 1) * this.pageSize) + 1;
    this.config.currentPage = i;
  }
  getPage(pageIndex: number, pageSize: number) {
    this.loading = true;
    this.pages = [];
    let model: iUserSearch = {
      userName: this.userName, displayName: this.displayName, mobile: this.mobile,
      pageIndex: pageIndex, pageSize: pageSize
    };
    this.service.get(model).subscribe(result => {
      console.log(result);
      let output = result as any;
      if (output != undefined) {
          this.users = (output.item3 != undefined) ? output.item1 : null;
       
          this.totalPage = output.item3;
  

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

    onReserve(id: number) {
        this.loading = true;
        this.reserves = undefined;
        this.service.getReserve(id).subscribe(res => {
            let output = res as any;
            if (output != undefined) {
                console.log(output);
                this.reserves = output.pages;
                this.loading = false;
            }
        })
        // this.reserves
  }








  //#region Seo config
  SeoConfig() {
    // seo >> set title page
    this.seoService.SetTitle('مدیریت کاربران سیستمی');
    // seo >> set meta for page
    let meta = {
      charset: '', content: 'مدیریت کاربران سیستمی', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

  //#endregion

}
