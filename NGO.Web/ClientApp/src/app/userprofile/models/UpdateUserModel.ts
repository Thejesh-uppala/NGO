import { ChildrensDetailsModel, SpouseDetails } from 'src/app/registration/models/UserRegistrationModel';

export class UserDetailModel {
  childrensDetails: ChildrensDetailsModel[] = [];
  spouseDetails!: SpouseDetails;
  id: string = '';
  userId: string = '';
  orgId: string = '';
  orgName: string = '';
  memberId: string = '';
  email: string = '';
  firstName: string = '';
  lastName: string = '';
  familyName: string = '';
  isApproved!: string;
  phoneNumber: string = '';
  yearOfBirth!: number;
  city: string = '';
  state: string = '';
  country: string = '';
  homeTown: string = '';
  preferredBy: string = '';
  reason: string = '';
  postalCode: string = '';
  emailAddress: string = '';
  addInWhatsAppOrTelegram: string = '';
  paymentDate: string = '';
  photo: string = '';
  membershipStatus: string = '';
  membershipValidityDate: Date | undefined;
}
