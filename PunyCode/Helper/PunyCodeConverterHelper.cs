using System;
using System.Collections.Generic;

namespace PunyCode.Helper
{
    public sealed class PunyCodeConverterHelper : IDisposable
    {
        private PunyCodeBaseCodeHelper _punyCodeBaseCodeHelper;
        private bool _disposed;

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
        /// <param name="inputLenght">input string lenght</param>
        /// <param name="inputString">input string</param>
        /// <param name="caseFlagsList">upper/lowercase flags</param>
        /// <param name="outputLenght">output length</param>
        /// <param name="outBytes">the output bytes in ASCII</param>
        /// <returns>
        /// WsPunyCodePunycodeStatus_Success,
        /// WsPunyCodePunycodeStatus_BadInput,   /* Input is invalid.                       */
        /// WsPunyCodePunycodeStatus_BigOutput,  /* Output would exceed the space provided. */
        /// WsPunyCodePunycodeStatus_Overflow    /* Input needs wider integers to process.  */
        /// </returns>
        public PunyCodeStatic.PunyCodeOperationStatus PunycodeEncode(uint inputLenght, string inputString,
                                                IList<bool> caseFlagsList,out uint outputLenght,
                                                out byte[] outBytes )
        {
            uint b, sout, j;
            outputLenght = PunyCodeStatic.MaxInputStringLenght;
            outBytes = new byte[PunyCodeStatic.MaxInputStringLenght];
            /* Initialize the state: */

            var n = (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringInitialN;
            var delta = sout = 0;
            var maxOut = outputLenght;
            var bias = (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringInitialBias;

            /* Handle the basic code points: */
            for (j = 0;  j < inputLenght;  ++j) {
                if (!_punyCodeBaseCodeHelper.IsCharacterBasic(inputString[(int)j])) continue;
                if ((maxOut - sout) < 2) 
                {
                    return PunyCodeStatic.PunyCodeOperationStatus.PunycodeStatusBigOutput;
                }
                outBytes[sout++] = (byte)((caseFlagsList != null) 
                    ? _punyCodeBaseCodeHelper.GetEncodedBasicCodeToAscii(inputString[(int)j], caseFlagsList[(int)j]) 
                    : inputString[(int)j]);
                /* else if (input[j] < n) return punycode_bad_input; */
            /* (not needed for Punycode with unsigned code points) */
            }

            var h = b = sout;

            /* h is the number of code points that have been handled, b is the  */
            /* number of basic code points, and out is the number of characters */
            /* that have been output.                                           */

            if (b > 0) outBytes[sout++] = (byte)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringDelimiter;

            /* Main encoding loop: */

            while (h < inputLenght) {
            /* All non-basic code points < n have been     */
            /* handled already.  Find the next larger one: */

                uint m;
                for (m = PunyCodeStatic.MaxUint, j = 0;  j < inputLenght;  ++j) {
              /* if (basic(input[j])) continue; */
              /* (not needed for Punycode) */
              if (inputString[(int)j] >= n && inputString[(int)j] < m) m = inputString[(int)j];
            }

            /* Increase delta enough to advance the decoder's    */
            /* <n,i> state to <m,0>, but guard against overflow: */

            if (m - n > (PunyCodeStatic.MaxUint - delta) / (h + 1)) return PunyCodeStatic.PunyCodeOperationStatus.PunycodeStatusOverflow;
            delta += (m - n) * (h + 1);
            n = m;

            for (j = 0;  j < inputLenght;  ++j) {
              /* Punycode does not need to check whether input[j] is basic: */
              if (inputString[(int)j] < n /* || basic(input[j]) */ ) {
                if (++delta == 0) return PunyCodeStatic.PunyCodeOperationStatus.PunycodeStatusOverflow;
              }

                if (inputString[(int) j] != n) continue;
                /* Represent delta as a generalized variable-length integer: */

                uint q;
                uint k;
                for (q = delta, k = (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringBase;  
                    ;  
                    k += (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringBase) {
                    if (sout >= maxOut) return PunyCodeStatic.PunyCodeOperationStatus.PunycodeStatusBigOutput;
                    var t = k <= bias /* + tmin */ ? (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringTmin :     /* +tmin not needed */
                        k >= bias + (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringTmax 
                        ? (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringTmax 
                        : k - bias;
                    if (q < t) break;
                    outBytes[sout++] = (byte)_punyCodeBaseCodeHelper.GetEncodedBasicCodeToAscii(t + (q - t) 
                        % ((uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringBase - t), false);
                    q = (q - t) / ((uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringBase - t);
                }

                outBytes[sout++] = (byte)_punyCodeBaseCodeHelper.GetEncodedBasicCodeToAscii(q, (caseFlagsList != null) && caseFlagsList[(int)j]);
                bias = _punyCodeBaseCodeHelper.Adapt(delta, h + 1, h == b);
                delta = 0;
                ++h;
            }

            ++delta;
            ++n;
            }

            outputLenght = sout;
            return PunyCodeStatic.PunyCodeOperationStatus.PunycodeStatusSuccess;
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
        /// <param name="inputLenght">input string lenght</param>
        /// <param name="inputBytes">input punycode</param>
        /// <param name="caseBools">upper/lowercase flags</param>
        /// <param name="outputLenght">output length</param>
        /// <param name="outChars">the output bytes in Unicode</param>
        /// <returns>
        /// WsPunyCodePunycodeStatus_Success,
        /// WsPunyCodePunycodeStatus_BadInput,   /* Input is invalid.                       */
        /// WsPunyCodePunycodeStatus_BigOutput,  /* Output would exceed the space provided. */
        /// WsPunyCodePunycodeStatus_Overflow    /* Input needs wider integers to process.  */
        /// </returns>
        public PunyCodeStatic.PunyCodeOperationStatus PunycodeDecode(uint inputLenght,IList<byte> inputBytes,
                                                out uint outputLenght,out char[] outChars,
                                                bool[] caseBools )
        {
            uint b, j, sin;
            outputLenght = PunyCodeStatic.MaxInputStringLenght;
            outChars = new char[PunyCodeStatic.MaxInputStringLenght];
            /* Initialize the state: */

            var n = (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringInitialN;
            uint sout = 0;
            var i = 0;
            var maxOut = outputLenght;
            var bias = (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringInitialBias;

            /* Handle the basic code points:  Let b be the number of input code */
            /* points before the last delimiter, or 0 if there is none, then    */
            /* copy the first b code points to the output.                      */

            for (b = j = 0;  j < inputLenght;  ++j) 
            {
                if (_punyCodeBaseCodeHelper.IsCharacterDelimiter(inputBytes[(int)j])) b = j;
            }
            if (b > maxOut) 
            {
                return PunyCodeStatic.PunyCodeOperationStatus.PunycodeStatusBigOutput;
            }

            for (j = 0;  j < b;  ++j) 
            {
                if (caseBools != null) 
                {
                    caseBools[sout] = _punyCodeBaseCodeHelper.IsUpperCaseOrNumber(inputBytes[(int)j]);
                }
                if (!_punyCodeBaseCodeHelper.IsCharacterBasic(inputBytes[(int)j])) 
                {
                    return PunyCodeStatic.PunyCodeOperationStatus.PunycodeStatusBadInput;
                }

                outChars[(int)sout++] = (char)inputBytes[(int)j];
            }

            /* Main decoding loop:  Start just after the last delimiter if any  */
            /* basic code points were copied; start at the beginning otherwise. */

            for (sin = b > 0 ? b + 1 : 0;  sin < inputLenght;  ++sout) {

            /* in is the index of the next character to be consumed, and */
            /* out is the number of code points in the output array.     */

            /* Decode a generalized variable-length integer into delta,  */
            /* which gets added to i.  The overflow checking is easier   */
            /* if we increase i as we go, then subtract off its starting */
            /* value at the end to obtain delta.                         */

                uint oldi;
                uint w;
                uint k;
                for (oldi = (uint)i, w = 1, k = (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringBase;  
                    ;  
                    k += (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringBase) {
              if (sin >= inputLenght) return PunyCodeStatic.PunyCodeOperationStatus.PunycodeStatusBadInput;
              var digit = _punyCodeBaseCodeHelper.GetAlphaNumericValueInBasicCode(inputBytes[(int)sin++]);
              if (digit >= (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringBase) 
                  return PunyCodeStatic.PunyCodeOperationStatus.PunycodeStatusBadInput;
              if (digit > (PunyCodeStatic.MaxUint - i) / w) return PunyCodeStatic.PunyCodeOperationStatus.PunycodeStatusOverflow;
              i += (int)(digit * w);
              var t = k <= bias /* + tmin */ ? (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringTmin :     /* +tmin not needed */
                  k >= bias + (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringTmax 
                  ? (uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringTmax 
                  : k - bias;
              if (digit < t) break;
              if (w > PunyCodeStatic.MaxUint / ((uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringBase - t)) 
                  return PunyCodeStatic.PunyCodeOperationStatus.PunycodeStatusOverflow;
              w *= ((uint)PunyCodeStatic.PunyCodeBootstringParams.PunycodeBootstringBase - t);
            }

                bias = _punyCodeBaseCodeHelper.Adapt((uint)(i - (int)oldi), sout + 1, oldi == 0);

            /* i was supposed to wrap around from out+1 to 0,   */
            /* incrementing n each time, so we'll fix that now: */

            if (i / (sout + 1) > PunyCodeStatic.MaxUint - n) 
                return PunyCodeStatic.PunyCodeOperationStatus.PunycodeStatusOverflow;
            n += (uint)i / (sout + 1);
            i %= (int)(sout + 1);

            /* Insert n at position i of the output: */

            /* not needed for Punycode: */
            /* if (decode_digit(n) <= base) return punycode_invalid_input; */
            if (sout >= maxOut) return PunyCodeStatic.PunyCodeOperationStatus.PunycodeStatusBigOutput;

            if (caseBools != null) {
                Array.Copy(caseBools, i, caseBools, i + 1, sout - 1);
              //memmove(case_flags + i + 1, case_flags + i, out - i);
              /* Case of last character determines uppercase flag: */
                caseBools[i] = _punyCodeBaseCodeHelper.IsUpperCaseOrNumber(inputBytes[(int)sin - 1]);
            }

            //Array.Copy(a_output, i, a_output, i + 1, (sout - i) * a_output.Length);
            //memmove(output + i + 1, output + i, (out - i) * sizeof *output);
            Array.Copy(outChars, i, outChars, i + 1, (sout - i) * 1);
            outChars[i++] = (char)n;
            }

            outputLenght = sout;
            return PunyCodeStatic.PunyCodeOperationStatus.PunycodeStatusSuccess;
        }
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _punyCodeBaseCodeHelper.Dispose();
                _punyCodeBaseCodeHelper = null;
            }

            _disposed = true;
        }

        ~PunyCodeConverterHelper()
        {
            Dispose(false);
        }

    }
}
