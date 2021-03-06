﻿using System;
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
        public PunyCodeStatic.OperationStatus PunycodeEncode(
            uint inputLenght, 
            string inputString,
            out uint outputLenght,
            out byte[] outBytes )
        {
            
            outputLenght = PunyCodeStatic.MaxInputStringLenght;            
            uint numberOfBasicCodePoints,
                 numberOfOutputBytes;
            var n = (uint)PunyCodeStatic.BootstringParams.InitialN;
            uint delta = 0;
            var maxOut = outputLenght;
            var bias = (uint)PunyCodeStatic.BootstringParams.InitialBias;

            if (_punyCodeBaseCodeHelper.AddAllAsciiCharsToOutBytes(
                inputString,
                maxOut, 
                out numberOfOutputBytes,
                out outBytes) != PunyCodeStatic.OperationStatus.Success)
                return PunyCodeStatic.OperationStatus.BigOutput;

            var numberOfHandledBasicCodePoints = numberOfBasicCodePoints = numberOfOutputBytes;

            if (numberOfBasicCodePoints > 0) outBytes[numberOfOutputBytes++] = (byte)PunyCodeStatic.BootstringParams.Delimiter;

            /* Main encoding loop: */

            while (numberOfHandledBasicCodePoints < inputLenght) {
            /* All non-basic code points < n have been     */
            /* handled already.  Find the next larger one: */

                uint m;
                uint j;
                for (m = PunyCodeStatic.MaxUint, j = 0;  j < inputLenght;  ++j) {
              /* if (basic(input[j])) continue; */
              /* (not needed for Punycode) */
              if (inputString[(int)j] >= n && inputString[(int)j] < m) m = inputString[(int)j];
            }

            /* Increase delta enough to advance the decoder's    */
            /* <n,i> state to <m,0>, but guard against overflow: */

            if (m - n > (PunyCodeStatic.MaxUint - delta) / (numberOfHandledBasicCodePoints + 1)) return PunyCodeStatic.OperationStatus.Overflow;
            delta += (m - n) * (numberOfHandledBasicCodePoints + 1);
            n = m;

            for (j = 0;  j < inputLenght;  ++j) {
              /* Punycode does not need to check whether input[j] is basic: */
              if (inputString[(int)j] < n /* || basic(input[j]) */ ) {
                if (++delta == 0) return PunyCodeStatic.OperationStatus.Overflow;
              }

                if (inputString[(int) j] != n) continue;
                /* Represent delta as a generalized variable-length integer: */

                uint q;
                uint k;
                for (q = delta, k = (uint)PunyCodeStatic.BootstringParams.Base;  
                    ;  
                    k += (uint)PunyCodeStatic.BootstringParams.Base) {
                    if (numberOfOutputBytes >= maxOut) return PunyCodeStatic.OperationStatus.BigOutput;
                    var t = k <= bias /* + tmin */ ? (uint)PunyCodeStatic.BootstringParams.Tmin :     /* +tmin not needed */
                        k >= bias + (uint)PunyCodeStatic.BootstringParams.Tmax 
                        ? (uint)PunyCodeStatic.BootstringParams.Tmax 
                        : k - bias;
                    if (q < t) break;
                    outBytes[numberOfOutputBytes++] = (byte)_punyCodeBaseCodeHelper.GetEncodedBasicCodeToAscii(t + (q - t) 
                        % ((uint)PunyCodeStatic.BootstringParams.Base - t), false);
                    q = (q - t) / ((uint)PunyCodeStatic.BootstringParams.Base - t);
                }

                //outBytes[numberOfOutputBytes++] = (byte)_punyCodeBaseCodeHelper.GetEncodedBasicCodeToAscii(q, (caseFlagsList != null) && caseFlagsList[(int)j]);
                bias = _punyCodeBaseCodeHelper.Adapt(delta, numberOfHandledBasicCodePoints + 1, numberOfHandledBasicCodePoints == numberOfBasicCodePoints);
                delta = 0;
                ++numberOfHandledBasicCodePoints;
            }

            ++delta;
            ++n;
            }

            outputLenght = numberOfOutputBytes;
            return PunyCodeStatic.OperationStatus.Success;
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
        public PunyCodeStatic.OperationStatus PunycodeDecode(uint inputLenght,IList<byte> inputBytes,
                                                out uint outputLenght,out char[] outChars,
                                                bool[] caseBools )
        {
            uint b, j, sin;
            outputLenght = PunyCodeStatic.MaxInputStringLenght;
            outChars = new char[PunyCodeStatic.MaxInputStringLenght];
            /* Initialize the state: */

            var n = (uint)PunyCodeStatic.BootstringParams.InitialN;
            uint sout = 0;
            var i = 0;
            var maxOut = outputLenght;
            var bias = (uint)PunyCodeStatic.BootstringParams.InitialBias;

            /* Handle the basic code points:  Let b be the number of input code */
            /* points before the last delimiter, or 0 if there is none, then    */
            /* copy the first b code points to the output.                      */

            for (b = j = 0;  j < inputLenght;  ++j) 
            {
                if (_punyCodeBaseCodeHelper.IsCharacterDelimiter(inputBytes[(int)j])) b = j;
            }
            if (b > maxOut) 
            {
                return PunyCodeStatic.OperationStatus.BigOutput;
            }

            for (j = 0;  j < b;  ++j) 
            {
                if (caseBools != null) 
                {
                    caseBools[sout] = _punyCodeBaseCodeHelper.IsUpperCaseOrNumber(inputBytes[(int)j]);
                }
                if (!_punyCodeBaseCodeHelper.IsCharacterBasic(inputBytes[(int)j])) 
                {
                    return PunyCodeStatic.OperationStatus.BadInput;
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
                for (oldi = (uint)i, w = 1, k = (uint)PunyCodeStatic.BootstringParams.Base;  
                    ;  
                    k += (uint)PunyCodeStatic.BootstringParams.Base) {
              if (sin >= inputLenght) return PunyCodeStatic.OperationStatus.BadInput;
              var digit = _punyCodeBaseCodeHelper.GetAlphaNumericValueInBasicCode(inputBytes[(int)sin++]);
              if (digit >= (uint)PunyCodeStatic.BootstringParams.Base) 
                  return PunyCodeStatic.OperationStatus.BadInput;
              if (digit > (PunyCodeStatic.MaxUint - i) / w) return PunyCodeStatic.OperationStatus.Overflow;
              i += (int)(digit * w);
              var t = k <= bias /* + tmin */ ? (uint)PunyCodeStatic.BootstringParams.Tmin :     /* +tmin not needed */
                  k >= bias + (uint)PunyCodeStatic.BootstringParams.Tmax 
                  ? (uint)PunyCodeStatic.BootstringParams.Tmax 
                  : k - bias;
              if (digit < t) break;
              if (w > PunyCodeStatic.MaxUint / ((uint)PunyCodeStatic.BootstringParams.Base - t)) 
                  return PunyCodeStatic.OperationStatus.Overflow;
              w *= ((uint)PunyCodeStatic.BootstringParams.Base - t);
            }

                bias = _punyCodeBaseCodeHelper.Adapt((uint)(i - (int)oldi), sout + 1, oldi == 0);

            /* i was supposed to wrap around from out+1 to 0,   */
            /* incrementing n each time, so we'll fix that now: */

            if (i / (sout + 1) > PunyCodeStatic.MaxUint - n) 
                return PunyCodeStatic.OperationStatus.Overflow;
            n += (uint)i / (sout + 1);
            i %= (int)(sout + 1);

            /* Insert n at position i of the output: */

            /* not needed for Punycode: */
            /* if (decode_digit(n) <= base) return punycode_invalid_input; */
            if (sout >= maxOut) return PunyCodeStatic.OperationStatus.BigOutput;

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
            return PunyCodeStatic.OperationStatus.Success;
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
