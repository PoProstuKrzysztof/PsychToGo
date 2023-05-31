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

### Data seed instructions 🌱
It works with your local SQL server for now, so you have to connect it to the program and then seed data by adding migration and applying it.

To seed data, after you connect it to database, open package manger (Tools > NuGet Package manager). Then write "Add-migration InitialCreate", next write "Update-database" and then run seed data by clicking on the project in "Solution explorer" and "Open in terminal" at the end of the list.
Run command "dotnet run seeddata", after seeding is done, you have to go into "DataSeed" class and go at the end of the file until you see commented code. Proceed as in instructions in code.
(Sometimes seeddata after executing instructions doesn't work, In that case, simply restart your IDE )
