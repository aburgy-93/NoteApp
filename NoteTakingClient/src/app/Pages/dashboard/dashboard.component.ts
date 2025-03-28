import { Component } from '@angular/core';
import { GetNotesService } from 'src/app/shared/get-notes.service';
import { GetProjectsService } from 'src/app/shared/get-projects.service';

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styles: [],
})
export class DashboardComponent {
  projects: any[] = [];
  notes: any[] = [];
  currentAction: string = '';

  constructor(
    public projectService: GetProjectsService,
    public noteService: GetNotesService
  ) {}

  handleAction(action: string) {
    this.currentAction = action;

    switch (action) {
      case 'GetProjects':
        this.projectService.getAllProjects().subscribe({
          next: (res: any) => {
            this.projects = res;
            console.log(this.projects);
          },
          error: (err) => {
            console.log(err);
          },
        });
        break;
      case 'GetNotes':
        this.noteService.getAllNotes().subscribe({
          next: (res: any) => {
            this.notes = res;
            console.log(this.notes);
          },
          error: (err) => {
            console.log(err);
          },
        });
        break;
      case 'GetStats':
        break;
      default:
        console.warn('Unknown action selected!');
        break;
    }
  }
}
