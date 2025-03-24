import { Component } from '@angular/core';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { LogoutServiceService } from 'src/app/shared/logout-service.service';

@Component({
  selector: 'app-layout',
  templateUrl: './layout.component.html',
  styles: [],
})
export class LayoutComponent {
  constructor(
    public service: LogoutServiceService,
    private router: Router,
    private toastr: ToastrService
  ) {}

  OnLogout() {
    this.service.logout();
    this.toastr.success('Successfully Logged Out!', 'User Logout Success');
    this.router.navigateByUrl('/login');
  }
}
