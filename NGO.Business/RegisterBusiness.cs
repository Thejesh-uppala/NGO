using System.Threading.Channels;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using NGO.Common;
using NGO.Data;
using NGO.Data.NGO.Entites;
using NGO.Model;
using NGO.Repository;
using NGO.Repository.Contracts;

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

        private readonly IMembershipUserTypesRepository _memberShipTypesRepository;
        private readonly IChildrensRepository _childrensRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IUserAddressRepository _userAddressRepository;
        private readonly CommonBusiness _commonBusiness;
        private readonly IPaymentRepository _paymentRepository;
        private readonly SMTPEmailProvider _smtpEmailProvider;
        private readonly PasswordHandler _passwordHandler;
        private readonly UserBusiness _userBusiness;
        private readonly Channel<EmailDataModel> _emailChannel;
        private readonly IOrganizationConfigService _configService;

        public RegisterBusiness(Channel<EmailDataModel> emailChannel,
            IOrganizationConfigService configService,
        IUserRepository userRepository, AppSettings appsettings, CommonBusiness commonBusiness, IMapper mapper, IUserRolesRepository userRolesRepository, IOrgRepository organizationRepository, IUserDetailRepository userDetailRepository, IMembershipUserTypesRepository memberShipTypesRepository, IChildrensRepository childrensRepository, IRoleRepository roleRepository, IPaymentRepository paymentRepository, SMTPEmailProvider sMTPEmailProvider, PasswordHandler passwordHandler, UserBusiness userBusiness, IUserAddressRepository userAddressRepository)
        {
            _userRepository = userRepository;
            _appsettings = appsettings;
            _configService = configService;
            _commonBusiness = commonBusiness;
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
            _userAddressRepository = userAddressRepository;
            _emailChannel = emailChannel;


        }

        public async Task<AuthModel> SignUp(string email, string password, string? phNumber, int orgId, string firstName, string? lastName, string? address, string city, string country, string state, int chapterId)
        {
            var userModel = new UserModel
            {
                Status = (int)Common.EnumLookUp.Active,
                ContactNumber = phNumber,
                Name = firstName,
                Email = email,
                CreatedOn = DateTime.UtcNow,
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

            //update userdetails
            var userDetails = (await _userDetailRepository.GetByAsync(x => x.UserId == userEntity.Id)).FirstOrDefault();
            if (userDetails == null)
            {
                userDetails = new UserDetail
                {
                    UserId = userEntity.Id,
                    OrgId = orgId,
                    ChapterId = chapterId,
                    FirstName = firstName,
                    LastName = lastName ?? "",
                    PhoneNumber = phNumber ?? "",
                    CreatedOn = DateTime.UtcNow,
                    UpdatedOn = DateTime.UtcNow,
                    IsApproved = "Pending",
                    MembershipStatus = "",
                    UniqueId = email
                };
                _userDetailRepository.Add(userDetails);
            }


            await _userDetailRepository.SaveAsync();

            var userAddress = (await _userAddressRepository.GetByAsync(x => x.Id == userDetails.AddressId)).FirstOrDefault();
            if (userAddress == null)
            {
                userAddress = new Address
                {
                    HomeTown = address ?? "",
                    City = city,
                    Country = country,
                    State = state,
                };
                _userAddressRepository.Add(userAddress);
            }
            else
            {
                userAddress.HomeTown = address ?? userAddress.HomeTown;
                userAddress.City = city ?? userAddress.City;
                userAddress.Country = country ?? userAddress.Country;
                userAddress.State = state ?? userAddress.State;

            }
            try
            {
                await _userAddressRepository.SaveAsync();
                userDetails.AddressId = userAddress.Id;
                await _userDetailRepository.SaveAsync();
            }
            catch (Exception ex)
            {

            }


            var (brandImageUrl, brandColor, orgName, orgWelMsg, brandLogo, primaryEmail, matrimonyEmail, eventEmail, orgHeadEmail) =
            await _userBusiness.GetBrandDetailsAsync(orgId);

            string emailBodyTemplate = await _commonBusiness.GetEmailTemplateFromCloudinaryAsync("user", "account-creation-notify.html", orgId);

            var emailBody = emailBodyTemplate

                .Replace("{Name}", userDetails.FirstName ?? "N/A")
                .Replace("{UserName}", email ?? "N/A")
                .Replace("{OrgName}", orgName ?? "N/A")
                .Replace("{SupportEmail}", primaryEmail ?? "")
                .Replace("{MembershipStatus}", "Not Active");

            var emailModel = new EmailDataModel
            {
                To = new List<string> { email },
                Subject = "User Account creation for " + orgName,
                Data = emailBody,
                OrgId = orgId
            };

            await _emailChannel.Writer.WriteAsync(emailModel);

            var authModel = new AuthModel
            {
                UserId = userEntity.Id, // Map Id to UserId
                Name = userEntity.Name,
                Email = userEntity.Email,
                Token = string.Empty, // Set a default or empty token
                TokenExpiryDate = DateTime.UtcNow.AddHours(1), // Example token expiry logic
                UserRoles = userEntity.UserRoles.Where(ur => ur.OrgId == orgId)
                    .Select(ur => new UserRoleModel
                    {
                        RoleId = ur.RoleId,
                        RoleName = "Member"
                    }).ToList(),
                Organizations = userEntity.UserOrganizations.Where(uo => uo.OrganizationId == orgId)
                    .Select(uo => new OrganizationModel
                    {
                        Id = uo.OrganizationId, // Assuming UserOrganization has OrganizationId
                        OrgName = organization.OrgName,
                        OrgWelMsg = organization.OrgWelMsg,
                        BrandImageUrl = organization.BrandImageUrl,
                        BrandLogo = organization.BrandLogo,
                        BrandColor = organization.BrandColor

                    }).ToList(),
                Error = null, // Set any default error message if needed
            };

            await _userBusiness.PopulateJwtTokenAsync(authModel, orgId);
            await _userBusiness.UpdatelastLogin(authModel);

            return authModel;

        }






        public async Task<string> RegisterUser(int orgId, RegistrationModel registrationModel, IFormFile? photo)
        {

            try
            {
                string? photoUrl = null;

                if (photo != null)
                {
                    photoUrl = await _commonBusiness.UploadToCloudinaryAsync(photo, orgId);
                }

                var userDetails = (await _userDetailRepository.GetByAsync(x => x.UserId == registrationModel.UserId)).FirstOrDefault();


                userDetails.FirstName = registrationModel.FirstName;
                userDetails.FamilyName = registrationModel.FamilyName;
                userDetails.LastName = registrationModel.LastName;
                userDetails.PhoneNumber = registrationModel.PhoneNumber;
                userDetails.Reason = registrationModel.Reason;
                userDetails.YearOfBirth = registrationModel.YearOfBirth;
                userDetails.Photo = photoUrl ?? userDetails.Photo;
                userDetails.PreferredBy = registrationModel.PreferredBy;
                userDetails.UpdatedOn = DateTime.UtcNow;
                userDetails.IsApproved = "Pending";
                userDetails.AddInWhatsAppOrTelegram = registrationModel.AddInWhatsAppOrTelegram;
                //Spouse Details
                if (registrationModel.SpouseDetails != null)
                {
                    userDetails.SpouseFirstName = string.IsNullOrWhiteSpace(registrationModel.SpouseDetails.SpouseFirstName) ? userDetails.SpouseFirstName : registrationModel.SpouseDetails.SpouseFirstName;
                    userDetails.SpouseLastName = string.IsNullOrWhiteSpace(registrationModel.SpouseDetails.SpouseLastName) ? userDetails.SpouseLastName : registrationModel.SpouseDetails.SpouseLastName;
                    userDetails.SpouseHometown = string.IsNullOrWhiteSpace(registrationModel.SpouseDetails.SpouseHometown) ? userDetails.SpouseHometown : registrationModel.SpouseDetails.SpouseHometown;
                    userDetails.SpouseState = string.IsNullOrWhiteSpace(registrationModel.SpouseDetails.SpouseState) ? userDetails.SpouseState : registrationModel.SpouseDetails.SpouseState;
                    userDetails.SpouseEmail = string.IsNullOrWhiteSpace(registrationModel.SpouseDetails.SpouseEmail) ? userDetails.SpouseEmail : registrationModel.SpouseDetails.SpouseEmail;
                    userDetails.SpouseCity = string.IsNullOrWhiteSpace(registrationModel.SpouseDetails.SpouseCity) ? userDetails.SpouseCity : registrationModel.SpouseDetails.SpouseCity;
                    userDetails.SpouseCountry = string.IsNullOrWhiteSpace(registrationModel.SpouseDetails.SpouseCountry) ? userDetails.SpouseCountry : registrationModel.SpouseDetails.SpouseCountry;
                    userDetails.SpouseYearOfBirth = registrationModel.SpouseDetails.SpouseYearOfBirth ?? userDetails.SpouseYearOfBirth;
                    userDetails.SpousePhoneNumber = string.IsNullOrWhiteSpace(registrationModel.SpouseDetails.spousePhoneNumber) ? userDetails.SpousePhoneNumber : registrationModel.SpouseDetails.spousePhoneNumber;

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
                            HomeTown = registrationModel.HomeTown,
                            City = registrationModel.City,
                            Country = registrationModel.Country,
                            State = registrationModel.State,
                            PostalCode = registrationModel.PostalCode
                        };
                        _userAddressRepository.Add(userAddress);
                        await _userAddressRepository.SaveAsync();
                        userDetails.AddressId = userAddress.Id;
                        await _userDetailRepository.SaveAsync();
                    }
                    else
                    {
                        userAddress.HomeTown = string.IsNullOrWhiteSpace(registrationModel.HomeTown) ? userAddress.HomeTown : registrationModel.HomeTown;
                        userAddress.City = string.IsNullOrWhiteSpace(registrationModel.City) ? userAddress.City : registrationModel.City;
                        userAddress.State = string.IsNullOrWhiteSpace(registrationModel.State) ? userAddress.State : registrationModel.State;
                        userAddress.Country = string.IsNullOrWhiteSpace(registrationModel.Country) ? userAddress.Country : registrationModel.Country;
                        userAddress.PostalCode = string.IsNullOrWhiteSpace(registrationModel.PostalCode) ? userAddress.PostalCode : registrationModel.PostalCode;

                        _userAddressRepository.Update(userAddress);
                        await _userAddressRepository.SaveAsync();
                        userDetails.AddressId = userAddress.Id;
                        await _userDetailRepository.SaveAsync();
                    }

                }
                catch (Exception ex)
                {

                }


                if (registrationModel.request_type == "draft")
                {

                }
                else
                {
                    var existingMembership = await _memberShipTypesRepository
    .GetByAsync(m => m.UserDetailId == registrationModel.UserId && m.MembershipId == registrationModel.MemberShipId);

                    if (existingMembership == null || !existingMembership.Any())
                    {
                        var memberShipTypeModel = new MemberShipTypeModel
                        {
                            UserDetailId = registrationModel.UserId,
                            MembershipId = registrationModel.MemberShipId
                        };

                        var memberShipData = _mapper.Map<Data.MemberShipType>(memberShipTypeModel);
                        _memberShipTypesRepository.Add(memberShipData, false, true);
                        await _memberShipTypesRepository.SaveAsync();
                    }


                    var userMembershipData = await _memberShipTypesRepository.GetUserMembershipDataAsync(orgId, registrationModel.MemberShipId);
                    userDetails.MembershipStatus = userMembershipData.MemberShipType;
                    DateTime TODAYDATE = DateTime.UtcNow;
                    DateTime membershipValidityDate = TODAYDATE.AddMonths((int)userMembershipData.ValidityPeriod); // Add validity period in months

                    userDetails.MembershipValidityDate = membershipValidityDate;
                    _userDetailRepository.Update(userDetails);
                    await _userDetailRepository.SaveAsync();
                    //paymentdata
                    var paymentModel = new PaymentModel
                    {



                        UserDetailId = registrationModel.UserId,
                        TransactionId = registrationModel.TransactionId,
                        PaymentMethod = registrationModel.PaymentMethod,
                        PaymentDate = DateTime.UtcNow,
                        CheckNumber = registrationModel.CheckNumber,
                        BankName = registrationModel.BankName,
                        CheckDate = registrationModel.CheckDate,
                        Amount = registrationModel.Amount


                    };
                    var paymentData = _mapper.Map<Payment>(paymentModel);
                    _paymentRepository.Add(paymentData, false, true);
                    await _paymentRepository.SaveAsync();
                }

                var existingChildren = (await _childrensRepository.GetByAsync(x => x.UserDetailId == userDetails.UserId)).ToList();

                int newUserIndex = 0;
                List<ChildrensDetail> childrenToAdd = new List<ChildrensDetail>();
                List<ChildrensDetail> childrenToRemove = new List<ChildrensDetail>();

                foreach (var sourceValue in registrationModel.ChildrensDetails)
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
                            UserDetailId = registrationModel.UserId,
                            ResidentState = sourceValue.ChildState,
                            ResidentCity = sourceValue.ChildCity,
                            ResidentCountry = sourceValue.ChildCountry,
                            EmailId = sourceValue.ChildEmailAddress,
                            PhoneNo = sourceValue.ChildPhoneNumber
                        };

                        childrenToAdd.Add(newChild); // Add to the list of children to be added
                    }
                }


                var incomingChildIds = registrationModel.ChildrensDetails.Select(c => c.ChildUniqueId).ToList();
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
               await _userBusiness.GetBrandDetailsAsync(orgId);
                string emailBodyTemplate = await _commonBusiness.GetEmailTemplateFromCloudinaryAsync("user", "member-registration.html", orgId);

                var toList = new List<string>();
                if (!string.IsNullOrWhiteSpace(registrationModel.EmailAddress))
                    toList.Add(registrationModel.EmailAddress);
                if (registrationModel.SpouseDetails != null && !string.IsNullOrWhiteSpace(registrationModel.SpouseDetails.SpouseEmail))
                    toList.Add(registrationModel.SpouseDetails.SpouseEmail);

                // Replace placeholders
                var emailBody = emailBodyTemplate
                    .Replace("{FirstName}", userDetails.FirstName)
                    .Replace("{LastName}", userDetails.LastName)
                    .Replace("{PhoneNumber}", userDetails.PhoneNumber ?? "N/A")
                    .Replace("{EmailAddress}", registrationModel.EmailAddress)

 .Replace("{SupportEmail}", primaryEmail)
                    .Replace("{City}", userAddress.City)
                    .Replace("{State}", userAddress.State)
                    .Replace("{PostalCode}", userAddress.PostalCode)
                    .Replace("{Country}", userAddress.Country)
                    .Replace("{HomeTown}", userAddress.HomeTown)

                    .Replace("{PaymentMethod}", registrationModel.PaymentMethod.ToString())
                    .Replace("{MemberShipStatus}", "In-Review")
                    .Replace("{PaymentStatus}", "Success")

                    .Replace("{SpouseFirstName}", userDetails.SpouseFirstName ?? "N/A")
                    .Replace("{SpouseLastName}", userDetails.SpouseLastName ?? " ")
                    .Replace("{SpouseEmail}", userDetails.SpouseEmail ?? "N/A")
                    .Replace("{spousePhoneNumber}", userDetails.SpousePhoneNumber ?? "N/A")

                    .Replace("{OrgName}", orgName ?? "N/A");



                // Send email
                var emailModel = new EmailDataModel
                {
                    To = toList,
                    Subject = "Membership Registration Request Received",
                    Data = emailBody,
                    OrgId = orgId
                };
                await _emailChannel.Writer.WriteAsync(emailModel);


                // Admin Email
                string adminEmailTemplate = await _commonBusiness.GetEmailTemplateFromCloudinaryAsync("admin", "member-registration-info.html", orgId);


                var adminEmailBody = adminEmailTemplate
                    .Replace("{FirstName}", userDetails.FirstName)
                    .Replace("{LastName}", userDetails.LastName)
                    .Replace("{PhoneNumber}", userDetails.PhoneNumber ?? "N/A")
                    .Replace("{EmailAddress}", registrationModel.EmailAddress)
                    .Replace("{MemberShipStatus}", "Not Approved")

                    .Replace("{MemberShipId}", registrationModel.MemberShipId.ToString())
                    .Replace("{PaymentDate}", registrationModel.PaymentDate.ToString())
                    .Replace("{MemberShipValidity}", userDetails.MembershipValidityDate.ToString())
                    .Replace("{PaymentMethod}", registrationModel.PaymentMethod.ToString())
                    .Replace("{PaymentStatus}", "Success")

                    .Replace("{OrgName}", orgName ?? "N/A");


                var emailModelToAdmin = new EmailDataModel

                {

                    To = new List<string> { primaryEmail },
                    Subject = "New Membership Request",
                    Data = adminEmailBody,
                    OrgId = orgId
                };
                await _emailChannel.Writer.WriteAsync(emailModelToAdmin);



                return $"Membership request sent for admin review";
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        public async Task SaveConfigAsync(OrganizationConfigInputModel inputModel)
        {
            await _configService.SaveConfigAsync(inputModel);
        }
    }
}
