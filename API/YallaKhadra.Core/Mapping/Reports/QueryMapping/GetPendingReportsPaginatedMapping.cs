using YallaKhadra.Core.Entities;
using YallaKhadra.Core.Features.Reports.Queries.Responses;

namespace YallaKhadra.Core.Mapping.Reports
{
    public partial class ReportProfile
    {
        public void GetPendingReportsPaginatedMapping()
        {
            CreateMap<Photo, ReportPhotoDto>();

            CreateMap<Report, GetPendingReportsPaginatedResponse>()
                .ForMember(dest => dest.UserName, 
                    opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.UserFullName,
                    opt => opt.MapFrom(src => src.User.FirstName + " " + src.User.LastName))
                .ForMember(dest => dest.Photos,
                    opt => opt.MapFrom(src => src.Photos));
        }
    }
}
