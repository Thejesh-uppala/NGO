using AutoMapper;
using NGO.Data;
using NGO.Model;

namespace NGO.Web.Infrastructure
{
    public class AutoMapperConfig : Profile
    {
        public AutoMapperConfig()
        {
            // Entity to Model
            CreateMap<User, UserModel>();
            CreateMap<UserDetail, FileModel>();
            CreateMap<UserDetail, UserDetailModel>();

            CreateMap<ChildrensDetail, ChildrensDetailsModel>();

            CreateMap<MatrimonialDataPhoto, MatrimonialPhotoDataResponse>();
            // Mapping EventData -> EventDto

            CreateMap<EventCreateDto, EventData>()
                 .ForMember(dest => dest.EventCosts, opt => opt.Ignore())

    .ForMember(dest => dest.EventPhoto, opt => opt.Ignore())
   .ForMember(dest => dest.EventDate, opt => opt.Ignore())
    .ForMember(dest => dest.RegistrationLastDate, opt => opt.Ignore())
    .ForMember(dest => dest.CreatedDate, opt => opt.Ignore()); // Ignore, as we set it in business layer

            CreateMap<EventCostDto, EventCost>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore())
            .ForMember(dest => dest.ParticipantTypeId, opt => opt.Ignore()); // Ignore Participant FTypeId since it’s not in DTO

            CreateMap<UpdateEventDto, EventData>()
          .ForMember(dest => dest.EventCosts, opt => opt.Ignore())
.ForMember(dest => dest.EventPhoto, opt => opt.Ignore()) // Ignore, as we set it in business layer
.ForMember(dest => dest.CreatedDate, opt => opt.Ignore()) // Ignore, as we set it in business layer
 .ForMember(dest => dest.EventDate, opt => opt.Ignore())
   .ForMember(dest => dest.RegistrationLastDate, opt => opt.Ignore());


            // Ignore, as we set it in business layer
            CreateMap<EventCostDto, EventCost>()
                .ForMember(dest => dest.CreatedDate, opt => opt.Ignore());

            CreateMap<EventData, EventDto>()
                .ForMember(dest => dest.EventCosts, opt => opt.MapFrom(src => src.EventCosts.ToList()));

            CreateMap<EventCost, EventCostDto>()
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.ParticipantType.Name)); // Map ParticipantType.Name to Type

            // Mapping from MatrimonialData (Entity) to MatrimonialDataResponse (Model)
            CreateMap<MatrimonialData, MatrimonialDataResponse>()
                .ForMember(dest => dest.MatrimonialDataPhotos,
                           opt => opt.MapFrom(src => src.MatrimonialDataPhotos.Select(photo => new MatrimonialPhotoDataResponse
                           {
                               Id = photo.Id,
                               PhotoType = photo.PhotoType,
                               PhotoPath = photo.PhotoPath
                           }).ToList()));
            CreateMap<MemberShipType, MemberShipTypeModel>();
            CreateMap<Payment, PaymentModel>();
            CreateMap<UserDetail, UserDetailModel>()
               .ForMember(s => s.ChildrensDetails, c => c.MapFrom(m => m.ChildrensDetails));
            // Model to Entity
            CreateMap<UserModel, User>();
            CreateMap<UserDetailModel, UserDetail>();
            CreateMap<ChildrensDetailsModel, ChildrensDetail>();
            CreateMap<FileModel, UserDetail>();
            CreateMap<PaymentModel, Payment>();
            // Mapping EventDto -> EventData
            CreateMap<EventDto, EventData>()
                .ForMember(dest => dest.EventCosts, opt => opt.MapFrom(src => src.EventCosts.ToList()));

            CreateMap<EventCostDto, EventCost>()
                            .ForMember(dest => dest.ParticipantTypeId, opt => opt.Ignore()); // Ignore since EventCostDto doesn’t have ParticipantTypeId

            CreateMap<MemberShipTypeModel, MemberShipType>();
            CreateMap<UserDetailModel, UserDetail>()
               .ForMember(s => s.ChildrensDetails, c => c.MapFrom(m => m.ChildrensDetails));
        }
    }
}
