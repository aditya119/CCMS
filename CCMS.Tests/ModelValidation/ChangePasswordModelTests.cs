using CCMS.Shared.Models.AppUserModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace CCMS.Tests.ModelValidation
{
    public class ChangePasswordModelTests
    {
        [Theory]
        [InlineData("abcde1", "Password must contain at least one upper-case letter")]
        [InlineData("ABCDE1", "Password must contain at least one lower-case letter")]
        [InlineData("ABcdef", "Password must contain at least one digit")]
        public void ChangePasswordModel_InvalidPassword(string password, string errorMessage)
        {
            // Arrange
            var sut = new ChangePasswordModel
            {
                UserId = 1,
                OldPassword = "abc",
                NewPassword = password,
                NewPasswordAgain = password
            };
            var context = new ValidationContext(sut, null, null);

            // Act
            var results = new List<ValidationResult>();
            var isModelStateValid = Validator.TryValidateObject(sut, context, results, true);

            // Assert
            Assert.False(isModelStateValid);
            Assert.Single(results);
            foreach (var item in results)
            {
                Assert.Equal(errorMessage, item.ErrorMessage);
                Assert.Single(item.MemberNames);
                Assert.Contains("NewPassword", item.MemberNames);
            }
        }
    }
}
