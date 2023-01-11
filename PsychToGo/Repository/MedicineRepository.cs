using Microsoft.EntityFrameworkCore;
using PsychToGo.Data;
using PsychToGo.DTO;
using PsychToGo.Interfaces;
using PsychToGo.Models;

namespace PsychToGo.Repository;

public class MedicineRepository : IMedicineRepository
{
    private readonly AppDbContext _context;
    public MedicineRepository(AppDbContext context)
    {
        _context= context;
    }

    //Post
    public async Task<bool> Save()
    {
        try
        {
            var entitySaved = await _context.SaveChangesAsync();
            return entitySaved > 0 ? true : false;
        }
        catch(Exception)
        {
            throw;
        }
    }

    public async Task<bool> CheckDuplicate( MedicineDTO medicine)
    {
        try
        {
            var medicines = await _context.Medicines.ToListAsync();
            var medicineDuplicate = medicines
                .Where( x => x.Name.TrimEnd().ToLower() == medicine.Name.TrimEnd().ToLower() )
                .FirstOrDefault();

            if (medicineDuplicate != null)
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

    public async Task<bool> CreateMedicine(int categoryId, Medicine medicine)
    {
        try
        {
            
            await _context.AddAsync( medicine );
         
            return await Save();

        }
        catch (Exception)
        {
            throw;
        }
    }

    //Get
    public Task<Medicine> GetMedicine(int id)
    {
        throw new NotImplementedException();
    }

    public Task<DateTime> GetMedicineExpireDate(int id)
    {
        throw new NotImplementedException();
    }

    public Task<int> GetMedicineInStock(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<ICollection<Medicine>> GetMedicines()
    {
        try
        {
            return await _context.Medicines.OrderBy( x => x.Id ).ToListAsync();
        }
        catch (Exception)
        {
            throw;
        }
        
    }

    public Task<bool> MedicineExists(int id)
    {
        throw new NotImplementedException();
    }


}
