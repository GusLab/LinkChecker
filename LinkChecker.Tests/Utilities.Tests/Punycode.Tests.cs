using LinkChecker.Utilities;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NUnit.Framework;

namespace LinkChecker.Tests.Utilities.Tests
{
    [TestFixture]
    public class PunycodeTests
    {
        [Test]
        public void IsCharacterBasicReturnsTrue()
        {
            var privateStaticAccessor = new PrivateType(typeof(Punycode));

            var actualResult = (bool)privateStaticAccessor.InvokeStatic("IsCharacterBasic", 'v');
            const bool expectedResult = true;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void IsCharacterBasicReturnsFalse()
        {
            var privateStaticAccessor = new PrivateType(typeof(Punycode));

            var actualResult = (bool)privateStaticAccessor.InvokeStatic("IsCharacterBasic", 'ö');
            const bool expectedResult = false;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void IsCharacterDelimiterReturnsTrue()
        {
            var privateStaticAccessor = new PrivateType(typeof(Punycode));

            var actualResult = (bool)privateStaticAccessor.InvokeStatic("IsCharacterDelimiter", '-');
            const bool expectedResult = true;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void IsCharacterDelimiterReturnsFalse()
        {
            var privateStaticAccessor = new PrivateType(typeof(Punycode));

            var actualResult = (bool)privateStaticAccessor.InvokeStatic("IsCharacterDelimiter", '+');
            const bool expectedResult = false;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetNumericValueInBasicCodeReturnsIsBasicCodeAndIsUpperCaseLetterRange()
        {
            var privateStaticAccessor = new PrivateType(typeof(Punycode));

            var actualResult = (uint)privateStaticAccessor.InvokeStatic("GetNumericValueInBasicCode", 'Z');
            const uint expectedResult = 25;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetNumericValueInBasicCodeReturnsIsBasicCodeAndIsLowerCaseLetterRange()
        {
            var privateStaticAccessor = new PrivateType(typeof(Punycode));

            var actualResult = (uint)privateStaticAccessor.InvokeStatic("GetNumericValueInBasicCode", 'z');
            const uint expectedResult = 25;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetNumericValueInBasicCodeReturnsIsBasicCodeAndIsNaturalNumberRange()
        {
            var privateStaticAccessor = new PrivateType(typeof(Punycode));

            var actualResult = (uint)privateStaticAccessor.InvokeStatic("GetNumericValueInBasicCode", '0');
            const uint expectedResult = 26;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetNumericValueInBasicCodeReturnsIsNonBasicCodeRange()
        {
            var privateStaticAccessor = new PrivateType(typeof(Punycode));

            var actualResult = (uint)privateStaticAccessor.InvokeStatic("GetNumericValueInBasicCode", '?');
            const uint expectedResult = 36;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetNumericValueInBasicCodeReturnsIsGurkhumiCharacterRange()
        {
            var privateStaticAccessor = new PrivateType(typeof(Punycode));

            var actualResult = (uint)privateStaticAccessor.InvokeStatic("GetNumericValueInBasicCode", 'ਐ');
            const uint expectedResult = 36;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetNumericValueInBasicCodeReturnsIsMathematicSymbolCharacterRange()
        {
            var privateStaticAccessor = new PrivateType(typeof(Punycode));

            var actualResult = (uint)privateStaticAccessor.InvokeStatic("GetNumericValueInBasicCode", '∈');
            const uint expectedResult = 36;

            NUnit.Framework.Assert.AreEqual(expectedResult, actualResult);
        }

        [Test]
        public void GetEncodedToBasicCodeReturnsIsLowerCaseLetter()
        {
            var privateStaticAccessor = new PrivateType(typeof(Punycode));

            var actualResult = (char)privateStaticAccessor.InvokeStatic("GetEncodedToBasicCode", uint.MaxValue, false);
            const char expectedResult = 'c';

            NUnit.Framework.StringAssert.AreNotEqualIgnoringCase(expectedResult.ToString(), actualResult.ToString());
        }
    }
}
