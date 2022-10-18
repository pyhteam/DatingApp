import { ToastrService } from 'ngx-toastr';
import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_model/member';
import { User } from '../../_model/user';
import { AccountService } from '../../_services/account.service';
import { MembersService } from '../../_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css'],
})
export class MemberEditComponent implements OnInit {
  @ViewChild('editForm') editForm;
  member: Member;
  user: User;

  // su kien khi nguoi dung thoat khoi trang toi trang web khac luon
  // leave site
  @HostListener('window:beforeunload', ['$event']) unloadNotification(
    $event: any
  ) {
    if (this.editForm.dirty) {
      $event.returnValue = true;
    }
  }

  constructor(
    private accountService: AccountService,
    private meberService: MembersService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.accountService.currentUser$
      .pipe(take(1))
      .subscribe((user) => (this.user = user));
    this.loadMember();
  }

  loadMember() {

    this.meberService
      .getByUsername(this.user.userName)
      .subscribe((member) => (this.member = member));
  }

  //  edit member
  updateMemner() {
    this.meberService.updateMember(this.member).subscribe((result) => {
      this.toastr.success('Update successfully');
      this.editForm.reset(this.member);
    }),
      (error) => {
        this.toastr.error(error);
      };
  }
}
