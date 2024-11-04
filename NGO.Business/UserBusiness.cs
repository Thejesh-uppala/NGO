using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NGO.Common;
using NGO.Common.Helpers;
using NGO.Data;
using NGO.Model;
using NGO.Repository;
using NGO.Repository.Contracts;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
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
        private readonly PasswordHandler _passwordHandler; 
        private readonly AppSettings _appsettings;
        private readonly IMapper _mapper;
        public UserBusiness(IUserRepository userRepository, IUserDetailRepository userDetailRepository, IRoleRepository roleRepository, IUserRolesRepository userRolesRepository, IChildrensRepository childrensRepository, IOrgChapterRepository orgChapterRepository, IOrgRepository orgRepository, IPaymentRepository paymentRepository, SMTPEmailProvider smtpEmailProvider, PasswordHandler passwordHandler, IMapper mapper, AppSettings appsettings, IOrgRepository organizationRepository)
        {
            _userRepository = userRepository;
            _userDetailRepository = userDetailRepository;
            _roleRepository = roleRepository;
            _userRolesRepository = userRolesRepository;
            _childrensRepository = childrensRepository;
            _orgChapterRepository = orgChapterRepository;
            _orgRepository = orgRepository;
            _paymentRepository = paymentRepository;
            _smtpEmailProvider = smtpEmailProvider;
            _passwordHandler=passwordHandler;
            _appsettings = appsettings;
            _organizationRepository = organizationRepository;
            _mapper = mapper;
        }

        public async Task<FileModel> GetCurrentUserProfile(int userId)
        {
            FileModel fileModel = new FileModel();
            var user = (await _userRepository.GetByAsync(x => x.Id == userId)).FirstOrDefault();
            if (user != null)
            {
                var userDetails = (await _userDetailRepository.GetByAsync(x => x.UserId == user.Id)).FirstOrDefault();
                var childDetails = (await _childrensRepository.GetByAsync(x => x.UserDetailId == userDetails.Id)).ToList();
                try
                {
                    fileModel.Address = userDetails.Address;
                    fileModel.FamilyName = userDetails.FamilyName;
                    fileModel.FirstName = userDetails.FirstName;
                    fileModel.LastName = userDetails.LastName;
                    fileModel.PhoneNumber = userDetails.PhoneNumber;
                    fileModel.EmailAddress = user.Email;
                    fileModel.DOB = (DateTime)userDetails.Dob;
                    fileModel.PostalCode = userDetails.PostalCode;
                    fileModel.City = userDetails.City;
                    fileModel.State = userDetails.State;
                    fileModel.Country = userDetails.Country;
                    fileModel.UserDetailId = userDetails.UserId;
                    fileModel.UserId = user.Id;
                    fileModel.SocialMedia = userDetails.SocialMedia;
                    fileModel.PreferredBy = userDetails.PreferredBy;
                    fileModel.PhoneNumber = userDetails.PhoneNumber;
                    fileModel.WhatsAppNumber = userDetails.WhatsAppNumber;
                    fileModel.Reason = userDetails.Reason;
                    fileModel.HomeTown = userDetails.HomeTown;
                    fileModel.Refference = userDetails.Reason;
                    //Spouse
                    fileModel.SpouseDetails.SpouseCity = userDetails.SpouseCity;
                    fileModel.SpouseDetails.SpouseCountry = userDetails.SpouseCountry;
                    fileModel.SpouseDetails.SpouseDob = userDetails.SpouseDob;
                    fileModel.SpouseDetails.SpouseEmail = userDetails.SpouseEmail;
                    fileModel.SpouseDetails.SpouseFirstName = userDetails.SpouseFirstName;
                    fileModel.SpouseDetails.SpouseHometown = userDetails.SpouseHometown;
                    fileModel.SpouseDetails.SpouseLastName = userDetails.SpouseLastName;
                    fileModel.SpouseDetails.SpouseState = userDetails.SpouseState;
                    fileModel.SpouseDetails.SpousePhoneNumber = userDetails.SpousePhoneNumber;
                    fileModel.PhotoData = userDetails.Photo;
                    fileModel.OrgId = userDetails.OrgId;
                    fileModel.MemberId = userDetails.UniqueId;
                    //Child
                    List<ChildrensDetailsDataModel> children = new List<ChildrensDetailsDataModel>();
                    int i = 0;
                    foreach (var item in childDetails)
                    {
                        children.Add(new ChildrensDetailsDataModel());
                        children[i].ChildCity = item.ResidentCity;
                        children[i].ChildCountry = item.ResidentCountry;
                        children[i].ChildDOB = item.Dob;
                        children[i].ChildEmailAddress = item.EmailId;
                        children[i].ChildFirstName = item.FirstName;
                        children[i].ChildLastName = item.LastName;
                        children[i].ChildPhoneNumber = item.PhoneNo;
                        children[i].ChildState = item.ResidentState;
                        i++;
                    }
                    fileModel.ChildrensDetails = children;

                }
                catch (Exception ex)
                {

                    throw;
                }

            }
            return fileModel;
        }

        public async Task<byte[]> UploadPhoto(IFormFile photo, string userId)
        {
            var userData = (await this._userDetailRepository.GetByAsync(x => x.UserId == int.Parse(userId))).FirstOrDefault();
            if (userData != null)
            {
                if (photo != null)
                {
                    if (((photo.ContentType.ToLower() == "image/jpeg") || (photo.ContentType.ToLower() == "image/bmp") || (photo.ContentType.ToLower() == "image/png")))
                    {
                        using (var memorystream = new MemoryStream())
                        {
                            await photo.CopyToAsync(memorystream);
                            userData.Photo = memorystream.ToArray();

                        }
                    }
                }
                await this._userDetailRepository.SaveAsync();
            }
            return userData.Photo;
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

        public async Task<List<DropDownModel>> GetAllChapters()
        {
            List<DropDownModel> organizationChapterModels = new List<DropDownModel>();
            var orgChapter = await _orgChapterRepository.GetAllAsync();
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

        public async Task<HttpResponseMessage> MassPaymentRem(List<string> fIlterMassPaymentModels)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            List<string> emails = new List<string>();
            foreach (var item in fIlterMassPaymentModels)
            {
                emails.Add("parjanyakumar333@gmail.com");
                var user = (await _userRepository.GetByAsync(x => x.Id == int.Parse(item))).FirstOrDefault();
                if (user != null)
                {
                    emails.Add(user.Email);

                }
            }
            try
            {
                MailMessage message = new MailMessage();
                foreach (var item in emails)
                {
                    message.To.Add(new MailAddress(item));
                }
                var emailDataModel = new EmailDataModel()
                {
                    To = emails,
                    Data = "<p>" + "Payment Reminder Mail" + "</p>"
                          + "<p>" + "Please Do your payment" + "</P>",
                    Subject = "Payment Reminder"
                };
                emailDataModel = await this._smtpEmailProvider.SendAsync(emailDataModel);
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                httpResponseMessage.StatusCode = HttpStatusCode.BadRequest;
                throw ex;
            }
            return httpResponseMessage;
        }

        public async Task SaveMember(int userId, string memberId, int chapterId, int organizationId)
        {
            var userDetail = (await _userDetailRepository.GetByAsync(x => x.UserId == userId)).FirstOrDefault();
            if (userDetail != null)
            {
                userDetail.OrgId = organizationId;
                userDetail.ChapterId = chapterId;
                userDetail.UniqueId = memberId;
                await _userDetailRepository.SaveAsync();
            }
        }

        public async Task<List<PaymentModel>> GetPaymentDetails(int userId)
        {
            var payment = (await _paymentRepository.GetByAsync(x => x.UserDetailId == userId));
            List<PaymentModel> paymentModel = new List<PaymentModel>();
            int i = 0;
            foreach (var item in payment)
            {
                paymentModel.Add(new PaymentModel());
                paymentModel[i].PaymentId = item.PaymentId;
                paymentModel[i].PaymentDate = item.PaymentDate;
                i++;
            }
            return paymentModel;
        }

        public async Task<HttpResponseMessage> PaymentReminder(int userId)
        {
            PaymentModel paymentModel = new PaymentModel();
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            var user = (await _userRepository.GetByAsync(x => x.Id == userId)).FirstOrDefault();
            var userDetail = (await _userDetailRepository.GetByAsync(x => x.UserId == user.Id)).FirstOrDefault();
            //paymentModel.UserDetailId = userDetail.Id;
            //paymentModel.PaymentId = "1222";
            // paymentModel.PaymentDate = DateTime.UtcNow;
            //var payment = _mapper.Map<Payment>(paymentModel);
            //this._paymentRepository.Add(payment, false, true);
            //await _paymentRepository.SaveAsync();
            try
            {
                var emailDataModel = new EmailDataModel()
                {
                    To = new List<string>() {
                    user.Email
                },
                    Data = "<p>" + "Payment Reminder Mail" + "</p>"
                          + "<p>" + user.Name + "Please Do your payment" + "</P>"
                          + "<p>" + "Conatact Number" + ": " + user.ContactNumber + " </P>",
                    Subject = "Contact Me"
                };
                emailDataModel = await this._smtpEmailProvider.SendAsync(emailDataModel);
                httpResponseMessage.StatusCode = HttpStatusCode.OK;

                return httpResponseMessage;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public async Task<List<UserDetailModel>> GetAllUser()
        {
            var user = await _userDetailRepository.GetAllAsync();
            List<UserDetailModel> userDetailModels = new List<UserDetailModel>();
            foreach (var item in user)
            {
                Organization organization = null;
                var payment = (await _paymentRepository.GetByAsync(x => x.UserDetailId == item.UserId)).FirstOrDefault();
                if (item.OrgId != null)
                {
                    organization = (await _orgRepository.GetByAsync(x => x.Id == item.OrgId)).FirstOrDefault();
                }


                Random random = new Random();
                int id = 0;
                id = random.Next(100000, 999999);
                var memberId = "ANN" + id;
                userDetailModels.Add(new UserDetailModel
                {
                    FirstName = item.FirstName,
                    FamilyName = item.FamilyName,
                    LastName = item.LastName,
                    PhoneNumber = item.PhoneNumber,
                    Photo = item.Photo,
                    PreferredBy = item.PreferredBy,
                    City = item.City,
                    State = item.State,
                    Country = item.Country,
                    Reason = item.Reason,
                    HomeTown = item.HomeTown,
                    SocialMedia = item.SocialMedia,
                    UserId = item.Id,
                    MemberId = memberId,
                    PaymentDate = payment?.PaymentDate,
                    OrgName = new DropDownModel() { Code = organization?.Id.ToString(), Name = organization?.OrgName }
                });
            };
            return userDetailModels;
        }

      
        public async Task ForgotPassword(string emailId, string newPassword)
        {
            var currentUser = (await _userRepository.GetByAsync(x => x.Email == emailId)).FirstOrDefault();
            if (currentUser != null)
            {
                currentUser.CreatedOn = DateTime.UtcNow;
                currentUser.Password = Cryptography.ComputeSHA256Hash(newPassword, DateTime.UtcNow.ToString("dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture));
                await _userRepository.SaveAsync();
            }

        }

        public async Task<HttpResponseMessage> SendOTP(UserModel userModel)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            Random random = new Random();
            int otp = 0;
            otp = random.Next(100000, 999999);
            try
            {
                var emailDataModel = new EmailDataModel()
                {
                    To = new List<string>() {
                    userModel.Email
                },
                    Subject = "Your OTP for NGO",
                    Data = $"<p>" + "Hello " + userModel.Name + " ,</p>"
                         + "<p>Please enter below OTP to create your NGO account" + "</p>"
                         + "<br/>"
                         + "<p>" + "OTP:" + otp + "</p>"
                         + "<br />"
                         + "<p>Regards,</p>"
                         + "<p>NGO Team.</p>"
                         + "<p class='footer-text'>This is an autogenerated email.Please don't reply to this.</p> "
                };
                emailDataModel = await this._smtpEmailProvider.SendAsync(emailDataModel);
                httpResponseMessage.ReasonPhrase = otp.ToString();
                httpResponseMessage.StatusCode = HttpStatusCode.OK;
                return httpResponseMessage;
            }
            catch (Exception ex)
            {
                throw ex;
            }
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
                    Error = "The old password is incorrect."
                };
            }

            // Hash the new password
            user.Password = _passwordHandler.HashPassword(model.Password);

            try
            {
                // Update the password in the database
                await _userRepository.SaveAsync();
                var emailDataModel = new EmailDataModel
                {
                    To = new List<string> { user.Email },
                    Data = "<p>Password Rest is successful , use the below credentials to login</p>"
                       + $"<p>Username: {user.Email}</p>"
                       + $"<p>Password: {model.Password}</p>",  // Password as plain text is generally discouraged; consider a custom message instead.
                    Subject = "User Account Creation"
                };

                await _smtpEmailProvider.SendAsync(emailDataModel);

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

        public async Task<HttpResponseMessage> ForGotPasswordSendOTP(string emailId)
        {
            HttpResponseMessage httpResponseMessage = new HttpResponseMessage();
            var currentUser = (await _userRepository.GetByAsync(x => x.Email == emailId)).FirstOrDefault();
            if (currentUser != null)
            {
                var user = _mapper.Map<UserModel>(currentUser);
                httpResponseMessage = await SendOTP(user);
            }
            else
            {
                httpResponseMessage.StatusCode = HttpStatusCode.InternalServerError;
            }
            return httpResponseMessage;
        }

        public async Task UserContactEmailRem(int userId, int currentUserId)
        {
            var currentUser = (await _userRepository.GetByAsync(x => x.Id == currentUserId)).FirstOrDefault();
            var user = (await _userDetailRepository.GetByAsync(x => x.Id == userId)).FirstOrDefault();
            var userdetails = (await _userRepository.GetByAsync(x => x.Id == user.UserId)).FirstOrDefault();
            if (userdetails != null)
            {
                var emailDataModel = new EmailDataModel()
                {
                    To = new List<string>() {
                    userdetails.Email
                },
                    Data = "<p>" + "User Wants to Contact" + "</p>"
                           + "<p>" + "User Name" + ": " + userdetails.Name + "</P>"
                           + "<p>" + "Conatact Number" + ": " + userdetails.ContactNumber + " </P>",
                    Subject = "Contact Me"
                };
                emailDataModel = await this._smtpEmailProvider.SendAsync(emailDataModel);
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
            authModel.Email = userEntity.Email;
            authModel.PaymentInfo = userEntity.PaymentInfo;
            authModel.RefreshToken = userEntity.RefreshToken;
            // Retrieve user roles and organizations
            authModel.UserRoles = userEntity.UserRoles.Select(ur => new UserRoleModel
            {
                RoleId = ur.RoleId,
                RoleName = ur.Role.Name
            }).ToList();


            var organization = await _organizationRepository.GetByIdAsync(orgId); // Assuming _organizationRepository exists and has a GetByIdAsync method
            if (organization == null)
            {
                throw new InvalidOperationException("Organization is not registered.");
            }

            authModel.Organizations = userEntity.UserOrganizations.Select(uo => new OrganizationModel
            {
                Id = uo.OrganizationId,
                OrgName = organization.OrgName
            }).ToList();

            // Generate JWT Token for the authenticated user
            await PopulateJwtTokenAsync(authModel);
            await UpdatelastLogin(authModel);

            return authModel;
        }

        public async Task<UserDetailModel> GetCurrentUserDetails(int Id)
        {
            try
            {
                UserDetailModel userDetailModel = new UserDetailModel();
                var user = (await _userRepository.GetByAsync(x => x.Id == Id)).FirstOrDefault();
                if (user != null)
                {
                    var userDetails = (await _userDetailRepository.GetByAsync(x => x.UserId == user.Id)).FirstOrDefault();
                    var payment = (await _paymentRepository.GetByAsync(x => x.UserDetailId == userDetails.UserId)).FirstOrDefault();
                    var childDetails = (await _childrensRepository.GetByAsync(x => x.UserDetailId == userDetails.Id)).ToList();
                    userDetails.ChildrensDetails = childDetails;

                    userDetailModel = _mapper.Map<UserDetailModel>(userDetails);
                    userDetailModel.OrgId = userDetails.OrgId;
                    userDetailModel.PaymentDate = payment?.PaymentDate;



                }
                var userDataFromProc = await  _childrensRepository.GetCurrentUserDetails(Id);
                return userDetailModel;
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        public async Task<string> UpdateUser(FileModel userDetailModel)
        {
            var userDetailData = (await this._userDetailRepository.GetByAsync(x => x.UserId == userDetailModel.UserId)).FirstOrDefault();
            try
            {
                var childData = (await this._childrensRepository.GetAllAsync());
                var filteredChildDataforUser = childData.Select(x => x).Where(x => x.UserDetailId == userDetailData.Id).ToList();
                if (userDetailData != null)
                {
                    UserDetailModel userModel = new UserDetailModel();
                    userDetailData.FirstName = userDetailModel.FirstName;
                    userDetailData.FamilyName = userDetailModel.FamilyName;
                    userDetailData.LastName = userDetailModel.LastName;
                    userDetailData.PhoneNumber = userDetailModel.PhoneNumber;
                    userDetailData.Reason = userDetailModel.Reason;
                    userDetailData.Dob = userDetailModel.DOB;
                    userDetailData.City = userDetailModel.City;
                    userDetailData.State = userDetailModel.State;
                    userDetailData.Country = userDetailModel.Country;
                    userDetailData.HomeTown = userDetailModel.HomeTown;
                    userDetailData.SocialMedia = userDetailModel.SocialMedia;
                    userDetailData.PreferredBy = userDetailModel.PreferredBy;
                    userDetailData.CreatedOn = DateTime.Now;
                    userDetailData.UpdatedOn = DateTime.Now;
                    userDetailData.Address = userDetailModel.Address;
                    userDetailData.PostalCode = userDetailModel.PostalCode;
                    userDetailData.WhatsAppNumber = userDetailModel.WhatsAppNumber;
                    //Spouse Details
                    userDetailData.SpouseFirstName = userDetailModel.SpouseDetails.SpouseFirstName;
                    userDetailData.SpouseLastName = userDetailModel.SpouseDetails.SpouseLastName;
                    userDetailData.SpouseHometown = userDetailModel.SpouseDetails.SpouseHometown;
                    userDetailData.SpouseState = userDetailModel.SpouseDetails.SpouseState;
                    userDetailData.SpouseEmail = userDetailModel.SpouseDetails.SpouseEmail;
                    userDetailData.SpouseCity = userDetailModel.SpouseDetails.SpouseCity;
                    userDetailData.SpouseCountry = userDetailModel.SpouseDetails.SpouseCountry;
                    userDetailData.SpouseDob = userDetailModel.SpouseDetails.SpouseDob;
                    userDetailData.SpousePhoneNumber = userDetailModel.SpouseDetails.SpousePhoneNumber;
                    //Child Details
                    int newUserIndex = 0;
                    foreach (var sourceValue in userDetailModel.ChildrensDetails)
                    {
                        newUserIndex++;
                        var existChild = filteredChildDataforUser.Select(y => y).Where(x => x.ChildUniqueId == sourceValue.ChildUniqueId).ToList();
                        if (existChild.Any())
                        {
                            int i = 0;
                            foreach (var item in existChild)
                            {
                                item.FirstName = userDetailModel.ChildrensDetails[i].ChildFirstName;
                                item.LastName = userDetailModel.ChildrensDetails[i].ChildLastName;
                                item.EmailId = userDetailModel.ChildrensDetails[i].ChildEmailAddress;
                                item.PhoneNo = userDetailModel.ChildrensDetails[i].ChildPhoneNumber;
                                item.Dob = userDetailModel.ChildrensDetails[i].ChildDOB;
                                item.ResidentCity = userDetailModel.ChildrensDetails[i].ChildCity;
                                item.ResidentCountry = userDetailModel.ChildrensDetails[i].ChildCountry;
                                item.ResidentState = userDetailModel.ChildrensDetails[i].ChildState;
                                i++;
                            }
                            await this._childrensRepository.SaveAsync();
                        }
                        else
                        {
                            List<ChildrensDetailsModel> childrensDetailsModel = new List<ChildrensDetailsModel>();
                            int j = 0;
                            for (int i = newUserIndex - 1; i < userDetailModel.ChildrensDetails.Count; i++)
                            {
                                childrensDetailsModel.Add(new ChildrensDetailsModel());
                                childrensDetailsModel[j].FirstName = userDetailModel.ChildrensDetails[i].ChildFirstName;
                                childrensDetailsModel[j].ChildUniqueId = userDetailModel.ChildrensDetails[i].ChildUniqueId;
                                childrensDetailsModel[j].LastName = userDetailModel.ChildrensDetails[i].ChildLastName;
                                childrensDetailsModel[j].Dob = userDetailModel.ChildrensDetails[i].ChildDOB;
                                childrensDetailsModel[j].UserDetailId = userDetailModel.UserId;
                                childrensDetailsModel[j].ResidentState = userDetailModel.ChildrensDetails[i].ChildState;
                                childrensDetailsModel[j].ResidentCity = userDetailModel.ChildrensDetails[i].ChildCity;
                                childrensDetailsModel[j].ResidentCountry = userDetailModel.ChildrensDetails[i].ChildCountry;
                                childrensDetailsModel[j].EmailId = userDetailModel.ChildrensDetails[i].ChildEmailAddress;
                                childrensDetailsModel[j].PhoneNo = userDetailModel.ChildrensDetails[i].ChildPhoneNumber;
                                j++;
                            }
                            foreach (var item in childrensDetailsModel)
                            {
                                var child = _mapper.Map<ChildrensDetail>(item);
                                this._childrensRepository.Add(child);
                                await this._childrensRepository.SaveAsync();
                            }
                        }

                    }
                }
                await this._userDetailRepository.SaveAsync();
                return userDetailModel.UserId.ToString();
            }
            catch (Exception ex)
            {

                throw;
            }
            return userDetailModel.UserId.ToString();
        }

        public async Task UpdatelastLogin(AuthModel authModel)
        {
            //needs update based on userid and orgid
            var user = (await _userRepository.GetByAsync(x => x.Id == authModel.UserId)).SingleOrDefault();
            user.LastLogin = DateTime.UtcNow;
            this._userRepository.Update(user);
            await this._userRepository.SaveAsync();

        }
        public async Task UpdateRefreshToken(AuthModel authModel)
        {
            //needs update based on userid and orgid
            var user = (await _userRepository.GetByAsync(x => x.Id == authModel.UserId)).SingleOrDefault();
            user.RefreshToken = authModel.RefreshToken;
            this._userRepository.Update(user);
            await this._userRepository.SaveAsync();
        }

        public async Task PopulateJwtTokenAsync(AuthModel authModel)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appsettings.Secret);

            // Serialize complex data into JSON for storing in claims
            var organizationsJson = JsonConvert.SerializeObject(authModel.Organizations);
            var userRolesJson = JsonConvert.SerializeObject(authModel.UserRoles);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
            new Claim(ClaimTypes.UserData, authModel.UserId.ToString()),
            new Claim(ClaimTypes.Email, authModel.Email),
            new Claim(ClaimTypes.Name, authModel.Name),
            new Claim("Organizations", organizationsJson), // Custom claim for organizations
            new Claim("UserRoles", userRolesJson)          // Custom claim for roles
                }),
                Expires = authModel.TokenExpiryDate = DateTime.UtcNow.AddMinutes(_appsettings.TokenSettings.SessionExpiryInMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            authModel.Token = tokenHandler.WriteToken(token);
        }

        public async Task<AuthModel?> GenerateNewAccessTokenAsync(int userId, Guid refreshToken)
        {
            // Retrieve the user by refresh token and user ID
            var userEntity = await _userRepository.GetUserByRefreshTokenAndUserId(refreshToken, userId);

            //if (userEntity == null || userEntity.RefreshTokenExpiryDate <= DateTime.UtcNow)
            if (userEntity == null)

                return null;

            // Create AuthModel for token generation
            var authModel = new AuthModel
            {
                UserId = userEntity.Id,
                Name = userEntity.Name,
                Email = userEntity.Email,
                PaymentInfo = userEntity.PaymentInfo,
                UserRoles = userEntity.UserRoles.Select(ur => new UserRoleModel
                {
                    RoleId = ur.RoleId,
                    RoleName = ur.Role.Name // Assuming role entity has a name property
                }).ToList(),
                Organizations = userEntity.UserOrganizations.Select(uo => new OrganizationModel
                {
                    Id = uo.OrganizationId,
                    OrgName = uo.Organization.OrgName // Assuming Organization entity has OrgName property
                }).ToList()
            };

            // Generate a new JWT token
            await PopulateJwtTokenAsync(authModel);

            return authModel;
        }


    }
}


