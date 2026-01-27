using AutoMapper;
using YallaKhadra.Core.Entities.GreenEntities;
using YallaKhadra.Core.Features.CleanupTasks.Queries.Responses;

namespace YallaKhadra.Core.Mapping.CleanupTasks {
    public class CleanupTaskProfile : Profile {
        public CleanupTaskProfile() {
            CreateMap<CleanupTask, CleanupTaskResponse>()
                .ForMember(dest => dest.FinalWasteTypeName,
                    opt => opt.MapFrom(src => src.FinalWasteType.ToString()))
                .ForMember(dest => dest.WorkerName,
                    opt => opt.MapFrom(src => src.Worker != null
                        ? $"{src.Worker.FirstName} {src.Worker.LastName}"
                        : null))
                .ForMember(dest => dest.Images,
                    opt => opt.MapFrom(src => src.Images));

            CreateMap<CleanupImage, CleanupImageDto>();
        }
    }
}
