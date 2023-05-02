using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using PsychToGo.API.Controllers;
using PsychToGo.API.DTO;
using PsychToGo.API.Interfaces;

namespace PsychToGoAPI.Tests.Controller;

public class PatientControllerTests
{
    private readonly IPatientRepository _patientRepository;
    private readonly IPsychologistRepository _psychologistRepository;
    private readonly IPsychiatristRepository _psychiatristRepository;
    private readonly IMapper _mapper;

    public PatientControllerTests()
    {
        _patientRepository = A.Fake<IPatientRepository>();
        _psychiatristRepository = A.Fake<IPsychiatristRepository>();
        _psychologistRepository = A.Fake<IPsychologistRepository>();
        _mapper = A.Fake<IMapper>();
    }

    [Test]
    public async Task PatientController_GetAllPatients_ReturnOK()
    {
        //Arrange
        ICollection<PatientDTO> patients = A.Fake<ICollection<PatientDTO>>();
        List<PatientDTO> patientsList = A.Fake<List<PatientDTO>>();
        A.CallTo( () => _mapper.Map<List<PatientDTO>>( patients ) ).Returns( patientsList );
        PatientController controller = new( _patientRepository,
            _mapper,
            _psychiatristRepository,
            _psychologistRepository );
        //Act
        IActionResult result = await controller.GetAllPatients();

        //Assert
        result.Should().NotBeNull();
        result.Should().BeOfType( typeof( OkObjectResult ) );
    }
}