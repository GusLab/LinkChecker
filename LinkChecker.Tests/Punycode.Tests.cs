using NUnit.Framework;

namespace Punycode.Tests
{
    [TestFixture]
    public class PunycodeTests
    {
        [Test]
        public void IsCharacterBasicReturnsTrue()
        {
            var punycodeImp = new PunyCode.Punycode();;

            var actualResult = punycodeImp.IsCharacterBasic('v');
            const bool expectedResult = true;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void IsCharacterBasicReturnsFalse()
        {
            var punycodeImp = new PunyCode.Punycode();

            var actualResult = punycodeImp.IsCharacterBasic('ö');
            const bool expectedResult = false;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void IsCharacterDelimiterReturnsTrue()
        {
            var punycodeImp = new PunyCode.Punycode();

            var actualResult = punycodeImp.IsCharacterDelimiter('-');
            const bool expectedResult = true;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void IsCharacterDelimiterReturnsFalse()
        {
            var punycodeImp = new PunyCode.Punycode();

            var actualResult = punycodeImp.IsCharacterDelimiter('+');
            const bool expectedResult = false;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetAlphaNumericValueInBasicCodeReturnsIsBasicCodeAndIsUpperCaseLetterRange()
        {
            var punycodeImp = new PunyCode.Punycode();

            var actualResult = punycodeImp.GetAlphaNumericValueInBasicCode('Z');
            const uint expectedResult = 25;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetAlphaNumericValueInBasicCodeReturnsIsBasicCodeAndIsLowerCaseLetterRange()
        {
            var punycodeImp = new PunyCode.Punycode();

            var actualResult = punycodeImp.GetAlphaNumericValueInBasicCode('z');
            const uint expectedResult = 25;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetAlphaNumericValueInBasicCodeReturnsIsBasicCodeAndIsNaturalNumberRange()
        {
            var punycodeImp = new PunyCode.Punycode();

            var actualResult = punycodeImp.GetAlphaNumericValueInBasicCode('0');
            const uint expectedResult = 26;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetAlphaNumericValueInBasicCodeReturnsIsNonBasicCodeRange()
        {
            var punycodeImp = new PunyCode.Punycode();

            var actualResult = punycodeImp.GetAlphaNumericValueInBasicCode('?');
            const uint expectedResult = 36;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetAlphaNumericValueInBasicCodeReturnsIsGurkhumiCharacterRange()
        {
            var punycodeImp = new PunyCode.Punycode();

            var actualResult = punycodeImp.GetAlphaNumericValueInBasicCode('ਐ');
            const uint expectedResult = 36;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetAlphaNumericValueInBasicCodeReturnsIsMathematicSymbolCharacterRange()
        {
            var punycodeImp = new PunyCode.Punycode();

            var actualResult = punycodeImp.GetAlphaNumericValueInBasicCode('∈');
            const uint expectedResult = 36;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetEncodedToBasicCodeReturnsIsLowerCaseLetter()
        {
            var punycodeImp = new PunyCode.Punycode();

            var actualResult = punycodeImp.GetEncodedBasicCodeToAscii((char)2, true);
            const char expectedResult = 'c';

            NUnit.Framework.Assert.AreEqual(expectedResult.ToString(), actualResult.ToString());
        }

        [Test]
        public void GetEncodedToBasicCodeReturnsIsUpperCaseLetter()
        {
            var punycodeImp = new PunyCode.Punycode();

            var actualResult = punycodeImp.GetEncodedBasicCodeToAscii((char)2, false);
            const char expectedResult = 'C';

            NUnit.Framework.Assert.AreEqual(expectedResult.ToString(), actualResult.ToString());
        }

        [Test]
        public void GetEncodedToBasicCodeReturnsIsNumber()
        {
            var punycodeImp = new PunyCode.Punycode();

            var actualResult = punycodeImp.GetEncodedBasicCodeToAscii((char)26, true);
            const char expectedResult = '0';

            NUnit.Framework.Assert.AreEqual(expectedResult.ToString(), actualResult.ToString());
        }

        [Test]
        public void GetEncodedToBasicCodeReturnsIsAboveRange()
        {
            var punycodeImp = new PunyCode.Punycode();

            var actualResult = punycodeImp.GetEncodedBasicCodeToAscii((char)2226, false);
            const char expectedResult = 'Z';

            NUnit.Framework.Assert.AreEqual(expectedResult.ToString(), actualResult.ToString());
        }
    }
}
