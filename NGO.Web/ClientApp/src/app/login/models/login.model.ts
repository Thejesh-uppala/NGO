export class AuthModel {
  userId!: number;
  name!: string;
  email!: string; // Added this field
  tokenExpiryDate!: string;
  token!: string;
  error: string = '';
  userRoles: { roleId: number; roleName: string }[] = [];
  organizations: {
    id: number;
    orgName: string;
    orgWelMsg: string | null;
    brandImageUrl: string | null;
    brandLogo: string | null;
    createdDate: string | null;
    organizationChapters: any | null; // Adjust type if necessary
    userDetailModels: any | null; // Adjust type if necessary
  }[] = []; // Added this field
}
