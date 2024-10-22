import { environment } from "src/environments/environment";

const apiUrl = environment.apiUrl;
export class LoginURLConstants {
    static LOGIN = apiUrl + '/api/v1/user';
}
export class RegistrationURLConstants {
    static REGISTER = apiUrl + '/api/v1/register/RegisterUser';
    static SIGNUP = apiUrl + '/api/v1/register/SignUp';
    static UPLOAD_PHOTO = apiUrl + '/api/v1/register/UploadPhoto';
  }
  export class UserProfileURLConstants {
    static USER_PROFILE = apiUrl + '/api/v1/user/UpdateUser';
    static UPLOAD_PHOTO = apiUrl + '/api/v1/user/UploadPhoto';
  }
  export class ContactUSURLConatants {
    static SEND_MAIL = apiUrl + '/api/v1/emailData/ContactUs';
  }
  export class DashBoardURLConatants {
    static LOAD_DATA = apiUrl + '/api/v1/user/GetCurrentUserDetails';
    static GET_ALL_USERS = apiUrl + '/api/v1/user/GetAllUser';
    static CONTACT_ME = apiUrl + '/api/v1/user/UserContactEmailRem';
    static GET_CURRENT_USER_PROFILE = apiUrl + '/api/v1/user/GetCurrentUserProfile';
    static PAYMENT_REMINDER = apiUrl + '/api/v1/user/PaymentReminder';
    static GET_PAYMENT_DETAILS = apiUrl + '/api/v1/user/GetPaymentDetails';
    static GET_ALL_CHAPTERS = apiUrl + '/api/v1/user/GetAllChapters';
    static GET_ALL_ORGANIZATIONS = apiUrl + '/api/v1/user/GetAllOrganizations';
    static SAVE_MEMBER = apiUrl + '/api/v1/user/SaveMember';
    static MASS_PAYMENT_REMINDER = apiUrl + '/api/v1/user/MassPaymentRem';
  }
  export class ResetPasswordConstants {
    static RESET_PASSWORD = apiUrl + '/api/v1/user/ChangePassWord';
    static SEND_OTP = apiUrl + '/api/v1/user/SendOTP';
    static FORGOT_PASSWORD = apiUrl + '/api/v1/user/ForgotPassword';
}