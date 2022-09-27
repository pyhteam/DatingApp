import { Injectable } from "@angular/core";
import { ActivatedRouteSnapshot, Resolve, RouterStateSnapshot } from "@angular/router";
import { Observable } from "rxjs";
import { Member } from "../_model/member";
import { MembersService } from "../_services/members.service";

@Injectable({
  providedIn: 'root'
})
export class MemberDetailResolver implements Resolve<Member>{

  constructor(
    private memberService: MembersService
  ) { }

  resolve(route: ActivatedRouteSnapshot, state: RouterStateSnapshot): Member | Observable<Member> | Promise<Member> {
    return this.memberService.getByUsername(route.paramMap.get('username'));
  }
   
}