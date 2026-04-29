using Microsoft.EntityFrameworkCore;
using YallaKhadra.Core.Entities.GreenEntities;
using YallaKhadra.Core.Enums;
using YallaKhadra.Infrastructure.Data;

namespace YallaKhadra.API.Bases.DataSeeding
{
    public static class WasteReportSeeder
    {
        private static readonly string[] ReportImageUrls =
        [
            "https://picsum.photos/id/100/600/600", "https://picsum.photos/id/101/600/600", "https://picsum.photos/id/102/600/600",
            "https://picsum.photos/id/103/600/600", "https://picsum.photos/id/104/600/600", "https://picsum.photos/id/105/600/600",
            "https://picsum.photos/id/106/600/600", "https://picsum.photos/id/107/600/600", "https://picsum.photos/id/108/600/600",
            "https://picsum.photos/id/109/600/600", "https://picsum.photos/id/110/600/600", "https://picsum.photos/id/111/600/600",
            "https://picsum.photos/id/112/600/600", "https://picsum.photos/id/113/600/600", "https://picsum.photos/id/114/600/600",
            "https://picsum.photos/id/115/600/600", "https://picsum.photos/id/116/600/600", "https://picsum.photos/id/117/600/600",
            "https://picsum.photos/id/118/600/600", "https://picsum.photos/id/119/600/600"
        ];

        private static readonly (string Address, decimal Latitude, decimal Longitude)[] Locations =
        [
            ("123 Green Street, Cairo", 30.0444m, 31.2357m),
            ("456 Eco Lane, Giza", 30.0131m, 31.2089m),
            ("789 Sustainable Ave, Helwan", 29.8626m, 31.3331m),
            ("321 Nature Drive, 6th of October City", 30.0129m, 30.8311m),
            ("654 Environment Blvd, Zamalek", 30.7614m, 31.7669m),
            ("987 Clean Park, Nasr City", 30.0880m, 31.4183m),
            ("111 Recycle Road, Heliopolis", 30.0844m, 31.3621m),
            ("222 Green Valley, Maadi", 29.9617m, 31.2697m),
            ("333 Eco Plaza, New Cairo", 30.0100m, 31.4874m),
            ("444 Clean Quarter, Shubra", 30.1305m, 31.2542m),
            ("555 Nature Park, Dokki", 30.0391m, 31.2158m),
            ("666 Sustainable City, Agouza", 30.0301m, 31.2003m),
            ("777 Green District, Imbaba", 30.0872m, 31.1608m),
            ("888 Eco Zone, Bulaq", 30.1055m, 31.2357m),
            ("999 Clean Hub, Mohandiseen", 30.0436m, 31.1965m),
            ("101 Environment Park, Manial", 30.0177m, 31.2455m),
            ("202 Recycling Center, Giza", 30.0150m, 30.9869m),
            ("303 Green Village, 6th of October", 30.0176m, 30.8129m),
            ("404 Sustainable Boulevard, New Cairo", 29.9891m, 31.4927m),
            ("505 Clean Avenue, Helwan", 29.8545m, 31.3456m)
        ];

        private static readonly string[] ReportDescriptions =
        [
            "Large accumulation of plastic waste in residential area",
            "Mixed waste pile near public park",
            "Organic waste dumped in green space",
            "Electrical waste scattered in alley",
            "Construction debris blocking street access",
            "Textile waste at abandoned facility",
            "Paper and cardboard near commercial zone",
            "Metal scraps at industrial area",
            "Glass bottles and containers found",
            "Hazardous chemical containers discovered"
        ];

        private static readonly string[] CleanupDescriptions =
        [
            "Professional cleanup completed successfully",
            "Area sanitized and waste properly disposed",
            "Thorough cleanup with waste segregation",
            "Emergency cleanup completed",
            "Full remediation and restoration done",
            "Deep cleaning and waste management",
            "Comprehensive cleanup and area restoration",
            "Advanced cleanup with environmental assessment",
            "Complete area rehabilitation"
        ];

        public static async Task SeedAsync(AppDbContext context)
        {
            var now = DateTime.UtcNow;

            await SeedWasteReportsAsync(context, now);
        }

