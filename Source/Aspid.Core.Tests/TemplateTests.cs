using System;

using NUnit.Framework;

namespace Aspid.Core.Tests
{
    [TestFixture]
    public class StringplateTests
    {
        class TestObject
        {
            public string NullString { get; private set; }
            public string NotNullString { get { return "NotNull"; } }
            public DateTime? NullNullableDate { get { return null; } }
            public DateTime? NotNullNullableDate { get { return DateTime.MinValue; } }
        }

        [Test]
        public void ShouldReturnEmptyStringForEmptyTemplate()
        {
            Assert.AreEqual("", Template.Parse(""));
        }

        [Test]
        public void ShouldReturnGivenStringForParameterlessTemplate()
        {
            Assert.AreEqual("GivenString", Template.Parse("GivenString"));
        }

        [Test]
        public void ShouldReturnEmptyStringForUnboundParameter()
        {
            Assert.AreEqual("", Template.Parse("{}"));
        }

        [Test]
        public void ShouldReturnEmptyStringForUnboundNamedParameter()
        {
            Assert.AreEqual("", Template.Parse("{$Name}"));
        }

        [Test]
        public void ShouldReturnTextForUnboundParameterAndText()
        {
            Assert.AreEqual("GivenString", Template.Parse("Given{}String"));
        }

        [Test]
        public void ShouldReturnTextForUnboundNamedParameterAndText()
        {
            Assert.AreEqual("GivenString", Template.Parse("Given{$Name}String"));
        }

        [Test]
        public void ShouldBeAbleToBindToAPropertyOfAGivenObject()
        {
            Assert.AreEqual("GivenSomeString", Template.Parse("Given{$SomeProperty}String", new { SomeProperty = "Some" }));
        }

        [Test]
        public void ShouldBeAbleToBindToMultiplePropertiesOfAGivenObject()
        {
            Assert.AreEqual("GivenSomeLongString",
                Template.Parse("Given{$SomeProperty}{$AnotherProperty}String{$ThirdProperty}",
                new { SomeProperty = "Some", AnotherProperty = "Long", ThirdProperty = "" }));
        }

        [Test]
        public void ShouldBeAbleToBindToMultipleObjects()
        {
            Assert.AreEqual("GivenSomeGreatStringLength",
                Template.Parse("Given{$SomeProperty}{$AnotherObjectProperty}String{$AnotherProperty}",
                new
                {
                    SomeProperty = "Some",
                    AnotherProperty = "Length",
                },
                new
                {
                    AnotherObjectProperty = "Great"
                }));
        }

        [Test]
        public void ShouldBeAbleToBindToMultiplePropertiesOfAGivenObjectAndReturnEmptyForUnexistentOnes()
        {
            Assert.AreEqual("GivenSomeStringValue",
                Template.Parse("Given{$SomeProperty}{$UnexistentProperty}String{$ThirdProperty}",
                new { SomeProperty = "Some", AnotherProperty = "Long", ThirdProperty = "Value" }));
        }

        [Test]
        public void ShouldBeAbleToBindSubPropertiesOfGivenObjects()
        {
            Assert.AreEqual("GivenSomeStringValue",
                Template.Parse("Given{$SomeProperty.SubProperty}Value",
                new { SomeProperty = new { SubProperty = "SomeString" } }));
        }

        [Test]
        public void ShouldReturnEmptyForNullParameter()
        {
            var test = new TestObject();
            Assert.AreEqual("GivenString", Template.Parse("Given{$NullString}String", test));
        }

        [Test]
        public void ShouldReturnEmptyForNullParameterEvenWithSubProperty()
        {
            var test = new TestObject();
            Assert.AreEqual("GivenString", Template.Parse("Given{$NullString.Length}String", test));
        }

        [Test]
        public void ShouldBeAbleToBindTextAsParameter()
        {
            Assert.AreEqual("Given Some 10 Char String", Template.Parse("Given{ Some 10 Char }String"));
        }

        [Test]
        public void ShouldBeAbleToMixTextAndPropertyBindingInsideParameter()
        {
            Assert.AreEqual("Given Some 4 Char String", Template.Parse("Given{ Some $SomeProperty.Length Char }String", new { SomeProperty = "1234" }));
        }

        [Test]
        public void ShouldNotReplacePropertyCharactersOutsideParameters()
        {
            Assert.AreEqual("Given Some $SomeProperty String", Template.Parse("Given {$SomeProperty} $SomeProperty String", new { SomeProperty = "Some" }));
        }

