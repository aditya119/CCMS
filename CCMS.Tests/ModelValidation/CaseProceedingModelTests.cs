using CCMS.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace CCMS.Tests.ModelValidation
{
    public class CaseProceedingModelTests
    {
        [Fact]
        public void CaseProceedingModel_NextHearingBeforeProceeding()
        {
            // Arrange
            var sut = new CaseProceedingModel
            {
                CaseProceedingId = 1,
                JudgementFile = 0,
                ProceedingDecision = 1,
                ProceedingDate = DateTime.Today,
                NextHearingOn = DateTime.Today.AddDays(-1)
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
                Assert.Equal("Proceeding Date cannot be later than Next Hearing date", item.ErrorMessage);
                Assert.Single(item.MemberNames);
                Assert.Contains("NextHearingOn", item.MemberNames);
            }
        }
    }
}