        private static async Task SeedWasteReportsAsync(AppDbContext context, DateTime now)
        {
            // Check if reports already exist
            var existingReportCount = await context.Reports.CountAsync();
            if (existingReportCount > 0)
                return;

            // Get all users from database
            var users = await context.Users
                .Where(u => u.Id > 0)
                .ToListAsync();

            if (users.Count == 0)
                return;

            // Get the worker user
            var workerUser = users.FirstOrDefault(u => u.Email == "worker@project.com");
            if (workerUser == null)
                return;

            var userRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == UserRole.User.ToString());
            if (userRole == null)
                return;

            var leaderboardUsers = await (
                from user in context.Users
                join userRoleMap in context.UserRoles on user.Id equals userRoleMap.UserId
                where userRoleMap.RoleId == userRole.Id
                orderby user.Id
                select user)
                .Take(15)
                .ToListAsync();

            if (leaderboardUsers.Count < 15)
                return;

            // Get existing public IDs to avoid duplicates
            var existingReportImagePublicIds = new HashSet<string>(
                await context.ReportImages
                    .Select(ri => ri.PublicId)
                    .ToListAsync(),
                StringComparer.OrdinalIgnoreCase);

            var existingCleanupImagePublicIds = new HashSet<string>(
                await context.CleanupImages
                    .Select(ci => ci.PublicId)
                    .ToListAsync(),
                StringComparer.OrdinalIgnoreCase);

            var existingCleanupTaskCount = await context.CleanupTasks.CountAsync();

            var reports = new List<WasteReport>();
            var reportMetadata = new List<(WasteReport Report, DateTime CreatedAt, int? WorkerId, bool HasCleanupTask, WasteType? FinalWasteType)>();

            var wasteTypes = new[] { WasteType.Plastic, WasteType.Paper, WasteType.Glass, WasteType.Metal, WasteType.Organic, WasteType.Electronic, WasteType.Construction, WasteType.Textile, WasteType.Mixed };
            var finalWasteTypes = new[] { WasteType.Plastic, WasteType.Paper, WasteType.Glass, WasteType.Metal, WasteType.Organic };

            var locationIndex = 0;

            // Create 25 Pending Reports
            for (int i = 0; i < 25; i++)
            {
                var user = users[i % users.Count];
                var location = Locations[locationIndex++ % Locations.Length];
                var wasteType = wasteTypes[i % wasteTypes.Length];
                var reportCreatedAt = now.AddDays(-Random.Shared.Next(1, 30));

                var report = new WasteReport
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    Address = location.Address,
                    Status = ReportStatus.Pending,
                    WasteType = wasteType,
                    CreatedAt = reportCreatedAt,
                    UserId = user.Id
                };

                reports.Add(report);
                reportMetadata.Add((report, reportCreatedAt, null, false, null));
            }

            // Create 1 InProgress Report with CleanupTask - linked to worker@project.com
            var inProgressUser = users[10 % users.Count];
            var inProgressLocation = Locations[locationIndex++ % Locations.Length];
            var inProgressWasteType = wasteTypes[10 % wasteTypes.Length];
            var inProgressReportCreatedAt = now.AddDays(-10);

            var inProgressReport = new WasteReport
            {
                Latitude = inProgressLocation.Latitude,
                Longitude = inProgressLocation.Longitude,
                Address = inProgressLocation.Address,
                Status = ReportStatus.InProgress,
                WasteType = inProgressWasteType,
                CreatedAt = inProgressReportCreatedAt,
                UserId = inProgressUser.Id
            };

            reports.Add(inProgressReport);
            reportMetadata.Add((inProgressReport, inProgressReportCreatedAt, workerUser.Id, true, null));

            // Create Done Reports for 15 users with role User (leaderboard data)
            for (int i = 0; i < leaderboardUsers.Count; i++)
            {
                var user = leaderboardUsers[i];
                var location = Locations[locationIndex++ % Locations.Length];
                var wasteType = wasteTypes[(11 + i) % wasteTypes.Length];
                var finalWasteType = finalWasteTypes[i % finalWasteTypes.Length];
                var reportCreatedAt = now.AddDays(-(30 + i));

                var report = new WasteReport
                {
                    Latitude = location.Latitude,
                    Longitude = location.Longitude,
                    Address = location.Address,
                    Status = ReportStatus.Done,
                    WasteType = wasteType,
                    CreatedAt = reportCreatedAt,
                    UserId = user.Id
                };

                reports.Add(report);
                reportMetadata.Add((report, reportCreatedAt, workerUser.Id, true, finalWasteType));
            }

