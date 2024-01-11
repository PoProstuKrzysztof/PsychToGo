# PsychToGo 👨‍⚕️
A web-based application for mental health clinics that manages data flow between patients, psychiatrists, and psychologists. Psychiatrists and psychologists are able to collaborate by directing patients to each other and prescribing prescriptions if necessary. 

### A brief description of how the application work 📑

The entire application will rely on the administrator to add new users. These users will include patients, psychologists, and psychiatrists. The administrator enters all of their necessary information after creating a new account.

By default, the patient gets assigned a psychologist. When it is required, the psychologist might refer him to a psychiatrist, who will issue him a prescription for drugs. Because they will be able to collaborate depending on the patient's needs, the psychiatrist and psychologist's job will be a lot faster and easier.
My first larger project, in which I developed my own API using the REST standard. 

### Technologies used 💻
+ C# 11
+ .NET 7
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
 Update-database
```

Now in Developer PowerShell

```bash
cd PsychToGo
```
Next
```bash
dotnet run seeddata
```
Sometimes it throws an exception, just try command again and it should work just fine.


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

