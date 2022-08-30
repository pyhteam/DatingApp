import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { NgForm } from '@angular/forms';
import { Observable } from 'rxjs';
import { User } from '../_model/user';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  model: any = {};

  constructor(public accountService: AccountService) {}

  ngOnInit(): void {

  }

  login() {
    this.accountService.login(this.model).subscribe(
      (next) => {},
      (error) => {
        console.log(error);
      }
    );
  }

  logout() {
    this.accountService.logout();
  }
}