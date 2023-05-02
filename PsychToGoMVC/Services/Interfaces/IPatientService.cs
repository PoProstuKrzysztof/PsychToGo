﻿using PsychToGo.API.DTO;
using PsychToGo.API.Models;
using PsychToGo.Client.Models;

namespace PsychToGo.Client.Services.Interfaces;

public interface IPatientService
{
    Task<Patient> CreatePatientInstance(PatientViewModel pvm);

    Task<PatientViewModel> CreateParsedPatientInstance(int id);

    Task<ICollection<PsychologistDTO>> PsychologistsList();

    Task<ICollection<PsychiatristDTO>> PsychiatristsList();

    Task<ICollection<MedicineDTO>> MedicinesList();
}