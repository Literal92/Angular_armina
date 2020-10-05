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
import { iCSV } from '../../interfaces/iCategory';
import { SharedService } from 'src/app/shared/services/shared.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { CategoryService } from '../../services/category.service';
import { EnableLoading, DisableLoading } from 'src/app/core/loading/loading';
import { DatePickerDirective, DatePickerComponent } from 'ng2-jalali-date-picker';

@Component({
  selector: 'category-add',
  templateUrl: './add.component.html',
  styles: [`.btn-day{width:80px}`]
})
export class AddComponent implements OnInit, AfterViewInit {
  //#region Fields
  datePickerConfig = {
    drops: 'down',
    opens: 'right',
    format: 'YYYY/MM/DD'
  };

  private readonly notifier: NotifierService;
  public addForm: FormGroup | undefined;
  errorFile: any = null;
  csvFile: any;

  ////#endregion

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
    private sharedService: SharedService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
    this.notifier = notifierService;
  }
  //#endregion
  //#region metods
  ngOnInit() {
    this.addForm = this.formBuilder.group({
      fileName: [''],
      description: [''],
      sendDate: [''],
     
    });

    // Seo
    this.SeoConfig();
  }
  ngAfterViewInit() {
  
  }
  ngOnDestroy() { }

  onSubmit(items: any) {
    EnableLoading();
    console.log(items);
    items.fileName = this.csvFile;
    items.sendDate = moment(items.sendDate, 'jYYYY/jMM/jDD').locale('en').format('YYYY/MM/DD');
    this.service.postCSV(items).subscribe(result => {
      DisableLoading();
      this.router.navigate(['/csv/display']);
    }, err => {
      DisableLoading();

      this.notifier.show({
        type: 'error',
        message: err.error
      });
      throw err;
    });
   
  }
  uploadIcon(files: any) {
    this.errorFile=null;

    this.getBase64(files[0]);
  }

  
   getBase64(file,flag?: boolean) {
    let type = file.type;
     let name = file.name;
     
     if (type == 'text/csv' || type == 'application/vnd.ms-excel' || type == 'image/jpg' || type == 'image/svg' || type == 'image/svg+xml') {
      var reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = (e: any) => {
        reader = e.target.result;
        //  var help=e.target.result;
        // this.base64Imgs.push(btoa(help));
          console.log(reader);
          this.csvFile = reader;
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

  //#region Seo config
  SeoConfig() {
    // seo >> set title page
    this.seoService.SetTitle('ثبت دسته بندی');
    // seo >> set meta for page
    let meta = {
      charset: '', content: 'ثبت دسته بندی', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

  //#endregion

}
