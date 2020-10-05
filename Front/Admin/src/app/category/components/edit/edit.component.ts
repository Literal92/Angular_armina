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
import { iCategory } from '../../interfaces/iCategory';
import { SharedService } from 'src/app/shared/services/shared.service';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

import { iCategorySearch } from '../../interfaces/iCategorySearch';
import { CategoryService } from '../../services/category.service';
import { EnableLoading, DisableLoading } from 'src/app/core/loading/loading';

@Component({
  selector: 'category-edit',
  templateUrl: './edit.component.html',
  styles: [`.btn-day{width:80px}`]
})
export class EditComponent implements OnInit, AfterViewInit {
  //#region Fields
  private readonly notifier: NotifierService;
  public editForm: FormGroup | undefined;
  errorFile: any = null;
  iconFile: any; serviceItem: any; Id: number = 0;
  serviceItems: any[] = [];
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
    this.route.params.subscribe(params => {
      if (params['id'] != undefined) {
        this.Id = params['id'];

        let modelAll: iCategorySearch = { pageIndex: 0, pageSize: 0 };

          this.service.getddown(modelAll).subscribe(result => {
          console.log(result);
          let output = result as any;
          if (output != undefined) {
            this.serviceItems = output.pages;
            let model: iCategorySearch = { id: this.Id };
            this.service.get(model).subscribe(result => {
              console.log(result);
              let output = result as any;
              if (output != undefined) {
                this.serviceItem = output.pages[0];
                this.editForm = this.formBuilder.group({
                  title: [this.serviceItem.title != undefined ? this.serviceItem.title : '', Validators.required],
                  parentId: [this.serviceItem.parentId],
                  icon: [''],
                  order: [this.serviceItem.order]
                });
              }
            }, err => {
              throw err;
            });
          }
        }, err => {
          throw err;
        });
      }
    });
    // Seo
    this.SeoConfig();
  }
  ngAfterViewInit() {
   
  }
  ngOnDestroy() { }

  onSubmit(items: iCategory) {
    debugger;
    EnableLoading();
    console.log(items);
    items.icon = this.iconFile;
    let model: iCategory = { id: this.Id, icon: items.icon, order: items.order, title: items.title, parentId: items.parentId };
    this.service.put(model).subscribe(result => {
      DisableLoading();

      let output = result as any;
      if (output != undefined) {
        this.notifier.show({
          type: 'success',
          message: 'با موفقیت ثبت شد.'
        });
      }
      else {
        this.notifier.show({
          type: 'error',
          message: 'خطا در ثبت بازی !'
        });
      }
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

  getBase64(file, flag?: boolean) {
    let type = file.type;
    let name = file.name;

 if (type == 'image/jpeg' || type == 'image/png' || type == 'image/jpg' || type == 'image/svg' || type == 'image/svg+xml') {
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

  //#region Seo config
  SeoConfig() {
    // seo >> set title page
    this.seoService.SetTitle('ویرایش دسته بندی');
    // seo >> set meta for page
    let meta = {
      charset: '', content: 'ویرایش دسته بندی', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

  //#endregion

}
