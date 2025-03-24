import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { RegisterDetail } from './register-detail.model';
import { NgForm } from '@angular/forms';

@Injectable({
  providedIn: 'root',
})
export class RegisterDetailService {
  url: string = environment.apiBaseUrl + '/User/register';
  formData: RegisterDetail = new RegisterDetail();
  formSubmitted: boolean = false;
  constructor(private http: HttpClient) {}

  postRegisterUser() {
    return this.http.post(this.url, this.formData);
  }

  resetForm(form: NgForm) {
    form.form.reset();
    this.formData = new RegisterDetail();
    this.formSubmitted = false;
  }
}
