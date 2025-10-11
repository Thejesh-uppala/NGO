export class RegistrationModel {
  userId: number = 0;
  firstName: string = '';
  familyName: string | null = null;
  emailAddress: string = '';
  memberShipId: number = 0;
  yearOfBirth!: number;
  lastName: string = '';
  postalCode: string = '';
  city: string = '';
  state: string = '';
  country: string = '';
  homeTown: string = '';
  preferredBy: string | null = null;
  reason: string = '';
  isApproved!: string;
  phoneNumber: string | null = null;
  addInWhatsAppOrTelegram: string | null = null;
  paymentMethod: string = ''; // Zelle, PayPal, Check
  transactionId: string | null = null; // For Zelle/PayPal
  paymentDate: string | null = null; // ISO string format for Date (For Zelle/PayPal)
  checkNumber: string | null = null; // For Check
  bankName: string | null = null; // For Check
  checkDate: string | null = null; // ISO string format for Date (For Check)
  amount: number = 0; // Payment amount

  request_type: string = '';
  childrensDetails: ChildrensDetailsModel[] = [];
  spouseDetails: SpouseDetails = new SpouseDetails();
}

export class ChildrensDetailsModel {
  childFirstName: string = '';
  childLastName: string = '';
  childEmailAddress: string = '';
  childPhoneNumber: string = '';
  childYearOfBirth: string = ''; // ISO string format for Date
  childCity: string = '';
  childState: string = '';
  childCountry: string = '';
  childUniqueId: string = '';
}

export class SpouseDetails {
  spouseFirstName: string = '';
  spouseLastName: string | null = null;
  spouseEmail: string = '';
  spousePhoneNumber: string = '';
  spouseCity: string | null = null;
  spouseState: string | null = null;
  spouseCountry: string | null = null;
  spouseHometown: string = '';
  spouseYearOfBirth: string | null = null; // ISO string format for Date
}

// models/PaymentDetails.ts
export interface PaymentDetails {
  paymentMethod: string;
  transactionId?: string;
  paymentDate?: Date;
  checkNumber?: string;
  bankName?: string;
  checkDate?: Date;
  amount: number;
}
