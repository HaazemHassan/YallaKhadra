using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Helpers
{
    public static class PointsWasteCalculatorHelper
    {
        private static readonly Dictionary<WasteType, int> BasePoints = new()
        {
            { WasteType.Plastic, 10 },
            { WasteType.Paper, 8 },
            { WasteType.Glass, 12 },
            { WasteType.Metal, 15 },
            { WasteType.Organic, 5 },
            { WasteType.Electronic, 20 },
            { WasteType.Construction, 25 },
            { WasteType.Textile, 10 },
            { WasteType.Mixed, 3 },
            { WasteType.Unknown, 0 }
        };

        public static int CalculatePoints(WasteType type, bool isPredictionCorrect, decimal weightKg)
        {
            if (type == WasteType.Unknown || weightKg <= 0)
                return 0;

            int basePoints = BasePoints.ContainsKey(type) ? BasePoints[type] : 5;
            if (isPredictionCorrect)
            {
                basePoints = (int)(basePoints * 1.5m);
            }
            int totalPoints = (int)(basePoints * weightKg);

            return totalPoints;
        }
    }
}
