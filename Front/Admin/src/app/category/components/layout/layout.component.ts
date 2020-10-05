import { Component,OnInit } from '@angular/core';
import { NotifierService } from 'angular-notifier';
import { HelperError } from 'src/app/core/services/helper.error.service';
import { ActivatedRoute, Router } from '@angular/router';

@Component({
  selector: 'service-app',
  templateUrl: './layout.component.html'
})
export class LayoutComponent implements OnInit {

  ngOnInit() {
  }
}
