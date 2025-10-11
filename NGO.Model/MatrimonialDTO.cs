using Microsoft.AspNetCore.Http;

namespace NGO.Model
{
    public class MatrimonialDTO
    {
        public int OrgId { get; set; }

        public int AssociatedWithUserId { get; set; }

        public string RegisteringFor { get; set; }
        public string ProfileFor { get; set; }

        // Auth Primary Person Details
        public string AuthPrimaryPersonFName { get; set; }
        public string? AuthPrimaryPersonLName { get; set; }
        public string AuthPrimaryPersonEmail { get; set; }
        public string AuthPrimaryPersonPhoneNo { get; set; }
        public bool AuthPrimaryContactCanBeShared { get; set; }
        public string? AuthPrimaryContactPref { get; set; }

        // Bride/Groom Details
        public string BrideOrGroomFName { get; set; }
        public string? BrideOrGroomLName { get; set; }
        public string BrideOrGroomEmail { get; set; }
        public string BrideGroomDOB { get; set; }
        public string BrideGroomTimeOfBirth { get; set; }
        public string BrideGroomPlaceOfBirth { get; set; }
        public bool BrideGroomAccessToCommunication { get; set; }
        public string BrideGroomCurrentLocation { get; set; }
        public string BrideGroomCurrentCity { get; set; }
        public string BrideGroomCurrentState { get; set; }
        public string BrideGroomCurrentCountry { get; set; }
        public decimal? BrideGroomHeight { get; set; }
        public string BrideGroomGothra { get; set; }
        public string BrideGroomPravara { get; set; }
        public string BrideGroomNakshatra { get; set; }
        public string BrideGroomRashi { get; set; }
        public string BrideGroomEducation { get; set; }
        public string BrideGroomEducationDegree { get; set; }
        public string BrideGroomUniversity { get; set; }
        public string BrideGroomEmploymentInfo { get; set; }
        public string? BrideGroomCanRelocate { get; set; }
        public string? BrideGroomPrevMarried { get; set; }
        public string BrideGroomNativePlace { get; set; }
        public string? BrideGroomLegalStatus { get; set; }
        public string BrideGroomGrownUpCountryAndPlace { get; set; }
        public string? BrideGroomDiet { get; set; }

        // Partner Preferences
        public string PartnersPref { get; set; }
        public string? PartnersLegalStayPref { get; set; }

        // References
        public string OrgReFName { get; set; }
        public string OrgRefPhoneno { get; set; }

        // Family Details
        public string FatherFName { get; set; }
        public string? FatherEmail { get; set; }
        public string? FatherPhoneNo { get; set; }
        public string? FatherNativePlace { get; set; }
        public string? FatherOccupation { get; set; }

        public string MotherFName { get; set; }
        public string? MotherEmail { get; set; }
        public string? MotherPhoneNo { get; set; }
        public string? MotherPlaceIndia { get; set; }
        public string? MotherOccupation { get; set; }
        public string? ParentLocation { get; set; }
        public string SiblingsInformation { get; set; }
        public string BrideOrGroomPhoneNumber { get; set; }
        public string Gender { get; set; }


        // Additional Information
        public string? AdditionalInformation { get; set; }
        public string? ContactPref { get; set; }

        // Photos
        public List<MatrimonialPhotoDto> MatrimonialDataPhotos { get; set; }
    }



    public class MatrimonialPhotoDto
    {
        public string PhotoType { get; set; } // e.g., Primary, Casual, Business, etc.

        public IFormFile PhotoFile { get; set; }
    }



}

