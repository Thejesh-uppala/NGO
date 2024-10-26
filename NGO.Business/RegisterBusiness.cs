using AutoMapper;
using Microsoft.AspNetCore.Http;
using NGO.Common;
using NGO.Common.Helpers;
using NGO.Data;
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
        private readonly IUserDetailRepository _userDetailRepository;
        private readonly IMemberShipTypesRepository _memberShipTypesRepository;
        private readonly IChildrensRepository _childrensRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly SMTPEmailProvider _smtpEmailProvider;

        public RegisterBusiness(IUserRepository userRepository, AppSettings appsettings, IMapper mapper, IUserRolesRepository userRolesRepository, IUserDetailRepository userDetailRepository, IMemberShipTypesRepository memberShipTypesRepository, IChildrensRepository childrensRepository, IRoleRepository roleRepository, IPaymentRepository paymentRepository, SMTPEmailProvider sMTPEmailProvider)
        {
            _userRepository = userRepository;
            _appsettings = appsettings;
            _mapper = mapper;
            _userRolesRepository = userRolesRepository;
            _userDetailRepository = userDetailRepository;
            _memberShipTypesRepository = memberShipTypesRepository;
            _childrensRepository = childrensRepository;
            _roleRepository = roleRepository;
            _paymentRepository = paymentRepository;
            _smtpEmailProvider = sMTPEmailProvider;

        }

        public async Task<UserModel> SignUp(string userName, string email, string password, string phNumber)
        {
            UserModel userModel = new UserModel();
            userModel.Status = (int)Common.EnumLookUp.Active;
            userModel.ContactNumber = phNumber;
            userModel.Name = userName;
            userModel.Email = email;
            userModel.CreatedOn = DateTime.UtcNow;
            userModel.Password = Cryptography.ComputeSHA256Hash(password, userModel.CreatedOn.ToString("dd-MM-yyyy hh:mm:ss tt", CultureInfo.InvariantCulture));
            userModel.PaymentInfo = ((int)PaymentInfo.PENDING).ToString();
            var user = _mapper.Map<User>(userModel);
            var userdetails = _mapper.Map<UserModel>(user);
            //var emailDataModel = new EmailDataModel()
            //{
            //    To = new List<string>() {
            //        email
            //    },
            //    Data = "<p>" + "User created successfully, use the below creadentials to login" + "</p>"
            //                 + "<p>" + "User Name" + ": " + userdetails.Name + "</P>"
            //                 + "<p>" + "Password" + ": " + userdetails.Password + " </P>",
            //    Subject = "User creation"
            //};
            //emailDataModel = await this._smtpEmailProvider.SendAsync(emailDataModel);
            try
            {
                this._userRepository.Add(user, false, true);
                await this._userRepository.SaveAsync();


            }
            catch (Exception ex)
            {

                throw;
            }

            return userdetails;
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