        [Test]
        public void ShouldBeAbleToBindMultipleTmeToTheSamePropertyOnDiffrentParameters()
        {
            Assert.AreEqual("Given Some And Some String", Template.Parse("Given {$SomeProperty} {And $SomeProperty} String", new { SomeProperty = "Some" }));
        }

        [Test]
        public void ShouldBeAbleToBindMultiplePropertiesInsideSameParameter()
        {
            Assert.AreEqual("Given1234 0String", Template.Parse("Given{$SomeProperty $OtherProperty.Length}String", new { SomeProperty = "1234", OtherProperty = "" }));
        }

        [Test]
        public void ShouldBeAbleToBindSamePropertyMultipleTimesInsideSameParameter()
        {
            Assert.AreEqual("Given1234 4String", Template.Parse("Given{$SomeProperty $SomeProperty.Length}String", new { SomeProperty = "1234" }));
        }

        [Test]
        public void ShouldBeAbleToUseCommonTextSeparatorsForBindedProperties()
        {
            Assert.AreEqual("Testing Separators: OK?", Template.Parse("Testing Separators: {$SomeProperty?}", new { SomeProperty = "OK" }));
            Assert.AreEqual("Testing Separators: OK!", Template.Parse("Testing Separators: {$SomeProperty!}", new { SomeProperty = "OK" }));
            Assert.AreEqual("Testing Separators: OK.", Template.Parse("Testing Separators: {$SomeProperty.}", new { SomeProperty = "OK" }));
            Assert.AreEqual("Testing Separators: OK,", Template.Parse("Testing Separators: {$SomeProperty,}", new { SomeProperty = "OK" }));
            Assert.AreEqual("Testing Separators: OK;", Template.Parse("Testing Separators: {$SomeProperty;}", new { SomeProperty = "OK" }));
            Assert.AreEqual("Testing Separators: OK-", Template.Parse("Testing Separators: {$SomeProperty-}", new { SomeProperty = "OK" }));
            Assert.AreEqual("Testing Separators: OK/", Template.Parse("Testing Separators: {$SomeProperty/}", new { SomeProperty = "OK" }));
            Assert.AreEqual("Testing Separators: OK*", Template.Parse("Testing Separators: {$SomeProperty*}", new { SomeProperty = "OK" }));
            Assert.AreEqual("Testing Separators: OK+", Template.Parse("Testing Separators: {$SomeProperty+}", new { SomeProperty = "OK" }));
            Assert.AreEqual("Testing Separators: OK ", Template.Parse("Testing Separators: {$SomeProperty }", new { SomeProperty = "OK" }));

            Assert.AreEqual("Testing Separators: ?OK", Template.Parse("Testing Separators: {?$SomeProperty}", new { SomeProperty = "OK" }));
            Assert.AreEqual("Testing Separators: !OK", Template.Parse("Testing Separators: {!$SomeProperty}", new { SomeProperty = "OK" }));
            Assert.AreEqual("Testing Separators: .OK", Template.Parse("Testing Separators: {.$SomeProperty}", new { SomeProperty = "OK" }));
            Assert.AreEqual("Testing Separators: ,OK", Template.Parse("Testing Separators: {,$SomeProperty}", new { SomeProperty = "OK" }));
            Assert.AreEqual("Testing Separators: ;OK", Template.Parse("Testing Separators: {;$SomeProperty}", new { SomeProperty = "OK" }));
            Assert.AreEqual("Testing Separators: -OK", Template.Parse("Testing Separators: {-$SomeProperty}", new { SomeProperty = "OK" }));
            Assert.AreEqual("Testing Separators: /OK", Template.Parse("Testing Separators: {/$SomeProperty}", new { SomeProperty = "OK" }));
            Assert.AreEqual("Testing Separators: *OK", Template.Parse("Testing Separators: {*$SomeProperty}", new { SomeProperty = "OK" }));
            Assert.AreEqual("Testing Separators: +OK", Template.Parse("Testing Separators: {+$SomeProperty}", new { SomeProperty = "OK" }));
            Assert.AreEqual("Testing Separators:  OK", Template.Parse("Testing Separators: { $SomeProperty}", new { SomeProperty = "OK" }));
        }

