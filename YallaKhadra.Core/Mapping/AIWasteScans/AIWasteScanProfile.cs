using AutoMapper;
using YallaKhadra.Core.Entities.GreenEntities;
using YallaKhadra.Core.Features.AIWasteScans.Queries.Responses;

namespace YallaKhadra.Core.Mapping.AIWasteScans {
    public class AIWasteScanProfile : Profile {
        public AIWasteScanProfile() {
            CreateMap<AIWasteScan, AIWasteScanResponse>()
                .ForMember(dest => dest.UserName,
                    opt => opt.MapFrom(src => src.User != null
                        ? $"{src.User.FirstName} {src.User.LastName}"
                        : null));

            CreateMap<WasteScanImage, WasteScanImageDto>();
        }
    }
}
