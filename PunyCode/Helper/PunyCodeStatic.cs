
namespace PunyCode.Helper
{
    public static class PunyCodeStatic
    {
        public const int MaxInputStringLenght = 8000;
        public const uint MaxUint = 4294967295;

        public enum OperationStatus
        {
            Success,
            BadInput,
            BigOutput,
            Overflow
        };

        public enum BootstringParams
        {
            Base = 36,
            Tmin = 1,
            Tmax = 26,
            Skew = 38,
            Damp = 700,
            InitialBias = 72,
            InitialN = 0x80,
            Delimiter = 0x2D
        };
    }
}
