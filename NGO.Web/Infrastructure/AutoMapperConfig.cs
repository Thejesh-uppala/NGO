using AutoMapper;
using NGO.Data;
using NGO.Model;

namespace NGO.Web.Infrastructure
{
    public class AutoMapperConfig:Profile
    {
        public AutoMapperConfig()
        {
            // Entity to Model
            CreateMap<User, UserModel>();
            CreateMap<UserDetail, UserDetailModel>();
            CreateMap<ChildrensDetail, ChildrensDetailsModel>();
            CreateMap<UserDetail, FileModel>();
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
            CreateMap<MemberShipTypeModel, MemberShipType>();
            CreateMap<UserDetailModel, UserDetail>()
               .ForMember(s => s.ChildrensDetails, c => c.MapFrom(m => m.ChildrensDetails));
        }
    }
}
