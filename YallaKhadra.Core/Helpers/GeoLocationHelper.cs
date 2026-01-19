namespace YallaKhadra.Core.Helpers {
    public static class GeoLocationHelper {
        private const double EarthRadiusKm = 6371.0; // Earth's radius in kilometers
        public static double CalculateDistance(decimal lat1, decimal lon1, decimal lat2, decimal lon2) {
            var dLat = DegreesToRadians((double)(lat2 - lat1));
            var dLon = DegreesToRadians((double)(lon2 - lon1));

            var lat1Rad = DegreesToRadians((double)lat1);
            var lat2Rad = DegreesToRadians((double)lat2);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2) * Math.Cos(lat1Rad) * Math.Cos(lat2Rad);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return EarthRadiusKm * c;
        }

        private static double DegreesToRadians(double degrees) {
            return degrees * Math.PI / 180.0;
        }
    }
}
