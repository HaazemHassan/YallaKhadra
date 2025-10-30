using YallaKhadra.Core.Entities;
using YallaKhadra.Core.Features.Reports.Queries.Responses;

namespace YallaKhadra.Core.Mapping.Reports
{
    public partial class ReportProfile
    {
        public void GetMyReportsPaginatedMapping()
        {
            CreateMap<Report, GetMyReportsPaginatedResponse>()
                .ForMember(dest => dest.ReviewedByName,
                    opt => opt.MapFrom(src => src.ReviewedBy != null 
                        ? src.ReviewedBy.FirstName + " " + src.ReviewedBy.LastName 
                        : null))
                .ForMember(dest => dest.Photos,
                    opt => opt.MapFrom(src => src.Photos));
        }
    }
}
