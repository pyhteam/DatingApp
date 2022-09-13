import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Member } from '../_model/member';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.API_URL + 'users/';

  constructor(private http: HttpClient) {}

  getMembers() {
    return this.http.get<Member[]>(this.baseUrl + 'get-all');
  }
  getByUsername(username: string) {
    return this.http.get<Member>(this.baseUrl + 'get-by-username/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'update', member);
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'set-main-photo/' + photoId, {});
  }
  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'delete-photo/' + photoId);
  }
}
