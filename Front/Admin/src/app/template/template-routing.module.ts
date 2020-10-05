import { NgModule } from "@angular/core";
import { RouterModule, Routes } from "@angular/router";
import { AdminLayoutComponent } from './layout/layout.component';
import { AuthGuard, AuthGuardPermission } from 'src/app/core';
import { ChangePasswordAdminComponent } from './change-password/change-password.component';
//import { RoleModule } from 'src/app/role/role.module';

const routes: Routes = [
  {
    path: "", component: AdminLayoutComponent,
    children: [
      {
        path: "",
        loadChildren: () => import('../dashboard/dashboard.module').then(m => m.DashboardModule),
      },
      {
        path: "usermanager",
        loadChildren: () => import('../user-manager/user-manager.module').then(m => m.UserManagerModule),
      },
      {
        path: "rolls",
        loadChildren: () => import('../rolls/rolls.module').then(m => m.RollsModule)
      },
      {
        path: "categories",
        loadChildren: () => import('../category/category.module').then(m => m.CategoryModule),
      },
      {
        path: "csv",
        loadChildren: () => import('../tracking-code/tracking-code.module').then(m => m.TrackinCodeModule),
      },
      {
        path: "product",
        loadChildren: () => import('../product/product.module').then(m => m.ProductModule),
      },
      {
        path: "field",
        loadChildren: () => import('../field/field.module').then(m => m.FieldModule),
      },
      {
        path: "tag",
        loadChildren: () => import('../tags/tag.module').then(m => m.TagModule),
      },
      {
        path: "product-option",
        loadChildren: () => import('../product-option/product-option.module').then(m => m.ProductOptionModule),
      }
      ,
      {
        path: "orders",
        loadChildren: () => import('../orders/order.module').then(m => m.OrderModule),
      },
      {
        path: "image-date",
        loadChildren: () => import('../image-date/imageDate.module').then(m => m.ImageDateModule),
      }
    ],
    data: {
      permission: {
        permittedRoles: ["Admin", "AccountantAdmin", "OrderAdmin", "ProductAdmin","ReportAdmin"]
      } as AuthGuardPermission
    }
    , canActivate: [AuthGuard]
  },
  //, { path: "**", redirectTo: "" }

];

@NgModule({
  imports: [RouterModule.forChild(routes)],
  exports: [RouterModule]
})
export class TemplateRoutingModule { }
