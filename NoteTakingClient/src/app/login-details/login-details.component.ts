import { Component, OnInit } from '@angular/core';
import { LoginDetailService } from '../shared/login-detail.service';

@Component({
  selector: 'app-login-details',
  templateUrl: './login-details.component.html',
  styles: [],
})
export class LoginDetailsComponent implements OnInit {
  constructor(public service: LoginDetailService) {}
  ngOnInit(): void {}
}
