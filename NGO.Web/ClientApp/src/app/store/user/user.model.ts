export interface Organization {
  id: number;
  orgName: string;
  orgWelMsg: string | null;
  brandImageUrl: string | null;
  brandLogo: string | null;
  createdDate: string | null;
  organizationChapters: any; // Adjust to specific type if known
  userDetailModels: any; // Adjust to specific type if known
}
export interface UserRole {
  roleId: number;
  roleName: string;
}

export interface UserState {
  userId: number;
  name: string;
  token: string;
  tokenExpiryDate: string;
  userRoles: UserRole[];
  organizations: Organization[];
  error: string | null;
}
