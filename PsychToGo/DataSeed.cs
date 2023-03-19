using Microsoft.AspNetCore.Identity;
using PsychToGo.Data;
using PsychToGo.Models;
using PsychToGo.Models.Identity;

namespace PsychToGo;

public class DataSeed
{
    private readonly AppDbContext _context;


    public DataSeed(AppDbContext context)
    {
        _context = context;
    }

    public void SeedDataContext()
    {
        if (!_context.Psychiatrists.Any())
        {
            var psychiatrist = new List<Psychiatrist>()
            {
                new Psychiatrist()
                {
                    Name = "Lena",
                    LastName = "Woźniak",
                    Email = "lena.wozniak@gmail.com",
                    Phone = "489256126",
                    DateOfBirth = DateTime.Parse( "1995-03-18" ),
                    Address = "Kryształowa 105",

                },
                 new Psychiatrist()
                 {
                            Name = "Patrycja ",
                            LastName = "Bednarska",
                            Email = "patrycja.bednarska@gmail.com",
                            Phone = "504231536",
                            DateOfBirth = DateTime.Parse("1987-02-06"),
                            Address = "Wróżkowa 5"

                 },

            };
            _context.Psychiatrists.AddRange( psychiatrist );
            _context.SaveChanges();
        }


        if (!_context.Psychologists.Any())
        {

            var psychologists = new List<Psychologist>()
            {

                new Psychologist()

                {
                Name = "Mariusz ",
                LastName = "Wilczyński",
                Email = "mariusz.wilczyn@gmail.com",
                Phone = "764238901",
                DateOfBirth = DateTime.Parse( "1987-05-21" ),
                    Address = "Migdałowa 33a",

                },


                new Psychologist()

                {
                 Name = "Antonina",
                 LastName = "Małecka",
                 Email = "antonina.malecka@gmail.com",
                 Phone = "454212566",
                 DateOfBirth = DateTime.Parse( "1998-04-06" ),
                 Address = "Migdałowa 33a",

                },

            };
            _context.Psychologists.AddRange( psychologists );
            _context.SaveChanges();

        };

        if (!_context.UserRoles.Any())
        {
            var roles = new List<IdentityRole>()
            {
                new IdentityRole()
                {
                    Name = "admin",
                    NormalizedName= "ADMIN",
                },
                new IdentityRole()
                {
                    Name = "psychologist",
                    NormalizedName= "PSYCHOLOGIST",
                },
                new IdentityRole()
                {
                    Name = "psychiatrist",
                    NormalizedName= "PSYCHIATRIST",
                },
                new IdentityRole()
                {
                    Name = "patient",
                    NormalizedName= "PATIENT",
                }
            };
            _context.Roles.AddRange( roles );
            _context.SaveChanges();
        }



        if (!_context.ApplicationUsers.Any())
        {
            var appUsers = new List<AppUser>()
            {
                new AppUser()
                {
            UserName = "admin@gmail.com",
            Email = "admin@gmail.com",
            NormalizedEmail = "ADMIN@GMAIL.COM",
            Name = "Admin",
            LastName = "Admin",

            //Hashing passoword, class for this is at the end of this entire class
            PasswordHash = HashPassword("Admin123")

                }
            };
            foreach (var user in appUsers)
            {
                _context.Users.Add( user );
            }

            //Add more app users like psychologist/patient/psychiatrist
            _context.ApplicationUsers.AddRange( appUsers );
            _context.SaveChanges();
        }
        if (!_context.Patients.Any())
        {
            var patients = new List<Patient>()
            {
                new Patient()
                {
                    Name = "Maciej",
                    LastName = "Kizug",
                    Email = "maciej.kizug@gmail.com",
                    Phone = "565432125",
                    DateOfBirth = DateTime.Parse("1990-01-06"),
                    Address = "Fiołkowa 15b",
                    PsychiatristId = 1,
                    PsychologistId = 2
                },
                new Patient()
                {
                    Name = "Jagoda ",
                    LastName = "Kowalik",
                    Email = "jagoda.kow@gmail.com",
                    Phone = "2456123534",
                    DateOfBirth = DateTime.Parse("2000-12-24"),
                    Address = "Akacjowa 15b",
                    PsychiatristId = 2,
                    PsychologistId = 1


                }

            };
            _context.Patients.AddRange( patients );
            _context.SaveChanges();
        }




        if (!_context.Medicines.Any())
        {
            var medicines = new List<Medicine>()
            {
                new Medicine()
                {
                    Name = "Desyrel",
                    Description = "Desyrel is known by the trade name trazodone and is used to treat depression. It is in a class of antidepressants known as serotonin modulators. Desyrel comes in the form of a tablet that you take by mouth. When a person starts taking this medication, their doctor will slowly adjust their dosage until their condition is under control. Desyrel does not begin working immediately; it may take several weeks before the effects of the drug are noticeable. As with any prescription medication, one should listen to and follow the instructions of their doctor and pharmacist very carefully.",
                    Ingredients = "Active ingredient: trazodone hydrochloride, USP. Inactive ingredients: 50 mg and 100 mg: Corn starch, dibasic calcium",
                    DailyDose = 2,
                    InStock = 54,
                    ProductionDate = DateTime.Parse("2020-07-06"),
                    ExpireDate =  DateTime.Parse("2026-10-26"),
                    Category= new MedicineCategory()
                    {
                        Name = "Antipsychotic"
                    }

                },
                new Medicine()
                {
                    Name = "Lexapro",
                    Description = "Lexapro (escitalopram) is a selective serotonin reuptake inhibitor (SSRI) antidepressant medication. It is commonly used to treat anxiety in adults, depression in adults and adolescents who are at least 12 years old, tension, and excessive worry. It works by primarily increasing the amount of the neurotransmitter serotonin. It does this by preventing its reuptake into the presynaptic cell membrane. This SSRI drug is available by a doctor or psychiatrist’s prescription only.",
                    Ingredients = "Active ingredient: escitalopram oxalate\r\nInactive ingredients: • Tablets: talc, croscarmellose sodium, microcrystalline cellulose/colloidal silicon dioxide, and magnesium stearate. The film coating contains hypromellose, titanium dioxide, and polyethylene glycol.",
                    DailyDose = 2,
                    InStock = 156,
                    ProductionDate =  DateTime.Parse("2019-04-18"),
                    ExpireDate =  DateTime.Parse("2025-10-30"),
                    Category = new MedicineCategory()
                    {
                         Name = "SSRI"

                    }
                },
                new Medicine()
                {
                     Name = "Zoloft",
                     Description = "Zoloft is a prescription medicine used to treat the symptoms of Major Depressive disorder, Obsessive-Compulsive Disorder, Panic Disorder, Posttraumatic Stress Disorder (PTSD), Social Anxiety Disorder, and Premenstrual Dysphoric Disorder (PMDD). Zoloft may be used alone or with other medications.",
                     Ingredients = "Active ingredient: Sertraline Hydrochloride, USP Inactive ingredients: dibasic calcium phosphate dihydrate, hydroxypropyl cellulose, microcrystalline cellulose, magnesium stearate, opadry green (titanium dioxide, hypromellose 3cP, hypromellose 6cP, Macrogol/ Peg 400, polysorbate 80, D&C Yellow #10 Aluminum Lake",
                     DailyDose = 1,
                     InStock = 78,
                     ProductionDate = DateTime.Parse("2019-03-12"),
                     ExpireDate = DateTime.Parse("2025-08-14"),
                     Category = new MedicineCategory()
                     {
                          Name = "Antidepressant"

                     }
                },
                new Medicine()
                {
                    Name = "Xanax",
                    Description = "Benzodiazepine or “Xanax is a type of medication that is used to treat anxiety, depression or panic disorders and works to balance out the chemicals in our brain. It belongs to the group of drugs known as benzodiazepines and it is currently the most prescribed anxiety medication that is used in the United States.",
                    Ingredients = "Active ingredient: alprazolam Page 3 MEDICATION GUIDE ALPRAZOLAM Tablets, C-IV Inactive ingredients: Cellulose, corn starch, docusate sodium, lactose, magnesium stearate, silicon dioxide and sodium benzoate ",
                    InStock = 24,
                    DailyDose = 1,
                    ProductionDate = DateTime.Parse("2021-07-10"),
                    ExpireDate = DateTime.Parse("2024-11-05"),
                    Category = new MedicineCategory()
                    {
                        Name = "Anxiolytic"
                    }


                }

            };
            _context.Medicines.AddRange( medicines );
            _context.SaveChanges();
        }

        //First seed code above, after that, comment it, uncomment code belove and run seeddata again
        //If it doesn't work, restart visual studio and try again

        //if (!_context.PatientMedicines.Any())
        //{
        //    var patientMedicines = new List<PatientMedicine>()
        //    {
        //        new PatientMedicine()
        //        {
        //            PatientId = 1,
        //            MedicineId = 1,
        //        },
        //        new PatientMedicine()
        //        {
        //            PatientId = 1,
        //            MedicineId = 2,
        //        },
        //        new PatientMedicine()
        //        {
        //            PatientId = 1,
        //            MedicineId = 3,
        //        },
        //        new PatientMedicine()
        //        {
        //            PatientId = 2,
        //            MedicineId = 3,
        //        },


        //    };
        //    _context.PatientMedicines.AddRange( patientMedicines );
        //    _context.SaveChanges();
        //}



    }
    /// <summary>
    /// Hashing class
    /// </summary>
    /// <param name="password"></param>
    /// <returns></returns>
    private string HashPassword(string password)
    {
        var passwordHasher = new PasswordHasher<AppUser>();
        return passwordHasher.HashPassword( null, password );
    }
}


