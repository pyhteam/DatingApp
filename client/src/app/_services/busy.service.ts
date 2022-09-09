import { Injectable } from '@angular/core';
import { NgxSpinnerService } from 'ngx-spinner';

@Injectable({
  providedIn: 'root'
})
export class BusyService {
   busyRequestCount = 0;
  constructor(private spinerService: NgxSpinnerService) { }

  // show loading khi dang request
  busy(){
    this.busyRequestCount++;
    this.spinerService.show(undefined, {
      type: 'line-scale-party',
      bdColor: 'rgba(255,255,255,0)',
      color: '#333333'
    });
  }

  // hide loading khi request xong
  idle(){
    this.busyRequestCount--;
    if(this.busyRequestCount <= 0){
      this.busyRequestCount = 0;
      this.spinerService.hide();
    }
  }
}
