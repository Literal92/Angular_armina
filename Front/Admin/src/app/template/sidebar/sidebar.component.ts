import { Component, OnInit, OnDestroy, ElementRef, ChangeDetectorRef} from '@angular/core';
import { Router, ActivatedRoute } from '@angular/router';
import { AppSidebarModule } from '@coreui/angular';
import { sidebarItems } from './interface/isidebar.admin';
import { SidebarService } from '../services/sidebar.service';
import { Subscription } from "rxjs";
import { AuthService } from 'src/app/core';
@Component({
  selector: 'admin-app-sidebar',
  templateUrl: './sidebar.component.html',
  styles: [`.overflow-y-auto{overflow-y:auto}
.cursor-pointer{cursor:pointer}
ul.nav li{padding-left:10px}`]
})
export class AdminSidebarComponent extends AppSidebarModule {
  subscription: Subscription | null = null;
  displayName = "";
  isLoggedIn = false;
  public navItems = sidebarItems; areas: any[] = [];
  isOpen: boolean = false; isOpenMsg: boolean = false; isOpenArea: boolean = false;
  isOpenReport: boolean = false; isOpenNews: boolean = false;
  constructor(private elem: ElementRef, private router: Router,
    private route: ActivatedRoute, private service: SidebarService,
    private authService: AuthService, private cdRef: ChangeDetectorRef) {
    super();
  }
  ngOnInit() { }
  ngAfterViewInit() {
    //this.service.GetAreaAccess().subscribe(result => {
    //  this.areas = result as any[];
    //}, err => {
    //  throw err;
    //  });
    this.subscription = this.authService.authStatus$.subscribe(status => {
      this.isLoggedIn = status;
      if (status) {
        const authUser = this.authService.getAuthUser();
        this.displayName = authUser ? authUser.displayName : "";
        this.cdRef.detectChanges();

      }
    });
  }
  onToggle(el: any) {
    const hasClass = el.target.classList.contains("open");
    const elem = this.elem.nativeElement;
    if (hasClass) {
      elem.removeClass(event.target, "open");
    } else {
      elem.addClass(event.target, "open");
    }
  }
  ngOnDestroy() {
    // prevent memory leak when component is destroyed
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  logout() {
    this.authService.logout(true);
  }
}
