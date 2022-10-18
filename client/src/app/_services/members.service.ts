import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Member } from '../_model/member';
import { PaginatedResult } from '../_model/pagination';
import { map, take } from 'rxjs/operators';
import { UserParams } from '../_model/userParams';
import { Observable, of } from 'rxjs';
import { AccountService } from './account.service';
import { User } from '../_model/user';
import { PaginationHelper } from '../_helper/paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.API_URL + 'users/';
  members: Member[] = [];
  memberCache = new Map();
  user: User;
  userParams: UserParams;

  constructor(
    private http: HttpClient,
    private accountService: AccountService
  ) {
    this.accountService.currentUser$.pipe(take(1)).subscribe((user) => {
      this.user = user;
      this.userParams = new UserParams(user);
    });
  }
  getUserParams() {
    return this.userParams;
  }
  setUserParams(params: UserParams) {
    this.userParams = params;
  }

  getMembers(userParams: UserParams) {
    // lay trong cache
    var response = this.memberCache.get(Object.values(userParams).join('-'));
    if (response) {
      return of(response);
    }
    let params = PaginationHelper.getPaginationHeaders(
      userParams.pageNumber,
      userParams.pageSize
    );
    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender);
    params = params.append('orderBy', userParams.orderBy);

    return PaginationHelper.getPaginatedResult<Observable<Member[]>>(
      this.baseUrl + 'get-all',
      params,
      this.http
    ).pipe(
      map((response) => {
        this.memberCache.set(Object.values(userParams).join('-'), response);
        return response;
      })
    );
  }

  getByUsername(username: string) { 
    // lay memner detail trong cache
    const member = [...this.memberCache.values()]
      .reduce((arr, elem) => arr.concat(elem.result), [])
      .find((member: Member) => member.username === username);
    if (member) {
      return of(member);
    }

    return this.http.get<Member>(this.baseUrl + 'get-by-username/' + username);
  }

  updateMember(member: Member) {
    return this.http.put(this.baseUrl + 'update', member);
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'set-main-photo/' + photoId, {});
  }

  addLike(username: string) {
    return this.http.post(environment.API_URL + 'likes/' + username, {});
  }
  getLikes(predicate: string, pageNumber, pageSize) {
    let params = PaginationHelper.getPaginationHeaders(pageNumber, pageSize);
    params = params.append('predicate', predicate);
    return PaginationHelper.getPaginatedResult<Partial<Member[]>>(
      environment.API_URL + 'likes',
      params,
      this.http
    );
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'delete-photo/' + photoId);
  }
}
