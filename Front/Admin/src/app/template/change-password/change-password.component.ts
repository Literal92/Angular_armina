import { HttpErrorResponse } from "@angular/common/http";
import { Component, OnInit, ElementRef } from "@angular/core";
import { NgForm, FormGroup, Validators, FormBuilder } from "@angular/forms";
import { Router, ActivatedRoute } from "@angular/router";
import { NotifierService } from 'angular-notifier';
import { ChangePasswordAdminService } from '../services/change-password.service';
import { AuthService } from 'src/app/core';

@Component({
  selector: "app-change-password",
  templateUrl: "./change-password.component.html",
  styleUrls: ["./change-password.component.scss"]
})
export class ChangePasswordAdminComponent implements OnInit {

  error = "";
 
  public ChPassForm: FormGroup | undefined;
  private readonly notifier: NotifierService;

  constructor(private router: Router,
    private authService: AuthService,
    private formBuilder: FormBuilder,
    private changePasswordService: ChangePasswordAdminService
    , notifierService: NotifierService) {
    this.notifier = notifierService;
  }

  ngOnInit() {
    this.ChPassForm = this.formBuilder.group({
      password: ['', Validators.compose([Validators.required, Validators.minLength(8)])],
      confirmpass: ['']
    }
      , { validator: this.checkPasswords('password', 'confirmpass') }
    );
  }


  onSubmit(items:any) {

    let serialNumber = this.authService.getAuthUser().serialNumber;
    let password = items.password;
    console.log(items);
    this.changePasswordService.changePassword(serialNumber, password).subscribe(result => {
      if (result == true) {
        this.notifier.show({
          type: 'success',
          message: 'با موفقیت اعمال شد.'
        });
      }
      else {
        this.notifier.show({
          type: 'error',
          message: result.toString()
        });
      }
    }, err => {
      throw err;
    });

  }

  checkPasswords(pass: string, confirm: string) {
    return (group: FormGroup) => {
      let passInput = group.controls['password'],
        confirmInput = group.controls['confirmpass'];
      if (passInput.value !== confirmInput.value) {
        return confirmInput.setErrors({ 'notmatch': 'با رمز عبور یکسان نیست' })
      }
      else {
        return confirmInput.setErrors(null);
      }
    }
  }

  
}
