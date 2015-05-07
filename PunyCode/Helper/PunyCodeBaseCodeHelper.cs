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

        /// <summary>
        /// encode_basic(bcp,flag) forces a basic code point to lowercase
        /// if flag is zero, uppercase if flag is nonzero, and returns
        /// the resulting code point.  The code point is unchanged if it
        /// is caseless.  The behavior is undefined if bcp is not a basic
        /// code point.
        /// </summary>
        /// <param name="aBcp">input character</param>
        /// <param name="aFlag">a flag</param>
        /// <returns>encoded character</returns>
        static char EncodeBasic(uint aBcp, bool aFlag)
        {
            aBcp -= (uint)(((aBcp - 97 < 26) ? 1 : 0) << 5);
            return (char)(aBcp + (((!aFlag && (aBcp - 65 < 26)) ? 1 : 0) << 5));
        }

    }
}
