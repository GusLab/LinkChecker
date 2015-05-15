namespace PunyCode.Helper
{
    public sealed class PunyCodeBaseCodeHelper
    {        
        public bool IsCharacterBasic(uint inputCharacter)
        {
            return (inputCharacter < (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringInitialN);
        }

        public bool IsCharacterDelimiter(uint inputCharacter)
        {
            return (inputCharacter == (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringDelimiter);
        }

        public uint GetAlphaNumericValueInBasicCode(uint inputCharacter)
        {
            return (inputCharacter - 48 < 10 ? inputCharacter - 22 : inputCharacter - 65 < 26 ? inputCharacter - 65 :
                    inputCharacter - 97 < 26 ? inputCharacter - 97 : (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringBase);
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

        public char GetCharToUpperOrLowerCase(uint inputCharacter, bool isUpperCase)
        {
            inputCharacter -= (uint)(((inputCharacter - 97 < 26) ? 1 : 0) << 5);
            return (char)(inputCharacter + (((!isUpperCase && (inputCharacter - 65 < 26)) ? 1 : 0) << 5));
        }

        /// <summary>
        /// Bias adaptation function
        /// </summary>
        /// <param name="aDelta"></param>
        /// <param name="aNumpoints"></param>
        /// <param name="aFirsttime"></param>
        public uint Adapt(uint aDelta, uint aNumpoints, bool aFirsttime)
        {
            uint k;

            aDelta = aFirsttime ? aDelta / (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringDamp : aDelta >> 1;
            /* delta >> 1 is a faster way of doing delta / 2 */
            aDelta += aDelta / aNumpoints;

            for (k = 0; aDelta > (((uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringBase - (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringTmin) * (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringTmax) / 2; k += (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringBase)
            {
                aDelta /= (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringBase - (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringTmin;
            }

            return k + ((uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringBase - (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringTmin + 1) * aDelta / (aDelta + (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringSkew);
        }  

    }
}
