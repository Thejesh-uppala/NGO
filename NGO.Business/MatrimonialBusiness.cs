using System.Threading.Channels;
using AutoMapper;
using NGO.Common;
using NGO.Data;
using NGO.Model;
using NGO.Repository;
using NGO.Repository.Contracts;

namespace NGO.Business
{
    public class MatrimonialBusiness
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
        private readonly IMatrimonialDataRepository _matrimonialDataRepository;
        private readonly IMatrimonialPhotoDataRepository _matrimonialPhotoDataRepository;
        private readonly IPaymentRepository _paymentRepository;
        private readonly SMTPEmailProvider _smtpEmailProvider;
        private readonly PasswordHandler _passwordHandler;
        private readonly UserBusiness _userBusiness;
        private readonly CommonBusiness _commonBusiness;
        private readonly Channel<EmailDataModel> _emailChannel;


        public MatrimonialBusiness(Channel<EmailDataModel> emailChannel
        , CommonBusiness commonBusiness, IUserRepository userRepository, AppSettings appsettings, IMatrimonialDataRepository matrimonialDataRepository, IMatrimonialPhotoDataRepository matrimonialPhotoDataRepository, IMapper mapper, IUserRolesRepository userRolesRepository, IOrgRepository organizationRepository, IUserDetailRepository userDetailRepository, IMembershipUserTypesRepository memberShipTypesRepository, IChildrensRepository childrensRepository, IRoleRepository roleRepository, IPaymentRepository paymentRepository, SMTPEmailProvider sMTPEmailProvider, PasswordHandler passwordHandler, UserBusiness userBusiness, IUserAddressRepository userAddressRepository)
        {
            _commonBusiness = commonBusiness;
            _userRepository = userRepository;
            _appsettings = appsettings;
            _matrimonialDataRepository = matrimonialDataRepository;
            _matrimonialPhotoDataRepository = matrimonialPhotoDataRepository;
            _mapper = mapper;
            _userRolesRepository = userRolesRepository;
            _organizationRepository = organizationRepository;
            _userDetailRepository = userDetailRepository;
            _memberShipTypesRepository = memberShipTypesRepository;
            _childrensRepository = childrensRepository;
            _roleRepository = roleRepository;
            _paymentRepository = paymentRepository;
            _smtpEmailProvider = sMTPEmailProvider;
            _emailChannel = emailChannel;
            _passwordHandler = passwordHandler;
            _userBusiness = userBusiness;
            _userAddressRepository = userAddressRepository;


        }


        public async Task<MatrimonialDataResponse> GetMatrimonialDataByIdAsync(int id, int orgId)
        {

            var data = await _matrimonialDataRepository.GetMatrimonialDataByIdAsync(id, orgId);
            // Example usage in your service
            var result = _mapper.Map<MatrimonialDataResponse>(data);
            if (data.ApprovedBy.HasValue)
            {
                var approver = (await _userDetailRepository.GetByAsync(x => x.UserId == data.ApprovedBy)).FirstOrDefault();


                if (approver != null)
                {
                    result.ApprovedBy = $"{approver.FirstName} {approver.LastName}".Trim();
                }
            }
            return result;

        }

        public async Task<IEnumerable<MatrimonialDataResponse>> GetAllMatrimonialDataAsync(int orgId)
        {
            var data = await _matrimonialDataRepository.GetAllMatrimonialDataAsync(orgId);
            return _mapper.Map<IEnumerable<MatrimonialDataResponse>>(data);
        }

        public async Task<(IEnumerable<MatrimonialDataResponse> Data, int TotalRecords)> GetFilteredMatrimonialDataAsync(
        int orgId,
        string gender,
        int? minAge,
        int? maxAge,
        string? minHeight,
        string? maxHeight,
        string state,
        string country,
        string education,
        string profession,
        string prevMarried,
            string searchValue,
        int page,
        int pageSize)
        {
            var (data, totalRecords) = await _matrimonialDataRepository.GetFilteredMatrimonialDataAsync(
                orgId, gender, minAge, maxAge, minHeight, maxHeight, state, country, education, profession, prevMarried, searchValue, page, pageSize);

            return (_mapper.Map<IEnumerable<MatrimonialDataResponse>>(data), totalRecords);
        }

        public async Task<(IEnumerable<MatrimonialDataResponse> Data, int TotalRecords)> GetAllApprovedMatrimonialData(int orgId,
            int page,
            int pageSize)
        {
            var (data, totalRecords) = await _matrimonialDataRepository.GetAllApprovedMatrimonialDataAsync(orgId, page, pageSize);
            var mapped_result = _mapper.Map<IEnumerable<MatrimonialDataResponse>>(data);
            return (mapped_result, totalRecords);
        }

