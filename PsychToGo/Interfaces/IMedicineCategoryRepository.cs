using PsychToGo.DTO;
using PsychToGo.Models;

namespace PsychToGo.Interfaces;

public interface IMedicineCategoryRepository
{
    //Get
    Task<ICollection<MedicineCategory>> GetMedicinesCategories();
    Task<MedicineCategory> GetMedicineCategory(string categoryName);
    Task<ICollection<Medicine>> GetMedicineByCategory(string categoryName);
    Task<bool> MedicinneCategoryExist(string categoryName);
    Task<MedicineCategory> GetMedicineCategoryById(int categoryId);

    //Post
    Task<bool> CreateCategory(MedicineCategory category);
    Task<bool> Save();
    Task<bool> CheckDuplicate(MedicineCategoryDTO medicineCategory);

    //Put
    Task<bool> UpdateCategory(MedicineCategory patient);
}
