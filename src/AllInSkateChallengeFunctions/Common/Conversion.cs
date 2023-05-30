namespace AllInSkateChallenge.Features.Common
{
    public static class Conversion
    {
        public static decimal RatioMilesToFeet => 5280M;

        public static decimal RatioMilesToKilometres => 1.609344M;

        public static decimal RatioMilesToMetres => RatioMilesToKilometres * RatioKilometreToMetres;

        public static decimal RatioMetresToFeet => 3.2808398951M;

        public static decimal RatioKilometreToMetres => 1000M;

        public static decimal RatioKilometresToFeet => RatioMetresToFeet * RatioKilometreToMetres;

        public static decimal RatioSecondsToHour => 3600M;

        public static decimal FeetToMetres(decimal distance) => distance / RatioMetresToFeet;

        public static decimal FeetToMiles(decimal distance) => distance / RatioMilesToFeet;

        public static decimal FeetToKilometres(decimal distance) => distance / RatioKilometresToFeet;

        public static decimal KilometresToMetres(decimal distance) => distance * RatioKilometreToMetres;

        public static decimal KilometresToMiles(decimal distance) => distance / RatioMilesToKilometres;

        public static decimal KilometresToFeet(decimal distance) => distance * RatioKilometresToFeet;

        public static decimal MetresToKilometres(decimal distance) => distance / RatioKilometreToMetres;

        public static decimal MetresToMiles(decimal distance) => distance / RatioMilesToMetres;

        public static decimal MetresToFeet(decimal distance) => distance * RatioMetresToFeet;

        public static decimal MilesToMetres(decimal distance) => distance * RatioMilesToMetres;

        public static decimal MilesToFeet(decimal distance) => distance * RatioMilesToFeet;

        public static decimal MilesToKilometres(decimal distance) => distance * RatioMilesToKilometres;

        public static decimal MetresPerSecondToMilesPerHour(decimal velocity) => MetresToMiles(velocity) * RatioSecondsToHour;

        public static decimal KilometresPerHourToMilesPerHour(decimal velocity) => KilometresToMiles(velocity);

        public static decimal MilesPerHourToKilometresPerHour(decimal velocity) => MilesToKilometres(velocity);
    }
}