        public async Task<int> CreateMatrimonialDataAsync(MatrimonialDTO data)
        {

            var matrimonialEntity = new MatrimonialData
            {
                OrgId = data.OrgId,
                RecordStatus = "active",
                AssociatedWithUserId = data.AssociatedWithUserId,
                RegisteringFor = data.RegisteringFor,
                ProfileFor = data.ProfileFor,
                AuthPrimaryPersonFName = data.AuthPrimaryPersonFName,
                AuthPrimaryPersonLName = data.AuthPrimaryPersonLName,
                AuthPrimaryPersonEmail = data.AuthPrimaryPersonEmail,
                AuthPrimaryPersonPhoneNo = data.AuthPrimaryPersonPhoneNo,
                AuthPrimaryContactCanBeShared = data.AuthPrimaryContactCanBeShared,
                AuthPrimaryContactPref = data.AuthPrimaryContactPref,
                BrideOrGroomFName = data.BrideOrGroomFName,
                BrideOrGroomLName = data.BrideOrGroomLName,
                BrideOrGroomEmail = data.BrideOrGroomEmail,
                BrideGroomDOB = data.BrideGroomDOB,
                BrideGroomTimeOfBirth = data.BrideGroomTimeOfBirth,
                BrideGroomPlaceOfBirth = data.BrideGroomPlaceOfBirth,
                BrideGroomAccessToCommunication = data.BrideGroomAccessToCommunication,
                BrideGroomCurrentLocation = data.BrideGroomCurrentLocation,
                BrideGroomCurrentCity = data.BrideGroomCurrentCity,
                BrideGroomCurrentState = data.BrideGroomCurrentState,
                BrideGroomCurrentCountry = data.BrideGroomCurrentCountry,
                BrideGroomHeight = data.BrideGroomHeight,
                BrideGroomGothra = data.BrideGroomGothra,
                BrideGroomPravara = data.BrideGroomPravara,
                BrideGroomNakshatra = data.BrideGroomNakshatra,
                BrideGroomRashi = data.BrideGroomRashi,
                BrideGroomEducation = data.BrideGroomEducation,
                BrideGroomEducationDegree = data.BrideGroomEducationDegree,
                BrideGroomUniversity = data.BrideGroomUniversity,
                BrideGroomEmploymentInfo = data.BrideGroomEmploymentInfo,
                BrideGroomCanRealocate = data.BrideGroomCanRelocate,
                BrideGroomPrevMarried = data.BrideGroomPrevMarried,
                BrideGroomNativePlace = data.BrideGroomNativePlace,
                BrideGroomLegalStatus = data.BrideGroomLegalStatus,
                BrideGroomGrownUpCountryandPlace = data.BrideGroomGrownUpCountryAndPlace,
                BrideGroomDiet = data.BrideGroomDiet,
                PartnersPref = data.PartnersPref,
                PartnersLegalStayPref = data.PartnersLegalStayPref,
                OrgReFName = data.OrgReFName,
                OrgRefPhoneno = data.OrgRefPhoneno,
                FatherFName = data.FatherFName,
                FatherEmail = data.FatherEmail,
                FatherPhoneNo = data.FatherPhoneNo,
                FatherNativePlace = data.FatherNativePlace,
                FatherOccupation = data.FatherOccupation,
                MotherFName = data.MotherFName,
                MotherEmail = data.MotherEmail,
                MotherPhoneNo = data.MotherPhoneNo,
                MotherPlaceIndia = data.MotherPlaceIndia,
                MotherOccupation = data.MotherOccupation,
                ParentLocation = data.ParentLocation,
                BrideOrGroomPhoneNumber = data.BrideOrGroomPhoneNumber,
                Gender = data.Gender,
                SiblingsInformation = data.SiblingsInformation,
                AdditionalInformation = data.AdditionalInformation,
                ContactPref = data.ContactPref,
                Status = "Pending",
                ExpiringOn = null,
                ConsentsAgree = true,
                ApprovedBy = null,
                ApprovedDate = null,
                CreatedDate = DateTime.UtcNow

            };
            _matrimonialDataRepository.Add(matrimonialEntity);
            await _matrimonialDataRepository.SaveAsync();


            var matrimonialPhotoEntities = new List<MatrimonialDataPhoto>();
            foreach (var photo in data.MatrimonialDataPhotos)
            {
                var photoUrl = await _commonBusiness.UploadToCloudinaryAsync(photo.PhotoFile, data.OrgId);
                matrimonialPhotoEntities.Add(new MatrimonialDataPhoto
                {
                    Id = 0,
                    MatrimonailId = matrimonialEntity.Id,
                    PhotoType = photo.PhotoType,
                    PhotoPath = photoUrl
                });
            }

            var (brandImageUrl, brandColor, orgName, orgWelMsg, brandLogo, primaryEmail, matrimonyEmail, eventEmail, orgHeadEmail) =
             await _userBusiness.GetBrandDetailsAsync(data.OrgId);

            string emailBodyTemplate = await _commonBusiness.GetEmailTemplateFromCloudinaryAsync("user", "create-matrimony.html", data.OrgId);



            // Replace placeholders
            var emailBody = emailBodyTemplate
                .Replace("{AuthPrimaryPersonFName}", data.AuthPrimaryPersonFName)
                .Replace("{AuthPrimaryPersonLName}", data.AuthPrimaryPersonLName)
                .Replace("{AuthPrimaryPersonPhoneNo}", data.AuthPrimaryPersonPhoneNo)
                .Replace("{AuthPrimaryPersonEmail}", data.AuthPrimaryPersonEmail)
                .Replace("{RegisteringFor}", data.RegisteringFor)
                .Replace("{ProfileFor}", data.ProfileFor)
                .Replace("{BrideOrGroomFName}", data.BrideOrGroomFName)
                .Replace("{BrideOrGroomLName}", data.BrideOrGroomLName)
                .Replace("{BrideGroomDOB}", data.BrideGroomDOB ?? "N/A")
                .Replace("{BrideGroomTimeOfBirth}", data.BrideGroomTimeOfBirth)
                .Replace("{BrideGroomPlaceOfBirth}", data.BrideGroomPlaceOfBirth)
                .Replace("{BrideGroomCurrentLocation}", data.BrideGroomCurrentLocation)
                .Replace("{BrideGroomCurrentState}", data.BrideGroomCurrentState)
                .Replace("{BrideOrGroomEmail}", data.BrideOrGroomEmail)
                .Replace("{BrideOrGroomPhoneNumber}", data.BrideOrGroomPhoneNumber)
                .Replace("{BrideGroomHeight}", data.BrideGroomHeight.ToString())
                .Replace("{BrideGroomGothra}", data.BrideGroomGothra)
                .Replace("{BrideGroomPravara}", data.BrideGroomPravara)
                .Replace("{BrideGroomNakshatra}", data.BrideGroomNakshatra)
                .Replace("{BrideGroomRashi}", data.BrideGroomRashi)
                .Replace("{BrideGroomEducation}", data.BrideGroomEducation)
                .Replace("{BrideGroomUniversity}", data.BrideGroomUniversity)
                .Replace("{BrideGroomEmploymentInfo}", data.BrideGroomEmploymentInfo)
                .Replace("{BrideGroomCanRealocate}", data.BrideGroomCanRelocate?.ToString() ?? "N/A")
                .Replace("{FatherFName}", data.FatherFName)
                .Replace("{FatherOccupation}", data.FatherOccupation)
                .Replace("{MotherFName}", data.MotherFName)
                .Replace("{MotherOccupation}", data.MotherOccupation)
                .Replace("{supportEmail}", primaryEmail)
                .Replace("{OrgName}", orgName ?? "N/A");

            var toList = new List<string>();
            if (!string.IsNullOrWhiteSpace(data.AuthPrimaryPersonEmail))
                toList.Add(data.AuthPrimaryPersonEmail);
            if (!string.IsNullOrWhiteSpace(data.BrideOrGroomEmail))
                toList.Add(data.BrideOrGroomEmail);


            // Send email
            var emailModel = new EmailDataModel
            {
                To = toList,
                Subject = "Matrimony Registration Successful",
                Data = emailBody,
                OrgId = data.OrgId
            };

            await _emailChannel.Writer.WriteAsync(emailModel);



            string adminEmailTemplate = await _commonBusiness.GetEmailTemplateFromCloudinaryAsync("admin", "matrimony-profile-creation-info.html", data.OrgId);




            var adminEmailBody = adminEmailTemplate
                .Replace("{AuthPrimaryPersonFName}", data.AuthPrimaryPersonFName)
                .Replace("{AuthPrimaryPersonLName}", data.AuthPrimaryPersonLName)
                .Replace("{AuthPrimaryPersonPhoneNo}", data.AuthPrimaryPersonPhoneNo)
                .Replace("{AuthPrimaryPersonEmail}", data.AuthPrimaryPersonEmail)
                .Replace("{RegisteringFor}", data.RegisteringFor)
                .Replace("{ProfileFor}", data.ProfileFor)
                .Replace("{BrideOrGroomFName}", data.BrideOrGroomFName)
                .Replace("{BrideOrGroomLName}", data.BrideOrGroomLName)
                .Replace("{BrideGroomDOB}", data.BrideGroomDOB ?? "N/A")
                .Replace("{BrideGroomTimeOfBirth}", data.BrideGroomTimeOfBirth)
                .Replace("{BrideGroomPlaceOfBirth}", data.BrideGroomPlaceOfBirth)
                .Replace("{BrideGroomCurrentLocation}", data.BrideGroomCurrentLocation)
                .Replace("{BrideGroomCurrentState}", data.BrideGroomCurrentState)
                .Replace("{BrideOrGroomEmail}", data.BrideOrGroomEmail)
                .Replace("{BrideOrGroomPhoneNumber}", data.BrideOrGroomPhoneNumber)
                .Replace("{BrideGroomHeight}", data.BrideGroomHeight.ToString())
                .Replace("{BrideGroomGothra}", data.BrideGroomGothra)
                .Replace("{BrideGroomPravara}", data.BrideGroomPravara)
                .Replace("{BrideGroomNakshatra}", data.BrideGroomNakshatra)
                .Replace("{BrideGroomRashi}", data.BrideGroomRashi)
                .Replace("{BrideGroomEducation}", data.BrideGroomEducation)
                .Replace("{BrideGroomUniversity}", data.BrideGroomUniversity)
                .Replace("{BrideGroomEmploymentInfo}", data.BrideGroomEmploymentInfo)
                .Replace("{BrideGroomCanRealocate}", data.BrideGroomCanRelocate?.ToString() ?? "N/A")
                .Replace("{FatherFName}", data.FatherFName)
                .Replace("{FatherOccupation}", data.FatherOccupation)
                .Replace("{MotherFName}", data.MotherFName)
                .Replace("{MotherOccupation}", data.MotherOccupation)

                .Replace("{BrideGroomNativePlace}", data.BrideGroomNativePlace)
                .Replace("{PartnersPref}", data.PartnersPref)
                .Replace("{CreatedDate}", DateTime.UtcNow.ToString("dd MMM yyyy"));


            var emailModelToAdmin = new EmailDataModel

            {

                To = new List<string> { primaryEmail },
                Subject = "New Matrimony Registration",
                Data = adminEmailBody,
                OrgId = data.OrgId
            };

            await _emailChannel.Writer.WriteAsync(emailModelToAdmin);
            return await _matrimonialPhotoDataRepository.BulkAddMatrimonialDataAsync(matrimonialPhotoEntities);

        }

