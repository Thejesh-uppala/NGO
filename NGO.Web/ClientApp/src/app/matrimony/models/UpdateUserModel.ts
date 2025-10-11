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

export class ChildrensDetailsModel {
  firstName: string = '';
  lastName: string = '';
  childUniqueId: string = '';
  emialId: string = '';
  phoneNo: string = '';
  yearOfBirth!: number;
  childCity: string = '';
  childState: string = '';
  childCountry: string = '';
}
export class SpouseDetails {
  spouseHometown: string = '';
  spouseCountry: string = '';
  spouseCity: string = '';
  spouseState: string = '';
  spouseYearOfBirth: string = '';
  spousePhoneNumber: string = '';
  spouseLastName: string = '';
  spouseEmail: string = '';
  spouseFirstName: string = '';
}
