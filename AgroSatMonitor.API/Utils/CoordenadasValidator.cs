using AgroSatMonitor.API.Exceptions;

namespace AgroSatMonitor.API.Utils
{
    public static class CoordenadasValidator
    {
        public static void Validar(double latitude, double longitude)
        {
            if (latitude < -90 || latitude > 90 || longitude < -180 || longitude > 180)
            {
                throw new CoordenadasInvalidasException(latitude, longitude);
            }
        }

        public static bool EhValida(double latitude, double longitude)
        {
            return latitude >= -90 && latitude <= 90 && longitude >= -180 && longitude <= 180;
        }

        public static string FormatarCoordenadas(double latitude, double longitude)
        {
            string latDir = latitude >= 0 ? "N" : "S";
            string lonDir = longitude >= 0 ? "L" : "O";
            return $"{Math.Abs(latitude):F6}°{latDir}, {Math.Abs(longitude):F6}°{lonDir}";
        }
    }
}
