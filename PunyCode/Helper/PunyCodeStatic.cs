
namespace PunyCode.Helper
{
    public static class PunyCodeStatic
    {
        public const int MaxInputStringLenght = 8000;
        public const uint MaxUint = 4294967295;

        public enum PunyCodeOperationStatus
        {
            PunycodeStatusSuccess,
            PunycodeStatusBadInput,
            PunycodeStatusBigOutput,
            PunycodeStatusOverflow
        };

        public enum PunyCodeBootstringParams
        {
            PunycodeBootstringBase = 36,
            PunycodeBootstringTmin = 1,
            PunycodeBootstringTmax = 26,
            PunycodeBootstringSkew = 38,
            PunycodeBootstringDamp = 700,
            PunycodeBootstringInitialBias = 72,
            PunycodeBootstringInitialN = 0x80,
            PunycodeBootstringDelimiter = 0x2D
        };
    }
}
