import { Component, OnInit, AfterViewInit, ViewChild, Inject, TemplateRef } from '@angular/core';
import { FormGroup, Validators, FormBuilder } from '@angular/forms';
import { NotifierService } from 'angular-notifier';
import { Router, ActivatedRoute } from '@angular/router';
import { DomSanitizer, MetaDefinition } from '@angular/platform-browser';
import { SeoService } from 'src/app/core/services/seo.service';
import { SharedService } from 'src/app/shared/services/shared.service';
import { IAppConfig, APP_CONFIG } from 'src/app/core';
import { DisableLoading, EnableLoading } from 'src/app/core/loading/loading';
import { ProductOptionService } from '../../services/product-option.service';
import { FieldService } from 'src/app/field/services/field.service';
import { iFieldSearch } from 'src/app/field/interfaces/iFieldSearch';
import { iproductSearch } from 'src/app/product/interfaces/iproductSearch';
import { ProductService } from 'src/app/product/services/product.service';
import { iProductOption } from '../../interfaces/iProductOption';
import { modal } from 'src/app/shared/js/modal';
import { isNgTemplate } from '@angular/compiler';
import { iProductOptionSearch } from '../../interfaces/iProductOptionSearch';

@Component({
  selector: 'product-field-edit',
  templateUrl: './edit.component.html',
  styles: [`.btn-day{width:80px}`]
})
export class EditComponent implements OnInit {
  //#region Fields

  public editForm: FormGroup | undefined;
  public colorForm: FormGroup | undefined;
  private readonly notifier: NotifierService;
  public id: number = 0;
  public fields: any[] = [];
  public product: any;
  public colors: any[] = [];
  public productOption: iProductOption;
  public colorEditId: number = 0;
  public editItem: any = null;
  public types: any[] = [
    { id: 0, display: 'متن' },
    { id: 1, display: 'یک انتخابی' },
    { id: 2, display: 'چند انتخابی' }
  ];
  ////#endregion

  //#region Ctor
  constructor(private router: Router,
    protected route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private fieldService: FieldService,
    // loading
    private sanitizer: DomSanitizer,
    private notifierService: NotifierService,
    private seoService: SeoService,
    private shareService: SharedService,
    private service: ProductOptionService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
    this.notifier = notifierService;
    let model: iFieldSearch = {};
    this.fieldService.get(model).subscribe(result => {
      let output = result as any;
      if (output != undefined) {
        this.fields = output.pages;
        //this.editForm.controls['fieldId'].setValue(this.fields.length > 0 ?this.fields[0].id:0);
      }
    });
  }
  //#endregion
  //#region metods
  ngOnInit() {
    // Seo
    this.SeoConfig();

    this.route.params.subscribe(params => {
      if (params['id'] != undefined) {
        this.id = +params['id'];
        let model: iProductOptionSearch = { id: this.id };
        EnableLoading();

        this.service.get(model).subscribe(result => {
          DisableLoading();
          let output = result as any;
          if (output.count > 0)
          this.product = output.pages[0].product;
          this.colors = output.pages[0].optionColors;
          this.productOption = output.pages[0];
          if (this.productOption != undefined) {
            this.editForm = this.formBuilder.group({
              fieldId: [this.productOption ? this.productOption.fieldId : 0, Validators.required],
              productId: [this.product ? this.product.id : 0, Validators.required],
              title: [this.productOption  ? this.productOption.title : 0, Validators.required],
              count: [this.productOption ? this.productOption.count : 0],
              price: [this.productOption ? this.productOption.price ? this.productOption.price: '' : ''],
              order: [this.productOption ? this.productOption.order : 1]
            });
            this.colorForm = this.formBuilder.group({
              title: ['', Validators.required],
              price: [0],
              resellerPrice: [0],
              count: [0],
              order: [1],
              code: ['']
            });
          }
        }, err => {
            DisableLoading();

          throw err;
        });
      }
    });
  }
  ngAfterViewInit() { }
  onAddOption() {
    modal('#modal', true);
  }
  onEditItem(item: any) {
    debugger;
    console.log(item);
    this.colorEditId = item.id;
    this.editItem = item;
    this.colorForm.controls['title'].setValue(item.title ? item.title: '');
    this.colorForm.controls['price'].setValue(item.price ? item.price : '');
    this.colorForm.controls['resellerPrice'].setValue(item.resellerPrice ? item.resellerPrice : '');
    this.colorForm.controls['count'].setValue(item.count ? item.count : '');
    this.colorForm.controls['order'].setValue(item.order ? item.order : '');
    this.colorForm.controls['code'].setValue(item.code ? item.code:'');
    modal('#modal', true);
  }
  onColor(items: any) {
    debugger;

    if (this.colorEditId ==0) {
        this.colors.push({
          title: items.title, fieldId: this.id, order: items.order, productOptionId:this.id,
          price: items.price, resellerPrice: items.resellerPrice, code: items.code, count: items.count
        });
        // order by order
        this.colors = this.colors.sort((a, b) => (a.order > b.order) ? 1 : -1);
        modal('#modal', false);
        this.colorForm.reset();
      return;
    }
    items.id = this.colorEditId;
    items.productOptionId = this.editItem.productOptionId;
    EnableLoading();
    this.service.UpdateColor(items).subscribe(result => {
      DisableLoading();
      let index = this.colors.findIndex(c => c.id == this.colorEditId);
      this.colorEditId = 0;
      if (index > -1) {
        this.colors[index].title = items.title;
        this.colors[index].fieldId= items.fieldId;
        this.colors[index].order = items.order;
        this.colors[index].productOptionId = this.id;
        this.colors[index].price = items.price;
        this.colors[index].resellerPrice = items.resellerPrice;
        this.colors[index].code = items.code;
        this.colors[index].count= items.count
      }
      this.notifier.show({
        type: 'success',
        message:'باموفقیت ثبت شد'
      });
      modal('#modal', false);
      this.colorForm.reset();
    }, err => {
        DisableLoading();

        this.notifier.show({
          type: 'error',
          message: err.error.message
        });


        throw err;
    });
  }
  onRemove(index: number) {
    this.colors.splice(index, 1);
  }
  onSubmit(items: iProductOption) {
    EnableLoading();
    console.log(items);
    items.id= this.id;
    items.optionColors = this.colors;

    this.service.put(items).subscribe(result => {
      DisableLoading();
      this.router.navigate(['/product-option/display/', this.product.id]);

      // let output = result as any;
      // if (output != undefined) {
      //   this.notifier.show({
      //     type: 'success',
      //     message: 'با موفقیت ثبت شد.'
      //   });
      // }
    }, err => {
      DisableLoading();

      this.notifier.show({
        type: 'error',
        message: err.error.text
      });
      throw err;
    });

  }
  //#region Seo config
  SeoConfig() {
    // seo >> set title page
    this.seoService.SetTitle('ویرایش فیلد محصول');
    // seo >> set meta for page
    let meta = {
      charset: '', content: 'ویرایش فیلد محصول', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

  //#endregion
}
