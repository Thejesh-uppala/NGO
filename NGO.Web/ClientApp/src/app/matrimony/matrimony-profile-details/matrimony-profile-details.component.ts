import { Location } from '@angular/common';
import { Component } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { MatrimonyService } from '../services/matrimony.service';

@Component({
  selector: 'app-matrimony-profile-details',
  templateUrl: './matrimony-profile-details.component.html',
  styleUrl: './matrimony-profile-details.component.css',
})
export class MatrimonyProfileDetailsComponent {
  profile: any;
  color: string = '';
  currentIndex: number = 0;
  zoomedImage: string | null = null;
  dynamicColor: string = 'red';
  userData!: { userId: number; orgId: number; organizations: any[]; roles: any[] };
  userId!: number;
  orgId!: number;
  isAdmin!: boolean;
  userIds: number[] = [];
  constructor(
    private route: ActivatedRoute,
    private commonService: CommonService,
    private matrimonyProfileService: MatrimonyService,
    private location: Location
  ) {}

  ngOnInit(): void {
    // Get query parameters (id, orgId) from URL
    const idParam = this.route.snapshot.queryParamMap.get('id');
    const orgIdParam = this.route.snapshot.queryParamMap.get('orgId');

    // Convert values to numbers, ensuring they're not null
    const id = idParam ? +idParam : 0;
    const orgId = orgIdParam ? +orgIdParam : 0;

    this.commonService.getUserData().subscribe({
      next: userData => {
        this.userData = userData;
        this.userId = userData.userId;
        this.orgId = userData.orgId;
      },
      error: () => {},
    });
    if (this.userData && this.userId) {
      this.isAdmin = this.userData.roles.some((role: { RoleName: string }) => role.RoleName === 'Admin');

      this.color = this.userData.organizations[0].BrandColor;

      this.changeColor(this.color);
    }

    // Fetch the selected matrimony profile based on id and orgId
    this.matrimonyProfileService.getMatrimonyProfileDetails(id, orgId).subscribe((data: any) => {
      this.profile = data;
    });
  }

  changeColor(newColor: string) {
    this.dynamicColor = newColor;
  }

  approveProfile(userId: number): void {
    if (!userId || isNaN(userId)) {
      return;
    }
    if (!this.userIds.includes(userId)) {
      this.userIds.push(userId);
    }

    // Call the service with orgId as a query string
    this.matrimonyProfileService.approveProfiles(this.orgId, this.userIds, this.userId).subscribe({
      next: () => {
        this.userIds.forEach(() => {
          this.commonService.showSuccess('Profile Approved Successfully');

          if (this.profile) {
            this.profile.status = 'Approved'; // Update the status directly
          }
        });
      },
      error: () => {
        this.commonService.showError('Something went wrong');
      },
    });
  }

  rejectProfile(userId: number): void {
    if (!userId || isNaN(userId)) {
      return;
    }
    if (!this.userIds.includes(userId)) {
      this.userIds.push(userId);
    }
    // Call the service
    this.matrimonyProfileService.rejectProfiles(this.orgId, this.userIds, this.userId).subscribe({
      next: () => {
        this.commonService.showSuccess('Profile Rejected Successfully');

        this.userIds.forEach(() => {
          if (this.profile) {
            this.profile.status = 'Rejected'; // Update the status directly
          }
        });
      },
      error: () => {
        this.commonService.showError('Something went wrong');
      },
    });
  }

  get currentPhoto() {
    return this.profile?.matrimonialDataPhotos[this.currentIndex];
  }
  // Method to open zoom modal
  openZoomModal(photo: any) {
    this.zoomedImage = photo.photoPath;
  }

  // Method to close zoom modal
  closeZoomModal() {
    this.zoomedImage = null;
  }
  // Move to the next photo
  nextPhoto() {
    if (this.currentIndex < this.profile?.matrimonialDataPhotos.length - 1) {
      this.currentIndex++;
    }
  }

  goBack(): void {
    this.location.back();
  }
  // Move to the previous photo
  previousPhoto() {
    if (this.currentIndex > 0) {
      this.currentIndex--;
    }
  }
}
