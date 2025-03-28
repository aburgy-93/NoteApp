import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment.development';

@Injectable({
  providedIn: 'root',
})
export class GetNotesService {
  url: string = environment.apiBaseUrl + '/Notes';

  constructor(private http: HttpClient) {}

  getAllNotes() {
    return this.http.get(this.url);
  }
}
