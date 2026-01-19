using AutoMapper;
using YallaKhadra.Core.Entities;
using YallaKhadra.Core.Features.PointsTransactions.Queries.Responses;

namespace YallaKhadra.Core.Mapping.PointsTransactions {
    public class PointsTransactionProfile : Profile {
        public PointsTransactionProfile() {
            CreateMap<PointsTransaction, PointsTransactionResponse>()
                .ForMember(dest => dest.TransactionTypeName,
                    opt => opt.MapFrom(src => src.TransactionType.ToString()))
                .ForMember(dest => dest.PointsSourceName,
                    opt => opt.MapFrom(src => src.pointsSource.ToString()));
        }
    }
}
