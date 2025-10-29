using Microsoft.AspNetCore.Http;
using YallaKhadra.Core.Bases;
using YallaKhadra.Core.Entities;

namespace YallaKhadra.Core.Abstracts.ServicesContracts
{
    public interface IReportService
    {
        public Task<ServiceOperationResult<Report?>> AddReportAsync(Report report, IList<IFormFile>? photos);
    }
}
