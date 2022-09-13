import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import {
  AbstractControl,
  FormBuilder,
  FormGroup,
  ValidatorFn,
  Validators,
} from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { AccountService } from '../_services/account.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css'],
})
export class RegisterComponent implements OnInit {
  // @Input() userFromChilComponet: any;
  @Output() cancelRegister = new EventEmitter();
  registerForm: FormGroup;
  maxDate: Date;
  validatorErrors: string[] = [];
  constructor(
    private accountService: AccountService,
    private toastr: ToastrService,
    private formBuider: FormBuilder
  ) {}

  ngOnInit(): void {
    this.intializeForm();
    this.maxDate = new Date();
    this.maxDate.setFullYear(this.maxDate.getFullYear() - 18);
  }

  intializeForm() {
    this.registerForm = this.formBuider.group({
      username: ['', Validators.required],
      gender: ['male'],
      knowAs: ['', Validators.required],
      dateOfBirth: ['', Validators.required],
      city: ['', Validators.required],
      country: ['', Validators.required],
      password: [
        '',
        [Validators.required, Validators.minLength(6), Validators.maxLength(8)],
      ],
      confirmPassword: [
        '',
        [Validators.required, this.matchVaules('password')],
      ],
    });
    this.registerForm.controls.password.valueChanges.subscribe(() => {
      this.registerForm.controls.confirmPassword.updateValueAndValidity();
    });
  }
  matchVaules(matchTo: string): ValidatorFn {
    return (control: AbstractControl) => {
      return control?.value === control?.parent?.controls[matchTo].value
        ? null
        : { isMatching: true };
    };
  }

  register() {
    this.accountService.register(this.registerForm.value).subscribe(
      (response) => {
        this.cancel();
      },
      (error) => {
        this.validatorErrors = error;
      }
    );
  }
  cancel() {
    this.cancelRegister.emit(true);
  }
}
