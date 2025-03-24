import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root',
})
export class LogoutServiceService {
  constructor(private http: HttpClient) {}

  logout() {
    localStorage.removeItem('loginToken');
  }
}
