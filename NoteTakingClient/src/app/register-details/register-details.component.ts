import { Component } from '@angular/core';
import { RegisterDetailService } from '../shared/register-detail.service';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';

@Component({
  selector: 'app-register-details',
  templateUrl: './register-details.component.html',
  styles: [],
})
export class RegisterDetailsComponent {
  constructor(
    public service: RegisterDetailService,
    private toastr: ToastrService
  ) {}

  OnSubmit(form: NgForm) {
    this.service.formSubmitted = true;
    if (form.valid) {
      this.service.postRegisterUser().subscribe({
        next: (res) => {
          console.log(res);
          this.service.resetForm(form);
          this.toastr.success(
            'User Created Successfully',
            'New User Registration'
          );
        },
        error: (err) => {
          console.log(err);
        },
      });
    }
  }
}
