using AutoMapper;
using Microsoft.AspNetCore.Http;
using NGO.Common;
using NGO.Common.Helpers;
using NGO.Data;
using NGO.Data.NGO.Entites;
using NGO.Model;
using NGO.Repository;
using NGO.Repository.Contracts;
using System.Globalization;

namespace NGO.Business
{
    public class RegisterBusiness
    {
        private readonly IUserRepository _userRepository;
        private readonly AppSettings _appsettings;
        private readonly IMapper _mapper;
        private readonly IUserRolesRepository _userRolesRepository;
        private readonly IOrgRepository _organizationRepository;
        private readonly IUserDetailRepository _userDetailRepository;
        private readonly IMemberShipTypesRepository _memberShipTypesRepository;
        private readonly IChildrensRepository _childrensRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly SMTPEmailProvider _smtpEmailProvider;
        private readonly PasswordHandler _passwordHandler;
        private readonly UserBusiness _userBusiness;

        public RegisterBusiness(IUserRepository userRepository, AppSettings appsettings, IMapper mapper, IUserRolesRepository userRolesRepository, IOrgRepository organizationRepository, IUserDetailRepository userDetailRepository, IMemberShipTypesRepository memberShipTypesRepository, IChildrensRepository childrensRepository, IRoleRepository roleRepository, IPaymentRepository paymentRepository, SMTPEmailProvider sMTPEmailProvider, PasswordHandler passwordHandler, UserBusiness userBusiness)
        {
            _userRepository = userRepository;
            _appsettings = appsettings;
            _mapper = mapper;
            _userRolesRepository = userRolesRepository;
            _organizationRepository = organizationRepository;
            _userDetailRepository = userDetailRepository;
            _memberShipTypesRepository = memberShipTypesRepository;
            _childrensRepository = childrensRepository;
            _roleRepository = roleRepository;
            _paymentRepository = paymentRepository;
            _smtpEmailProvider = sMTPEmailProvider;
            _passwordHandler = passwordHandler;
            _userBusiness = userBusiness;


        }

        public async Task<AuthModel> SignUp(string userName, string email, string password, string phNumber, int orgId)
        {
            var userModel = new UserModel
            {
                Status = (int)Common.EnumLookUp.Active,
                ContactNumber = phNumber,
                Name = userName,
                Email = email,
                CreatedOn = DateTime.UtcNow,
                PaymentInfo = ((int)PaymentInfo.PENDING).ToString(),
            };

            // Encrypt password using Argon2Id hashing algorithm
            userModel.Password = _passwordHandler.HashPassword(password);

            // Check if user already exists for the specified orgId
            var existingUser = await _userRepository.GetUserByEmailAndOrg(email, orgId);
            if (existingUser.Any())
            {
                throw new InvalidOperationException("User already registered with this organization.");
            }

            var organization = await _organizationRepository.GetByIdAsync(orgId); // Assuming _organizationRepository exists and has a GetByIdAsync method
            if (organization == null)
            {
                throw new InvalidOperationException("Organization is not registered.");
            }

            // Map user model to user entity and add to the repository
            var userEntity = _mapper.Map<User>(userModel);
            _userRepository.Add(userEntity, false, true);

            try
            {
                
                await _userRepository.SaveAsync();
               
                // Add organization relationship to the user
                userEntity.UserOrganizations = new List<UserOrganization>
                {
                    new UserOrganization { UserId = userEntity.Id, OrganizationId = orgId }
                };

                await _userRepository.SaveAsync();


            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred during signup.", ex);
            }
            try
            {
                var memberRole = await _roleRepository.GetRoleByNameAsync("Member");
                if (memberRole == null)
                {
                    throw new Exception("Default 'Member' role is not found in the database.");
                }

                // Assign the "Member" role to the user
                var userRole = new UserRole
                {
                    UserId = userEntity.Id, // Use the user ID after it's generated
                    RoleId = memberRole.Id,
                    OrgId = orgId
                };
                _userRolesRepository.Add(userRole);
                await _userRepository.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Error occurred during Role assigning.", ex);
            }

            // Send confirmation email
            var emailDataModel = new EmailDataModel
            {
                To = new List<string> { email },
                Data = "<p>User created successfully, use the below credentials to login</p>"
                        + $"<p>Username: {userModel.Email}</p>"
                        + $"<p>Password: {password}</p>",  // Password as plain text is generally discouraged; consider a custom message instead.
                Subject = "User Account Creation"
            };

            await _smtpEmailProvider.SendAsync(emailDataModel);


            var authModel = new AuthModel
                {
                    UserId = userEntity.Id, // Map Id to UserId
                    Name = userEntity.Name,
                    Email = userEntity.Email,
                    PaymentInfo = userEntity.PaymentInfo,
                    Token = string.Empty, // Set a default or empty token
                    TokenExpiryDate = DateTime.UtcNow.AddHours(1), // Example token expiry logic
                    RefreshToken = Guid.NewGuid(), // Generate a new RefreshToken
                    UserRoles = userEntity.UserRoles.Select(ur => new UserRoleModel
                    {
                        RoleId = ur.RoleId,
                        RoleName = "Member"
                    }).ToList(),
                    Organizations = userEntity.UserOrganizations.Select(uo => new OrganizationModel
                    {
                         Id = uo.OrganizationId, // Assuming UserOrganization has OrganizationId
                        OrgName = organization.OrgName // Assuming UserOrganization has OrganizationName
                    }).ToList(),
                    Error = null, // Set any default error message if needed
                };

            await _userBusiness.PopulateJwtTokenAsync(authModel);
            await _userBusiness.UpdatelastLogin(authModel);

            return authModel;

        }


