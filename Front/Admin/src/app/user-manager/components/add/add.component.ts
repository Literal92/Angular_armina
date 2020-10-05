import { Component, OnInit, AfterViewInit, ViewChild, Inject, TemplateRef } from '@angular/core';
import { FormGroup, Validators, FormBuilder, MaxLengthValidator } from '@angular/forms';
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

@Component({
  selector: 'user-add',
  templateUrl: './add.component.html',
  styles: [`.btn-day{width:80px}`]
})
export class AddComponent implements OnInit {
  //#region Fields
  private readonly notifier: NotifierService;
  public addForm: FormGroup | undefined;
  errorFile: any = null;
  iconFile: any;
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

    this.addForm = this.formBuilder.group({
      userName: ['', Validators.required],
      phoneNumber: [''],
      firstName: ['', Validators.required],
     // lastName: ['', Validators.required]
      //,
      photoFileName: [''],
     // userName: ['', Validators.required],
      password: ['', Validators.required]
    });
  }
  ngAfterViewInit() { }

  onSubmit(items: iUser) {
    console.log(items);
    this.loading = true;
    let model = items;
    model.mobile = items.userName;
    model.photoFileName = this.iconFile;
    this.service.post(model).subscribe(result => {
      this.loading = false;
      let output = result as any;
      if (output != undefined) {
       
        this.router.navigate(['/usermanager']);
      }
    }, err => {
      this.loading = false;
      this.notifier.show({
        type: 'error',
        message: err.error ? err.error.message ? err.error.message.description : err.error.message:'خطا در ثبت'
      });
      throw err;
    });

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