            // Save all reports to get their IDs
            if (reports.Count > 0)
            {
                await context.Reports.AddRangeAsync(reports);
                await context.SaveChangesAsync();
            }

            // Now create images using the saved report IDs - filter out existing ones
            var images = new List<ReportImage>();
            var imageUrlIndex = 0;

            for (int i = 0; i < reports.Count; i++)
            {
                var reportCreatedAt = reportMetadata[i].CreatedAt;

                // Add 3 images per report
                for (int j = 0; j < 3; j++)
                {
                    var publicId = $"report-{i}-{j}";

                    // Skip if this image already exists
                    if (existingReportImagePublicIds.Contains(publicId))
                    {
                        imageUrlIndex++;
                        continue;
                    }

                    images.Add(new ReportImage
                    {
                        ReportId = reports[i].Id,
                        Url = ReportImageUrls[imageUrlIndex++ % ReportImageUrls.Length],
                        PublicId = publicId,
                        UploadedBy = 0,
                        UploadedAt = reportCreatedAt
                    });
                }
            }

            if (images.Count > 0)
            {
                await context.ReportImages.AddRangeAsync(images);
                await context.SaveChangesAsync();
            }

            // Create cleanup tasks for InProgress and Done reports - skip if already exist
            var cleanupTasks = new List<CleanupTask>();

            for (int i = 0; i < reportMetadata.Count; i++)
            {
                var metadata = reportMetadata[i];

                if (!metadata.HasCleanupTask || metadata.WorkerId == null)
                    continue;

                // Check if cleanup task already exists for this report
                var existingCleanupTask = await context.CleanupTasks
                    .FirstOrDefaultAsync(ct => ct.ReportId == reports[i].Id);

                if (existingCleanupTask != null)
                    continue;

                var reportCreatedAt = metadata.CreatedAt;
                var assignedAt = reportCreatedAt.AddDays(1);
                var completedAt = assignedAt.AddDays(Random.Shared.Next(1, 7));

                var cleanupTask = new CleanupTask
                {
                    AssignedAt = assignedAt,
                    CompletedAt = reports[i].Status == ReportStatus.Done ? completedAt : null,
                    FinalWasteType = metadata.FinalWasteType,
                    FinalWeightInKg = reports[i].Status == ReportStatus.Done ? Random.Shared.Next(50, 500) + (decimal)Random.Shared.NextDouble() : null,
                    WorkerId = metadata.WorkerId.Value,
                    ReportId = reports[i].Id
                };

                cleanupTasks.Add(cleanupTask);
            }

            if (cleanupTasks.Count > 0)
            {
                await context.CleanupTasks.AddRangeAsync(cleanupTasks);
                await context.SaveChangesAsync();
            }

            // Create cleanup images for completed cleanup tasks - filter out existing ones
            var cleanupImages = new List<CleanupImage>();

            for (int i = 0; i < cleanupTasks.Count; i++)
            {
                var cleanupTask = cleanupTasks[i];

                if (cleanupTask.CompletedAt == null)
                    continue;

                // Add 2 images per completed cleanup task
                for (int j = 0; j < 2; j++)
                {
                    var publicId = $"cleanup-{cleanupTask.ReportId}-{j}";

                    // Skip if this image already exists
                    if (existingCleanupImagePublicIds.Contains(publicId))
                    {
                        imageUrlIndex++;
                        continue;
                    }

                    cleanupImages.Add(new CleanupImage
                    {
                        CleanupTaskId = cleanupTask.Id,
                        Url = ReportImageUrls[imageUrlIndex++ % ReportImageUrls.Length],
                        PublicId = publicId,
                        UploadedBy = 0,
                        UploadedAt = cleanupTask.CompletedAt.Value
                    });
                }
            }

            if (cleanupImages.Count > 0)
            {
                await context.CleanupImages.AddRangeAsync(cleanupImages);
                await context.SaveChangesAsync();
            }
        }
    }
}
