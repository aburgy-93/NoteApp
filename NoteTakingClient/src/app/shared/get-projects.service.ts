import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class GetProjectsService {
  url: string = environment.apiBaseUrl + '/Project';

  constructor(private http: HttpClient) {}

  getAllProjects() {
    return this.http.get(this.url);
  }
}
