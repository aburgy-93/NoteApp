import { NgFor } from '@angular/common';
import { Component } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { LoginDetailService } from 'src/app/shared/login-detail.service';

@Component({
  selector: 'app-login-details-form',
  templateUrl: './login-details-form.component.html',
  styles: [],
})
export class LoginDetailsFormComponent {
  constructor(
    public service: LoginDetailService,
    private toastr: ToastrService,
    private router: Router
  ) {}

  OnLogin(form: NgForm) {
    this.service.formSubmitted = true;
    if (form.valid) {
      this.service.postLoginUser().subscribe({
        next: (res: any) => {
          console.log(res);
          const token = res.token;
          console.log(token);
          this.service.resetForm(form);
          this.toastr.success('Successfully Logged In!', 'User Login Success');
          localStorage.setItem('loginToken', token);
          this.router.navigateByUrl('/dashboard');
        },
        error: (err) => {
          console.log(err);
          this.toastr.error('Invalid Credentials!', 'User Login Unsuccessful');
        },
      });
    }
  }
}
