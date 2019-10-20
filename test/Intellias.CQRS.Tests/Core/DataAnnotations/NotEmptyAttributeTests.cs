using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using FluentAssertions;
using Intellias.CQRS.Core.DataAnnotations;
using Xunit;

namespace Intellias.CQRS.Tests.Core.DataAnnotations
{
    public class NotEmptyAttributeTests
    {
        public static IEnumerable<object[]> CollectionTheoryData()
        {
            yield return new object[] { null, true };
            yield return new object[] { Array.Empty<string>(), false };
            yield return new object[] { new List<string>(), false };
            yield return new object[] { new List<int> { 1 }, true };
        }

        [Fact]
        public void NotEmpty_ValidationFailed_ReturnsCorrectResult()
        {
            var expectedInvalidProperty = nameof(FakeInstance.Property);
            var expectedErrorMessage = string.Format(CultureInfo.InvariantCulture, NotEmptyAttribute.ErrorMessageTemplate, expectedInvalidProperty);

            var instance = new FakeInstance();
            var context = new ValidationContext(instance);

            var results = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(instance, context, results, true);

            isValid.Should().BeFalse();
            results.Single().Should().BeEquivalentTo(new ValidationResult(expectedErrorMessage, new[] { expectedInvalidProperty }));
        }

        [Theory]
        [InlineData((string)null, true)]
        [InlineData("", false)]
        [InlineData(" ", false)]
        [InlineData("x", true)]
        public void NotEmpty_StringValue_ValidatesCorrectly(object value, bool isValid)
        {
            var attribute = new NotEmptyAttribute();

            var result = attribute.GetValidationResult(value, new ValidationContext(this));

            (result == ValidationResult.Success).Should().Be(isValid);
        }

        [Theory]
        [MemberData(nameof(CollectionTheoryData))]
        public void NotEmpty_CollectionValue_ValidatesCorrectly(object value, bool isValid)
        {
            var attribute = new NotEmptyAttribute();

            var result = attribute.GetValidationResult(value, new ValidationContext(this));

            (result == ValidationResult.Success).Should().Be(isValid);
        }

        private class FakeInstance
        {
            [NotEmpty]
            public string Property { get; set; } = string.Empty;
        }
    }
}