        [Test]
        public void ShouldBeAbleToBindSamePropertyMultipleTimesInsideSameParameterAlsoWithText()
        {
            Assert.AreEqual("There are one string, the first one is: 1, and the second one is: 1", Template.Parse("There are one string, {the first one is: $SomeProperty, and the second one is: $SomeProperty}", new { SomeProperty = "1" }));
        }

        [Test]
        public void ShouldReturnEmptyTextForAParameterWhenAParameterPropertyIsNull()
        {
            var test = new TestObject();
            Assert.AreEqual("This should be empty: ", Template.Parse("This should be empty: { But is it? -$NullString}", test));
        }

        [Test]
        public void ShouldReturnEmptyTextForAParameterWhenAnyParameterPropertyIsNull()
        {
            var test = new TestObject();
            Assert.AreEqual("This should be empty: ", Template.Parse("This should be empty: { But is it? -$NullString $NotNullString}", test));
        }

        [Test]
        public void ShouldRecongnizeANullNullableTypeAsANullParameterValue()
        {
            var test = new TestObject();
            Assert.AreEqual("This should be empty too: , right?", Template.Parse("This should be empty too: { But is it? $Date $NotNullString}, right?", new { Date = test.NullNullableDate, NotNullString = "123" }));
        }

        [Test]
        public void ShouldRecongnizeANotNullNullableTypeAsANotNullParameterValue()
        {
            var test = new TestObject();
            Assert.AreEqual(String.Format("This shouldn't be empty: {0}, right?", test.NotNullNullableDate), Template.Parse("This shouldn't be empty: {$Date$NotNullString}, right?", new { Date = test.NotNullNullableDate, NotNullString = "" }));
        }

        [Test]
        public void ShouldAcceptMultipleAdjacentPropertiesOnAParameter()
        {
            Assert.AreEqual("OneTwoThree", Template.Parse("{$_1$_2$_3}", new { _1 = "One", _2 = "Two", _3 = "Three" }));
        }

        [Test]
        public void ShouldReturnEmptyTextForAnUnnamedArgument()
        {
            Assert.AreEqual("SomeString", Template.Parse("Some{$}String"));
        }

        [Test]
        public void ShouldReturnEmptyTextForMalformedArguments()
        {
            Assert.AreEqual("SomeString", Template.Parse("Some{$.SomeProperty}String", new { SomeProperty = "SomeProperty" }));
            Assert.AreEqual("SomeString", Template.Parse("Some{$*--;/}String", new { SomeProperty = "SomeProperty" }));
        }

        [Test]
        [ExpectedException(typeof(ArgumentNullException))]
        public void ShouldNotAllowNullParseOptions()
        {
            Template.Parse("Something", (ParseOptions)null);
        }

        [Test]
        public void ShouldUseProvidedStringsToConsiderAsNullAsIfTheyWereNull()
        {
            Assert.AreEqual("Something", Template.Parse("Some{$IExist}thing", new ParseOptions { ConsiderAsNull = new[] { "ImNull" } }, new { IExist = "ImNull" }));
        }

        [Test]
        public void ShouldNotTreatEmptyStringAsNullByDefault()
        {
            Assert.AreEqual("SomeNicething", Template.Parse("Some{Nice$IExist}thing", new { IExist = "" }));
        }

        [Test]
        public void ShouldAcceptANullReferenceWithoutBeignAnError()
        {
            object item = null;
            Assert.AreEqual("Something", Template.Parse("Some{$No}thing", item));
        }

        [Test]
        public void ShouldSupportParametersThatSpanThroughNewLines()
        {
            Assert.AreEqual("Some\nGreat\nthing", Template.Parse("Some{\n$Stuff\n}thing", new { Stuff = "Great" }));
        }

        [Test]
        public void ShouldNotSwallowNewLinesAfterEmptyParametersIfThatOptionIsNotSet()
        {
            Assert.AreEqual("Some\nthing", Template.Parse("Some{$IDontExist}\nthing", new ParseOptions { SwallowNewlineOnEmptyParameter = false }, new {}));
        }

        [Test]
        public void ShouldSwallowNewLinesAfterEmptyParametersIfThatOptionIsSet()
        {
            Assert.AreEqual("Something", Template.Parse("Some{$IDontExist}\nthing", new ParseOptions { SwallowNewlineOnEmptyParameter = true }, new {}));
        }

