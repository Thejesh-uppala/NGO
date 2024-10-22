export class UserDetailModel {
    childrensDetails: ChildrensDetailsModel[]=[];
    spouseDetails!: SpouseDetails
    id:string="";
    userId: string = "";
    orgId: string = "";
    orgName: string = "";
    memberId:string="";
    email: string = "";
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
    whatsAppNumber: string = "";
    paymentDate:string="";
    photoData!:File;
    photo!:File
}
export class PaymentModel {
    paymentId:string="";
    paymentDate:string="";
}
export class FileData {
    size: number =0;
    type: string = "";
    name: string = "";
    webkitRelativePath:string=""
}
export class UserModel {
    email: string = "";
    password: string = "";
    name: string = "";
    contactNumber: string = "";
    status: number = 0;
    userDetails: UserDetailModel[]=[];
}
export class ChildrensDetailsModel {
    firstName: string = "";
    lastName: string = "";
    childUniqueId:string=""
    emialId: string = "";
    phoneNo: string = "";
    dob: string = "";
    resident: string = "";
    childCity: string = "";
    childState: string = "";
    childCountry: string = "";
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
export interface CountryInterface {
    name?: string;
    id?:string;
    states:any[];
   
  }
  export interface StateInterface {
    name?: string;
    cities:any[];
    id?:string
  }
  export interface CityInterface {
    name?: string;
    id?:string
  }