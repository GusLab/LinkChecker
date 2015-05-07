using NUnit.Framework;

namespace Punycode.Tests
{
    [TestFixture]
    public class PunyCodeBaseCodeHelper
    {
        [Test]
        public void IsCharacterBasicReturnsTrue()
        {
            var punycodeImp = new PunyCode.Helper.PunyCodeBaseCodeHelper();;

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
    }
}