        public async Task UploadPhoto(IFormFile photo, string userId)
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
            
        }


        public async Task<string> RegisterUser(RegistrationModel registrationModel)
        {
            UserModel userModel = new UserModel();
            userModel.Status = (int)Common.EnumLookUp.Active;
            userModel.ContactNumber = registrationModel.PhoneNumber;
            userModel.Name = registrationModel.FirstName;
            userModel.Email = registrationModel.EmailAddress;
            userModel.CreatedOn = DateTime.UtcNow;
            userModel.Password = Cryptography.ComputeSHA256Hash(registrationModel.Password, userModel.CreatedOn.ToString("dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture));
            userModel.PaymentInfo = ((int)PaymentInfo.PENDING).ToString();
            var user = _mapper.Map<User>(userModel);
            var userdetails = _mapper.Map<UserModel>(user);
           // user.PaymentInfo = ((int)PaymentInfo.PAID).ToString();
            //var userdetails = _mapper.Map<UserModel>(user);
            try
            {
                UserRole userRole = new UserRole();
                this._userRepository.Add(user, false, true);
                await this._userRepository.SaveAsync();
                userRole.UserId = user.Id;
                userRole.RoleId = 1;
                this._userRolesRepository.Add(userRole, false, true);
                await this._userRolesRepository.SaveAsync();

                UserDetailModel userDetailModel = new UserDetailModel();
                userDetailModel.FirstName = registrationModel.FirstName;
                userDetailModel.FamilyName = registrationModel.FamilyName;
                userDetailModel.LastName = registrationModel.LastName;
                userDetailModel.PhoneNumber = registrationModel.PhoneNumber;
                userDetailModel.Reason = registrationModel.Reason;
                userDetailModel.Dob = registrationModel.DOB;
                userDetailModel.City = registrationModel.City;
                userDetailModel.State = registrationModel.State;
                userDetailModel.Country = registrationModel.Country;
                userDetailModel.HomeTown = registrationModel.HomeTown;
                userDetailModel.SocialMedia = registrationModel.SocialMedia;
                userDetailModel.PreferredBy = registrationModel.PreferredBy;
                userDetailModel.CreatedOn = DateTime.Now;
                userDetailModel.UpdatedOn = DateTime.Now;
                userDetailModel.UserId = user.Id;
                userDetailModel.Address = registrationModel.Address;
                userDetailModel.PostalCode = registrationModel.PostalCode;
                userDetailModel.WhatsAppNumber = registrationModel.WhatsAppNumber;
                //Spouse Details
                userDetailModel.SpouseFirstName = registrationModel.SpouseDetails.SpouseFirstName;
                userDetailModel.SpouseLastName = registrationModel.SpouseDetails.SpouseLastName;
                userDetailModel.SpouseHometown = registrationModel.SpouseDetails.SpouseHometown;
                userDetailModel.SpouseState = registrationModel.SpouseDetails.SpouseState;
                userDetailModel.SpouseEmail = registrationModel.SpouseDetails.SpouseEmail;
                userDetailModel.SpouseCity = registrationModel.SpouseDetails.SpouseCity;
                userDetailModel.SpouseCountry = registrationModel.SpouseDetails.SpouseCountry;
                userDetailModel.SpouseDob = registrationModel.SpouseDetails.SpouseDob;

                if (userDetailModel.Photo != null)
                {
                    if (((registrationModel.Photo.ContentType.ToLower() == "image/jpeg") || (registrationModel.Photo.ContentType.ToLower() == "image/bmp") || (registrationModel.Photo.ContentType.ToLower() == "image/png")))
                    {
                        using (var memorystream = new MemoryStream())
                        {
                            await registrationModel.Photo.CopyToAsync(memorystream);
                            userDetailModel.Photo = memorystream.ToArray();

                        }
                    }
                }
                var userDetailData = _mapper.Map<UserDetail>(userDetailModel);
                this._userDetailRepository.Add(userDetailData, false, true);
                await this._userDetailRepository.SaveAsync();
                var getData = (await this._userDetailRepository.GetByAsync(x => x.UserId == userDetailModel.UserId)).FirstOrDefault();
                
                MemberShipTypeModel memberShipTypeModel = new MemberShipTypeModel();
                memberShipTypeModel.UserDetailId = getData.Id;
                memberShipTypeModel.MemberShipTypesData = registrationModel.MemberShipType;
                var memberShipData = _mapper.Map<Data.MemberShipType>(memberShipTypeModel);
                this._memberShipTypesRepository.Add(memberShipData, false, true);
                await this._memberShipTypesRepository.SaveAsync();
                //Payemnt
                PaymentModel paymentModel = new PaymentModel();
                paymentModel.UserDetailId = getData.Id;
                paymentModel.PaymentId = registrationModel.PaymentId;
                paymentModel.PaymentDate = registrationModel.PaymentDate;
                paymentModel.PayerId = registrationModel.Payer_id;
                var payementData = _mapper.Map<Payment>(paymentModel);
                this._paymentRepository.Add(payementData, false, true);
                await this._paymentRepository.SaveAsync();
                //Child Details
                List<ChildrensDetailsModel> childrensDetailsModel = new List<ChildrensDetailsModel>();
                int i = 0;
                foreach (var item in registrationModel.ChildrensDetails)
                {
                    childrensDetailsModel.Add(new ChildrensDetailsModel());
                    childrensDetailsModel[i].FirstName = item.ChildFirstName;
                    childrensDetailsModel[i].LastName = item.ChildLastName;
                    childrensDetailsModel[i].Dob = item.ChildDOB;
                    childrensDetailsModel[i].UserDetailId = getData.Id;
                    childrensDetailsModel[i].ResidentState = item.ChildState;
                    childrensDetailsModel[i].ResidentCity = item.ChildCity;
                    childrensDetailsModel[i].ResidentCountry = item.ChildCountry;
                    childrensDetailsModel[i].EmailId = item.ChildEmailAddress;
                    childrensDetailsModel[i].PhoneNo = item.ChildPhoneNumber;
                    childrensDetailsModel[i].ChildUniqueId = item.ChildPhoneNumber;
                    i++;
                }
                foreach (var item in childrensDetailsModel)
                {
                    var child = _mapper.Map<ChildrensDetail>(item);
                    this._childrensRepository.Add(child, false, true);
                    await this._childrensRepository.SaveAsync();
                }
                //var emailDataModel = new EmailDataModel()
                //{
                //    To = new List<string>() {
                //    registrationModel.EmailAddress
                //},
                //    Data = "<p>" + "User created successfully, use the below creadentials to login" + "</p>"
                //             + "<p>" + "User Name" + ": " + registrationModel.FirstName + "</P>",
                //    Subject = "Payment"
                //};
                //emailDataModel = await this._smtpEmailProvider.SendAsync(emailDataModel);
            }
            catch (Exception ex)
            {

                throw;
            }

            return user.Id.ToString();
        }

    }
}
