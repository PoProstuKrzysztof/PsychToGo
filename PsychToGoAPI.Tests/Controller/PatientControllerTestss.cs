using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using PsychToGo.API.Controllers;
using PsychToGo.API.Interfaces;
using PsychToGo.API.Models;

namespace PsychToGoAPI.Tests.Controller;

public class PatientControllerTest
{
    [TestClass]
    public class PatientControllerTestsss
    {
        private readonly Mock<IPatientRepository> _patientRepositoryMock = new();
        private readonly Mock<IMapper> _mapperMock = new();
        private readonly Mock<IPsychiatristRepository> _psychiatristRepositoryMock = new();
        private readonly Mock<IPsychologistRepository> _psychologistRepositoryMock = new();

        [TestMethod]
        public async Task GetPatientPsychiatrist_ReturnsOkObjectResult_WhenPatientHasPsychiatrist()
        {
            // Arrange
            var patientId = 1;
            var psychiatrist = new Psychiatrist();
            var controller = new PatientController(_patientRepositoryMock.Object, _mapperMock.Object, _psychiatristRepositoryMock.Object, _psychologistRepositoryMock.Object);
            _patientRepositoryMock.Setup(x => x.GetPatientPsychiatrist(patientId)).ReturnsAsync(psychiatrist);

            // Act
            var result = await controller.GetPatientPsychiatrist(patientId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetPatientPsychologist_ReturnsOkObjectResult_WhenPatientHasPsychologist()
        {
            // Arrange
            var patientId = 1;
            var psychologist = new Psychologist();
            var controller = new PatientController(_patientRepositoryMock.Object, _mapperMock.Object, _psychiatristRepositoryMock.Object, _psychologistRepositoryMock.Object);
            _patientRepositoryMock.Setup(x => x.GetPatientPsychologist(patientId)).ReturnsAsync(psychologist);

            // Act
            var result = await controller.GetPatientPsychologist(patientId);

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task GetAllPatients_ReturnsOkObjectResult_WhenCalled()
        {
            // Arrange
            var patients = new List<Patient>();
            var controller = new PatientController(_patientRepositoryMock.Object, _mapperMock.Object, _psychiatristRepositoryMock.Object, _psychologistRepositoryMock.Object);
            _patientRepositoryMock.Setup(x => x.GetPatients()).ReturnsAsync(patients);

            // Act
            var result = await controller.GetAllPatients();

            // Assert
            Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsInstanceOfType(result, typeof(OkObjectResult));
        }
    }
}