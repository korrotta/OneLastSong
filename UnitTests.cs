using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.AppContainer;
using OneLastSong.Converters;
using OneLastSong.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace UnitTest
{
    [TestClass]
    public class CategoryVisibilityConverterTests
    {
        [TestMethod]
        public void TestBasicCalculation_OnePlusOneEqualsTwo()
        {
            int a = 1;
            int b = 1;
            int expected = 2;

            int actual = a + b;
            Xunit.Assert.Equal(expected, actual);
        }

        [TestMethod]
        public void Convert_ShouldReturnCollapsed_WhenValueIsNull()
        {
            // Arrange
            var converter = new CategoryVisibilityConverter();

            // Act
            var result = converter.Convert(null, typeof(Visibility), "TargetCategory", "en");

            // Xunit.Assert
            Xunit.Assert.Equal(Visibility.Collapsed, result);
        }

        [TestMethod]
        public void Convert_ShouldReturnCollapsed_WhenParameterIsNull()
        {
            // Arrange
            var converter = new CategoryVisibilityConverter();

            // Act
            var result = converter.Convert("CurrentCategory", typeof(Visibility), null, "en");

            // Xunit.Assert
            Xunit.Assert.Equal(Visibility.Collapsed, result);
        }

        [TestMethod]
        public void Convert_ShouldReturnVisible_WhenCurrentCategoryIsAllCategory()
        {
            // Arrange
            var converter = new CategoryVisibilityConverter();
            var currentCategory = SearchPageViewModel.ALL_CATEGORY;

            // Act
            var result = converter.Convert(currentCategory, typeof(Visibility), "TargetCategory", "en");

            // Xunit.Assert
            Xunit.Assert.Equal(Visibility.Visible, result);
        }

        [TestMethod]
        public void Convert_ShouldReturnVisible_WhenCurrentCategoryMatchesTargetCategory()
        {
            // Arrange
            var converter = new CategoryVisibilityConverter();
            var currentCategory = "Category1";
            var targetCategory = "Category1";

            // Act
            var result = converter.Convert(currentCategory, typeof(Visibility), targetCategory, "en");

            // Xunit.Assert
            Xunit.Assert.Equal(Visibility.Visible, result);
        }

        [TestMethod]
        public void Convert_ShouldReturnCollapsed_WhenCurrentCategoryDoesNotMatchTargetCategory()
        {
            // Arrange
            var converter = new CategoryVisibilityConverter();
            var currentCategory = "Category1";
            var targetCategory = "Category2";

            // Act
            var result = converter.Convert(currentCategory, typeof(Visibility), targetCategory, "en");

            // Xunit.Assert
            Xunit.Assert.Equal(Visibility.Collapsed, result);
        }

        [TestMethod]
        public void ConvertBack_ShouldThrowNotImplementedException()
        {
            // Arrange
            var converter = new CategoryVisibilityConverter();

            // Act & Xunit.Assert
            Xunit.Assert.Throws<NotImplementedException>(() => converter.ConvertBack(null, null, null, null));
        }
        
    }
    [TestClass]
    public class DurationToTimeConverterTests
    {
        private readonly DurationToTimeConverter _converter = new DurationToTimeConverter();

        [TestMethod]
        public void TestConvert_Duration125Seconds_Returns02Colon05()
        {
            // Arrange
            int duration = 125;
            string expected = "02:05";

            // Act
            string actual = _converter.Convert(duration, null, null, null) as string;

            // Xunit.Assert
            Xunit.Assert.Equal(expected, actual);
        }

        [TestMethod]
        public void TestConvert_Duration0Seconds_Returns00Colon00()
        {
            // Arrange
            int duration = 0;
            string expected = "00:00";

            // Act
            string actual = _converter.Convert(duration, null, null, null) as string;

            // Xunit.Assert
            Xunit.Assert.Equal(expected, actual);
        }

        [TestMethod]
        public void TestConvertBack_Time02Colon05_Returns125()
        {
            // Arrange
            string time = "02:05";
            int expected = 125;

            // Act
            int actual = (int)_converter.ConvertBack(time, null, null, null);

            // Xunit.Assert
            Xunit.Assert.Equal(expected, actual);
        }

        [TestMethod]
        public void TestConvertBack_Time59Colon59_Returns3599()
        {
            // Arrange
            string time = "59:59";
            int expected = 3599;

            // Act
            int actual = (int)_converter.ConvertBack(time, null, null, null);

            // Xunit.Assert
            Xunit.Assert.Equal(expected, actual);
        }

        [TestMethod]
        public void TestConvertBack_InvalidFormat_ThrowsFormatException()
        {
            // Arrange
            string invalidTime = "invalid";

            // Act & Xunit.Assert
            Xunit.Assert.Throws<System.FormatException>(() =>
                _converter.ConvertBack(invalidTime, null, null, null));
        }
    }

    [TestClass]
    public class HtmlToRichTextConverterTests
    {
        private readonly HtmlToRichTextConverter _converter = new HtmlToRichTextConverter();

        [TestMethod]
        public void Convert_ValidHtmlString_ReturnsRichTextBlockWithBoldText()
        {
            // Arrange
            string htmlString = "This is **bold text** in a sentence.";

            // Act
            var result = _converter.Convert(htmlString, null, null, null) as RichTextBlock;

            // Xunit.Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Single(result.Blocks);

            var paragraph = result.Blocks.First() as Paragraph;
            Xunit.Assert.NotNull(paragraph);

            // Check the bold text
            var boldRun = paragraph.Inlines.First() as Run;
            Xunit.Assert.NotNull(boldRun);
            Xunit.Assert.Equal("This is ", boldRun.Text);

            var boldPart = paragraph.Inlines.ElementAt(1) as Bold;
            Xunit.Assert.NotNull(boldPart);
            var boldText = boldPart.Inlines.First() as Run;
            Xunit.Assert.NotNull(boldText);
            Xunit.Assert.Equal("bold text", boldText.Text);

            var remainingText = paragraph.Inlines.ElementAt(2) as Run;
            Xunit.Assert.NotNull(remainingText);
            Xunit.Assert.Equal(" in a sentence.", remainingText.Text);
        }

        [TestMethod]
        public void Convert_EmptyString_ReturnsEmptyRichTextBlock()
        {
            // Arrange
            string htmlString = "";

            // Act
            var result = _converter.Convert(htmlString, null, null, null) as RichTextBlock;

            // Xunit.Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Single(result.Blocks);

            var paragraph = result.Blocks.First() as Paragraph;
            Xunit.Assert.NotNull(paragraph);
            Xunit.Assert.Empty(paragraph.Inlines);
        }

        [TestMethod]
        public void Convert_NullValue_ReturnsNull()
        {
            // Arrange
            string htmlString = null;

            // Act
            var result = _converter.Convert(htmlString, null, null, null);

            // Xunit.Assert
            Xunit.Assert.Null(result);
        }

        [TestMethod]
        public void Convert_TextWithoutBold_ReturnsPlainTextInRichTextBlock()
        {
            // Arrange
            string htmlString = "Just normal text without bold.";

            // Act
            var result = _converter.Convert(htmlString, null, null, null) as RichTextBlock;

            // Xunit.Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Single(result.Blocks);

            var paragraph = result.Blocks.First() as Paragraph;
            Xunit.Assert.NotNull(paragraph);

            Xunit.Assert.Single(paragraph.Inlines);
            var run = paragraph.Inlines.First() as Run;
            Xunit.Assert.NotNull(run);
            Xunit.Assert.Equal("Just normal text without bold.", run.Text);
        }

        [TestMethod]
        public void Convert_InvalidHtmlString_HandledGracefully()
        {
            // Arrange
            string htmlString = "**invalid** text without proper regex.";

            // Act
            var result = _converter.Convert(htmlString, null, null, null) as RichTextBlock;

            // Xunit.Assert
            Xunit.Assert.NotNull(result);
            Xunit.Assert.Single(result.Blocks);

            var paragraph = result.Blocks.First() as Paragraph;
            Xunit.Assert.NotNull(paragraph);

            var boldPart = paragraph.Inlines.First() as Bold;
            Xunit.Assert.NotNull(boldPart);

            var boldText = boldPart.Inlines.First() as Run;
            Xunit.Assert.NotNull(boldText);
            Xunit.Assert.Equal("invalid", boldText.Text);
        }
    }
}
