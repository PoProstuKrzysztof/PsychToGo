using AutoMapper;
using PsychToGo.DTO;
using PsychToGo.Models;
using PsychToGo.Models.Identity;

namespace PsychToGo.AutoMapper;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<Patient, PatientDTO>();
        CreateMap<PatientDTO, Patient>();

        CreateMap<Psychologist, PsychologistDTO>();
        CreateMap<PsychologistDTO,Psychologist>();
        
        CreateMap<Medicine, MedicineDTO>();
        CreateMap<MedicineDTO,Medicine>();

        CreateMap<MedicineCategory, MedicineCategoryDTO>();
        CreateMap<MedicineCategoryDTO,MedicineCategory>();

        CreateMap<Psychiatrist, PsychiatristDTO>();
        CreateMap<PsychiatristDTO,Psychiatrist>();

        CreateMap<AppUser,UserDTO>().ReverseMap();
    }
}
