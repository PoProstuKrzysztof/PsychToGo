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

    public Task<bool> CheckDuplicate(MedicineDTO medicine)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> CreateMedicine(int patientId, Medicine medicine)
    {
        try
        {
            var medicineEntity = await _context.Medicines.Where( x => x.Id == medicine.Id ).FirstOrDefaultAsync();

            //var patientMedicine = new PatientMedicine()
            //{
            //    Medicine = medicineEntity,
            //    Patient = patient
            //};
            //await _context.AddAsync( patientMedicine );

            //await _context.AddAsync( patient );
            return await Save();

        }
        catch (Exception)
        {
            throw;
        }
    }

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

    public Task<bool> Save()
    {
        throw new NotImplementedException();
    }
}
