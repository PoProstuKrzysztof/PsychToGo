using Microsoft.EntityFrameworkCore;
using PsychToGo.API.Data;
using PsychToGo.API.DTO;
using PsychToGo.API.Interfaces;
using PsychToGo.API.Models;

namespace PsychToGo.API.Repository;

public class MedicineRepository : IMedicineRepository
{
    private readonly AppDbContext _context;

    public MedicineRepository(AppDbContext context)
    {
        _context = context;
    }

    //Post
    public async Task<bool> Save()
    {
        try
        {
            int entitySaved = await _context.SaveChangesAsync();
            return entitySaved > 0 ? true : false;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> CheckDuplicate(MedicineDTO medicine)
    {
        try
        {
            List<Medicine> medicines = await _context.Medicines.ToListAsync();
            Medicine? medicineDuplicate = medicines
                .Where(x => x.Name.TrimEnd().ToLower() == medicine.Name.TrimEnd().ToLower())
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
            await _context.AddAsync(medicine);

            return await Save();
        }
        catch (Exception)
        {
            throw;
        }
    }

    //Get
    public async Task<Medicine> GetMedicine(int id)
    {
        try
        {
            return await _context.Medicines
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<string> GetMedicineExpireDate(int medicineId)
    {
        try
        {
            DateTime expireDate = await _context.Medicines
                .Where(x => x.Id == medicineId)
                .Select(x => x.ExpireDate)
                .FirstOrDefaultAsync();

            string expireDateFormat = expireDate.ToString("yyyy-MM-dd");
            return expireDateFormat;
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<int> GetMedicineInStock(int medicineId)
    {
        try
        {
            return await _context.Medicines
                .Where(x => x.Id == medicineId)
                .Select(m => m.InStock)
                .FirstOrDefaultAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<ICollection<Medicine>> GetMedicines()
    {
        try
        {
            return await _context.Medicines.
                OrderBy(x => x.Id)
                .AsNoTracking().
                ToListAsync();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> MedicineExists(int id)
    {
        try
        {
            Medicine? medicine = await _context.Medicines
                .Where(x => x.Id == id)
                .FirstOrDefaultAsync();
            if (medicine == null)
            {
                return false;
            }

            return true;
        }
        catch (Exception)
        {
            throw;
        }
    }

    //Put
    public async Task<bool> UpdateMedicine(int categoryId, Medicine medicine)
    {
        try
        {
            _context.Entry(medicine).State = EntityState.Detached;
            _context.ChangeTracker.Clear();
            _context.Update(medicine);

            return await Save();
        }
        catch (Exception)
        {
            throw;
        }
    }

    public async Task<bool> DeleteMedicine(Medicine medicine)
    {
        try
        {
            _context.Remove(medicine);
            return await Save();
        }
        catch (Exception)
        {
            throw;
        }
    }
}