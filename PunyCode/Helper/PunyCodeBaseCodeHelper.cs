using System;
using System.Collections.Generic;
using System.Linq;

namespace PunyCode.Helper
{
    public sealed class PunyCodeBaseCodeHelper : IDisposable
    {        
        public bool IsCharacterBasic(uint inputCharacter)
        {
            return (inputCharacter < (uint)PunyCodeStatic.BootstringParams.InitialN);
        }

        public bool IsCharacterDelimiter(uint inputCharacter)
        {
            return (inputCharacter == (uint)PunyCodeStatic.BootstringParams.Delimiter);
        }

        public uint GetAlphaNumericValueInBasicCode(uint inputCharacter)
        {
            return (inputCharacter - 48 < 10 ? inputCharacter - 22 : inputCharacter - 65 < 26 ? inputCharacter - 65 :
                    inputCharacter - 97 < 26 ? inputCharacter - 97 : (uint)PunyCodeStatic.BootstringParams.Base);
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

        public uint Adapt(uint delta, uint numPoints, bool firstTimeFlag)
        {
            uint k;

            delta = firstTimeFlag ? delta / (uint)PunyCodeStatic.BootstringParams.Damp : delta >> 1;
            delta += delta / numPoints;

            for (k = 0; delta > (((uint)PunyCodeStatic.BootstringParams.Base - (uint)PunyCodeStatic.BootstringParams.Tmin) * (uint)PunyCodeStatic.BootstringParams.Tmax) / 2; k += (uint)PunyCodeStatic.BootstringParams.Base)
            {
                delta /= (uint)PunyCodeStatic.BootstringParams.Base - (uint)PunyCodeStatic.BootstringParams.Tmin;
            }

            return k + ((uint)PunyCodeStatic.BootstringParams.Base - (uint)PunyCodeStatic.BootstringParams.Tmin + 1) * delta / (delta + (uint)PunyCodeStatic.BootstringParams.Skew);
        }

        public PunyCodeStatic.OperationStatus AddAllAsciiCharsToOutBytes(
            string inputString,
            uint maxOut,
            out uint numberOfOutputBytes,
            out byte[] outBytes            
        )
        {
            numberOfOutputBytes = 0;
            outBytes = new byte[PunyCodeStatic.MaxInputStringLenght];
            var inputStringChars = inputString.ToCharArray();

            foreach (var c in inputStringChars.Where(c => IsCharacterBasic(c)))
            {
                if ((maxOut - numberOfOutputBytes) < 2) 
                {
                    return PunyCodeStatic.OperationStatus.BigOutput;
                }
                outBytes[numberOfOutputBytes++] = (byte) c;
            }
            return PunyCodeStatic.OperationStatus.Success;
        }

        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }

        ~PunyCodeBaseCodeHelper()
        {
            
        }
    }
}
