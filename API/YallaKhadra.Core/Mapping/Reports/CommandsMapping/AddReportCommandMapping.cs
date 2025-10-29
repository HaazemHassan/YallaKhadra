
using YallaKhadra.Core.Entities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.Reports.Commands.RequestsModels;

namespace YallaKhadra.Core.Mapping.Reports
{
    public partial class ReportProfile
    {
        public void AddReportCommandMapping()
        {
            CreateMap<AddReportCommand, Report>()
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ReportStatus.Pending))
            .ForMember(dest => dest.PointsAwarded, opt => opt.Ignore())
            .ForMember(dest => dest.ReviewedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Notes, opt => opt.Ignore())
            .ForMember(dest => dest.ReviewedById, opt => opt.Ignore())
            .ForMember(dest => dest.ReviewedBy, opt => opt.Ignore())
            .ForMember(dest => dest.UserId, opt => opt.Ignore())
            .ForMember(dest => dest.User, opt => opt.Ignore())
            .ForMember(dest => dest.Photos, opt => opt.Ignore());
        }
    }
}
