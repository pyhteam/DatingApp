import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Member } from '../_model/member';
import { PaginatedResult } from '../_model/pagination';
import { map } from 'rxjs/operators';
import { UserParams } from '../_model/userParams';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.API_URL + 'users/';
  members: Member[] = [];

  constructor(private http: HttpClient) {}

  getMembers(userParams: UserParams) {
    let params = this.getPaginationHeaders(
      userParams.pageNumber,
      userParams.pageSize
    );
    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);
    

    return this.getPaginatedResult<Member[]>(this.baseUrl + 'get-all', params);
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

  private getPaginatedResult<T>(url, params) {
    const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();
    return this.http.get<T>(url, { observe: 'response', params }).pipe(
      map((response) => {
        paginatedResult.result = response.body;
        if (response.headers.get('Pagination') !== null) {
          paginatedResult.pagination = JSON.parse(
            response.headers.get('Pagination')
          );
        }
        return paginatedResult;
      })
    );
  }

  private getPaginationHeaders(pageNumber: number, pageSize: number) {
    let params = new HttpParams();
    params = params.append('pageNumber', pageNumber.toString());
    params = params.append('pageSize', pageSize.toString());
    return params;
  }
}
