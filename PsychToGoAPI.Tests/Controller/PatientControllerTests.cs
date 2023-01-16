using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using PsychToGo.Controllers;
using PsychToGo.DTO;
using PsychToGo.Interfaces;
using Xunit;

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
        var patients = A.Fake<ICollection<PatientDTO>>();
        var patientsList = A.Fake<List<PatientDTO>>();
        A.CallTo( () => _mapper.Map<List<PatientDTO>>( patients ) ).Returns( patientsList );
        var controller = new PatientController( _patientRepository,_mapper,_psychiatristRepository,_psychologistRepository  );
        //Act
        var result = await controller.GetAllPatients();

        //Assert
        result.Should().NotBeNull();
         result.Should().BeOfType(typeof(OkObjectResult));
    }
}
