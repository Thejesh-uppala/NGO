using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Channels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NGO.Common;
using NGO.Data;
using NGO.Model;
using NGO.Repository;
using NGO.Repository.Contracts;
using static NGO.Model.FileModel;

namespace NGO.Business
{
    public class UserBusiness
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserDetailRepository _userDetailRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserRolesRepository _userRolesRepository;
        private readonly IChildrensRepository _childrensRepository;
        private readonly IOrgChapterRepository _orgChapterRepository;
        private readonly IOrgRepository _orgRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly IOrgRepository _organizationRepository;
        private readonly SMTPEmailProvider _smtpEmailProvider;
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly IMembershipUserTypesRepository _membershipTypesRepository;
        private readonly CommonBusiness _commonBusiness;
        private readonly PasswordHandler _passwordHandler;
        private readonly AppSettings _appsettings;
        private readonly IMapper _mapper;
        private readonly Channel<EmailDataModel> _emailChannel;
        private readonly IOrganizationConfigService _configService;
        public UserBusiness(Channel<EmailDataModel> emailChannel, IOrganizationConfigService configService,
        IUserRepository userRepository, CommonBusiness commonBusiness, IUserDetailRepository userDetailRepository, IRoleRepository roleRepository, IUserRolesRepository userRolesRepository, IChildrensRepository childrensRepository, IOrgChapterRepository orgChapterRepository, IOrgRepository orgRepository, IPaymentRepository paymentRepository, SMTPEmailProvider smtpEmailProvider, PasswordHandler passwordHandler, IMapper mapper, AppSettings appsettings, IMembershipUserTypesRepository membershipTypesRepository, IOrgRepository organizationRepository, IUserAddressRepository userAddressRepository)//IUserSpouseRepository userSpouseRepository
        {
            _userRepository = userRepository;
            _commonBusiness = commonBusiness;
            _userDetailRepository = userDetailRepository;
            _roleRepository = roleRepository;
            _userRolesRepository = userRolesRepository;
            _childrensRepository = childrensRepository;
            _orgChapterRepository = orgChapterRepository;
            _orgRepository = orgRepository;
            _paymentRepository = paymentRepository;
            _smtpEmailProvider = smtpEmailProvider;
            _passwordHandler = passwordHandler;
            _appsettings = appsettings;
            _membershipTypesRepository = membershipTypesRepository;
            _userAddressRepository = userAddressRepository;
            _configService = configService;
            _organizationRepository = organizationRepository;
            _emailChannel = emailChannel;
            _mapper = mapper;
        }

        public async Task<UserDetail> GetCurrentUserCustomData(int orgId, int userId)
        {

            return (await _userDetailRepository.GetByAsync(x => x.UserId == userId)).FirstOrDefault();


        }