        [Test]
        public void ShouldSwallowNewLinesAfterEmptyParametersIfThatOptionIsSetEvenIfTheyAreTheLastCharacter()
        {
            Assert.AreEqual("Some", Template.Parse("Some{$IDontExist}\n", new ParseOptions { SwallowNewlineOnEmptyParameter = true }, new { }));
        }

        [Test]
        public void ShouldPreserveSpacesAfterParameters()
        {
            Assert.AreEqual("Some1   a", Template.Parse("Some{$IExist}   a", new { IExist = "1" }));
        }

        [Test]
        public void ShouldPreserveSpacesAfterParametersEvenWhenSwallowingNewLines()
        {
            Assert.AreEqual("Some   a", Template.Parse("Some{$IExist}   a", new ParseOptions { SwallowNewlineOnEmptyParameter = true }, new {}));
        }

        [Test]
        public void ShouldSupportEmptyingParameterOnHTMLCommentsScenario()
        {
            string htmlComment = "<!--{-->" +
                                 "<tr>\n" +
                                 "\t<td style=\"background-color:#f5f5f5; border: solid 1px #d3e6ff; font-weight:bold;\"> Note: </td>\n" +
                                 "\t<td style=\"border: solid 1px #d3e6ff;\"> $CO.Text </td>\n" +
                                 "</tr>\n" +
                                 "<!--}-->";

            Assert.AreEqual("<!---->", Template.Parse(htmlComment, new { CO = new { Text = (string)null } }));
        }

        [Test]
        public void ShouldSwallowNewLinesAfterEmptyParametersIfThatOptionIsSetEvenIfSeparatedBySpaces()
        {
            Assert.AreEqual("Some", Template.Parse("Some{$IDontExist}   \n", new ParseOptions { SwallowNewlineOnEmptyParameter = true }, new { }));
        }

        [Test]
        public void ShouldNotSwallowNewLinesAfterEmptyParametersIfThatOptionIsSetAndParameterIsNotNull()
        {
            Assert.AreEqual("Some   \n", Template.Parse("Some{$IExist}   \n", new ParseOptions { SwallowNewlineOnEmptyParameter = true }, new { IExist = "" }));
        }

        [Test]
        public void ShouldBeAbleToParseDates()
        {
            var now = DateTime.Now;
            Assert.AreEqual("Prefix " + now.ToString() + " Suffix", Template.Parse("Prefix {$Date} Suffix", new { Date = now }));
        }

        [Test]
        public void ShouldBeAbleToParseFormattedDates()
        {
            DateTime? now = DateTime.Now;
            Assert.AreEqual("Prefix " + now.Value.ToString("g") + " Suffix", Template.Parse("Prefix {$Date:g} Suffix", new { Date = now }));
        }

        [Test]
        public void FormatOptionShouldNotAffectStrings()
        {
            Assert.AreEqual("Something With Useless format", Template.Parse("Something{$Format:G}", new { Format = " With Useless format" }));
        }

        [Test]
        public void ShouldIgnoreUnrecognizableDateFormat()
        {
            var now = DateTime.Now;
            Assert.AreEqual("Prefix " + now.ToString() + " Suffix", Template.Parse("Prefix {$Date:Z} Suffix", new { Date = now }));
        }

        [Test]
        public void ShouldNotTrimArgumentsIfOptionIsNotSet()
        {
            var options = new ParseOptions
            {
                Trim = false,
            };

            const string str = " Yikes!  ";
            Assert.AreEqual("Prefix " + str + " Suffix", Template.Parse("Prefix {$Arg} Suffix", options, new { Arg = str }));
        }

        [Test]
        public void ShouldTrimArgumentsIfOptionIsSet()
        {
            var options = new ParseOptions
            {
                Trim = true,
            };

            const string str = " Yikes!  ";
            Assert.AreEqual("Prefix " + str.Trim() + " Suffix", Template.Parse("Prefix {$Arg} Suffix", options, new { Arg = str }));
        }

        //TODO: Template is not smart enough to handle this kind of cases
        //[TestMethod]
        //public void ShouldBeSmartAndDontLetValuesMessWithTheTemplate()
        //{
        //    Assert.AreEqual("Some$IAlsoExistWOW!thing", Template.Parse("Some{$IExist}{$IAlsoExist}thing", new { IExist = "$IAlsoExist", IAlsoExist = "WOW!" }));
        //}
        
        //Support for nullable value types?
        //Nesting?
    }
}