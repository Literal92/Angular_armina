import { Component, Inject,OnInit,AfterViewInit, ChangeDetectorRef } from '@angular/core';
import { AppHeaderModule } from '@coreui/angular';
import { Subscription } from "rxjs";
import { AuthService } from 'src/app/core';
@Component({
  selector: 'admin-header',
  templateUrl: './header.component.html',
  styles: [`.cursor-pointer{cursor:pointer}`]
})
export class AdminHeaderComponent extends AppHeaderModule implements OnInit, AfterViewInit{
  title = "Angular.Jwt.Core";

  isLoggedIn = false;
  subscription: Subscription | null = null;
  displayName = "";
 
  constructor(private authService: AuthService, private cdRef: ChangeDetectorRef) {
    super();
  }
  ngOnInit() { }

  ngAfterViewInit() {
    this.subscription = this.authService.authStatus$.subscribe(status => {
      this.isLoggedIn = status;
      if (status) {
        const authUser = this.authService.getAuthUser();
        this.displayName = authUser ? authUser.displayName : "";
        this.cdRef.detectChanges();
      }
    });
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
