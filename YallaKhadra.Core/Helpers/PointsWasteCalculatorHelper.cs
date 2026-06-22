using YallaKhadra.Core.Enums;

namespace YallaKhadra.Core.Helpers
{
    public static class PointsWasteCalculatorHelper
    {
        private static readonly Dictionary<WasteType, int> BasePoints = new()
        {
            { WasteType.Battery, 25 },
            { WasteType.Brown_Glass, 12 },
            { WasteType.Clothes, 8 },
            { WasteType.Bilogical, 6 },
            { WasteType.Green_Glass, 12 },
            { WasteType.Paper, 8 },
            { WasteType.Trash, 0 },
            { WasteType.Shoes, 6 },
            { WasteType.White_Glass, 12 },
            { WasteType.Metal, 15 },
            { WasteType.Cardboard, 7 },
            { WasteType.Plastic, 10 }
        };

        public static int CalculatePoints(WasteType type, bool isPredictionCorrect, decimal weightKg)
        {
            if (weightKg <= 0)
            {
                return 0;
            }

            var basePoints = BasePoints.TryGetValue(type, out var points) ? points : 5;
            var predictionMultiplier = isPredictionCorrect ? 1.5m : 1m;
            var totalPoints = basePoints * weightKg * predictionMultiplier;

            return (int)Math.Round(totalPoints, MidpointRounding.AwayFromZero);
        }
    }
}
