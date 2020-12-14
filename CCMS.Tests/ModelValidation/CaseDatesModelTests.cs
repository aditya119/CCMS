using CCMS.Shared.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Xunit;

namespace CCMS.Tests.ModelValidation
{
    public class CaseDatesModelTests
    {
        [Fact]
        public void CaseDatesModel_CaseFiledAfterNoticeReceived()
        {
            // Arrange
            var sut = new CaseDatesModel
            {
                CaseId = 1,
                CaseFiledOn = DateTime.Now,
                NoticeReceivedOn = DateTime.Now.AddDays(-1)
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
                Assert.Equal("Case cannot be filed after Notice is received", item.ErrorMessage);
                Assert.Single(item.MemberNames);
                Assert.Contains("CaseFiledOn", item.MemberNames);
            }
        }

        [Fact]
        public void CaseDatesModel_CaseFiledAfterFirstHearing()
        {
            // Arrange
            var sut = new CaseDatesModel
            {
                CaseId = 1,
                CaseFiledOn = DateTime.Now,
                FirstHearingOn = DateTime.Now.AddDays(-1)
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
                Assert.Equal("First Hearing Date cannot be before the date Case was filed on", item.ErrorMessage);
                Assert.Single(item.MemberNames);
                Assert.Contains("FirstHearingOn", item.MemberNames);
            }
        }

        [Fact]
        public void CaseDatesModel_NoticeReceivedAfterFirstHearing()
        {
            // Arrange
            var sut = new CaseDatesModel
            {
                CaseId = 1,
                NoticeReceivedOn = DateTime.Now.AddDays(1),
                FirstHearingOn = DateTime.Now.AddDays(-1)
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
                Assert.Equal("First Hearing Date cannot be before the date on which Notice was received", item.ErrorMessage);
                Assert.Single(item.MemberNames);
                Assert.Contains("NoticeReceivedOn", item.MemberNames);
            }
        }
    }
}
