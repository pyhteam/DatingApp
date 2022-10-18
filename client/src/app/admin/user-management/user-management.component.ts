import { Component, OnInit } from '@angular/core';
import { BsModalService, ModalOptions, BsModalRef } from 'ngx-bootstrap/modal';
import { ToastrService } from 'ngx-toastr';
import { User } from 'src/app/_model/user';
import { AdminService } from 'src/app/_services/admin.service';
import { RolesModalComponent } from '../../modals/roles-modal/roles-modal.component';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css'],
})
export class UserManagementComponent implements OnInit {
  users: Partial<User[]>;
  bsModalRef?: BsModalRef;
  constructor(
    private adminService: AdminService,
    private modalService: BsModalService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    this.loadUserRole();
  }

  loadUserRole() {
    this.adminService.getUsersWithRoles().subscribe((users) => {
      this.users = users;
    });
  }
  openRolesModal(user: User) {
    const config: ModalOptions  = {
      class: 'modal-dialog-centered',
      initialState: {
        user: this.users,
        roles: this.getRolesArray(user),
      },
    };

    this.bsModalRef = this.modalService.show(RolesModalComponent,config);
    this.bsModalRef.content.updateSelectedRoles.subscribe((values) => {
      const rolesToUpdate = {
        roles: [
          ...values.filter((el) => el.checked === true).map((el) => el.name),
        ],
      };
      if (rolesToUpdate) {
        this.adminService
          .updateUserRoles(user.username, rolesToUpdate.roles)
          .subscribe(() => {
            user.roles = [...rolesToUpdate.roles];
            this.toastr.success('Roles updated successfully');
          });
      }
    });
  }

  private getRolesArray(user: User) {
    const roles = [];
    const userRoles = user.roles;
    const availableRoles: any[] = [
      { name: 'Admin', value: 'Admin' },
      { name: 'Moderator', value: 'Moderator' },
      { name: 'Member', value: 'Member' },
    ];

    availableRoles.forEach((role) => {
      let isMatch = false;
      for (const userRole of userRoles) {
        if (role.name === userRole) {
          isMatch = true;
          role.checked = true;
          roles.push(role);
          break;
        }
      }
      if (!isMatch) {
        role.checked = false;
        roles.push(role);
      }
    });
    return roles;
  }
}
