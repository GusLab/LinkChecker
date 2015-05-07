namespace PunyCode.Helper
{
    public sealed class PunyCodeBaseCodeHelper
    {

        const int MaxInputStringLenght = 8000;
        const uint MaxUint = 4294967295;

        private enum PunyCodeOperationStatus
        {
            PunycodeStatusSuccess,
            PunycodeStatusBadInput,
            PunycodeStatusBigOutput,
            PunycodeStatusOverflow
        };

        private enum PunyCodeBootstringParams
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

        public bool IsCharacterBasic(uint inputCharacter)
        {
            return (inputCharacter < (uint)PunyCodeBootstringParams.PunycodeBootstringInitialN);
        }

        public bool IsCharacterDelimiter(uint inputCharacter)
        {
            return (inputCharacter == (uint)PunyCodeBootstringParams.PunycodeBootstringDelimiter);
        }

        public uint GetAlphaNumericValueInBasicCode(uint inputCharacter)
        {
            return (inputCharacter - 48 < 10 ? inputCharacter - 22 : inputCharacter - 65 < 26 ? inputCharacter - 65 :
                    inputCharacter - 97 < 26 ? inputCharacter - 97 : (uint)PunyCodeBootstringParams.PunycodeBootstringBase);
        }

        public char GetEncodedBasicCodeToAscii(uint inputCharacter, bool isLowerCase)
        {
            if (inputCharacter > 35) return (isLowerCase != true ? 'Z' : 'z');
            if (inputCharacter >= 26) isLowerCase = true;
            return (char)(inputCharacter + 22 + 75 * (inputCharacter < 26 ? 1 : 0) - ((isLowerCase != true ? 1 : 0) << 5));
            /*  0..25 map to ASCII a..z or A..Z */
            /* 26..35 map to ASCII 0..9         */
        }

        public bool IsUpperCaseOrNumber(uint inputCharacter)
        {
            return (inputCharacter <= 90);
        }

    }
}
