using AutoMapper;
using YallaKhadra.Core.Entities.GreenEntities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Core.Features.WasteReports.Commands.RequestModels;
using YallaKhadra.Core.Features.WasteReports.Queries.Responses;

namespace YallaKhadra.Core.Mapping.WasteReports {
    public class WasteReportProfile : Profile {
        public WasteReportProfile() {
            CreateMap<CreateWasteReportCommand, WasteReport>()
                .ForMember(dest => dest.WasteType,
                    opt => opt.MapFrom(src => (WasteType)src.WasteType))
                .ForMember(dest => dest.Status,
                    opt => opt.MapFrom(src => ReportStatus.Pending))
                .ForMember(dest => dest.CreatedAt,
                    opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Images,
                    opt => opt.Ignore())
                .ForMember(dest => dest.User,
                    opt => opt.Ignore())
                .ForMember(dest => dest.Id,
                    opt => opt.Ignore());

            CreateMap<WasteReport, WasteReportResponse>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.User != null
                        ? $"{src.User.FirstName} {src.User.LastName}"
                        : null))
                .ForMember(dest => dest.Images,
                    opt => opt.MapFrom(src => src.Images));


            CreateMap<WasteReport, WasteReportBriefDto>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.User != null
                        ? $"{src.User.FirstName} {src.User.LastName}"
                        : null))
                .ForMember(dest => dest.StatusName,
                    opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.WasteTypeName,
                    opt => opt.MapFrom(src => src.WasteType.ToString()))
                .ForMember(dest => dest.FirstImage,
                    opt => opt.MapFrom(src => src.Images.OrderBy(i => i.Id).FirstOrDefault()));

            CreateMap<ReportImage, ReportImageDto>();
        }
    }
}
