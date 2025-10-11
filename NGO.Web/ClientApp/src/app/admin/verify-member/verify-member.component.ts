import { Component } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { ConfirmationService, MessageService } from 'primeng/api';
import { Subscription } from 'rxjs';
import { CommonService } from 'src/app/shared/components/services/common.service';
import { PaymentMethodType } from 'src/app/shared/enums/enums';
import { SharedModule } from 'src/app/shared/shared.module';
import { UserProfileService } from 'src/app/userprofile/services/userprofile.service';
import { AdminService } from '../services/admin.service';

interface ChildDetails {
  childCountry?: string;
  childUniqueId?: string;
  childCity?: string;
  childState?: string;
  childPhoneNumber?: string;
  childYearOfBirth?: number;
  childLastName?: string;
  childEmailAddress?: string;
  childFirstName: string;
}

interface SpouseDetails {
  spouseEmail: string;
  spouseCity: string;
  spouseState: string;
  spouseCountry: string;
  spouseHometown: string;
  spouseFirstName: string;
  spouseLastName: string;
  spousePhoneNumber: string;
  spouseYearOfBirth: number;
}

interface PaymentInfo {
  paymentMethod: string | null;
  transactionId: string | null;
  paymentDate: string | null;
  checkNumber: string | null;
  bankName: string | null;
  checkDate: string | null;
  amount: number;
}

interface UserData extends PaymentInfo {
  firstName: string;
  orgId: number;
  memberId: string;
  familyName: string;
  isApproved: string;
  emailAddress: string;
  yearOfBirth: number;
  lastName: string;
  postalCode: string;
  city: string;
  state: string;
  country: string;
  userId: number;
  preferredBy: string;
  reason: string;
  homeTown: string;
  phoneNumber: string;
  addInWhatsAppOrTelegram: string;
  photo: string;
  membershipValidityDate?: string;
  membershipStatus: string;
  spouseYearOfBirth?: number | null;
  approvedBy?: string;
  childrensDetails?: ChildDetails[];
  spouseDetails?: SpouseDetails;
}

@Component({
  selector: 'app-verify-member',
  standalone: true,
  imports: [SharedModule],
  templateUrl: './verify-member.component.html',
  styleUrl: './verify-member.component.css',
})
export class VerifyMemberComponent {
  subscriptions: Subscription[] = [];
  userId!: string;
  userData: UserData = {} as UserData; // ✅ Ensures userData is never undefined
  storedOrgId!: number;
  hideActions: boolean = false;
  ActionUserId!: number;
  childrensDetails: ChildDetails[] = [];
  userDataStore!: { userId: number; orgId: number; organizations: any[]; roles: any[] };

  constructor(
    private messageService: MessageService,
    private adminService: AdminService,
    private commonService: CommonService,
    private confirmationService: ConfirmationService,
    private route: ActivatedRoute,
    private router: Router,
    private userprofileService: UserProfileService
  ) {}

  ngOnInit(): void {
    this.commonService.getUserData().subscribe({
      next: userData => {
        this.userDataStore = userData;
        this.ActionUserId = userData.userId;
        this.storedOrgId = userData.orgId;
      },
      error: () => {},
    });

    this.route.queryParams.subscribe(params => {
      this.userId = params['userId'];
      if (this.userId) {
        const isAdmin = this.userDataStore.roles.some(role => role.RoleName === 'Admin');

        if (!isAdmin) {
          this.router.navigate(['/home']);
        }
        this.getUserDetails();
      }
    });

    this.route.url.subscribe(() => {
      const currentRoute = this.router.url;
      this.hideActions = currentRoute.includes('/user-profile-details');
    });
  }

  isDigitalPayment(method: string | null | undefined): boolean {
    return (
      method?.toLocaleLowerCase() === PaymentMethodType.ZELLE ||
      method?.toLocaleLowerCase() === PaymentMethodType.PAYPAL
    );
  }

  isCheckPayment(method: string | null | undefined): boolean {
    return method?.toLocaleLowerCase() === PaymentMethodType.CHECK;
  }

  getUserDetails() {
    if (this.storedOrgId) {
      this.subscriptions.push(
        this.userprofileService.getCurrentUserProfile(this.storedOrgId, this.userId).subscribe({
          next: res => {
            this.userData = res.userData;
            this.childrensDetails = res.userData?.childrensDetails;
          },
          error: () => {},
        })
      );
    }
  }

  goBack() {
    if (this.hideActions) {
      this.router.navigate(['/admin']);
    } else {
      this.router.navigate(['/memberships']);
    }
  }

  acceptMembership() {
    const userId = parseInt(this.userId, 10);
    if (isNaN(userId)) {
      return;
    }

    this.confirmationService.confirm({
      message: 'Are you sure you want to Accept this Membership?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.adminService.acceptMembership(this.storedOrgId, [userId], this.ActionUserId).subscribe({
          next: () => {
            this.commonService.showSuccess('Membership Approved Successfully');

            this.userData.isApproved = 'Approved'; // Update the status directly
          },
          error: () => {
            this.commonService.showError();
          },
        });
      },
    });
  }

  rejectMembership() {
    const userId = parseInt(this.userId, 10);
    if (isNaN(userId)) {
      return;
    }

    this.confirmationService.confirm({
      message: 'Are you sure you want to Reject this Membership?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.adminService.rejectMembership(this.storedOrgId, [userId], this.ActionUserId).subscribe({
          next: () => {
            this.commonService.showSuccess('Request Rejected Successfully');
          },
          error: () => {
            this.commonService.showError();
          },
        });
      },
    });
  }

  ngOnDestroy() {
    this.subscriptions.forEach(sub => sub.unsubscribe());
  }
}
