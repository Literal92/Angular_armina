import { Component, OnInit, AfterViewInit, ViewChild, Inject, TemplateRef } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NotifierService } from 'angular-notifier';
import { Router, ActivatedRoute } from '@angular/router';
import { DomSanitizer, MetaDefinition } from '@angular/platform-browser';
import { SeoService } from 'src/app/core/services/seo.service';
import { SharedService } from 'src/app/shared/services/shared.service';
import { IAppConfig, APP_CONFIG } from 'src/app/core';
import * as moment from 'jalali-moment';
import { ngxLoadingAnimationTypes } from 'ngx-loading';
import { iUser } from '../../interfaces/iUser';
import { UserManagerService } from '../../services/user-manager.service';
import { iUserSearch } from '../../interfaces/iUserSearch';

@Component({
  selector: 'user-edit',
  templateUrl: './edit.component.html',
  styles: [`.btn-day{width:80px}`]
})
export class EditComponent implements OnInit {
  //#region Fields
  private readonly notifier: NotifierService;
  public editForm: FormGroup | undefined;
  errorFile: any = null;
  iconFile: any;
  Id: number = 0;
  editItem: any = null;
  //#region loading

  public ngxLoadingAnimationTypes = ngxLoadingAnimationTypes;
  public loading = false;
  public coloursEnabled = false;
  public loadingTemplate: TemplateRef<any>;
  public searchForm: FormGroup | undefined;

  public configLoading = {
    animationType: ngxLoadingAnimationTypes.circle,
    primaryColour: '#dd0031', secondaryColour: '#006ddd',
    tertiaryColour: '#ffffff', backdropBorderRadius: '3px',
    backdropBackgroundColour: 'rgba(0,0,0,0.1)',
  };

  //#endregion


  ////#endregion

  //#region Ctor
  constructor(private router: Router,
    protected route: ActivatedRoute,
    private formBuilder: FormBuilder,
    // loading
    private sanitizer: DomSanitizer,
    private notifierService: NotifierService,
    private seoService: SeoService,
    private shareService: SharedService,
    private service: UserManagerService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
    this.notifier = notifierService;
  }
  //#endregion
  //#region metods
  ngOnInit() {
    // Seo
    this.SeoConfig();
    this.route.params.subscribe(params => {
      this.Id = +(params['id'] || '0');
      let model: iUserSearch = { id: this.Id, pageIndex: 0, pageSize: 0 };
      this.service.get(model).subscribe(result => {
          let output = result as any;
          console.log(output);
        this.editItem = output.item1[0];
        this.editForm = this.formBuilder.group({
          //mobile: [this.editItem ? this.editItem.mobile : '', Validators.required],
          phoneNumber: [this.editItem ? this.editItem.phoneNumber : ''],
          firstName: [this.editItem ? this.editItem.firstName : ''],
          lastName: [this.editItem ? this.editItem.lastName : ''],
          natioalCode: [this.editItem ? this.editItem.natioalCode : '', Validators.compose([this.NationalCodeValidator])],
          mobile: [this.editItem ? this.editItem.mobile : '', Validators.compose([this.MoileValidator])],
          //photoFileName: [''],
          userName: [this.editItem ? this.editItem.userName : '', Validators.required]
        });
      }, err => {
        throw err;
      });
    });
  }
  ngAfterViewInit() { }

  onSubmit(items: iUser) {
    console.log(items);
    this.loading = true;
    let model = items;
    model.id = this.Id;
    //model.photoFileName = this.iconFile;
    this.service.put(model).subscribe(result => {
      this.loading = false;
      let output = result as any;
      if (output != undefined) {
        this.router.navigate(['/usermanager']);
      }
    }, err => {
      this.loading = false;
      this.notifier.show({
        type: 'error',
        message: err.error
      });
      throw err;
    });

  }

  MoileValidator(control: any) {
    let val = control.value;//.trim();
    if (val == null) 
      return;// { 'mobile': 'الزامی.' };

    let len = val.length;
    if (isNaN(val)) {
      return { 'mobile': 'مقادیر نادرست است' };
    }
    else if (len > 11) {
      return { 'mobile': '11 رقم باید باشد.' };

    }
    else {
      return null;
    }
  }
  NationalCodeValidator(control: any) {
    let val = control.value;//.trim();
    if (val == null)
      return;

    let len = val.length;
    if (len > 10) {
      return { 'natioalCode': '10 رقم باید باشد.' };
    }
    else {
      return null;
    }
  }


  uploadIcon(files: any) {
    this.getBase64(files[0]);
  }


  getBase64(file, flag?: boolean) {
    let type = file.type;
    let name = file.name;

    if (type == 'image/jpeg' || type == 'image/png' || type == 'image/jpg') {
      var reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = (e: any) => {
        reader = e.target.result;
        //  var help=e.target.result;
        // this.base64Imgs.push(btoa(help));
        console.log(reader);
        this.iconFile = reader;
      };
      //reader.onload = function () {
      //  console.log(reader.result);
      //};
      reader.onerror = function (error) {
        console.log('Error: ', error);
      };
    }
    else {
      this.errorFile = "فرمت های مجاز:jpg,png";
    }
  }



  // #region Seo config
  SeoConfig() {
    // seo >> set title page
    this.seoService.SetTitle('افزودن کاربر سیستمی');
    //  seo >> set meta for page
    let meta = {
      charset: '', content: 'افزودن کاربر سیستمی', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

  //#endregion
}
