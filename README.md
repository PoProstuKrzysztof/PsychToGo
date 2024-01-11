# PsychToGo 👨‍⚕️
A web application for mental health centers that organizes data flow within the organization for patients, psychiatrists, and psychologists. By sending patients to each other and giving prescriptions if necessary, psychiatrists and psychologists can collaborate with one another. 

### A brief description of how the application work 📑

The entire application will rely on an administrator to add new users. These users will be patients, psychologists and psychiatrists. Administrator fill's all of their's sensitive informations after creating a new account.

By default, the psychologist is assigned to the patient. When it's necessary, the psychologist has the option to place him under the care of a psychiatrist, who will write him a prescription for medicine. Since they will be able to collaborate based on the needs of the patient, this will make the work of the psychiatrist and psychologist much quicker and simpler.

### Technologies used 💻
+ C# 11
+ .NET 7 Framework
+ SQL 
+ JWT 
+ Entity Framework
+ HTML5, CSS, Boostrap 5
+ AutoMapper

### What has already been implemented 🔽
+ JWT Token
+ Searching 
+ Sorting
+ Creating accounts
+ API
+ Prescribing medicine
+ Assigning psychiatrist to patient
+ CRUD Operations on patients/psychiatrists/psychologists/medicines
+ Caching
+ Razor components

### Data seed instructions 🌱
It works with your local SQL server for now, so you have to connect it to the program and then seed data by adding migration and applying it.
In PM Console

```bash
  Add-Migration InitialCreate
```
next
```bash
 Update-databse
```

After that, in PowerShell 

```bash
dotnet run seeddata
```
After seeding is done, in 
```bash
 PsychToGo.API.DataSeed 
```
Comment code between line 19 and 313, and uncomment code below, and run 

```bash
dotnet run seeddata
```

Sometimes seeddata after executing instructions doesn't work, In that case, simply restart your IDE


## Login credentials

All credentials are fictitious and unrelated to real people. 

| Role | Login     | Password                |
| :-------- | :------- | :------------------------- |
| `Admin` | `admin@gmail.com` | Admin123 |
| `Psychologist` | `antonina.malecka@gmail.com` | Test!23 |
| `Psychologist` | `mariusz.wilczyn@gmail.com` | Test!23 |
| `Psychiatrist` | `lena.wozniak@gmail.com` | Test!23 |
| `Psychiatrist` | `patrycja.bednarska@gmail.com` | Test!23 |
| `Patient` | `maciej.kizug@gmail.com` | Test!23 |
| `Patient` | `jagoda.kow@gmail.com` | Test!23 |

