import { environment } from 'src/environments/environment';

const apiUrl = environment.apiUrl;
export class LoginURLConstants {
  static LOGIN(storedOrgId: string) {
    return `${apiUrl}/api/v1/user?orgId=${storedOrgId}`;
  }
}

export class BrandURLConstants {
  static GetBrand(orgId: string) {
    return `${apiUrl}/api/v1/user/${orgId}/brand-details`;
  }
}
export class RegistrationURLConstants {
  static REGISTER(orgId: string) {
    return `${apiUrl}/api/v1/register/RegisterUser?orgId=${orgId}`;
  }
  static SIGNUP(OrgId: string) {
    return `${apiUrl}/api/v1/register/SignUp?orgId=${OrgId}`;
  }

  static GET_CHAPTERS(OrgId: string) {
    return `${apiUrl}/api/v1/user/GetOrgChapters?orgId=${OrgId}`;
  }

  static GET_MEMBERSHIP_PLANS(orgId: string) {
    return `${apiUrl}/api/v1/user/GetOrgMembershipTypes?orgId=${orgId}`;
  }
  static UPLOAD_PHOTO = apiUrl + '/api/v1/register/UploadPhoto';
}
export class UserProfileURLConstants {
  static USER_PROFILE(orgId: string) {
    return `${apiUrl}/api/v1/user/UpdateUser?orgId=${orgId}`;
  }

  static UPLOAD_PHOTO = apiUrl + '/api/v1/user/UploadPhoto';
}
export class ContactUSURLConstants {
  static SEND_MAIL = apiUrl + '/api/v1/reminder/ContactUs';
}

export class MatrimonialURLConstants {
  static GET_MATRIMONY_PROFILE_DETAILS(id: number, OrgId: number) {
    return `${apiUrl}/api/v1/matrimonial/GetMatrimonialDataById?id=${id}&orgId=${OrgId}`;
  }
  static APPROVE_PROFILES(orgId: number): string {
    return `${apiUrl}/api/v1/matrimonial/ApproveProfiles?orgId=${orgId}`;
  }

  static REJECT_PROFILES(orgId: number): string {
    return `${apiUrl}/api/v1/matrimonial/RejectProfiles?orgId=${orgId}`;
  }

  static GET_ALL_MATRIMONIAL_USERS(OrgId: number) {
    return `${apiUrl}/api/v1/matrimonial/GetAllMatrimonialData?orgId=${OrgId}`;
  }
  static GET_ALL_APPROVED_MATRIMONIAL_USERS(OrgId: number, page: number, pageSize: number) {
    return `${apiUrl}/api/v1/matrimonial/GetAllApprovedMatrimonialData?orgId=${OrgId}&page=${page}&pageSize=${pageSize}`;
  }

  static GET_FILTERED_MATRIMONIAL_USERS(orgId: number, page: number, pageSize: number) {
    return `${apiUrl}/api/v1/matrimonial/GetFilteredMatrimonialData?orgId=${orgId}&page=${page}&pageSize=${pageSize}`;
  }
  static CREATE_MATRIMONIAL_PROFILE = apiUrl + '/api/v1/matrimonial/CreateMatrimonialData';
}
export class EventURLConstants {
  static GET_EVENT_BY_ID: string = `${apiUrl}/api/v1/event/GetEventById`;

  static GET_ALL_UPCOMING_EVENTS(OrgId: number) {
    return `${apiUrl}/api/v1/event/GetUpcomingEvents?orgId=${OrgId}`;
  }
  static GET_ALL_EVENTS(OrgId: number) {
    return `${apiUrl}/api/v1/event/GetAllEvents?orgId=${OrgId}`;
  }

