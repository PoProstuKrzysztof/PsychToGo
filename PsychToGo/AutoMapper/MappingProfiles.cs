using AutoMapper;
using PsychToGo.DTO;
using PsychToGo.Models;

namespace PsychToGo.AutoMapper;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Patient, PatientDTO>();
    }
}
