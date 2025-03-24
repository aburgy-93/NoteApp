import { Component, OnInit } from '@angular/core';
import { GetProjectsService } from 'src/app/shared/get-projects.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styles: [],
})
export class DashboardComponent implements OnInit {
  constructor(public service: GetProjectsService) {}
  ngOnInit(): void {
    this.service.getAllProjects().subscribe({
      next: (res: any) => {
        console.log(res);
      },
      error: (err) => {
        console.log(err);
      },
    });
  }
}