        public async Task<bool> ValidateOTP(int orgId, string emailId, int otp)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u =>
                u.Email == emailId &&
                u.UserOrganizations.Any(o => o.OrganizationId == orgId) &&
                !u.IsDeleted);

            if (user == null)
            {
                return false;
            }

            // Handle nullable OTP and OTPUsed
            if (user.OTP.HasValue && user.OTP == otp && (!user.OTPUsed.HasValue || !user.OTPUsed.Value))
            {
                // Mark OTP as used
                user.OTPUsed = true;
                user.UpdatedOn = DateTime.UtcNow;
                await _userRepository.UpdateAsync(user);
                await _userRepository.SaveAsync();
                return true;
            }

            return false;
        }
        public async Task<FileModel> GetCurrentUserProfile(int orgId, int userId)
        {
            var fileModel = new FileModel();

            // Optimize database queries with includes
            var user = await _userRepository.GetUserByIdAndOrg(userId, orgId);
            if (user == null) return fileModel;

            try
            {
                // Optimize by combining queries and using includes
                var userDetails = await _userDetailRepository.Query()
                    .Include(x => x.AddressEntity)
                    .Include(x => x.ChildrensDetails)
                    .FirstOrDefaultAsync(x => x.UserId == user.Id);



                if (userDetails == null) return fileModel;

                // Map basic user details
                MapBasicUserDetails(fileModel, user, userDetails);

                // Map address details
                if (userDetails.AddressEntity != null)
                {
                    MapAddressDetails(fileModel, userDetails.AddressEntity);
                }

                // Map spouse details
                MapSpouseDetails(fileModel, userDetails);

                // Map children details
                MapChildrenDetails(fileModel, userDetails.ChildrensDetails);

                var latestPayment = (await _paymentRepository.GetByAsync(x => x.UserDetailId == userDetails.UserId))
                   .OrderByDescending(p => p.PaymentDate)
                     .FirstOrDefault();


                if (latestPayment != null)
                {
                    MapPaymentDetails(fileModel, latestPayment);
                }

                // Get approver details if exists
                if (userDetails.ApprovedBy.HasValue)
                {
                    var approver = (await _userDetailRepository.GetByAsync(x => x.UserId == userDetails.ApprovedBy)).FirstOrDefault();


                    if (approver != null)
                    {
                        fileModel.ApprovedBy = $"{approver.FirstName} {approver.LastName}".Trim();
                    }
                }

                return fileModel;
            }
            catch (Exception)
            {
                // Log exception details here
                throw;
            }
        }

        private void MapBasicUserDetails(FileModel model, User user, UserDetail details)
        {
            model.FamilyName = details.FamilyName;
            model.FirstName = details.FirstName;
            model.LastName = details.LastName;
            model.PhoneNumber = details.PhoneNumber ?? user.ContactNumber;
            model.OrgId = details.OrgId;
            model.EmailAddress = user.Email;
            model.YearOfBirth = details.YearOfBirth;
            model.Photo = details.Photo;
            model.UserId = user.Id;
            model.IsApproved = details.IsApproved;
            model.PreferredBy = details.PreferredBy;
            model.AddInWhatsAppOrTelegram = details.AddInWhatsAppOrTelegram;
            model.Reason = details.Reason;
            model.MembershipStatus = details.MembershipStatus;
            model.MembershipValidityDate = details.MembershipValidityDate;
            model.MemberId = details.UniqueId;
        }

        private void MapAddressDetails(FileModel model, Address address)
        {
            model.PostalCode = address.PostalCode;
            model.City = address.City;
            model.State = address.State;
            model.Country = address.Country;
            model.HomeTown = address.HomeTown;
        }

        private void MapSpouseDetails(FileModel model, UserDetail details)
        {
            model.SpouseDetails.SpouseCity = details.SpouseCity;
            model.SpouseDetails.SpouseCountry = details.SpouseCountry;
            model.SpouseDetails.SpouseYearOfBirth = details.SpouseYearOfBirth;
            model.SpouseDetails.SpouseEmail = details.SpouseEmail;
            model.SpouseDetails.SpouseFirstName = details.SpouseFirstName;
            model.SpouseDetails.SpouseHometown = details.SpouseHometown;
            model.SpouseDetails.SpouseLastName = details.SpouseLastName;
            model.SpouseDetails.SpouseState = details.SpouseState;
            model.SpouseDetails.SpousePhoneNumber = details.SpousePhoneNumber;
        }

        private void MapChildrenDetails(FileModel model, ICollection<ChildrensDetail> childDetails)
        {
            model.ChildrensDetails = childDetails?.Select(item => new ChildrensDetailsDataModel
            {
                ChildCity = item.ResidentCity,
                ChildCountry = item.ResidentCountry,
                ChildYearOfBirth = item.YearOfBirth,
                ChildUniqueId = item.ChildUniqueId,
                ChildEmailAddress = item.EmailId,
                ChildFirstName = item.FirstName,
                ChildLastName = item.LastName,
                ChildPhoneNumber = item.PhoneNo,
                ChildState = item.ResidentState
            }).ToList() ?? new List<ChildrensDetailsDataModel>();
        }

        private void MapPaymentDetails(FileModel model, Payment payment)
        {

            model.PaymentMethod = payment.PaymentMethod;
            model.Amount = payment.Amount;
            model.TransactionId = payment.TransactionId;
            model.PaymentDate = payment.PaymentDate;
            model.CheckNumber = payment.CheckNumber;
            model.BankName = payment.BankName;
            model.CheckDate = payment.CheckDate;
        }



        public async Task<int> GetUserCount(int orgId)
        {
            var users = await _userRepository.GetUsersByOrg(orgId);
            return users.Count(user => !user.IsDeleted &&
                user.UserRoles.All(ur => ur.Role.Name != "Admin"));
        }

        public async Task<int> GetMemberCount(int orgId)
        {
            var users = await _userRepository.GetUsersByOrg(orgId);
            var filteredUsers = users.Where(user =>
                !user.IsDeleted &&
                user.UserRoles.All(ur => ur.Role.Name != "Admin")
            ).ToList();

            var userIds = filteredUsers.Select(u => u.Id).ToList();
            var userDetails = await _userDetailRepository.GetByAsync(x => userIds.Contains(x.UserId) &&
        x.IsApproved == "Approved");

            return userDetails.Count(ud =>
                IsMembershipValid(ud.MembershipStatus, ud.MembershipValidityDate));
        }



        public async Task<List<GetAllUsersResponse>> GetAllUserProfiles(int orgId)
        {
            var users = await _userRepository.GetUsersByOrg(orgId);
            var filteredUsers = users.Where(user =>
                !user.IsDeleted &&
                user.UserRoles.All(ur => ur.Role.Name != "Admin")
            ).ToList();

            var userIds = filteredUsers.Select(u => u.Id).ToList();

            var userDetails = await _userDetailRepository.GetByAsync(x => userIds.Contains(x.UserId));
            var response = new List<GetAllUsersResponse>();

            foreach (var user in filteredUsers)
            {
                var userDetail = userDetails.FirstOrDefault(ud => ud.UserId == user.Id);
                var userAddress = (await _userAddressRepository.GetByAsync(x => x.Id == userDetail.AddressId)).FirstOrDefault();

                var userResponse = new GetAllUsersResponse
                {
                    UserId = user.Id,
                    FirstName = userDetail?.FirstName,
                    LastName = userDetail?.LastName,
                    FamilyName = userDetail?.FamilyName,
                    EmailAddress = user?.Email,
                    IsApproved = userDetail?.IsApproved,

                    YearOfBirth = userDetail?.YearOfBirth,
                    PostalCode = userAddress?.PostalCode,
                    City = userAddress?.City,
                    State = userAddress?.State,
                    Country = userAddress?.Country,
                    PreferredBy = userDetail?.PreferredBy,
                    PhoneNumber = user?.ContactNumber,
                    Photo = userDetail?.Photo,
                    MembershipStatus = userDetail?.MembershipStatus,
                    MembershipValidityDate = userDetail?.MembershipValidityDate
                };


                // Add to final response list
                response.Add(userResponse);
            }

            return response;
        }

        public async Task<List<GetAllUsersResponse>> GetAllMembersProfiles(int orgId)
        {
            var users = await _userRepository.GetUsersByOrg(orgId);
            var filteredUsers = users.Where(user =>
                !user.IsDeleted &&
                user.UserRoles.All(ur => ur.Role.Name != "Admin")
            ).ToList();

            var userIds = filteredUsers.Select(u => u.Id).ToList();

            var userDetails = await _userDetailRepository.GetByAsync(x => userIds.Contains(x.UserId));
            var response = new List<GetAllUsersResponse>();

            foreach (var user in filteredUsers)
            {
                var userDetail = userDetails.FirstOrDefault(ud => ud.UserId == user.Id);
                var userAddress = (await _userAddressRepository.GetByAsync(x => x.Id == userDetail.AddressId)).FirstOrDefault();

                var userResponse = new GetAllUsersResponse
                {
                    UserId = user.Id,
                    FirstName = userDetail?.FirstName,
                    LastName = userDetail?.LastName,
                    FamilyName = userDetail?.FamilyName,
                    EmailAddress = user.Email,
                    YearOfBirth = userDetail?.YearOfBirth,
                    IsApproved = userDetail?.IsApproved,
                    PostalCode = userAddress?.PostalCode,
                    City = userAddress?.City,
                    State = userAddress?.State,
                    Country = userAddress?.Country,
                    PreferredBy = userDetail?.PreferredBy,
                    PhoneNumber = user.ContactNumber,
                    Photo = userDetail?.Photo,
                    MembershipStatus = userDetail?.MembershipStatus,
                    MembershipValidityDate = userDetail?.MembershipValidityDate
                };

                if (IsMembershipValid(userResponse.MembershipStatus, userResponse.MembershipValidityDate))
                {
                    response.Add(userResponse);
                }

            }

            return response;


        }

        private bool IsMembershipValid(string? membershipStatus, DateTimeOffset? membershipValidityDate)
        {
            if (string.IsNullOrEmpty(membershipStatus) || !membershipValidityDate.HasValue)
                return false;

            // Ensure membership is active and not expired
            return membershipValidityDate.Value.Date >= DateTime.UtcNow.Date;

        }


        public async Task<List<DropDownModel>> GetAllOrganizations()
        {
            List<DropDownModel> organizationModels = new List<DropDownModel>();
            var organizations = await _orgRepository.GetAllAsync();
            int i = 0;
            foreach (var item in organizations)
            {
                organizationModels.Add(new DropDownModel());
                organizationModels[i].Code = item.Id.ToString();
                organizationModels[i].Name = item.OrgName;
                i++;
            }
            return organizationModels;
        }

        public async Task<(string BrandImageUrl, string BrandColor, string OrgName, string OrgWelMsg, string BrandLogo, string PrimaryEmail, string MatrimonyEmail, string EventEmail, string OrgHeadEmail)> GetBrandDetailsAsync(int orgId)
        {
            var organization = await _organizationRepository.GetByIdAsync(orgId);
            if (organization == null)
            {
                throw new ArgumentException($"No organization found with Id: {orgId}");
            }

            // Return the brand details (image and color)
            return (organization.BrandImageUrl, organization.BrandColor, organization.OrgName, organization.OrgWelMsg, organization.BrandLogo, organization.PrimaryEmail,
                    organization.MatrimonyEmail,
                    organization.EventEmail,
                    organization.OrgHeadEmail);
        }

        public async Task<List<DropDownModel>> GetAllChapters(int orgId)
        {
            List<DropDownModel> organizationChapterModels = new List<DropDownModel>();
            var orgChapter = await _orgChapterRepository.GetOrgChaptersAsync(orgId);
            int i = 0;
            foreach (var item in orgChapter)
            {
                organizationChapterModels.Add(new DropDownModel());
                organizationChapterModels[i].Code = item.Id.ToString();
                organizationChapterModels[i].Name = item.ChapterName;
                i++;
            }
            return organizationChapterModels;
        }

        public async Task<List<GetMemberShipTypeDropdown>> GetOrgMembershipData(int orgId)
        {
            List<GetMemberShipTypeDropdown> organizationMembershipModels = new List<GetMemberShipTypeDropdown>();
            var orgMemebershipType = await _membershipTypesRepository.GetOrgMembershipDataAsync(orgId);
            int i = 0;
            foreach (var item in orgMemebershipType)
            {
                organizationMembershipModels.Add(new GetMemberShipTypeDropdown());
                organizationMembershipModels[i].Code = item.Id;
                organizationMembershipModels[i].Name = item.MemberShipType;
                organizationMembershipModels[i].membershipamt = item.MemberShipAmount;
                organizationMembershipModels[i].validityperiod = item.ValidityPeriod;
                i++;
            }
            return organizationMembershipModels;
        }


        public async Task<Result> ForgotPassword(int orgId, string emailId, string newPassword)
        {
            try
            {
                // Validate inputs
                if (string.IsNullOrWhiteSpace(emailId) || string.IsNullOrWhiteSpace(newPassword) || orgId <= 0)
                {
                    return Result.Failure(400, "Invalid input parameters.");
                }

                var currentUser = (await _userRepository.GetUserByEmailAndOrg(emailId, orgId)).FirstOrDefault();

                if (currentUser == null)
                {
                    return Result.Failure(404, "User not found.");
                }

                // Update user details
                currentUser.UpdatedOn = DateTime.UtcNow;
                currentUser.Password = _passwordHandler.HashPassword(newPassword);

                await _userRepository.SaveAsync();

                var (brandImageUrl, brandColor, orgName, orgWelMsg, brandLogo, primaryEmail, matrimonyEmail, eventEmail, orgHeadEmail) =
             await GetBrandDetailsAsync(orgId);

                string emailBodyTemplate = await _commonBusiness.GetEmailTemplateFromCloudinaryAsync("user", "password-reset.html", orgId);



                var emailBody = emailBodyTemplate

                    .Replace("{Name}", currentUser.Name ?? "N/A")
                    .Replace("{ResetTime}", DateTime.UtcNow.ToString() ?? "N/A")
                    .Replace("{SupportEmail}", primaryEmail ?? "N/A")
                    .Replace("{OrgName}", orgName ?? "N/A");

                var emailModel = new EmailDataModel
                {
                    To = new List<string> { currentUser.Email },
                    Subject = "Your Password Resetted Succesfully for " + orgName,
                    Data = emailBody,
                    OrgId = orgId
                };
                await _emailChannel.Writer.WriteAsync(emailModel);

                return Result.Success(200, "Password updated successfully.");
            }
            catch (Exception ex)
            {

                return Result.Failure(500, "An unexpected error occurred.");
            }
        }


        public async Task<HttpResponseMessage> SendOTP(User userModel)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();

            try
            {
                int otp = GenerateSecureOTP();


                userModel.OTP = otp;
                userModel.OTPUsed = false;
                userModel.UpdatedOn = DateTime.UtcNow;

                await _userRepository.UpdateUser(userModel);

                var userOrganization = userModel.UserOrganizations.FirstOrDefault();

                var (brandImageUrl, brandColor, orgName, orgWelMsg, brandLogo, primaryEmail, matrimonyEmail, eventEmail, orgHeadEmail) =
              await GetBrandDetailsAsync(userOrganization.OrganizationId);

                string emailBodyTemplate = await _commonBusiness.GetEmailTemplateFromCloudinaryAsync("user", "otp-mail.html", userOrganization.OrganizationId);


                var emailBody = emailBodyTemplate

                    .Replace("{Name}", userModel.Name ?? "N/A")
                    .Replace("{Otp}", otp.ToString() ?? "Error")
                    .Replace("{SupportEmail}", primaryEmail)
                    .Replace("{OrgName}", orgName ?? "N/A");

                var emailModel = new EmailDataModel
                {
                    To = new List<string> { userModel.Email },
                    Subject = "Your OTP for " + orgName,
                    Data = emailBody,
                    OrgId = userOrganization.OrganizationId
                };

                await _emailChannel.Writer.WriteAsync(emailModel);

                httpResponseMessage.ReasonPhrase = otp.ToString();
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                return httpResponseMessage;
            }
            catch (Exception ex)
            {
                throw new Exception("Error while sending OTP", ex);
            }
        }

        // Secure OTP generator
        private int GenerateSecureOTP()
        {
            byte[] randomNumber = new byte[4]; // 4 bytes = 32 bits
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }

            // Convert to a positive integer and limit to 6 digits
            int otp = Math.Abs(BitConverter.ToInt32(randomNumber, 0)) % 1000000;

            // Ensure the OTP is 6 digits by adding leading zeros if necessary
            return otp;
        }

        public async Task<ResultModel> ChangePasswordAsync(int orgId, ResetPasswordModel model)
        {
            // Fetch the user based on orgId and model.UserId
            var user = await _userRepository.GetUserByIdAndOrg(model.UserId, orgId);
            if (user == null)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    StatusCode = 404,
                    Error = "User not found or not associated with the provided organization."
                };
            }

            // Verify the old password
            var isOldPasswordValid = _passwordHandler.VerifyPassword(model.OldPassword, user.Password);
            if (!isOldPasswordValid)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    StatusCode = 400,
                    Error = "old password is incorect."
                };
            }

            // Hash the new password
            user.Password = _passwordHandler.HashPassword(model.Password);

            try
            {
                // Update the password in the database
                await _userRepository.SaveAsync();

                var (brandImageUrl, brandColor, orgName, orgWelMsg, brandLogo, primaryEmail, matrimonyEmail, eventEmail, orgHeadEmail) =
            await GetBrandDetailsAsync(orgId);

                string emailBodyTemplate = await _commonBusiness.GetEmailTemplateFromCloudinaryAsync("user", "password-reset.html", orgId);


                var emailBody = emailBodyTemplate

                    .Replace("{Name}", user.Name ?? "N/A")
                    .Replace("{ResetTime}", DateTime.UtcNow.ToString() ?? "N/A")
                    .Replace("{SupportEmail}", primaryEmail ?? "N/A")
                    .Replace("{OrgName}", orgName ?? "N/A");

                var emailModel = new EmailDataModel
                {
                    To = new List<string> { user.Email },
                    Subject = "Your Password Changed Succesfully for " + orgName,
                    Data = emailBody,
                    OrgId = orgId
                };
                await _emailChannel.Writer.WriteAsync(emailModel);

            }
            catch (Exception ex)
            {
                return new ResultModel
                {
                    IsSuccess = false,
                    StatusCode = 500,
                    Error = "An error occurred while updating the password.",
                    Details = ex.Message
                };
            }

            return new ResultModel { IsSuccess = true, StatusCode = 200 };
        }

        public async Task<HttpResponseMessage> ForgotPasswordSendOTP(string emailId, int orgId)
        {
            try
            {
                // Fetch the user based on orgId and emailId
                var users = await _userRepository.GetUserByEmailAndOrg(emailId, orgId);

                // Check if a valid user exists
                if (users == null || !users.Any())
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        ReasonPhrase = "User not found or not associated with the provided organization."
                    };
                }

                // Use the first matching user (assuming it's the intended logic)
                var user = users.FirstOrDefault();

                // Ensure user is not null before proceeding
                if (user == null)
                {
                    return new HttpResponseMessage
                    {
                        StatusCode = HttpStatusCode.InternalServerError,
                        ReasonPhrase = "An unexpected error occurred while retrieving user data."
                    };
                }

                // Send OTP
                var httpResponseMessage = await SendOTP(user);

                return httpResponseMessage;
            }
            catch (Exception ex)
            {
                // Log exception here if logging is implemented
                // LogError(ex);

                return new HttpResponseMessage
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    ReasonPhrase = "An error occurred while processing the request."
                };
            }
        }


        public async Task<AuthModel> GetUserDetails(int orgId, LoginModel loginModel)
        {
            var authModel = new AuthModel();

            // Retrieve user details by email
            var userDetails = await _userRepository.GetUserByEmailAndOrg(loginModel.UserName, orgId);
            if (userDetails == null || !userDetails.Any())
            {
                authModel.Error = "User does not exist or is not associated with the provided organization.";
                return authModel;
            }

            var userEntity = userDetails.First();

            // Verify the password
            var isPasswordValid = _passwordHandler.VerifyPassword(loginModel.Password, userEntity.Password);
            if (!isPasswordValid)
            {
                authModel.Error = "Invalid password.";
                return authModel;
            }

            // Populate authModel with user details
            authModel.UserId = userEntity.Id;
            authModel.Name = userEntity.Name;
            // Retrieve user roles and organizations
            authModel.UserRoles = userEntity.UserRoles
                .Select(ur => new UserRoleModel
                {
                    RoleId = ur.RoleId,
                    RoleName = ur.Role.Name
                }).ToList();


            var organization = await _organizationRepository.GetByIdAsync(orgId); // Assuming _organizationRepository exists and has a GetByIdAsync method
            if (organization == null)
            {
                throw new InvalidOperationException("Organization is not registered.");
            }

            authModel.Organizations = userEntity.UserOrganizations.Where(uo => uo.OrganizationId == orgId)
                .Select(uo => new OrganizationModel
                {
                    Id = uo.OrganizationId,
                    OrgName = organization.OrgName,
                    BrandLogo = organization.BrandLogo,
                    OrgWelMsg = organization.OrgWelMsg,
                    BrandImageUrl = organization.BrandImageUrl,
                    BrandColor = organization.BrandColor
                }).ToList();

            // Generate JWT Token for the authenticated user
            await PopulateJwtTokenAsync(authModel, orgId);
            await UpdatelastLogin(authModel);

            return authModel;
        }


        public async Task<string> UpdateUser(int orgId, RegistrationModel userUpdateModel, IFormFile? photo)
        {

            try
            {
                string? photoUrl = null;

                if (photo != null)
                {
                    photoUrl = await _commonBusiness.UploadToCloudinaryAsync(photo, orgId);
                }

                var userDetails = (await _userDetailRepository.GetByAsync(x => x.UserId == userUpdateModel.UserId)).FirstOrDefault();


                userDetails.FirstName = userUpdateModel.FirstName;
                userDetails.FamilyName = userUpdateModel.FamilyName;
                userDetails.LastName = userUpdateModel.LastName;
                userDetails.PhoneNumber = userUpdateModel.PhoneNumber;
                userDetails.Reason = userUpdateModel.Reason;
                userDetails.YearOfBirth = userUpdateModel.YearOfBirth;
                userDetails.Photo = photoUrl ?? userDetails.Photo;
                userDetails.PreferredBy = userUpdateModel.PreferredBy;
                userDetails.UpdatedOn = DateTime.UtcNow;
                userDetails.AddInWhatsAppOrTelegram = userUpdateModel.AddInWhatsAppOrTelegram;
                //Spouse Details
                if (userUpdateModel.SpouseDetails != null)
                {
                    userDetails.SpouseFirstName = string.IsNullOrWhiteSpace(userUpdateModel.SpouseDetails.SpouseFirstName) ? userDetails.SpouseFirstName : userUpdateModel.SpouseDetails.SpouseFirstName;
                    userDetails.SpouseLastName = string.IsNullOrWhiteSpace(userUpdateModel.SpouseDetails.SpouseLastName) ? userDetails.SpouseLastName : userUpdateModel.SpouseDetails.SpouseLastName;
                    userDetails.SpouseHometown = string.IsNullOrWhiteSpace(userUpdateModel.SpouseDetails.SpouseHometown) ? userDetails.SpouseHometown : userUpdateModel.SpouseDetails.SpouseHometown;
                    userDetails.SpouseState = string.IsNullOrWhiteSpace(userUpdateModel.SpouseDetails.SpouseState) ? userDetails.SpouseState : userUpdateModel.SpouseDetails.SpouseState;
                    userDetails.SpouseEmail = string.IsNullOrWhiteSpace(userUpdateModel.SpouseDetails.SpouseEmail) ? userDetails.SpouseEmail : userUpdateModel.SpouseDetails.SpouseEmail;
                    userDetails.SpouseCity = string.IsNullOrWhiteSpace(userUpdateModel.SpouseDetails.SpouseCity) ? userDetails.SpouseCity : userUpdateModel.SpouseDetails.SpouseCity;
                    userDetails.SpouseCountry = string.IsNullOrWhiteSpace(userUpdateModel.SpouseDetails.SpouseCountry) ? userDetails.SpouseCountry : userUpdateModel.SpouseDetails.SpouseCountry;
                    userDetails.SpouseYearOfBirth = userUpdateModel.SpouseDetails.SpouseYearOfBirth ?? userDetails.SpouseYearOfBirth;
                    userDetails.SpousePhoneNumber = string.IsNullOrWhiteSpace(userUpdateModel.SpouseDetails.spousePhoneNumber) ? userDetails.SpousePhoneNumber : userUpdateModel.SpouseDetails.spousePhoneNumber;
                }
                _userDetailRepository.Update(userDetails);
                await _userDetailRepository.SaveAsync();



                var userAddress = (await _userAddressRepository.GetByAsync(x => x.Id == userDetails.AddressId)).FirstOrDefault();

                try
                {
                    if (userAddress == null)
                    {
                        userAddress = new Address
                        {
                            HomeTown = userUpdateModel.HomeTown,
                            City = userUpdateModel.City,
                            Country = userUpdateModel.Country,
                            State = userUpdateModel.State,
                            PostalCode = userUpdateModel.PostalCode
                        };
                        _userAddressRepository.Add(userAddress);
                        await _userAddressRepository.SaveAsync();
                        userDetails.AddressId = userAddress.Id;
                        await _userDetailRepository.SaveAsync();
                    }
                    else
                    {
                        userAddress.HomeTown = string.IsNullOrWhiteSpace(userUpdateModel.HomeTown) ? userAddress.HomeTown : userUpdateModel.HomeTown;
                        userAddress.City = string.IsNullOrWhiteSpace(userUpdateModel.City) ? userAddress.City : userUpdateModel.City;
                        userAddress.State = string.IsNullOrWhiteSpace(userUpdateModel.State) ? userAddress.State : userUpdateModel.State;
                        userAddress.Country = string.IsNullOrWhiteSpace(userUpdateModel.Country) ? userAddress.Country : userUpdateModel.Country;
                        userAddress.PostalCode = string.IsNullOrWhiteSpace(userUpdateModel.PostalCode) ? userAddress.PostalCode : userUpdateModel.PostalCode;

                        _userAddressRepository.Update(userAddress);
                        await _userAddressRepository.SaveAsync();
                        userDetails.AddressId = userAddress.Id;
                        await _userDetailRepository.SaveAsync();
                    }

                }
                catch (Exception ex)
                {

                }

                var existingChildren = (await _childrensRepository.GetByAsync(x => x.UserDetailId == userDetails.UserId)).ToList();

                int newUserIndex = 0;
                List<ChildrensDetail> childrenToAdd = new List<ChildrensDetail>();
                List<ChildrensDetail> childrenToRemove = new List<ChildrensDetail>();
                foreach (var sourceValue in userUpdateModel.ChildrensDetails)
                {
                    newUserIndex++;

                    // Check if child exists based on ChildUniqueId
                    var existChild = existingChildren
                        .Where(x => x.ChildUniqueId == sourceValue.ChildUniqueId)
                        .FirstOrDefault();

                    if (existChild != null)
                    {
                        existChild.FirstName = string.IsNullOrWhiteSpace(sourceValue.ChildFirstName) ? existChild.FirstName : sourceValue.ChildFirstName;
                        existChild.LastName = string.IsNullOrWhiteSpace(sourceValue.ChildLastName) ? existChild.LastName : sourceValue.ChildLastName;
                        existChild.EmailId = string.IsNullOrWhiteSpace(sourceValue.ChildEmailAddress) ? existChild.EmailId : sourceValue.ChildEmailAddress;
                        existChild.PhoneNo = string.IsNullOrWhiteSpace(sourceValue.ChildPhoneNumber) ? existChild.PhoneNo : sourceValue.ChildPhoneNumber;
                        existChild.YearOfBirth = sourceValue.ChildYearOfBirth;
                        existChild.ResidentCity = string.IsNullOrWhiteSpace(sourceValue.ChildCity) ? existChild.ResidentCity : sourceValue.ChildCity;
                        existChild.ResidentCountry = string.IsNullOrWhiteSpace(sourceValue.ChildCountry) ? existChild.ResidentCountry : sourceValue.ChildCountry;
                        existChild.ResidentState = string.IsNullOrWhiteSpace(sourceValue.ChildState) ? existChild.ResidentState : sourceValue.ChildState;
                    }
                    else
                    {
                        // Prepare new child to be added
                        var newChild = new ChildrensDetail
                        {
                            FirstName = sourceValue.ChildFirstName,
                            ChildUniqueId = sourceValue.ChildUniqueId,
                            LastName = sourceValue.ChildLastName,
                            YearOfBirth = sourceValue.ChildYearOfBirth,
                            UserDetailId = userUpdateModel.UserId,
                            ResidentState = sourceValue.ChildState,
                            ResidentCity = sourceValue.ChildCity,
                            ResidentCountry = sourceValue.ChildCountry,
                            EmailId = sourceValue.ChildEmailAddress,
                            PhoneNo = sourceValue.ChildPhoneNumber
                        };

                        childrenToAdd.Add(newChild); // Add to the list of children to be added
                    }
                }


                var incomingChildIds = userUpdateModel.ChildrensDetails.Select(c => c.ChildUniqueId).ToList();
                childrenToRemove = existingChildren.Where(ec => !incomingChildIds.Contains(ec.ChildUniqueId)).ToList();
                foreach (var childToRemove in childrenToRemove)
                {
                    _childrensRepository.Delete(childToRemove); // Remove from the repository (assumes entity is tracked)
                }

                // Save all updated children at once
                if (existingChildren.Any() || childrenToRemove.Any())
                {
                    await _childrensRepository.SaveAsync();  // Save the updated existing children
                }

                // Add new children and save them all at once
                foreach (var newChild in childrenToAdd)
                {
                    _childrensRepository.Add(newChild); // Add each new child individually
                }

                if (childrenToAdd.Any())
                {
                    await _childrensRepository.SaveAsync(); // Save all new children
                }

                var (brandImageUrl, brandColor, orgName, orgWelMsg, brandLogo, primaryEmail, matrimonyEmail, eventEmail, orgHeadEmail) =
          await GetBrandDetailsAsync(orgId);

                string emailBodyTemplate = await _commonBusiness.GetEmailTemplateFromCloudinaryAsync("user", "update-profile-notify.html", orgId);


                var emailBody = emailBodyTemplate
                    .Replace("{Name}", userDetails.FirstName ?? "N/A")
                    .Replace("{SupportEmail}", primaryEmail ?? "N/A")
                    .Replace("{OrgName}", orgName ?? "N/A");

                var emailModel = new EmailDataModel
                {
                    To = new List<string> { userUpdateModel.EmailAddress },
                    Subject = "Your Profile Updated for " + orgName,
                    Data = emailBody,
                    OrgId = orgId
                };
                await _emailChannel.Writer.WriteAsync(emailModel);



                return $"User {userDetails.UserId} Profile Updation Successful.";
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        public async Task DeleteUsersAsync(List<int> userIds, int updatedBy, int orgId)
        {
            await _userRepository.DeleteUsersAsync(userIds, updatedBy, orgId);
        }



        public async Task UpdatelastLogin(AuthModel authModel)
        {
            //needs update based on userid and orgid
            var user = (await _userRepository.GetByAsync(x => x.Id == authModel.UserId)).SingleOrDefault();
            user.LastLogin = DateTime.UtcNow;
            this._userRepository.Update(user);
            await this._userRepository.SaveAsync();

        }

        public async Task PopulateJwtTokenAsync(AuthModel authModel, int orgId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var orgConfig = await _configService.GetConfigByOrgId(orgId);

            // var key = Encoding.ASCII.GetBytes(orgConfig.Secret);

            var key = Encoding.ASCII.GetBytes(_appsettings.Secret);

            // Serialize complex data into JSON for storing in claims
            var organizationsJson = JsonConvert.SerializeObject(authModel.Organizations);
            var userRolesJson = JsonConvert.SerializeObject(authModel.UserRoles);

            // Create list of claims
            var claims = new List<Claim>
    {
        new Claim("UserId", authModel.UserId.ToString()),
        new Claim(ClaimTypes.Name, authModel.Name),
        new Claim("OrgId", orgId.ToString()),
        new Claim("Organizations", organizationsJson),
        new Claim("UserRoles", userRolesJson)
    };

            // Add role claims
            claims.AddRange(authModel.UserRoles.Select(role =>
                new Claim(ClaimTypes.Role, role.RoleName)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(orgConfig.ShortExpiryInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            authModel.Token = tokenHandler.WriteToken(token);
            authModel.TokenExpiryDate = DateTime.UtcNow.AddMinutes(orgConfig.ShortExpiryInMinutes);
        }

        public async Task<bool> HandleMembershipStatusAsync(int orgId, List<int> userIds, int actionUserId, bool isApproved)
        {
            if (userIds == null || !userIds.Any())
                throw new ArgumentException("UserIds cannot be null or empty.");

            var (brandImageUrl, brandColor, orgName, orgWelMsg, brandLogo, primaryEmail, matrimonyEmail, eventEmail, orgHeadEmail) =
                await GetBrandDetailsAsync(orgId);

            // Update membership status in the database
            bool isUpdated = isApproved
                ? await _userRepository.AcceptMemberships(orgId, userIds, actionUserId)
                : await _userRepository.RejectMemberships(orgId, userIds, actionUserId);

            if (isUpdated)
            {
                // Fetch emails using the existing repository method
                var emailsResult = await _userRepository.GetEmailsByUserIdsAsync(userIds);
                List<string> emailList = emailsResult is string singleEmail ? new List<string> { singleEmail } : emailsResult as List<string>;

                if (emailList != null && emailList.Any())
                {
                    // Read the single email template
                    string emailBodyTemplate = await _commonBusiness.GetEmailTemplateFromCloudinaryAsync("user", "membership-status-notify.html", orgId);


                    string statusColor = isApproved ? "#4CAF50" : "#DC3545"; // Green for Approved, Red for Rejected
                    string textColor = isApproved ? "#155724" : "#721c24";
                    string bgColor = isApproved ? "#D4EDDA" : "#F8D7DA";
                    string membershipStatus = isApproved ? "Approved ✅" : "Rejected ❌";
                    string statusMessage = isApproved
                        ? "Congratulations! Your membership request has been approved. You are now a valued member."
                        : "We regret to inform you that your membership request has been rejected.";


                    foreach (var email in emailList)
                    {
                        string emailBody = emailBodyTemplate
                    .Replace("{OrgName}", orgName ?? "Our Organization")
                    .Replace("{MembershipStatus}", membershipStatus)
                    .Replace("{StatusMessage}", statusMessage)
                    .Replace("{StatusColor}", statusColor)
                    .Replace("{TextColor}", textColor)
                    .Replace("{BgColor}", bgColor);


                        // Prepare and send email
                        var emailModel = new EmailDataModel
                        {
                            To = new List<string> { email },
                            Subject = $"Your Membership Request Has Been {(isApproved ? "Approved" : "Rejected")}",
                            Data = emailBody,
                            OrgId = orgId
                        };

                        await _emailChannel.Writer.WriteAsync(emailModel);
                    }
                }
            }

            return isUpdated;
        }

        // Wrapper methods for clarity
        public async Task<bool> AcceptMemberships(int orgId, List<int> userIds, int actionUserId)
        {
            return await HandleMembershipStatusAsync(orgId, userIds, actionUserId, true);
        }

        public async Task<bool> RejectMemberships(int orgId, List<int> userIds, int actionUserId)
        {
            return await HandleMembershipStatusAsync(orgId, userIds, actionUserId, false);
        }




    }
}