using System;
using System.Runtime.InteropServices.ComTypes;
using NUnit.Framework;
using PunyCode.Helper;

namespace Punycode.Tests
{
    [TestFixture]
    public class PunyCodeBaseCodeHelper
    {
        [Test]
        public void IsCharacterBasicReturnsTrue()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.IsCharacterBasic('v');
            const bool expectedResult = true;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void IsCharacterBasicReturnsFalse()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.IsCharacterBasic('ö');
            const bool expectedResult = false;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void IsCharacterDelimiterReturnsTrue()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.IsCharacterDelimiter('-');
            const bool expectedResult = true;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void IsCharacterDelimiterReturnsFalse()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.IsCharacterDelimiter('+');
            const bool expectedResult = false;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetAlphaNumericValueInBasicCodeReturnsIsBasicCodeAndIsUpperCaseLetterRange()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.GetAlphaNumericValueInBasicCode('Z');
            const uint expectedResult = 25;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetAlphaNumericValueInBasicCodeReturnsIsBasicCodeAndIsLowerCaseLetterRange()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.GetAlphaNumericValueInBasicCode('z');
            const uint expectedResult = 25;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetAlphaNumericValueInBasicCodeReturnsIsBasicCodeAndIsNaturalNumberRange()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.GetAlphaNumericValueInBasicCode('0');
            const uint expectedResult = 26;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetAlphaNumericValueInBasicCodeReturnsIsNonBasicCodeRange()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.GetAlphaNumericValueInBasicCode('?');
            const uint expectedResult = 36;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetAlphaNumericValueInBasicCodeReturnsIsGurkhumiCharacterRange()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.GetAlphaNumericValueInBasicCode('ਐ');
            const uint expectedResult = 36;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetAlphaNumericValueInBasicCodeReturnsIsMathematicSymbolCharacterRange()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.GetAlphaNumericValueInBasicCode('∈');
            const uint expectedResult = 36;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetEncodedToBasicCodeReturnsIsLowerCaseLetter()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.GetEncodedBasicCodeToAscii((char)2, true);
            const char expectedResult = 'c';

            Assert.AreEqual(expectedResult.ToString(), actualResult.ToString());
        }

        [Test]
        public void GetEncodedToBasicCodeReturnsIsUpperCaseLetter()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.GetEncodedBasicCodeToAscii((char)2, false);
            const char expectedResult = 'C';

            Assert.AreEqual(expectedResult.ToString(), actualResult.ToString());
        }

        [Test]
        public void GetEncodedToBasicCodeReturnsIsNumber()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.GetEncodedBasicCodeToAscii((char)26, true);
            const char expectedResult = '0';

            Assert.AreEqual(expectedResult.ToString(), actualResult.ToString());
        }

        [Test]
        public void GetEncodedToBasicCodeReturnsIsAboveRange()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.GetEncodedBasicCodeToAscii((char)2226, false);
            const char expectedResult = 'Z';

            Assert.AreEqual(expectedResult.ToString(), actualResult.ToString());
        }

        [Test]
        public void IsUpperCaseOrNumberIsUpperCase()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.IsUpperCaseOrNumber('A');
            const bool expectedResult = true;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void IsUpperCaseOrNumberIsNumber()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.IsUpperCaseOrNumber('9');
            const bool expectedResult = true;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void IsUpperCaseOrNumberIsLowerCase()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.IsUpperCaseOrNumber('z');
            const bool expectedResult = false;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void IsUpperCaseOrNumberIsOutOfAsciiRange()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.IsUpperCaseOrNumber('ਐ');
            const bool expectedResult = false;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetCharToUpperOrLowerCaseLowerToLowerCase()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.GetCharToUpperOrLowerCase('a',false);
            const char expectedResult = 'a';

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetCharToUpperOrLowerCaseLowerToUpperCase()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.GetCharToUpperOrLowerCase('a', true);
            const char expectedResult = 'A';

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetCharToUpperOrLowerCaseUpperToUpperCase()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.GetCharToUpperOrLowerCase('A', true);
            const char expectedResult = 'A';

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetCharToUpperOrLowerCaseUpperToLowerCase()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.GetCharToUpperOrLowerCase('a', true);
            const char expectedResult = 'A';

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetCharToUpperOrLowerCaseNumberToLowerCase()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.GetCharToUpperOrLowerCase('9', false);
            const char expectedResult = '9';

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetCharToUpperOrLowerCaseNumberToUpperCase()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.GetCharToUpperOrLowerCase('9', true);
            const char expectedResult = '9';

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetCharToUpperOrLowerCaseOutOfRangeToUpperCase()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.GetCharToUpperOrLowerCase('ਐ', true);
            const char expectedResult = 'ਐ';

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetCharToUpperOrLowerCaseOutOfRangeToLowerCase()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.GetCharToUpperOrLowerCase('ਐ', false);
            const char expectedResult = 'ਐ';

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void AdaptSmallFirstDeltaSmallNumpoint()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.Adapt(10,5,false);
            const uint expectedResult = 4;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void AdaptBigFirstDeltaSmallNumpoint()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.Adapt(20000, 5, false);
            const uint expectedResult = 68;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void AdaptSmallFirstDeltaBigNumpoint()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.Adapt(10, 50000, false);
            const uint expectedResult = 4;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void AdaptBigFirstDeltaBigNumpoint()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            var actualResult = punycodeImp.Adapt(20000, 50000, false);
            const uint expectedResult = 67;

            Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void AddAllAsciiCharsToOutBytesReturnsBigOutput()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();

            const string inputString = "abcdefghijklmnopqrstuvwxyz0123456789-";
            uint numberOfOutPutBytes;
            byte[] outBytes;
            var actualResult = punycodeImp.AddAllAsciiCharsToOutBytes(inputString, (uint)inputString.Length, PunyCodeStatic.MaxInputStringLenght,
                out numberOfOutPutBytes, out outBytes);
            const PunyCodeStatic.OperationStatus expectedResult = PunyCodeStatic.OperationStatus.Success;
            var expectedNumberOfOutBytes = inputString.Length;
            var expectedOutBytes = new byte[PunyCodeStatic.MaxInputStringLenght];
            var existingOutBytes = System.Text.Encoding.ASCII.GetBytes(inputString);
            Buffer.BlockCopy(existingOutBytes, 0, expectedOutBytes, 0, expectedNumberOfOutBytes);

            Assert.AreEqual(expectedResult, actualResult);
            Assert.AreEqual(expectedNumberOfOutBytes,numberOfOutPutBytes);
            Assert.AreEqual(outBytes.Length, expectedOutBytes.Length);
            Assert.AreSame(expectedOutBytes,outBytes);
        }
    }
}
