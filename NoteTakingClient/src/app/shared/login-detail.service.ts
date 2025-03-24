import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { environment } from 'src/environments/environment.development';
import { LoginDetail } from './login-detail.model';
import { NgForm } from '@angular/forms';

@Injectable({
  providedIn: 'root',
})
export class LoginDetailService {
  url: string = environment.apiBaseUrl + '/User/login';
  formData: LoginDetail = new LoginDetail();
  formSubmitted: boolean = false;
  constructor(private http: HttpClient) {}

  postLoginUser() {
    return this.http.post(this.url, this.formData);
  }

  resetForm(form: NgForm) {
    form.form.reset();
    this.formData = new LoginDetail();
    this.formSubmitted = false;
  }
}
