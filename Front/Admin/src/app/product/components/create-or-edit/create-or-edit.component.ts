import { Component, OnInit, TemplateRef, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { Validators } from '@angular/forms';
import { FormBuilder } from '@angular/forms';
import { FormGroup } from '@angular/forms';
import { NotifierService } from 'angular-notifier';
import * as moment from 'jalali-moment';
import { ActivatedRoute } from '@angular/router';
import { SharedService } from 'src/app/shared/services/shared.service';
import { CategoryService} from '../../../category/services/category.service';
import { ProductService } from './../../services/product.service';
import { ngxLoadingAnimationTypes } from 'ngx-loading';
import { SeoService } from 'src/app/core/services/seo.service';
import { iCategorySearch } from 'src/app/category/interfaces/iCategorySearch';
import { EnableLoading, DisableLoading } from 'src/app/core/loading/loading';
import { MetaDefinition } from '@angular/platform-browser';
import { iproductSearch } from '../../interfaces/iproductSearch';
import { Message } from '@angular/compiler/src/i18n/i18n_ast';

@Component({
  selector: 'app-create-or-edit',
  templateUrl: './create-or-edit.component.html',
  styleUrls: ['./create-or-edit.component.scss']
})
export class CreateOrEditComponent implements OnInit, AfterViewInit {
  public dataForm: FormGroup | undefined;
  public id: number=0;
  public model: any;
  public categories: any[] = [];
  public categoryId: number = 0;
  private readonly notifier: NotifierService;
  public pageTitle = "افزودن محصولات";
  public oldPic:string=null;
  errorFile: any = null;
  iconFile: any;
  pics: any[] = [];
  errorGallery: any = null;
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

  constructor(private formBuilder: FormBuilder, private sharedService: SharedService,
    private categoryService: CategoryService,
    private route: ActivatedRoute, private router: Router,
    private notifierService: NotifierService,
    private seoService: SeoService,
    private service: ProductService) {
    this.notifier = notifierService;

    this.dataForm = this.formBuilder.group({

      categoryId: [0],
      title: [''],
      code:[''],
      pic:[''],
      picturs:[''],
      price: [0],
      count: [0],
      description:['']
    });
    let modelCat:iCategorySearch={contionRoot: false};
    this.categoryService.getddown(modelCat).subscribe(result =>{      
      let output= result as any;
      if (output != undefined){
        this.categories= output.pages;
        this.dataForm.controls['categoryId'].setValue(this.categories != undefined? 
                                                      this.categories[0].id:0);

      }     

    });
  }
  ngOnInit() {    

    this.route.params.subscribe(params => {
      this.id = +(params['id'] || '0');

      if (this.id > 0) {
        this.pageTitle = "ویرایش محصولات";
        EnableLoading();
        let prdSearch:iproductSearch={id: this.id};
        this.service.get(prdSearch).subscribe(data => {
          DisableLoading();
          console.log(data);
          var output = data as any;
          if(output != undefined && output.count>0)
          {
            let result = output.pages[0];
            this.model = result;

            this.dataForm.controls['title'].setValue(result.price != undefined ?result.title:'');
            this.dataForm.controls['code'].setValue(result != undefined ? result.code : '');
            this.dataForm.controls['price'].setValue(result.price != undefined ? result.price : 0);
            this.dataForm.controls['count'].setValue(result.count != undefined? result.count : 0);
            this.dataForm.controls['description'].setValue(result.description != undefined? result.description:'');
            this.dataForm.controls['categoryId'].setValue(result.categoryId != undefined? result.categoryId:0);
            this.oldPic = result.oldPic != undefined ? result.oldPic :'';
          }
        }, err => {
          DisableLoading();
          throw err;
        });
      }
    });

    this.SeoConfig(this.id==0?'ثبت محصول':'ویرایش محصول');

  }

  ngAfterViewInit() {}
 
  onSubmit(data: any) {
    data.pic = this.iconFile;
    data.gallery = this.pics;
     if (this.id ==0) {
      this.loading = true;
      this.service.post(data).subscribe(result => {
        this.loading = false;
        // this.dataForm = null;
        if (result as any) {
          this.router.navigate(['/product']);
        }
      }, err => {

        this.loading = false;
        this.notifier.show({
          type: 'error',
          message: err.error.message != undefined ? err.error.message : err.error
        });
        throw err;
      });
    } 
    else {
      this.loading = true;
        data.id= this.id;
      this.service.put(this.id, data).subscribe(result => {
        this.loading = false;

        if (result as any) {
          this.router.navigate(['/product']);
        }
      }, err => {

        this.loading = false;

        this.notifier.show({
          type: 'error',
          message: err.error.message != undefined ? err.error.message : err.error
        });
        throw err;

      });
    }
  }
  deletePic(id: number, index, number) {
    EnableLoading();
    this.service.deletePic(id).subscribe(result => {
      DisableLoading();
      this.notifier.show({
        type: 'success',
        message: 'با موفقیت حذف شد.'
      });
    this.model.pictures.splice(index, 1);
    }, err => {
        DisableLoading();

        this.notifier.show({
          type: 'error',
          message: err.error.message
        });
      throw err;
    });
  }
  uploadIcon(files: any) {
    this.errorFile=null;
    this.getBase64(files[0]);
  }
  uploadFiles(files: any) {
    for (let item of files) {
      this.errorFile = null;
      this.getBase64(item,true);
    }
  }
  
   getBase64(file,flag?: boolean) {
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
        if (flag) {
          this.pics.push(reader);
        }
        else {
         this.iconFile = reader;
        }
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
   SeoConfig(title:string) {
    // seo >> set title page
    this.seoService.SetTitle(title);
    // seo >> set meta for page
    let meta = {
      charset: '', content: title, httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion
}
