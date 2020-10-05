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
import { iImageDate } from '../../interfaces/iImageDate';
import { SharedService } from 'src/app/shared/services/shared.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { ImageDateService } from '../../services/imageDate.service';
import { EnableLoading, DisableLoading } from 'src/app/core/loading/loading';
import { HttpEvent, HttpEventType } from '@angular/common/http';
import { iImageDateSearch } from '../../interfaces/iImageDateSearch';

@Component({
  selector: 'image-date-add',
  templateUrl: './add.component.html',
  styles: [`.btn-day{width:80px}`]
})
export class AddComponent implements OnInit, AfterViewInit {
  //#region Fields
  private readonly notifier: NotifierService;
  public addForm: FormGroup | undefined;
  errorPhoto: any = null;
  images: any[] = [];
  existImages: any[] = [];
  progress: number = 0;
  datePickerConfig = {
    drops: 'down',
    opens: 'right',
    format: 'YYYY/MM/DD'
  };
  id: number = 0;
  oldDate :any;
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
    private service: ImageDateService,
    private sharedService: SharedService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
    this.notifier = notifierService;
  }
  //#endregion
  //#region metods
  ngOnInit() {
    this.route.params.subscribe(params => {
      if (params['id'] != undefined) {
        this.id = params['id'];
        let model: iImageDateSearch = { id: this.id, pageIndex: 0, pageSize: 0 };
        this.service.get(model).subscribe(result => {
          let output = result as any;
          if (output != undefined) {
            if (output.pages.length > 0) {
              this.existImages = output.pages[0].images != undefined ? output.pages[0].images : [];
              this.oldDate = output.pages[0].datePersian != undefined ? output.pages[0].datePersian : null;
              this.addForm = this.formBuilder.group({
                date: ['', Validators.required],
                images: ['', Validators.required],
              });
            }
            else {
              this.router.navigate(['/image-date']);
            }
          }
        }, err => {
          throw err;
        });
      }
      else {
        this.addForm = this.formBuilder.group({
          date: ['', Validators.required],
          images: ['', Validators.required],
        });
      }
    });

    // Seo
    this.SeoConfig(this.id);
  }
  ngAfterViewInit() {
  }
  ngOnDestroy() { }


  onSubmit(items: iImageDate) {
    if (this.images == null) {
      this.notifier.show({
        type: 'error',
        message: 'اپلود عکس الزامی است.'
      })
      return;
    }
    let model: iImageDate = {
      date: items.date,
      images: this.images
    };
    this.service.post(model).subscribe((event: HttpEvent<any>) => {
      switch (event.type) {
        case HttpEventType.Sent:
          console.log('Request has been made!');
          break;
        case HttpEventType.ResponseHeader:
          console.log('Response header has been received!');
          break;
        case HttpEventType.UploadProgress:
          this.progress = Math.round(event.loaded / event.total * 100);
          console.log(`Uploaded! ${this.progress}%`);
          break;
        case HttpEventType.Response:
          console.log('User successfully created!', event.body);
          this.router.navigate(['/image-date']);
        //setTimeout(() => {
        //  this.progress = 0;
        //}, 1500);
      }
    }, err => {
      throw err;
    })

  }
  onDelete(date:string, item:string, index: number) {
    if (!confirm("ایا از حذف مطمئن هستید ؟"))
      return false;

    this.service.delete(null,date,item).subscribe(result => {
      this.notifier.show({
        type: 'success',
        message: 'با موفقیت حذف شد شد.'
      });
      this.existImages.splice(index, 1);
      if (this.existImages.length == 0) {
        this.router.navigate(['/image-date']);
      }
    }, err => {
      this.notifier.show({
        type: 'error',
        message: err.error.message
      });
      throw err;
    });
  }

  //#region FileUpload
  uploadPhoto(files: any) {
    this.errorPhoto = null;
    for (let item of files) {

      let validation = this.fileValidation(item);
      if (validation.success == false) {
        this.errorPhoto = validation.error;
        return;
      }
      else {
        this.images.push(item);
      }
    }
  }
  fileValidation(file: any) {
    let type = file.type;
    let name = file.name;
    if (Number(file.size) <= 5000000) {// 5mb
      if (type == 'image/jpeg' || type == 'image/png' || type == 'image/jpg') {
        return {
          success: true,
          error: null
        }
      }
      else {
        return {
          success: false,
          error: "فرمت های مجاز:jpg,jpeg,png"
        };
      }
    }
    else {
      return {
        success: false,
        error: "حداکثر سایز:5Mb"
      }
    }

  }

  //#region Seo config
  SeoConfig(id:number) {
    // seo >> set title page
    this.seoService.SetTitle(id === undefined || id===0 ?'ثبت عکس روز':'ویرایش عکس روز');
    // seo >> set meta for page
    let meta = {
      charset: '', content: id === undefined || id === 0 ? 'ثبت عکس روز' : 'ویرایش عکس روز', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

  //#endregion

}
