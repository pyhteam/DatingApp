import { Component, Input, OnInit } from '@angular/core';
import { FileUploader } from 'ng2-file-upload';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_model/member';
import { User } from 'src/app/_model/user';
import { MembersService } from 'src/app/_services/members.service';
import { environment } from '../../../environments/environment';
import { AccountService } from '../../_services/account.service';

@Component({
  selector: 'app-photo-edit',
  templateUrl: './photo-edit.component.html',
  styleUrls: ['./photo-edit.component.css'],
})
export class PhotoEditComponent implements OnInit {
  @Input() member: Member;

  uploader: FileUploader;
  hasBaseDropZoneOver: boolean;
  hasAnotherDropZoneOver: boolean;
  response: string;

  baseUrl = environment.API_URL;
  user: User;

  constructor(
    private accountService: AccountService,
    private memberService: MembersService,
    private toastr: ToastrService
  ) {
    this.accountService.currentUser$
      .pipe(take(1))
      .subscribe((user) => (this.user = user));
  }

  ngOnInit(): void {
    this.initializeUploader();
  }

  fileOverBase(e: any): void {
    this.hasBaseDropZoneOver = e;
  }
  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      authToken: 'Bearer ' + this.user.token,
      isHTML5: true,
      allowedFileType: ['image'],
      removeAfterUpload: true,
      autoUpload: false,
      maxFileSize: 10 * 1024 * 1024,
    });
    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };
    // update success
    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
        const photo = JSON.parse(response);
        this.member.photos.push(photo);
      }
    };
  }

  setMainPhoto = (photo) => {
    this.memberService.setMainPhoto(photo.id).subscribe(
      () => {
        this.user.photoUrl = photo.url;
        this.member.photoUrl = photo.url;
        this.accountService.setCurrentUser(this.user);
        this.member.photos.forEach((p) => {
          if (p.isMain) p.isMain = false;
          if (p.id === photo.id) p.isMain = true;
        });
      },
      (error) => {
        this.toastr.error(error.error);
      }
    );
  };
  //  remove photo
  deletePhoto = (photoId: number) => {
    this.memberService.deletePhoto(photoId).subscribe(
      () => {
        this.member.photos = this.member.photos.filter((x) => x.id !== photoId);
        this.toastr.success('Photo has been deleted');
      },
      (error) => {
        this.toastr.error(error.error);
      }
    );
  };
}
