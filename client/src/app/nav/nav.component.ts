import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Route, Router } from '@angular/router';
// toastr
import { ToastrService } from 'ngx-toastr';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css'],
})
export class NavComponent implements OnInit {
  model: any = {};
  loginForm: FormGroup;
  constructor(
    public accountService: AccountService,
    private router: Router,
    private toastr: ToastrService,
    private formBuider: FormBuilder
  ) {}

  ngOnInit(): void {
    this.intializeForm();
  }

  intializeForm() {
    this.loginForm = this.formBuider.group({
      username: ['', Validators.required],
      password: ['', Validators.required],
    });
  }

  login() {
    this.accountService.login(this.loginForm.value).subscribe(
      (next) => {
        this.router.navigateByUrl('/members');
      },
      (error) => {
        this.toastr.error(error.error);
      }
    );
  }

  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('');
  }
}
