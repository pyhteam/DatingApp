import { Component, Input, OnInit } from '@angular/core';
import { MembersService } from '../../_services/members.service';
import { ToastrService } from 'ngx-toastr';
import { PresenceService } from '../../_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css'],
})
export class MemberCardComponent implements OnInit {
  @Input() member: any;
  constructor(private memberService: MembersService,
    public presnse: PresenceService,
    private toastr: ToastrService) {}

  ngOnInit(): void {}

  addLike(member: any) {
    this.memberService.addLike(member.username).subscribe(() => {
      this.toastr.success('You have liked ' + member.knownAs);
    });
  }
}
