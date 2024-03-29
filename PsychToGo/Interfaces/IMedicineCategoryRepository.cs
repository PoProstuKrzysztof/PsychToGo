﻿using PsychToGo.API.DTO;
using PsychToGo.API.Models;

namespace PsychToGo.API.Interfaces;

public interface IMedicineCategoryRepository
{
    //Get
    Task<ICollection<MedicineCategory>> GetMedicinesCategories();

    Task<MedicineCategory> GetMedicineCategory(string categoryName);

    Task<ICollection<Medicine>> GetMedicineByCategory(string categoryName);

    Task<bool> MedicinneCategoryExist(string categoryName);

    Task<bool> MedicineCategoryExistById(int categoryId);

    Task<MedicineCategory> GetMedicineCategoryById(int categoryId);

    //Post
    Task<bool> CreateCategory(MedicineCategory category);

    Task<bool> Save();

    Task<bool> CheckDuplicate(MedicineCategoryDTO medicineCategory);

    //Put
    Task<bool> UpdateCategory(MedicineCategory medicineCategory);

    //Delete
    Task<bool> DeleteCategory(MedicineCategory medicineCategory);
}