using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PsychToGo.Data;
using PsychToGo.DTO;
using PsychToGo.Interfaces;
using PsychToGo.Models;

namespace PsychToGo.Repository;

public class MedicineCategoryRepository : IMedicineCategoryRepository
{
    private readonly AppDbContext _context;

    public MedicineCategoryRepository(AppDbContext context)
    {
        _context = context;
    }
    //Post
    public async Task<bool> CheckDuplicate(MedicineCategoryDTO medicineCategory)
    {
        try
        {
            var medicines = await _context.MedicinesCategories.ToListAsync();
            var medicineCategoryDuplicate =  medicines
                .Where( x => x.Name.TrimEnd().ToLower() == medicineCategory.Name.TrimEnd().ToLower())
                .FirstOrDefault();

            if (medicineCategoryDuplicate != null)
            {
                return true;
            }

            return false;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public  async Task<bool> CreateCategory( MedicineCategory category)
    {
        try
        {           
            await _context.AddAsync( category );
            return await Save();

        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> Save()
    {
        try
        {
            var savedEntity = await _context.SaveChangesAsync();
            return savedEntity > 0 ? true : false;
        }
        catch (Exception)
        {
            throw;
        }
    }
    //Get
    public async Task<ICollection<Medicine>> GetMedicineByCategory(string categoryName)
    {
        try
        {
           var category =  await _context.MedicinesCategories.Where( s => s.Name == categoryName.ToLower() ).FirstOrDefaultAsync();
            if (category == null)
            {
                return null;
            }

            var medicines =await _context.Medicines.Where( x => x.Category == category ).Select(m => m).ToListAsync();
            if(medicines == null)
            {
                return null;
            }
            return medicines;

        }
        catch (Exception) 
        {
            throw;
        }
    }

    public async Task<MedicineCategory> GetMedicineCategory(string categoryName)
    {
        try
        {
            if (categoryName.IsNullOrEmpty())
            {
                return null;
            }

            return await _context.MedicinesCategories.Where( s => s.Name == categoryName.ToLower() ).FirstOrDefaultAsync();

        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ICollection<MedicineCategory>> GetMedicinesCategories()
    {
        try
        {
            return await _context.MedicinesCategories.OrderBy( x => x.Name ).ToListAsync();
        }
        catch(Exception)
        {
            throw;
        }
        
    }

    public async Task<bool> MedicinneCategoryExist(string categoryName)
    {
        try
        {
            return await _context.MedicinesCategories.AnyAsync(x => x.Name == categoryName);
        }
        catch(Exception)
        {
            throw;
        }
    }

    
}
