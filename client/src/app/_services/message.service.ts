import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from '../../environments/environment';
import { PaginationHelper } from '../_helper/paginationHelper';
import { Message } from '../_model/message';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  baseUrl = environment.API_URL;
  constructor(private http: HttpClient) {}

  getMessages(pageNumber, pageSize, container) {
    let params = PaginationHelper.getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    return PaginationHelper.getPaginatedResult<Message[]>(
      this.baseUrl + 'messages',
      params,
      this.http
    );
  }
  getMessageThread(username:string){
    return this.http.get<Message[]>(this.baseUrl +'messages/thread/'+username);
  }
  sendMessage(username:string,content:string){
    return this.http.post<Message>(this.baseUrl+'messages',{recipientUsername:username,content});
  }
  deleteMessage(id:number){
    return this.http.delete(this.baseUrl+'messages/'+id);
  }
}
