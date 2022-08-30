import { ToastrService } from 'ngx-toastr';
import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  // @Input() userFromChilComponet: any;
  @Output() cancelRegister = new EventEmitter();
  user: any = {};
  constructor(
    private accountService: AccountService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {}

  register() {
    this.accountService.register(this.user).subscribe(
      (response) => {
        this.toastr.success('Registration Successful');
        this.cancel();
      },
      (error) => {
        this.toastr.error(error.error);
      }
    );
  }
  cancel() {
    this.cancelRegister.emit(true);
  }
}