  static DELETE_EVENT(eventId: number): string {
    return `${apiUrl}/api/v1/event/${eventId}`;
  }
  static GET_PAGINATED_EVENTS(orgId: number, page: number, pageSize: number) {
    return `${apiUrl}/api/v1/event/GetAllPaginatedEvents?orgId=${orgId}&page=${page}&pageSize=${pageSize}`;
  }
  static GET_USERREGISTERED_PAGINATED_EVENTS(eventId: number, page: number, pageSize: number) {
    return `${apiUrl}/api/v1/event/GetRegisteredUsersByEventId?eventId=${eventId}&page=${page}&pageSize=${pageSize}`;
  }
  static REGISTER_EVENT(orgId: number): string {
    return `${apiUrl}/api/v1/event/RegisterEvents?orgId=${orgId}`;
  }

  static GET_ALL_USER_REGISTERED_EVENTS(OrgId: number, userId: number) {
    return `${apiUrl}/api/v1/event/UserRegisteredEvents?orgId=${OrgId}&userId=${userId}`;
  }

  static CREATE_EVENT = apiUrl + '/api/v1/event/CreateEvent';

  static UPDATE_EVENT = apiUrl + '/api/v1/event/UpdateEvent';
}

export class DashBoardURLConstants {
  static LOAD_DATA = apiUrl + '/api/v1/user/GetCurrentUserDetails';
  static DELETE_USER = apiUrl + '/api/v1/user/DeleteUser';

  static ACCEPT_MEMBERSHIPS(orgId: number): string {
    return `${apiUrl}/api/v1/user/AcceptMemberships?orgId=${orgId}`;
  }

  static REJECT_MEMBERSHIPS(orgId: number): string {
    return `${apiUrl}/api/v1/user/RejectMemberships?orgId=${orgId}`;
  }

  static REMIND_USER(updatedBy: number, orgId: number) {
    return `${apiUrl}/api/v1/reminder/RemindUser?updatedBy=${updatedBy}&orgId=${orgId}`;
  }

  static GET_ALL_USERS(OrgId: number) {
    return `${apiUrl}/api/v1/user/GetAllUser?orgId=${OrgId}`;
  }

  static GET_SUMMARY_COUNTS(OrgId: number) {
    return `${apiUrl}/api/v1/user/GetSummaryCounts?orgId=${OrgId}`;
  }
  static GET_ALL_MEMBERS(OrgId: number) {
    return `${apiUrl}/api/v1/user/GetAllMembers?orgId=${OrgId}`;
  }

  static GetBrand(orgId: string) {
    return `${apiUrl}/api/v1/user/${orgId}/brand-details`;
  }

  static GET_CURRENT_USER_PROFILE(storedOrgId: number) {
    return `${apiUrl}/api/v1/user/GetCurrentUserProfile?orgId=${storedOrgId}`;
  }

  static GET_CURRENT_USER_CUSTOM_DATA(storedOrgId: number) {
    return `${apiUrl}/api/v1/user/GetCurrentUserCustomData?orgId=${storedOrgId}`;
  }

  static PAYMENT_REMINDER = apiUrl + '/api/v1/user/PaymentReminder';
  static GET_PAYMENT_DETAILS = apiUrl + '/api/v1/user/GetPaymentDetails';
  static GET_ALL_CHAPTERS = apiUrl + '/api/v1/user/GetAllChapters';
  static GET_ALL_ORGANIZATIONS = apiUrl + '/api/v1/user/GetAllOrganizations';
}
export class ResetPasswordConstants {
  static RESET_PASSWORD(orgId: string) {
    return `${apiUrl}/api/v1/user/ChangePassWord?orgId=${orgId}`;
  }

  static SEND_OTP(OrgId: string) {
    return `${apiUrl}/api/v1/user/SendOTP?orgId=${OrgId}`;
  }
  static FORGOT_PASSWORD(OrgId: string) {
    return `${apiUrl}/api/v1/user/ForgotPassword?orgId=${OrgId}`;
  }
  static VALIDATE_OTP(orgId: string) {
    return `${apiUrl}/api/v1/user/ValidateOTP?orgId=${orgId}`;
  }
}
