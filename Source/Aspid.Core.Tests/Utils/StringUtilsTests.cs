#region License
#endregion

using System;
using System.Linq;
using System.Globalization;
using System.Threading;

using NUnit.Framework;

namespace Aspid.Core.Utils.Tests
{
    [TestFixture]
    public class StringUtilsTests
    {
        [Test]
        public void InvariantFormat_GivenANullFormat_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => StringUtils.InvariantFormat(null, "anything"));
        }

        [Test]
        public void InvariantFormat_GivenACultureDependantFormat_ReturnsTheStringFormattedUsingInvariantCulture()
        {
            //Get some culture and set it as the current culture changing the negative symbol and checking that it works (preparing for the test)
            var currentCulture = CultureInfo.GetCultures(CultureTypes.InstalledWin32Cultures)[0];
            currentCulture.NumberFormat.NegativeSign = "+";
            Thread.CurrentThread.CurrentCulture = currentCulture;
            Assert.AreEqual("+1", string.Format("{0}", -1));

            //Make sure that we get an invariant culture representation when using InvariantFormat                      
            Assert.AreEqual(string.Format(CultureInfo.InvariantCulture, "{0}", -1), StringUtils.InvariantFormat("{0}", -1));
        }

        [Test]
        public void SafeRemove_GivenAStringLongerThanStartIndex_ReturnsTheTruncatedString()
        {
            Assert.AreEqual("123", StringUtils.SafeRemove("1234567890", 3));
        }

        [Test]
        public void SafeRemove_GivenAStringOfSameLengthThanStartIndex_ReturnsTheEntireGivenString()
        {
            const string testString = "1234567890";
            Assert.AreEqual(testString, StringUtils.SafeRemove(testString, testString.Length));
        }

        [Test]
        public void SafeRemove_GivenAStringShorterThanStartIndex_ReturnsTheEntireGivenString()
        {
            const string testString = "1234567890";
            Assert.AreEqual(testString, StringUtils.SafeRemove(testString, testString.Length + 1));
        }

        [Test]
        public void SafeRemove_GivenANullString_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>(() => StringUtils.SafeRemove(null, 0));
        }

        [Test]
        public void SafeRemove_GivenANegativeStartIndex_ThrowsArgumentOutOfRangeException()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => StringUtils.SafeRemove(string.Empty, -1));
        }

        [Test]
        public void SafeRemove_GivenAnStartIndexOfZero_ReturnsTheEmptyString()
        {
            Assert.AreEqual(string.Empty, StringUtils.SafeRemove("1234567890", 0));
        }

        [Test]
        public void SafeTrim_GivenANullString_ReturnsNull()
        {
            Assert.IsNull(StringUtils.SafeTrim(null));
        }

        [Test]
        public void SafeTrim_GivenAnEmptyString_ReturnsTheEmptyString()
        {
            Assert.AreEqual(string.Empty, StringUtils.SafeTrim(string.Empty));
        }

        [Test]
        public void SafeTrim_GivenANonEmptyStringThatDoesNotNeedTrimming_ReturnsTheGivenString()
        {
            const string testString = "This string does not need trimming";
            Assert.AreEqual(testString, StringUtils.SafeTrim(testString));
        }

        [Test]
        public void SafeTrim_GivenANonEmptyStringThatNeedsTrimming_ReturnsTheGivenString()
        {
            const string testString = "   This string needs trimming   ";
            Assert.AreEqual(testString.Trim(), StringUtils.SafeTrim(testString));
        }

        [Test]
        public void ContainsAny_WithNullText_ReturnsFalse()
        {
            var values = new[] { "v1", "v2", "v3" };
            Assert.IsFalse(StringUtils.ContainsAny(null, values));
        }

        [Test]
        public void ContainsAny_WithEmptyText_ReturnsFalse()
        {
            var values = new[] { "v1", "v2", "v3" };
            Assert.IsFalse(StringUtils.ContainsAny(string.Empty, values));
        }

        [Test]
        public void ContainsAny_WithEmptyValuesList_ReturnsTrue()
        {
            var values = new string[] {};
            Assert.IsTrue(StringUtils.ContainsAny("something", values));
        }

        [Test]
        public void ContainsAny_WithNullValuesList_ThrowsArgumentNullException()
        {
            string[] values = null;
            Assert.Throws<ArgumentNullException>(() => StringUtils.ContainsAny("something", values));
        }

        [Test]
        public void ContainsAny_WithEmptyStringAsValue_ReturnsTrue()
        {
            var values = new string[] { "" };
            Assert.IsTrue(StringUtils.ContainsAny("something", values));
        }

        [Test]
        public void CaseSplit_WhenGivenStringIsAllLowerCase_ReturnsArrayWithOnlyTheGivenString()
        {
            string lowerCaseString = "something";
            var result = StringUtils.CaseSplit(lowerCaseString);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(lowerCaseString, result[0]);
        }

        [Test]
        public void CaseSplit_WhenGivenStringIsAllUpperCase_ReturnsArrayWithOnlyTheGivenString()
        {
            string upperCaseString = "HTML";
            var result = StringUtils.CaseSplit(upperCaseString);

            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(upperCaseString, result[0]);
        }

        [Test]
        public void CaseSplit_WhenGivenCommonPascalCaseString_ReturnsArrayWithCorrectStringElements()
        {
            const string pascalCaseString = "ThisIsPascalCase";
            var result = StringUtils.CaseSplit(pascalCaseString);

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("This", result[0]);
            Assert.AreEqual("Is", result[1]);
            Assert.AreEqual("Pascal", result[2]);
            Assert.AreEqual("Case", result[3]);
        }

        [Test]
        public void CaseSplit_WhenGivenCommonCamelCaseString_ReturnsArrayWithCorrectStringElements()
        {
            const string camelCaseString = "thisIsCamelCase";
            var result = StringUtils.CaseSplit(camelCaseString);

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("this", result[0]);
            Assert.AreEqual("Is", result[1]);
            Assert.AreEqual("Camel", result[2]);
            Assert.AreEqual("Case", result[3]);
        }

        /// <summary>
        /// Whatch out for this behavior, as it may seem a little unexpected.
        /// These cases are handled like this to support acronyms inside text, ej. "GreatHTMLParser"
        /// </summary>
        [Test]
        public void CaseSplit_WhenStringHasUpperCaseFollowedByLowerCase_TheLastUpperCaseCharacterIsTakenAsTheFirstOfTheNextElement()
        {
            const string mixedString = "UPPERlow";
            var result = StringUtils.CaseSplit(mixedString);

            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("UPPE", result[0]);
            Assert.AreEqual("Rlow", result[1]);
        }

        [Test]
        public void CaseSplit_WhenGivenMixedCaseString_ReturnsArrayWithCorrectStringElements()
        {
            const string mixedCaseString = "GreatHTMLParser";
            var result = StringUtils.CaseSplit(mixedCaseString);

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("Great", result[0]);
            Assert.AreEqual("HTML", result[1]);
            Assert.AreEqual("Parser", result[2]);
        }

        [Test]
        public void CaseSplit_WhenGivenStringWithNumbersInTheMiddle_ReturnsArrayWithCorrectStringElements()
        {
            const string stringWithNumber = "ThisContains1Number";
            var result = StringUtils.CaseSplit(stringWithNumber);

            Assert.AreEqual(4, result.Count);
            Assert.AreEqual("This", result[0]);
            Assert.AreEqual("Contains", result[1]);
            Assert.AreEqual("1", result[2]);
            Assert.AreEqual("Number", result[3]);
        }

        [Test]
        public void CaseSplit_WhenGivenStringWithNumbersAtTheEnd_ReturnsArrayWithCorrectStringElements()
        {
            const string stringWithNumber = "ThisContains1";
            var result = StringUtils.CaseSplit(stringWithNumber);

            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("This", result[0]);
            Assert.AreEqual("Contains", result[1]);
            Assert.AreEqual("1", result[2]);
        }

        [Test]
        public void CaseSeparate_WhenGivenStringIsAllLowerCase_ReturnsTheGivenString()
        {
            string lowerCaseString = "something";
            Assert.AreEqual(lowerCaseString, StringUtils.CaseSeparate(lowerCaseString, ", "));
        }

        [Test]
        public void CaseSeparate_WhenGivenStringIsAllUpperCase_GivenString()
        {
            string upperCaseString = "HTML";
            Assert.AreEqual(upperCaseString, StringUtils.CaseSeparate(upperCaseString));
        }

        [Test]
        public void CaseSeparate_WhenGivenCommonPascalCaseString_ReturnsSeparatedString()
        {
            const string pascalCaseString = "ThisIsPascalCase";
            var result = StringUtils.CaseSeparate(pascalCaseString);

            Assert.AreEqual("This Is Pascal Case", result);
        }

        [Test]
        public void CaseSeparate_WhenGivenCommonCamelCaseString_ReturnsSeparatedString()
        {
            const string camelCaseString = "thisIsCamelCase";
            var result = StringUtils.CaseSeparate(camelCaseString, ", ");

            Assert.AreEqual("this, Is, Camel, Case", result);
        }

        [Test]
        public void CaseSeparate_WhenGivenMixedCaseString_ReturnsCorrectlySeparatedString()
        {
            //CaseSeparate basically uses CaseSplit internally and then Joins, so check tests for those methods to find out peculiarities, if any.
            const string mixedCaseString = "GreatHTMLParser";
            var result = StringUtils.CaseSeparate(mixedCaseString, "! ");
           
            Assert.AreEqual("Great! HTML! Parser", result);
        }
        
        [Test]
        public void SubstringSearch_WhenNoSubstringFoundAndNoIgnoredChars_ReturnsEmpty()
        {
            const string text = "SomeString";
            const string substring = "DoesNotExist";

            Assert.AreEqual(string.Empty, StringUtils.SubstringSearch(text, substring));
        }

        [Test]
        public void SubstringSearch_WhenSubstringFoundAndNoIgnoredChars_ReturnsFoundSubstring()
        {
            const string text = "SomeString";
            const string substring = "Str";

            Assert.AreEqual(substring, StringUtils.SubstringSearch(text, substring));
        }

        [Test]
        public void SubstringSearch_WhenSubstringFoundWhileIgnoringCharsButIgnoredCharsAreNotPresent_ReturnsFoundSubstring()
        {
            const string text = "SomeString";
            const string substring = "Str";
            var ignoredChars = new[] { '/', '*', '(' };

            Assert.AreEqual(substring, StringUtils.SubstringSearch(text, substring, ignoredChars));
        }

        [Test]
        public void SubstringSearch_WhenSubstringFoundWhileIgnoringChars_ReturnsFoundSubstring()
        {
            const string text = "Some(S/t*/(*ring";
            const string substring = "Str";
            const string substringToFind = "S/t*/(*r";
            var ignoredChars = new[] { '/', '*', '(' };

            Assert.AreEqual(substringToFind, StringUtils.SubstringSearch(text, substring, ignoredChars));
        }

        [Test]
        public void SubstringSearch_WhenSubstringFoundWhileIgnoringChars_ReturnsFoundSubstringWithoutTrailingIgnoredChars()
        {
            //This is to make sure implementation is not returning "S/t*/(*r/" in this case.
            const string text = "Some(S/t*/(*r/ing";
            const string substring = "Str";
            const string substringToFind = "S/t*/(*r";
            var ignoredChars = new[] { '/', '*', '(' };

            Assert.AreEqual(substringToFind, StringUtils.SubstringSearch(text, substring, ignoredChars));
        }

        [Test]
        public void SubstringSearch_WhenSubstringFoundWhileIgnoringCharsAndSubstringToFindHasIgnoredCharsOnIt_ReturnsFoundSubstring()
        {
            const string text = "Some(S/t*/(*ring";
            const string substring = "S/t/*r";
            const string substringToFind = "S/t*/(*r";
            var ignoredChars = new[] { '/', '*', '(' };

            Assert.AreEqual(substringToFind, StringUtils.SubstringSearch(text, substring, ignoredChars));
        }

        [Test]
        public void SubstringSearch_WhenDashIsAnIgnoredChar_ShouldNotTakeTheDashAsARange()
        {
            //To make sure that impl. using regular expressions handle this correctly
            const string text = "SomeStbring";
            const string substring = "Str";
            var ignoredChars = new[] { 'a', '-', 'z' };

            Assert.AreEqual(string.Empty, StringUtils.SubstringSearch(text, substring, ignoredChars));
        }

        ///// <summary>
        ///// Should pass stack overflow question use cases and other sanity checks.
        ///// </summary>
        ///// <remarks>http://stackoverflow.com/questions/2592613/find-substring-ignoring-specified-characters</remarks>
        [Test]
        public void SubstringSearch_ShouldPassCommonCasesAndSanityChecks()
        {
            Assert.AreEqual("Hello, -this", StringUtils.SubstringSearch("Hello, -this- is a string", "Hello this", new[] { ',', '-' }));
            Assert.AreEqual("A&3/3/C)41", StringUtils.SubstringSearch("?A&3/3/C)412&", "A41", new[] { '&', '/', '3', 'C', ')' }));
            Assert.AreEqual("A&3/3/C)412&", StringUtils.SubstringSearch("?A&3/3/C)412&", "A3C412&", new[] { '&', '/', '3', 'C', ')' }));
            Assert.AreEqual("10/MO", StringUtils.SubstringSearch("10/MO33433243", "10mo", ".-/;{}[]()&%$*+=_'?\\<>|,".ToArray()));
            Assert.AreEqual("123.456", StringUtils.SubstringSearch("123.456.322.545", "123456", ".-/;{}[]()&%$*+=_'?\\<>|,".ToArray()));
            Assert.AreEqual("123.456", StringUtils.SubstringSearch("123.456.322.545", "123.456", ".-/;{}[]()&%$*+=_'?\\<>|,".ToArray()));
            Assert.AreEqual("MOE.456.322", StringUtils.SubstringSearch("MOE.456.322.545", "moe456.322", ".-/;{}[]()&%$*+=_'?\\<>|,".ToArray()));
            Assert.AreEqual("MOE.456.322", StringUtils.SubstringSearch("MOE.456.322.545", "moe.456.322"));
        }

        [Test]
        public void Capitalize_GivenNullString_ReturnsEmptyString()
        {
            Assert.AreEqual(string.Empty, StringUtils.Capitalize((string)null));
        }

        [Test]
        public void Capitalize_GivenEmptyString_ReturnsEmptyString()
        {
            Assert.AreEqual(string.Empty, StringUtils.Capitalize(string.Empty));
        }

        [Test]
        public void Capitalize_GivenStringConsistingOfUpperCaseCharacter_ReturnsSameString()
        {
            const string testCharacter = "J";
            Assert.AreEqual(testCharacter, StringUtils.Capitalize(testCharacter));
        }

        [Test]
        public void Capitalize_GivenStringConsistingOfLowerCaseCharacter_ReturnsSameCharacterUpperCased()
        {
            const string testCharacter = "j";
            Assert.AreEqual(testCharacter.ToUpper(), StringUtils.Capitalize(testCharacter));
        }

        [Test]
        public void Capitalize_GivenMixedCaseString_ReturnsCorrectlyCapitalizedString()
        {
            Assert.AreEqual("This Is Capitalized", StringUtils.Capitalize("tHIs IS CAPitaLiZED"));
            Assert.AreEqual("Frank Ty. Coon", StringUtils.Capitalize("FRAnk tY. COON"));
            Assert.AreEqual("Te$t Wit|-| Symb0ls", StringUtils.Capitalize("Te$T Wit|-| SYmB0ls"));
        }

        [Test]
        public void Capitalize_GivenStringSeparatedByMultipleSpaces_PreservesSpaces()
        {
            Assert.AreEqual("This    Is        Capitalized", StringUtils.Capitalize("tHIs    IS        CAPitaLiZED"));
        }
    }
}
