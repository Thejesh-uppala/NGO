export interface UserPaidDetails {
  participantTypeId: number;
  cost: string; // "100", "200"
}

export interface EventRegistrationRequest {
  eventId: number;
  userId: number;
  access: string;
  userPaidDetails: UserPaidDetails[];
  guests: {
    firstName: string;
    lastName: string;
    yearOfBirth: number;
    email: string;
    phoneNo: string;
  }[];
  paymentDetails: {
    paymentMethod: string;
    transactionId: string | null;
    paymentDate: string | null;
    checkNumber: string | null;
    bankName: string | null;
    checkDate: string | null;
    amount: number;
  };
}
