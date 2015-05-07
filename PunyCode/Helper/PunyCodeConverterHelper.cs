#define Compile
#undef Compile

namespace PunyCode.Helper
{
    public class PunyCodeConverterHelper
    {
        
#if Compile
        

        

        /// <summary>
        /// Bias adaptation function
        /// </summary>
        /// <param name="aDelta"></param>
        /// <param name="aNumpoints"></param>
        /// <param name="aFirsttime"></param>
        static uint Adapt(uint aDelta, uint aNumpoints, bool aFirsttime)
        {
            uint k;

            aDelta = aFirsttime ? aDelta / (uint)PunyCodeBootstringParams.PunycodeBootstringDamp : aDelta >> 1;
            /* delta >> 1 is a faster way of doing delta / 2 */
            aDelta += aDelta / aNumpoints;

            for (k = 0; aDelta > (((uint)PunyCodeBootstringParams.PunycodeBootstringBase - (uint)PunyCodeBootstringParams.PunycodeBootstringTmin) * (uint)PunyCodeBootstringParams.PunycodeBootstringTmax) / 2; k += (uint)PunyCodeBootstringParams.PunycodeBootstringBase)
            {
                aDelta /= (uint)PunyCodeBootstringParams.PunycodeBootstringBase - (uint)PunyCodeBootstringParams.PunycodeBootstringTmin;
            }

            return k + ((uint)PunyCodeBootstringParams.PunycodeBootstringBase - (uint)PunyCodeBootstringParams.PunycodeBootstringTmin + 1) * aDelta / (aDelta + (uint)PunyCodeBootstringParams.PunycodeBootstringSkew);
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
            var outputLenght = (uint)(MaxInputStringLenght);
            b = new byte[MaxInputStringLenght];
            var status = PunycodeEncode((uint)aInput.Length, aInput, null, out outputLenght, out b);

            aInput = "";

            if (status != PunyCodeOperationStatus.PunycodeStatusSuccess) return aInput.ToLower();
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
                var outputLenght = (uint)(MaxInputStringLenght);
                char[] c;
                var status = PunycodeDecode((uint)aInput.Length, b, out outputLenght, out c, null);
                aInput = "";
                if (status == PunyCodeOperationStatus.PunycodeStatusSuccess)
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
        static PunyCodeOperationStatus PunycodeEncode(uint aInputLength, string aInput,
                                                IList<bool> aCaseFlags,out uint aOutputLength,
                                                out byte[] aOutput )
        {
            uint b, sout, j;
            aOutputLength = MaxInputStringLenght;
            aOutput = new byte[MaxInputStringLenght];
            /* Initialize the state: */

            var n = (uint)PunyCodeBootstringParams.PunycodeBootstringInitialN;
            var delta = sout = 0;
            var maxOut = aOutputLength;
            var bias = (uint)PunyCodeBootstringParams.PunycodeBootstringInitialBias;

            /* Handle the basic code points: */
            for (j = 0;  j < aInputLength;  ++j) {
                if (!IsCharacterBasic(aInput[(int) j])) continue;
                if ((maxOut - sout) < 2) 
                {
                    return PunyCodeOperationStatus.PunycodeStatusBigOutput;
                }
                aOutput[sout++] = (byte)((aCaseFlags != null) ?  EncodeBasic(aInput[(int)j], aCaseFlags[(int)j]) : aInput[(int)j]);
                /* else if (input[j] < n) return punycode_bad_input; */
            /* (not needed for Punycode with unsigned code points) */
            }

            var h = b = sout;

            /* h is the number of code points that have been handled, b is the  */
            /* number of basic code points, and out is the number of characters */
            /* that have been output.                                           */

            if (b > 0) aOutput[sout++] = (byte)PunyCodeBootstringParams.PunycodeBootstringDelimiter;

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

            if (m - n > (MaxUint - delta) / (h + 1)) return PunyCodeOperationStatus.PunycodeStatusOverflow;
            delta += (m - n) * (h + 1);
            n = m;

            for (j = 0;  j < aInputLength;  ++j) {
              /* Punycode does not need to check whether input[j] is basic: */
              if (aInput[(int)j] < n /* || basic(input[j]) */ ) {
                if (++delta == 0) return PunyCodeOperationStatus.PunycodeStatusOverflow;
              }

                if (aInput[(int) j] != n) continue;
                /* Represent delta as a generalized variable-length integer: */

                uint q;
                uint k;
                for (q = delta, k = (uint)PunyCodeBootstringParams.PunycodeBootstringBase;  ;  k += (uint)PunyCodeBootstringParams.PunycodeBootstringBase) {
                    if (sout >= maxOut) return PunyCodeOperationStatus.PunycodeStatusBigOutput;
                    var t = k <= bias /* + tmin */ ? (uint)PunyCodeBootstringParams.PunycodeBootstringTmin :     /* +tmin not needed */
                        k >= bias + (uint)PunyCodeBootstringParams.PunycodeBootstringTmax ? (uint)PunyCodeBootstringParams.PunycodeBootstringTmax : k - bias;
                    if (q < t) break;
                    aOutput[sout++] = (byte)GetEncodedBasicCodeToAscii(t + (q - t) % ((uint)PunyCodeBootstringParams.PunycodeBootstringBase - t), false);
                    q = (q - t) / ((uint)PunyCodeBootstringParams.PunycodeBootstringBase - t);
                }

                aOutput[sout++] = (byte)GetEncodedBasicCodeToAscii(q, (aCaseFlags != null) && aCaseFlags[(int)j]);
                bias = Adapt(delta, h + 1, h == b);
                delta = 0;
                ++h;
            }

            ++delta;
            ++n;
            }

            aOutputLength = sout;
            return PunyCodeOperationStatus.PunycodeStatusSuccess;
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
        static PunyCodeOperationStatus PunycodeDecode(uint aInputLength,IList<byte> aInput,
                                                out uint aOutputLength,out char[] aOutput,
                                                bool[] aCaseFlags )
        {
            uint b, j, sin;
            aOutputLength = MaxInputStringLenght;
            aOutput = new char[MaxInputStringLenght];
            /* Initialize the state: */

            var n = (uint)PunyCodeBootstringParams.PunycodeBootstringInitialN;
            uint sout = 0;
            var i = 0;
            var maxOut = aOutputLength;
            var bias = (uint)PunyCodeBootstringParams.PunycodeBootstringInitialBias;

            /* Handle the basic code points:  Let b be the number of input code */
            /* points before the last delimiter, or 0 if there is none, then    */
            /* copy the first b code points to the output.                      */

            for (b = j = 0;  j < aInputLength;  ++j) 
            {
                if (IsCharacterDelimiter(aInput[(int)j])) b = j;
            }
            if (b > maxOut) 
            {
                return PunyCodeOperationStatus.PunycodeStatusBigOutput;
            }

            for (j = 0;  j < b;  ++j) 
            {
                if (aCaseFlags != null) 
                {
                    aCaseFlags[sout] = IsFlagged(aInput[(int)j]);
                }
                if (!IsCharacterBasic(aInput[(int)j])) 
                {
                    return PunyCodeOperationStatus.PunycodeStatusBadInput;
                }

                aOutput[(int)sout++] = (char)aInput[(int)j];
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
                for (oldi = (uint)i, w = 1, k = (uint)PunyCodeBootstringParams.PunycodeBootstringBase;  ;  k += (uint)PunyCodeBootstringParams.PunycodeBootstringBase) {
              if (sin >= aInputLength) return PunyCodeOperationStatus.PunycodeStatusBadInput;
              var digit = GetAlphaNumericValueInBasicCode(aInput[(int)sin++]);
              if (digit >= (uint)PunyCodeBootstringParams.PunycodeBootstringBase) return PunyCodeOperationStatus.PunycodeStatusBadInput;
              if (digit > (MaxUint - i) / w) return PunyCodeOperationStatus.PunycodeStatusOverflow;
              i += (int)(digit * w);
              var t = k <= bias /* + tmin */ ? (uint)PunyCodeBootstringParams.PunycodeBootstringTmin :     /* +tmin not needed */
                  k >= bias + (uint)PunyCodeBootstringParams.PunycodeBootstringTmax ? (uint)PunyCodeBootstringParams.PunycodeBootstringTmax : k - bias;
              if (digit < t) break;
              if (w > MaxUint / ((uint)PunyCodeBootstringParams.PunycodeBootstringBase - t)) return PunyCodeOperationStatus.PunycodeStatusOverflow;
              w *= ((uint)PunyCodeBootstringParams.PunycodeBootstringBase - t);
            }

            bias = Adapt((uint)(i - (int)oldi), sout + 1, oldi == 0);

            /* i was supposed to wrap around from out+1 to 0,   */
            /* incrementing n each time, so we'll fix that now: */

            if (i / (sout + 1) > MaxUint - n) return PunyCodeOperationStatus.PunycodeStatusOverflow;
            n += (uint)i / (sout + 1);
            i %= (int)(sout + 1);

            /* Insert n at position i of the output: */

            /* not needed for Punycode: */
            /* if (decode_digit(n) <= base) return punycode_invalid_input; */
            if (sout >= maxOut) return PunyCodeOperationStatus.PunycodeStatusBigOutput;

            if (aCaseFlags != null) {
                Array.Copy(aCaseFlags, i, aCaseFlags, i + 1, sout - 1);
              //memmove(case_flags + i + 1, case_flags + i, out - i);
              /* Case of last character determines uppercase flag: */
              aCaseFlags[i] = IsFlagged(aInput[(int)sin - 1]);
            }

            //Array.Copy(a_output, i, a_output, i + 1, (sout - i) * a_output.Length);
            //memmove(output + i + 1, output + i, (out - i) * sizeof *output);
            Array.Copy(aOutput, i, aOutput, i + 1, (sout - i) * 1);
            aOutput[i++] = (char)n;
            }

            aOutputLength = sout;
            return PunyCodeOperationStatus.PunycodeStatusSuccess;
        }
#endif
    }
}
