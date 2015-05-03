using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkChecker.Utilities
{
    /// <summary>
    /// This class is used for punycode conversion
    /// </summary>
    public sealed class Punycode
    {
        /// <summary>
        /// the lock
        /// </summary>
        static object _pLock = new object();

        /// <summary>
        /// max input string
        /// </summary>
        const int EncodeOutputLenght = 8000;
        /// <summary>
        /// maxint is the maximum value of a punycode_uint variable
        /// Because maxint is unsigned, -1 becomes the maximum value.
        /// </summary>
        const uint MaxUint = 4294967295;

        /// <summary>
        /// hide constructor
        /// </summary>
        Punycode()
        {

        }

        /// <summary>
        /// Status enum
        /// </summary>
        private enum PunyCodePunycodeStatus
        {
            WsPunyCodePunycodeStatusSuccess,
            WsPunyCodePunycodeStatusBadInput,   /* Input is invalid.                       */
            WsPunyCodePunycodeStatusBigOutput,  /* Output would exceed the space provided. */
            WsPunyCodePunycodeStatusOverflow     /* Input needs wider integers to process.  */
        };

        /// <summary>
        /// Bootstring parameters for Punycode
        /// </summary>
        private enum PunyCodePunycodeBootstring
        {
            WsPunyCodePunycodeBootstringBase = 36,
            WsPunyCodePunycodeBootstringTmin = 1,
            WsPunyCodePunycodeBootstringTmax = 26,
            WsPunyCodePunycodeBootstringSkew = 38,
            WsPunyCodePunycodeBootstringDamp = 700,
            WsPunyCodePunycodeBootstringInitialBias = 72,
            WsPunyCodePunycodeBootstringInitialN = 0x80,
            WsPunyCodePunycodeBootstringDelimiter = 0x2D 
        };

        /// <summary>
        /// basic(cp) tests whether cp is a basic code point:
        /// </summary>
        /// <param name="aCp">the input char</param>
        /// <returns>true or false</returns>
        static bool IsBasic(uint aCp) 
        {
            return (aCp < 0x80);
        }

        /// <summary>
        /// delim(cp) tests whether cp is a delimiter:
        /// </summary>
        /// <param name="aCp">the input char</param>
        /// <returns>true or false</returns>
        static bool IsDelimiter(uint aCp)
        {
            return (aCp == (int)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringDelimiter);
        }

        /// <summary>
        /// returns the numeric value of a basic code
        /// point (for use in representing integers) in the range 0 to
        /// base-1, or base if cp is does not represent a value.
        /// </summary>
        /// <param name="aCp">the input</param>
        static uint DecodeDigit(uint aCp)
        {
            return (aCp - 48 < 10 ? aCp - 22 : aCp - 65 < 26 ? aCp - 65 :
                    aCp - 97 < 26 ? aCp - 97 : (int)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringBase);
        }

        /// <summary>
        /// encode_digit(d,flag) returns the basic code point whose value
        /// (when used for representing integers) is d, which needs to be in
        /// the range 0 to base-1.  The lowercase form is used unless flag is 
        /// nonzero, in which case the uppercase form is used.  The behavior
        /// is undefined if flag is nonzero and digit d has no uppercase form.
        /// </summary>
        /// <param name="aD">input character</param>
        /// <param name="aFlag">input flag</param>
        static char EncodeDigit(uint aD, bool aFlag)
        {
            return (char)(aD + 22 + 75 * (aD < 26 ? 1 : 0) - ((aFlag != true ? 1 : 0) << 5));
            /*  0..25 map to ASCII a..z or A..Z */
            /* 26..35 map to ASCII 0..9         */
        }

        /// <summary>
        /// flagged(bcp) tests whether a basic code point is flagged
        /// (uppercase).  The behavior is undefined if bcp is not a
        /// basic code point.
        /// </summary>
        /// <param name="aBcp">input character</param>
        /// <returns>true or false</returns>
        static bool IsFlagged(uint aBcp)
        {
            return (aBcp - 65 < 26);
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
          aBcp -= (uint)(((aBcp - 97 < 26) ? 1: 0) << 5);
          return (char)(aBcp + (((!aFlag && (aBcp - 65 < 26)) ? 1 : 0) << 5));
        }

        /// <summary>
        /// Bias adaptation function
        /// </summary>
        /// <param name="aDelta"></param>
        /// <param name="aNumpoints"></param>
        /// <param name="aFirsttime"></param>
        static uint Adapt(uint aDelta, uint aNumpoints, bool aFirsttime)
        {
            uint k;

            aDelta = aFirsttime ? aDelta / (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringDamp : aDelta >> 1;
            /* delta >> 1 is a faster way of doing delta / 2 */
            aDelta += aDelta / aNumpoints;

            for (k = 0; aDelta > (((uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringBase - (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringTmin) * (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringTmax) / 2; k += (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringBase)
            {
                aDelta /= (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringBase - (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringTmin;
            }

            return k + ((uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringBase - (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringTmin + 1) * aDelta / (aDelta + (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringSkew);
        }        

        /// <summary>
        /// Encodes Unicode to Punicode (forces unicode on string)
        /// </summary>
        /// <param name="aInput">input string</param>
        static public string Encode(string aInput)
        {
            var ws = new Punycode();

            // convert to ascii
            var b = Encoding.Unicode.GetBytes(aInput);
            aInput = Encoding.Unicode.GetString(b);
            var outputLenght = (uint)(EncodeOutputLenght);
            b = new byte[EncodeOutputLenght];
            var status = PunycodeEncode((uint)aInput.Length, aInput, null, out outputLenght, out b);

            aInput = "";

            if (status != PunyCodePunycodeStatus.WsPunyCodePunycodeStatusSuccess) return aInput.ToLower();
            for (var i = 0; i < b.Length; i++)
            {
                //fix numbers
                if (b[i] >= 16 && b[i] <= 25)
                {
                    b[i] += 32;
                }
                if (b[i] != 0)
                {
                    aInput += (char)b[i];
                }
            }
            return aInput.ToLower();
        }

        /// <summary>
        /// Encodes Punicode to Unicode
        /// not ready yet
        /// </summary>
        /// <param name="aInput">input punycode string</param>
        public static string Decode(string aInput)
        {
            try
            {
                var ws = new Punycode();

                // convert to unicode
                var b = Encoding.ASCII.GetBytes(aInput);
                var outputLenght = (uint)(EncodeOutputLenght);
                char[] c;
                var status = PunycodeDecode((uint)aInput.Length, b, out outputLenght, out c, null);
                aInput = "";
                if (status == PunyCodePunycodeStatus.WsPunyCodePunycodeStatusSuccess)
                {
                    aInput = c.Where(t => t != 0).Aggregate(aInput, (current, t) => current + t);
                }
            }
            catch (Exception e)
            {
                // ignored
            }
            return aInput.ToLower();

        }

        /// <summary>
        /// /* PunycodeEncode() converts Unicode to Punycode.  The input     */
        /// /* is represented as an array of Unicode code points (not code    */
        /// /* units; surrogate pairs are not allowed), and the output        */
        /// /* will be represented as an array of ASCII code points.  The     */
        /// /* output string is *not* null-terminated; it will contain        */
        /// /* zeros if and only if the input contains zeros.  (Of course     */
        /// /* the caller can leave room for a terminator and add one if      */
        /// /* needed.)  The input_length is the number of code points in     */
        /// /* the input.  The output_length is an in/out argument: the       */
        /// /* caller passes in the maximum number of code points that it     */
        /// /* can receive, and on successful return it will contain the      */
        /// /* number of code points actually output.  The case_flags array   */
        /// /* holds input_length boolean values, where nonzero suggests that */
        /// /* the corresponding Unicode character be forced to uppercase     */
        /// /* after being decoded (if possible), and zero suggests that      */
        /// /* it be forced to lowercase (if possible).  ASCII code points    */
        /// /* are encoded literally, except that ASCII letters are forced    */
        /// /* to uppercase or lowercase according to the corresponding       */
        /// /* uppercase flags.  If case_flags is a null pointer then ASCII   */
        /// /* letters are left as they are, and other code points are        */
        /// /* treated as if their uppercase flags were zero.  The return     */
        /// /* value can be any of the punycode_status values defined above   */
        /// /* except punycode_bad_input; if not punycode_success, then       */
        /// /* output_size and output might contain garbage.                  */
        /// </summary>
        /// <param name="aInputLength">input string lenght</param>
        /// <param name="aInput">input string</param>
        /// <param name="aCaseFlags">upper/lowercase flags</param>
        /// <param name="aOutputLength">output length</param>
        /// <param name="aOutput">the output bytes in ASCII</param>
        /// <returns>
        /// WsPunyCodePunycodeStatus_Success,
        /// WsPunyCodePunycodeStatus_BadInput,   /* Input is invalid.                       */
        /// WsPunyCodePunycodeStatus_BigOutput,  /* Output would exceed the space provided. */
        /// WsPunyCodePunycodeStatus_Overflow    /* Input needs wider integers to process.  */
        /// </returns>
        static PunyCodePunycodeStatus PunycodeEncode(uint aInputLength, string aInput,
                                                IList<bool> aCaseFlags,out uint aOutputLength,
                                                out byte[] aOutput )
        {
            uint b, sout, j;
            aOutputLength = EncodeOutputLenght;
            aOutput = new byte[EncodeOutputLenght];
            /* Initialize the state: */

            var n = (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringInitialN;
            var delta = sout = 0;
            var maxOut = aOutputLength;
            var bias = (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringInitialBias;

            /* Handle the basic code points: */
            for (j = 0;  j < aInputLength;  ++j) {
                if (!IsBasic(aInput[(int) j])) continue;
                if ((maxOut - sout) < 2) 
                {
                    return PunyCodePunycodeStatus.WsPunyCodePunycodeStatusBigOutput;
                }
                aOutput[sout++] = (byte)((aCaseFlags != null) ?  EncodeBasic(aInput[(int)j], aCaseFlags[j]) : aInput[(int)j]);
                /* else if (input[j] < n) return punycode_bad_input; */
            /* (not needed for Punycode with unsigned code points) */
            }

            var h = b = sout;

            /* h is the number of code points that have been handled, b is the  */
            /* number of basic code points, and out is the number of characters */
            /* that have been output.                                           */

            if (b > 0) aOutput[sout++] = (byte)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringDelimiter;

            /* Main encoding loop: */

            while (h < aInputLength) {
            /* All non-basic code points < n have been     */
            /* handled already.  Find the next larger one: */

                uint m;
                for (m = MaxUint, j = 0;  j < aInputLength;  ++j) {
              /* if (basic(input[j])) continue; */
              /* (not needed for Punycode) */
              if (aInput[(int)j] >= n && aInput[(int)j] < m) m = aInput[(int)j];
            }

            /* Increase delta enough to advance the decoder's    */
            /* <n,i> state to <m,0>, but guard against overflow: */

            if (m - n > (MaxUint - delta) / (h + 1)) return PunyCodePunycodeStatus.WsPunyCodePunycodeStatusOverflow;
            delta += (m - n) * (h + 1);
            n = m;

            for (j = 0;  j < aInputLength;  ++j) {
              /* Punycode does not need to check whether input[j] is basic: */
              if (aInput[(int)j] < n /* || basic(input[j]) */ ) {
                if (++delta == 0) return PunyCodePunycodeStatus.WsPunyCodePunycodeStatusOverflow;
              }

                if (aInput[(int) j] != n) continue;
                /* Represent delta as a generalized variable-length integer: */

                uint q;
                uint k;
                for (q = delta, k = (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringBase;  ;  k += (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringBase) {
                    if (sout >= maxOut) return PunyCodePunycodeStatus.WsPunyCodePunycodeStatusBigOutput;
                    var t = k <= bias /* + tmin */ ? (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringTmin :     /* +tmin not needed */
                        k >= bias + (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringTmax ? (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringTmax : k - bias;
                    if (q < t) break;
                    aOutput[sout++] = (byte)EncodeDigit(t + (q - t) % ((uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringBase - t), false);
                    q = (q - t) / ((uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringBase - t);
                }

                aOutput[sout++] = (byte)EncodeDigit(q, (aCaseFlags != null) && aCaseFlags[j]);
                bias = Adapt(delta, h + 1, h == b);
                delta = 0;
                ++h;
            }

            ++delta;
            ++n;
            }

            aOutputLength = sout;
            return PunyCodePunycodeStatus.WsPunyCodePunycodeStatusSuccess;
        }

        /// <summary>
        /// /* PunycodeDecode() converts Punycode to Unicode.  The input is  */
        /// /* represented as an array of ASCII code points, and the output   */
        /// /* will be represented as an array of Unicode code points.  The   */
        /// /* input_length is the number of code points in the input.  The   */
        /// /* output_length is an in/out argument: the caller passes in      */
        /// /* the maximum number of code points that it can receive, and     */
        /// /* on successful return it will contain the actual number of      */
        /// /* code points output.  The case_flags array needs room for at    */
        /// /* least output_length values, or it can be a null pointer if the */
        /// /* case information is not needed.  A nonzero flag suggests that  */
        /// /* the corresponding Unicode character be forced to uppercase     */
        /// /* by the caller (if possible), while zero suggests that it be    */
        /// /* forced to lowercase (if possible).  ASCII code points are      */
        /// /* output already in the proper case, but their flags will be set */
        /// /* appropriately so that applying the flags would be harmless.    */
        /// /* The return value can be any of the punycode_status values      */
        /// /* defined above; if not punycode_success, then output_length,    */
        /// /* output, and case_flags might contain garbage.  On success, the */
        /// /* decoder will never need to write an output_length greater than */
        /// /* input_length, because of how the encoding is defined.          */
        /// </summary>
        /// <param name="aInputLength">input string lenght</param>
        /// <param name="aInput">input punycode</param>
        /// <param name="aCaseFlags">upper/lowercase flags</param>
        /// <param name="aOutputLength">output length</param>
        /// <param name="aOutput">the output bytes in Unicode</param>
        /// <returns>
        /// WsPunyCodePunycodeStatus_Success,
        /// WsPunyCodePunycodeStatus_BadInput,   /* Input is invalid.                       */
        /// WsPunyCodePunycodeStatus_BigOutput,  /* Output would exceed the space provided. */
        /// WsPunyCodePunycodeStatus_Overflow    /* Input needs wider integers to process.  */
        /// </returns>
        static PunyCodePunycodeStatus PunycodeDecode(uint aInputLength,IList<byte> aInput,
                                                out uint aOutputLength,out char[] aOutput,
                                                bool[] aCaseFlags )
        {
            uint b, j, sin;
            aOutputLength = EncodeOutputLenght;
            aOutput = new char[EncodeOutputLenght];
            /* Initialize the state: */

            var n = (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringInitialN;
            uint sout = 0;
            var i = 0;
            var maxOut = aOutputLength;
            var bias = (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringInitialBias;

            /* Handle the basic code points:  Let b be the number of input code */
            /* points before the last delimiter, or 0 if there is none, then    */
            /* copy the first b code points to the output.                      */

            for (b = j = 0;  j < aInputLength;  ++j) 
            {
                if (IsDelimiter(aInput[j])) b = j;
            }
            if (b > maxOut) 
            {
                return PunyCodePunycodeStatus.WsPunyCodePunycodeStatusBigOutput;
            }

            for (j = 0;  j < b;  ++j) 
            {
                if (aCaseFlags != null) 
                {
                    aCaseFlags[sout] = IsFlagged(aInput[j]);
                }
                if (!IsBasic(aInput[j])) 
                {
                    return PunyCodePunycodeStatus.WsPunyCodePunycodeStatusBadInput;
                }

                aOutput[(int)sout++] = (char)aInput[j];
            }

            /* Main decoding loop:  Start just after the last delimiter if any  */
            /* basic code points were copied; start at the beginning otherwise. */

            for (sin = b > 0 ? b + 1 : 0;  sin < aInputLength;  ++sout) {

            /* in is the index of the next character to be consumed, and */
            /* out is the number of code points in the output array.     */

            /* Decode a generalized variable-length integer into delta,  */
            /* which gets added to i.  The overflow checking is easier   */
            /* if we increase i as we go, then subtract off its starting */
            /* value at the end to obtain delta.                         */

                uint oldi;
                uint w;
                uint k;
                for (oldi = (uint)i, w = 1, k = (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringBase;  ;  k += (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringBase) {
              if (sin >= aInputLength) return PunyCodePunycodeStatus.WsPunyCodePunycodeStatusBadInput;
              var digit = DecodeDigit(aInput[sin++]);
              if (digit >= (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringBase) return PunyCodePunycodeStatus.WsPunyCodePunycodeStatusBadInput;
              if (digit > (MaxUint - i) / w) return PunyCodePunycodeStatus.WsPunyCodePunycodeStatusOverflow;
              i += (int)(digit * w);
              var t = k <= bias /* + tmin */ ? (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringTmin :     /* +tmin not needed */
                  k >= bias + (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringTmax ? (uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringTmax : k - bias;
              if (digit < t) break;
              if (w > MaxUint / ((uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringBase - t)) return PunyCodePunycodeStatus.WsPunyCodePunycodeStatusOverflow;
              w *= ((uint)PunyCodePunycodeBootstring.WsPunyCodePunycodeBootstringBase - t);
            }

            bias = Adapt((uint)(i - (int)oldi), sout + 1, oldi == 0);

            /* i was supposed to wrap around from out+1 to 0,   */
            /* incrementing n each time, so we'll fix that now: */

            if (i / (sout + 1) > MaxUint - n) return PunyCodePunycodeStatus.WsPunyCodePunycodeStatusOverflow;
            n += (uint)i / (sout + 1);
            i %= (int)(sout + 1);

            /* Insert n at position i of the output: */

            /* not needed for Punycode: */
            /* if (decode_digit(n) <= base) return punycode_invalid_input; */
            if (sout >= maxOut) return PunyCodePunycodeStatus.WsPunyCodePunycodeStatusBigOutput;

            if (aCaseFlags != null) {
                Array.Copy(aCaseFlags, i, aCaseFlags, i + 1, sout - 1);
              //memmove(case_flags + i + 1, case_flags + i, out - i);
              /* Case of last character determines uppercase flag: */
              aCaseFlags[i] = IsFlagged(aInput[sin - 1]);
            }

            //Array.Copy(a_output, i, a_output, i + 1, (sout - i) * a_output.Length);
            //memmove(output + i + 1, output + i, (out - i) * sizeof *output);
            Array.Copy(aOutput, i, aOutput, i + 1, (sout - i) * 1);
            aOutput[i++] = (char)n;
            }

            aOutputLength = sout;
            return PunyCodePunycodeStatus.WsPunyCodePunycodeStatusSuccess;
        }


    }
}
