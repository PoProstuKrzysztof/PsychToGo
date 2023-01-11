# PsychToGo
Web application for psychologist and psychiatrist to improve the way how they manage their patients.

It's using REST API archicheture along with SQL Server.



It works with your local SQL server for now, so you have to connect it to the program and then seed data by adding migration and applying it.

To seed data, after you connect it to database, open package manger (Tools > NuGet Package manager). Then write "Add-migration InitialCreate", next write "Update-database" and then run seed data by clicking on the project in "Solution explorer" and "Open terminal" at the end of the list. Run command "dotnet run seeddata", after seeding is done, you have to go into "DataSeed" class and go at the end of the file until you see commented code. Proceed as in instructions in code

Application is still in progress :)
                                                                
                                                         
