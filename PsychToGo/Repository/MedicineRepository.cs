using Microsoft.EntityFrameworkCore;
using PsychToGo.Data;
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
    public async Task<ICollection<Medicine>> GetMedicines()
    {
        return await _context.Medicines.OrderBy( x => x.Id ).ToListAsync();
    }
}
