import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import {
  HubConnection,
  HubConnectionBuilder,
  Subject,
} from '@microsoft/signalr';
import { environment } from '../../environments/environment';
import { PaginationHelper } from '../_helper/paginationHelper';
import { Message } from '../_model/message';
import { User } from '../_model/user';
import { BehaviorSubject } from 'rxjs';
import { take } from 'rxjs/operators';

@Injectable({
  providedIn: 'root',
})
export class MessageService {
  baseUrl = environment.API_URL;
  hubUrl = environment.HUB_URL;
  private hubConnection: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient) {}

  createHubConnection(user: User, otherUsername: string) {
    this.hubConnection = new HubConnectionBuilder()
      .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
        accessTokenFactory: () => user.token,
      })
      .withAutomaticReconnect()
      .build();

    this.hubConnection
      .start()
      .catch((error) => console.log('Error establishing connection: ', error));

    this.hubConnection.on('ReceiveMessageThread', (messages) => {
      this.messageThreadSource.next(messages);
    });

    this.hubConnection.on('NewMessage', (message) => {
      this.messageThread$.pipe(take(1)).subscribe((messages) => {
        this.messageThreadSource.next([...messages, message]);
      });
    });

    this.hubConnection.on('UpdatedGroup', (group: any) => {
      if (group.connections.some((x) => x.username === otherUsername)) {
        this.messageThread$.pipe(take(1)).subscribe((messages) => {
          messages.forEach((message) => {
            if (!message.dateRead) {
              message.dateRead = new Date(Date.now());
            }
          });
          this.messageThreadSource.next([...messages]);
        });
      }
    });
  }
  stopHubConnection() {
    if (this.hubConnection) {
      this.hubConnection.stop().catch((error) => console.log(error));
    }
  }

  getMessages(pageNumber, pageSize, container) {
    let params = PaginationHelper.getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    return PaginationHelper.getPaginatedResult<Message[]>(
      this.baseUrl + 'messages',
      params,
      this.http
    );
  }
  getMessageThread(username: string) {
    return this.http.get<Message[]>(
      this.baseUrl + 'messages/thread/' + username
    );
  }
  async sendMessage(username: string, content: string) {
    return this.hubConnection
      .invoke('SendMessage', {
        recipientUsername: username,
        content,
      })
      .catch((error) => console.log(error));
  }
  deleteMessage(id: number) {
    return this.http.delete(this.baseUrl + 'messages/' + id);
  }
}
