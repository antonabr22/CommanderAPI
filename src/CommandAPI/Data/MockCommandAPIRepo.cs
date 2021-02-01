using System.Collections.Generic;
using CommandAPI.Models;

namespace CommandAPI.Data
{
    public class MockCommandAPIRepo : ICommandAPIRepo
    {
        public void CreateCommand(Command cmd)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteCommand(Command cmd)
        {
            throw new System.NotImplementedException();
        }

        public IEnumerable<Command> GetAllCommands()
        {
            var commands = new List<Command> 
            {
                new Command { 
                    Id = 0, HowTo = "Generate a migration", 
                    Line = "dotnet ef migrations add <Name of Migration>", 
                    Platform = ".NET Core EF" },
                new Command { 
                    Id = 1, HowTo = "Run Migrations", 
                    Line = "dotnet ef database update", 
                    Platform = ".NET Core EF" },
                new Command { 
                    Id = 2, HowTo = "List active migrations", 
                    Line = "dotnet ef migrations list",
                    Platform = ".NET Core EF" }
            };

            return commands; 
        }

        public Command GetCommandById(int id)
        {
            return new Command { 
                    Id = 0, HowTo = "Generate a migration", 
                    Line = "dotnet ef migrations add <Name of Migration>", 
                    Platform = ".NET Core EF" };
        }

        public bool SaveChanges()
        {
            throw new System.NotImplementedException();
        }

        public void UpdateCommand(Command cmd)
        {
            throw new System.NotImplementedException();
        }
    }
}