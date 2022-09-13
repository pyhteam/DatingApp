import { ControlValueAccessor, NgControl } from '@angular/forms';
import { Component, Input, OnInit, Self } from '@angular/core';
import { BsDatepickerConfig } from 'ngx-bootstrap/datepicker';

@Component({
  selector: 'app-date-input',
  templateUrl: './date-input.component.html',
  styleUrls: ['./date-input.component.css'],
})
export class DateInputComponent implements ControlValueAccessor {
  @Input() label: string;
  @Input() maxDate: Date;
  @Input() color: string;

  bsConfig: Partial<BsDatepickerConfig>;

  constructor(@Self() public ngControl: NgControl) {
    this.ngControl.valueAccessor = this;
    this.bsConfig = {
      containerClass: this.color ?? 'theme-red',
      dateInputFormat: 'DD MMMM YYYY',
      
    };
  }
  writeValue(obj: any): void {}

  registerOnChange(fn: any): void {}
  registerOnTouched(fn: any): void {}
}
