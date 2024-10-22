export class RegistrationModel {
    childrensDetails: ChildrensDetailsModel[]=[];
    spouseDetails!: SpouseDetails
    id:string="";
    userId: string = "";
    password:string="";
    firstName: string = "";
    lastName: string = "";
    familyName: string = "";
    phoneNumber: string = "";
    dob: string = "";
    city: string = "";
    state: string = "";
    country: string = "";
    homeTown: string = "";
    socialMedia: string = "";
    preferredBy: string = "";
    reason: string = "";
    address: string = "";
    refference: string = "";
    postalCode: string = "";
    emailAddress: string = "";
    memberShipType:string  ="";
    whatsAppNumber: string = "";
    paymentDate: string = "";
    paymentId:string  ="";
    payer_id: string = "";
    photoData!:File;
    photo!:FormData
}
export class ChildrensDetailsModel {
    firstName: string = "";
    lastName: string = "";
    emialId: string = "";
    phoneNo: string = "";
    dob: string = "";
    resident: string = "";
    residentCity: string = "";
    residentState: string = "";
    residentCountry: string = "";
}
export class SpouseDetails {
    spouseHometown: string = "";
    spouseCountry: string = "";
    spouseCity: string = "";
    spouseState: string = "";
    spouseDob: string = "";
    spousePhoneNumber: string = "";
    spouseLastName: string = "";
    spouseEmail: string = "";
    spouseFirstName: string = "";
}