        public async Task<bool> UpdateProfileStatusAsync(int orgId, List<int> userIds, int actionUserId, bool isApproved)
        {
            var profiles = await _matrimonialDataRepository.GetProfilesByIdsAsync(orgId, userIds);
            if (profiles == null || profiles.Count == 0)
                return false;

            var (brandImageUrl, brandColor, orgName, orgWelMsg, brandLogo, primaryEmail, matrimonyEmail, eventEmail, orgHeadEmail) =
       await _userBusiness.GetBrandDetailsAsync(orgId);

            string status = isApproved ? "Approved" : "Rejected";
            string emailTemplate = await _commonBusiness.GetEmailTemplateFromCloudinaryAsync("user", "matrimony-status-notify.html", orgId);

            foreach (var profile in profiles)
            {

                profile.Status = status;
                profile.ApprovedBy = actionUserId;
                profile.ApprovedDate = DateTime.UtcNow;
                _matrimonialDataRepository.Update(profile);

                // Prepare email variables
                string statusColor = isApproved ? "#4CAF50" : "#DC3545";
                string textColor = isApproved ? "#155724" : "#721c24";
                string bgColor = isApproved ? "#D4EDDA" : "#F8D7DA";
                string profileStatus = isApproved ? "Approved ✅" : "Rejected ❌";
                string statusMessage = isApproved
                    ? "🎉 Congratulations! The profile is now live on our platform."
                    : "Unfortunately, the profile has been rejected.";

                string profileCreatedDate = profile.CreatedDate.ToString("dd MMM yyyy");

                // Construct email body
                string emailBody = emailTemplate
                    .Replace("{OrgName}", orgName)
                    .Replace("{UserFullName}", (profile?.AuthPrimaryPersonFName + profile?.AuthPrimaryPersonLName) ?? "NA")
                    .Replace("{BrideOrGroomFName}", profile?.BrideOrGroomFName)
                    .Replace("{BrideOrGroomLName}", profile?.BrideOrGroomLName)
                    .Replace("{ProfileCreatedDate}", profileCreatedDate)
                    .Replace("{ProfileStatus}", profileStatus)
                    .Replace("{StatusMessage}", statusMessage)
                    .Replace("{StatusColor}", statusColor)
                    .Replace("{TextColor}", textColor)
                    .Replace("{BgColor}", bgColor);

                // Prepare and send email
                var emailModel = new EmailDataModel
                {
                    To = new List<string> { profile?.AuthPrimaryPersonEmail, profile?.BrideOrGroomEmail },
                    Subject = $"Matrimony Profile {status}",
                    Data = emailBody,
                    OrgId = orgId
                };

                await _emailChannel.Writer.WriteAsync(emailModel);
            }

            await _matrimonialDataRepository.SaveAsync();
            return true;
        }








    }
}
