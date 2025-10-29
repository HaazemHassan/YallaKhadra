using AutoMapper;

namespace YallaKhadra.Core.Mapping.Reports
{
    public partial class ReportProfile : Profile
    {
        public ReportProfile()
        {
            AddReportCommandMapping();
        }
    }
}
