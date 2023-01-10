using AutoMapper;
using PsychToGo.DTO;
using PsychToGo.Models;

namespace PsychToGo.AutoMapper;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Patient, PatientDTO>();
        CreateMap<PatientDTO, Patient>();

        CreateMap<Psychologist, PsychologistDTO>();
        
        CreateMap<Medicine, MedicineDTO>();

        CreateMap<MedicineCategory, MedicineCategoryDTO>();
        CreateMap<MedicineCategoryDTO,MedicineCategory>();

        CreateMap<Psychiatrist, PsychiatristDTO>();
    }
}
