import { Component,OnInit } from '@angular/core';
import { AppFooterModule, AppBreadcrumbModule } from '@coreui/angular';
import { Router, ActivatedRoute } from '@angular/router';
import * as moment from 'jalali-moment';
import { formatDate } from '@angular/common';
import * as momentJalaali from "moment-jalaali";

@Component({
  selector: 'admin-breadcrumb',
  templateUrl: './breadcrumb.component.html'
})
export class AdminBreadCrumbComponent extends AppBreadcrumbModule implements OnInit {
   date: string = null;
  clock: string = '00:00:00';
  constructor(private router: Router, private route: ActivatedRoute) {
    super();
  }
  ngOnInit() {
    let dateEn = formatDate(new Date(), 'yyyy/MM/dd', 'en');
    this.date = momentJalaali(dateEn).format("jYYYY/jMM/jDD");//moment(dateEn, 'YYYY/MM/DD').locale('fa').format('jYYYY/jMM/jDD');

    let timeoutId = setInterval(() => {
      let time = new Date();
      this.clock = ('0' + time.getHours()).substr(-2) + ':' + ('0' + time.getMinutes()).substr(-2) + ':' + ('0' + time.getSeconds()).substr(-2);
    }, 1000);
  }
}
