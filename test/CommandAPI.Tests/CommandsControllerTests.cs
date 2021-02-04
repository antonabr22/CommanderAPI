using System.Collections.Generic;
using AutoMapper;
using CommandAPI.Controllers;
using CommandAPI.Data;
using CommandAPI.Models;
using CommandAPI.Profiles;
using CommandAPI.Dtos;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Mvc;
using System;

namespace CommandAPI.Tests
{
    public class CommandsControllerTests : IDisposable
    {
        private Mock<ICommandAPIRepo> mockRepo;
        private CommandsProfile realProfile;
        private MapperConfiguration configuration;
        private IMapper mapper;

        public CommandsControllerTests()
        {
            mockRepo = new Mock<ICommandAPIRepo>();

            mockRepo.Setup(repo => 
                repo.GetAllCommands()).Returns(GetCommands(0));

            realProfile = new CommandsProfile();
            configuration = new MapperConfiguration(cfg =>
                cfg.AddProfile(realProfile));
            mapper = new Mapper(configuration);            
        }

        public void Dispose()
        {
            mockRepo = null;
            mapper = null;
            realProfile = null;
            configuration = null;
        }

        [Fact]
        public void GetCommandItems_ReturnsZeroItems_WhenDBIsEmpty()
        {
            // Arrange
            mockRepo.Setup(repo => 
                repo.GetAllCommands()).Returns(GetCommands(0));

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.GetAllCommands();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }

        [Fact]
        public void GetCommandItems_ReturnsOneItem_WhenDBHasOneResource()
        {
            // Arrange
            mockRepo.Setup(repo => 
                repo.GetAllCommands()).Returns(GetCommands(1));

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.GetAllCommands();

            // Assert 
            var okResult = result.Result as OkObjectResult;
            var commands = okResult.Value as IEnumerable<CommandReadDto>;

            Assert.Single(commands);
        }

        [Fact]
        public void GetCommandItems_Returns200OK_WhenDBHasOneResource()
        {
            // Arrange
            mockRepo.Setup(repo => 
                repo.GetAllCommands()).Returns(GetCommands(1));

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.GetAllCommands();

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }        


        [Fact]
        public void GetCommandItems_ReturnsCorrectType_WhenDBHasOneResource()   // most useful one
        {
            // Arrange
            mockRepo.Setup(repo => 
                repo.GetAllCommands()).Returns(GetCommands(1));

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.GetAllCommands();

            // Assert
            Assert.IsType<ActionResult<IEnumerable<CommandReadDto>>>(result);
        }

        [Fact]
        public void GetCommandByID_Returns404NotFound_WhenNonExistingIDProvided()
        {
            // Arrange
            mockRepo.Setup(repo => 
                repo.GetCommandById(1)).Returns(() => null);

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.GetCommandById(1);

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }
        
        [Fact]
        public void GetCommandByID_Returns200OK_WhenValidIDProvided()
        {
            // Arrange
            mockRepo.Setup(repo => 
                repo.GetCommandById(1)).Returns(new Command { Id = 1, HowTo = "mock",
                Platform = "mock", CommandLine = "mock"});

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.GetCommandById(1);

            // Assert
            Assert.IsType<OkObjectResult>(result.Result);
        }        

        [Fact]
        public void GetCommandByID_ReturnsCorrectType_WhenValidIDProvided()
        {
            // Arrange
            mockRepo.Setup(repo => 
                repo.GetCommandById(1)).Returns(new Command { Id = 1, HowTo = "mock",
                Platform = "mock", CommandLine = "mock"});

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.GetCommandById(1);

            // Assert
            Assert.IsType<ActionResult<CommandReadDto>>(result);
        } 

        [Fact]
        public void CreateCommand_ReturnsCorrectResourceType_WhenValidObjectSubmitted()
        {
            // Arrange
            mockRepo.Setup(repo => 
                repo.GetCommandById(1)).Returns(new Command { Id = 1, HowTo = "mock",
                Platform = "mock", CommandLine = "mock"});

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.CreateCommand(new CommandCreateDto {});

            // Assert
            Assert.IsType<ActionResult<CommandReadDto>>(result);
        } 

        [Fact]
        public void CreateCommand_Returns201Created_WhenValidObjectSubmitted()
        {
            // Arrange
            mockRepo.Setup(repo => 
                repo.GetCommandById(1)).Returns(new Command { Id = 1,HowTo = "mock",
                Platform = "mock", CommandLine = "mock"});

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.CreateCommand(new CommandCreateDto {});

            // Assert
            Assert.IsType<CreatedAtRouteResult>(result.Result);
        }

        [Fact]
        public void UpdateCommand_Returns204NoContent_WhenValidObjectSubmitted()
        {
            // Arrange
            mockRepo.Setup(repo => 
                repo.GetCommandById(1)).Returns(new Command { Id = 1, HowTo = "mock",
                Platform = "mock", CommandLine = "mock"});

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.UpdateCommand(1, new CommandUpdateDto {});

            // Assert
            Assert.IsType<NoContentResult>(result.Result);
        }

        [Fact]
        public void UpdateCommand_Returns404NotFound_WhenNonExistingResourceIDSubmitted()
        {
            // Arrange
            mockRepo.Setup(repo => 
                repo.GetCommandById(0)).Returns(() => null);

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.UpdateCommand(0, new CommandUpdateDto {});

            // Assert
            Assert.IsType<NotFoundResult>(result.Result);
        }

        [Fact]
        public void PartialCommandUpdate_Returns404NotFound_WhenNonExistingResourceIDSubmitted()
        {
            // Arrange
            mockRepo.Setup(repo => 
                repo.GetCommandById(0)).Returns(() => null);

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.PartialCommandUpdate(0, 
                new Microsoft.AspNetCore.JsonPatch.JsonPatchDocument<CommandUpdateDto>{});

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void DeleteCommand_Returns204NoContent_WhenExistingResourceIDSubmitted()
        {
            // Arrange
            mockRepo.Setup(repo => 
                repo.GetCommandById(1)).Returns(new Command { Id = 1, HowTo = "mock",
                Platform = "mock", CommandLine = "mock"});

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.DeleteCommand(1);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public void DeleteCommand_Returns404NotFound_WhenNonExistingResourceIDSubmitted()
        {
            // Arrange
            mockRepo.Setup(repo => 
                repo.GetCommandById(0)).Returns(() => null);

            var controller = new CommandsController(mockRepo.Object, mapper);

            // Act
            var result = controller.DeleteCommand(0);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        private List<Command> GetCommands(int num)
        {
            var commands = new List<Command>();
            if (num > 0) {
                commands.Add(new Command { 
                    Id = 0,
                    HowTo = "How to generate a migration", 
                    Platform = ".NET Core EF"}
                );
            }
            return commands;
        }
    }
}