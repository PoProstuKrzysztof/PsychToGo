# PsychToGo
Web application for psychologist and psychiatrist to improve the way how they manage their patients.

It's using REST API archicheture along with SQL Server.



It works with your local SQL server for now, so you have to connect it to the program and then seed data by adding migration and applying it.

To seed data, after you connect it to database, open package manger (Tools > NuGet Package manager). Then write "Add-migration InitialCreate", next write "Update-database" and then run seed data by clicking on the project in "Solution explorer" and "Open in terminal" at the end of the list. Run command "dotnet run seeddata", after seeding is done, you have to go into "DataSeed" class and go at the end of the file until you see commented code. Proceed as in instructions in code.
(Sometimes seeddata after executing instructions doesn't work, In that case, simply restart your IDE )

Application is still in progress :)


-------------------------------------------------------------------------------------------------------------------------------------------------------------------------
                                                                
                                                         
A brief description of how the application will work in the future .

The entire application will rely on an administrator to add new users. These users will be patients, psychologists and psychiatrists. When logging in for the first time, each of them will have to fill in basic information about themselves. 

The patient is assigned to psychologist by default. The psychologist after some time, can assign him to a psychiatrist who will assign him medication. With this, the work of the psychiatrist and psychologist will be simpler and faster because they will be able to work together depending on the needs of the patient.

