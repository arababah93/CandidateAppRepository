using CandidateApp.Controllers;
using CandidateApp.Data.DataTransferObjects;
using CandidateApp.Data;
using CandidateApp.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CandidateApp.Tests.Controllers
{
    public class CandidatesControllerTests
    {
        private readonly Mock<ICandidateRepository> _mockCandidateService;
        private readonly CandidatesController _controller;

        public CandidatesControllerTests()
        {
            _mockCandidateService = new Mock<ICandidateRepository>();
            _controller = new CandidatesController(_mockCandidateService.Object);
        }

        [Fact]
        public async Task AddOrUpdateCandidate_WithValidData_ReturnsOk()
        {
            // Arrange
            var candidate = new CandidateDto
            {
                FirstName = "Alaa",
                LastName = "Al-Rababah",
                Email = "alaa@example.com",
                PhoneNumber = "1234567890",
                LinkedInUrl = "https://linkedin.com/in/alaaRababah",
                GitHubUrl = "https://github.com/alaaRababah",
                Comment = "Looking forward to new opportunities."
            };

            _mockCandidateService.Setup(service => service.AddOrUpdateCandidate(candidate))
                .Returns(Task.FromResult(candidate));

            // Act
            var result = await _controller.AddOrUpdateCandidate(candidate);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Candidate>>(result);
            // Check if the result is OkObjectResult (if you expect a body in the response)
            if (actionResult.Result is OkObjectResult okObjectResult)
            {
                Assert.Equal(200, okObjectResult.StatusCode); // Ensure the status code is OK (200)

                var returnedCandidate = Assert.IsType<CandidateDto>(okObjectResult.Value);
                Assert.Equal("Alaa", returnedCandidate.FirstName);
                Assert.Equal("Al-Rababah", returnedCandidate.LastName);
                Assert.Equal("alaa@example.com", returnedCandidate.Email);
                Assert.Equal("1234567890", returnedCandidate.PhoneNumber);
                Assert.Equal("https://linkedin.com/in/alaaRababah", returnedCandidate.LinkedInUrl);
                Assert.Equal("https://github.com/alaaRababah", returnedCandidate.GitHubUrl);
                Assert.Equal("Looking forward to new opportunities.", returnedCandidate.Comment);
            }
            else if (actionResult.Result is OkResult okResult)
            {
                Assert.Equal(200, okResult.StatusCode); // Ensure the status code is OK (200)
            }
            else
            {
                Assert.True(false, "Expected result type is neither OkObjectResult nor OkResult.");
            }

            _mockCandidateService.Verify(service => service.AddOrUpdateCandidate(candidate), Times.Once);
        }

        [Fact]
        public async Task AddOrUpdateCandidate_WithInvalidData_ReturnsBadRequest()
        {
            // Arrange
            _controller.ModelState.AddModelError("Email", "Required");

            var candidate = new CandidateDto
            {
                FirstName = "Alaa",
                LastName = "Al-Rababah",
                // Missing required Email field
                PhoneNumber = "1234567890",
                LinkedInUrl = "https://linkedin.com/in/alaaRababah",
                GitHubUrl = "https://github.com/alaaRababah",
                Comment = "Looking forward to new opportunities."
            };

            // Act
            var result = await _controller.AddOrUpdateCandidate(candidate);


            // Assert
            var actionResult = Assert.IsType<ActionResult<Candidate>>(result);
            var badRequestResult = Assert.IsType<BadRequestResult>(actionResult.Result);

            _mockCandidateService.Verify(service => service.AddOrUpdateCandidate(It.IsAny<CandidateDto>()), Times.Never);
        }

        [Fact]
        public async Task AddOrUpdateCandidate_ExistingCandidate_UpdatesAndReturnsOk()
        {
            // Arrange
            var candidate = new CandidateDto
            {
                FirstName = "Alaa",
                LastName = "Al-Rababah",
                Email = "alaa@example.com",
                PhoneNumber = "0786978786",
                LinkedInUrl = "https://linkedin.com/in/alaaRababah",
                GitHubUrl = "https://github.com/alaaRababah",
                Comment = "Experienced developer."
            };

            _mockCandidateService.Setup(service => service.AddOrUpdateCandidate(candidate))
            .Returns(Task.FromResult(true));

            // Act
            var result = await _controller.AddOrUpdateCandidate(candidate);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Candidate>>(result);
            // Check if the result is OkObjectResult (if you expect a body in the response)
            if (actionResult.Result is OkObjectResult okObjectResult)
            {
                Assert.Equal(200, okObjectResult.StatusCode); // Ensure the status code is OK (200)

                var returnedCandidate = Assert.IsType<CandidateDto>(okObjectResult.Value);
                Assert.Equal("Alaa", returnedCandidate.FirstName);
                Assert.Equal("Al-Rababah", returnedCandidate.LastName);
                Assert.Equal("alaa@example.com", returnedCandidate.Email);
                Assert.Equal("0786978786", returnedCandidate.PhoneNumber);
                Assert.Equal("https://linkedin.com/in/alaaRababah", returnedCandidate.LinkedInUrl);
                Assert.Equal("https://github.com/alaaRababah", returnedCandidate.GitHubUrl);
                Assert.Equal("Experienced developer.", returnedCandidate.Comment);
            }
            else if (actionResult.Result is OkResult okResult)
            {
                Assert.Equal(200, okResult.StatusCode); // Ensure the status code is OK (200)
            }
            else
            {
                Assert.True(false, "Expected result type is neither OkObjectResult nor OkResult.");
            }

            _mockCandidateService.Verify(service => service.AddOrUpdateCandidate(candidate), Times.Once);
        }

        [Fact]
        public async Task AddOrUpdateCandidate_WithInvalidGitHubUrl_ReturnsBadRequest()
        {
            // Arrange
            var invalidCandidate = new CandidateDto
            {
                FirstName = "Alaa",
                LastName = "Al-Rababah",
                Email = "alaa@example.com",
                PhoneNumber = "1234567890",
                LinkedInUrl = "https://linkedin.com/in/alaaRababah",
                GitHubUrl = "invalid-url", // Invalid GitHub URL format
                Comment = "Looking forward to new opportunities."
            };

            // Simulate model validation error
            _controller.ModelState.AddModelError("GitHubUrl", "Invalid URL format");

            // Act
            var result = await _controller.AddOrUpdateCandidate(invalidCandidate);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Candidate>>(result);
            var badRequestResult = Assert.IsType<BadRequestResult>(actionResult.Result);

            _mockCandidateService.Verify(service => service.AddOrUpdateCandidate(It.IsAny<CandidateDto>()), Times.Never);
        }

        [Fact]
        public async Task AddOrUpdateCandidate_WithInvalidLinkedInUrl_ReturnsBadRequest()
        {
            // Arrange
            var invalidCandidate = new CandidateDto
            {
                FirstName = "Alaa",
                LastName = "Al-Rababah",
                Email = "alaa@example.com",
                PhoneNumber = "1234567890",
                LinkedInUrl = "invalid-url", // Invalid LinkedIn URL format
                GitHubUrl = "https://github.com/alaaRababah",
                Comment = "Looking forward to new opportunities."
            };

            // Simulate model validation error
            _controller.ModelState.AddModelError("LinkedInUrl", "Invalid URL format");

            // Act
            var result = await _controller.AddOrUpdateCandidate(invalidCandidate);

            // Assert
            var actionResult = Assert.IsType<ActionResult<Candidate>>(result);
            var badRequestResult = Assert.IsType<BadRequestResult>(actionResult.Result);

            _mockCandidateService.Verify(service => service.AddOrUpdateCandidate(It.IsAny<CandidateDto>()), Times.Never);
        }
    }
}
