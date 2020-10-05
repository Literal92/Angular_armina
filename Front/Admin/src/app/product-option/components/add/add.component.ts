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

@Component({
  selector: 'product-option-add',
  templateUrl: './add.component.html',
  styles: [`.btn-day{width:80px}`]
})
export class AddComponent implements OnInit{
  //#region Fields
  public addForm: FormGroup | undefined;
  public colorForm: FormGroup | undefined;
  private readonly notifier: NotifierService;
  public productId:number=0;
  public fields:any[]=[];
  public product:any;
  public colors:any[]=[];
  public types: any[] = [
    { id: 0, display: 'متن' },
    { id: 1, display: 'یک انتخابی' },
    { id: 2, display: 'چند انتخابی' }
  ];
  ////#endregion

  //#region Ctor
  constructor( private router: Router,
    protected route: ActivatedRoute,
    private formBuilder: FormBuilder,
    private fieldService: FieldService,

    // loading
    private sanitizer: DomSanitizer,
    private notifierService: NotifierService,
    private seoService: SeoService,
    private shareService: SharedService,
    private productService: ProductService,
    private service: ProductOptionService,
    @Inject(APP_CONFIG) private appConfig: IAppConfig) {
    this.notifier = notifierService;
    let model:iFieldSearch={};
    this.fieldService.get(model).subscribe(result =>{      
      let output= result as any;
      if (output != undefined){
        this.fields= output.pages;
        this.addForm.controls['fieldId'].setValue(this.fields.length > 0 ?this.fields[0].id:0);
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
        console.log(this.fields);
        this.productId = +params['id'];
        this.getProduct(this.productId);
        
        this.addForm = this.formBuilder.group({
          fieldId: [0, Validators.required],
          productId: [this.productId, Validators.required],
          title:[''],
          count: [0],
          price: [''],
          order: [1]
        });
        this.colorForm = this.formBuilder.group({
          title: [''],
          count: [0],
          price: [''],
          resellerPrice:[0],
          order: [1],
          code:['']
        });
      }
    });   
  }
  ngAfterViewInit() {}
  onAddOption(){
    modal('#modal', true);
  }
  onColor(items: any) {
    this.colors.push({
      title: items.title, order: items.order,
      price: items.price, resellerPrice: items.resellerPrice, code: items.code,
      count: items.count
    });
    // order by order
    this.colors = this.colors.sort((a, b) => (a.order > b.order) ? 1 : -1);
    modal('#modal', false);
    this.colorForm.reset();
  }
  onRemove(index: number) {
    this.colors.splice(index, 1);
  }
  getProduct(id: number){
    let model:iproductSearch={id: id};
    this.productService.get(model).subscribe(result =>{
      let output = result as any;
      if(output.count >0)
      this.product= output.pages[0];
    }, err=>{
      throw err;
    });
  }

  onSubmit(items: iProductOption) {
    EnableLoading();
    console.log(items);
    items.optionColors = this.colors;

    this.service.post(items).subscribe(result => {
      DisableLoading();
      this.router.navigate(['/product-option/display/',this.productId]);

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
    this.seoService.SetTitle('افزودن فیلد به محصول');
    // seo >> set meta for page
    let meta = {
      charset: '', content: 'افزودن فیلد به محصول', httpEquiv: '', id: '', itemprop: '',
      name: 'description', property: '', scheme: '', url: ''
    } as MetaDefinition;
    this.seoService.SetMeta([meta]);
  }
  //#endregion

   //#endregion
}
