using System;
using System.Collections.Generic;
using AutoMapper;
using CommandAPI.Data;
using CommandAPI.Dtos;
using CommandAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;

namespace CommandAPI.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("api/[controller]")]
    public class CommandsController : ControllerBase
    {
        private readonly ICommandAPIRepo _repository;
        private readonly IMapper _mapper;

        public CommandsController(ICommandAPIRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        /// <summary>
        /// Получить данные всех команд.
        /// </summary>
        /// <returns>Список всех команд</returns> 
        /// <response code="200"> Возвращает список всех команд </response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<CommandReadDto>> GetAllCommands()
        {
            var commandItems = _repository.GetAllCommands();
            
            return Ok(_mapper.Map<IEnumerable<CommandReadDto>>(commandItems));
        }

        /// <summary>
        /// Получить данные команды по идентификатору.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Команда с заданным идентификатором</returns>
        /// <response code="200"> Возвращает команду с заданным Id </response>
        /// <response code="404"> Команда с указанным Id не найдена </response>
        [HttpGet("{id}", Name="GetCommandById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CommandReadDto> GetCommandById(int id)
        {
            var commandItem = _repository.GetCommandById(id);
            if (commandItem == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<CommandReadDto>(commandItem));
        }

        /// <summary>
        /// Добавить новую команду.
        /// </summary>
        /// <remarks>
        /// Пример тела запроса:
        ///
        ///     {
        ///        "howTo": "Create an EF migration",
        ///        "platform" "Entity Framework Core Command Line",
        ///        "commandLine": "dotnet ef database update"
        ///     }
        ///
        /// </remarks>
        /// <param name="commandCreateDto"></param>
        /// <returns>Только что созданную команду</returns>
        /// <response code="201"> Возвращает созданную команду </response>  
        [Authorize]
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public ActionResult<CommandReadDto> CreateCommand(CommandCreateDto commandCreateDto)
        {
            var commandModel = _mapper.Map<Command>(commandCreateDto);
            _repository.CreateCommand(commandModel);
            _repository.SaveChanges();

            var commandReadDto = _mapper.Map<CommandReadDto>(commandModel);

            return CreatedAtRoute(nameof(GetCommandById), new {Id = commandReadDto.Id}, commandReadDto);

        }

        /// <summary>
        /// Обновить данные команды по идентификатору.
        /// </summary>
        /// <remarks>
        /// Пример тела запроса:
        ///
        ///     {
        ///        "howTo": "Create an EF migration",
        ///        "platform" "Entity Framework Core Command Line",
        ///        "commandLine": "dotnet ef database update"
        ///     }
        ///
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="commandUpdateDto"></param>
        /// <returns></returns>
        /// <response code="204"> Команда успешно обновлена </response>  
        /// <response code="404"> Команда с указанным Id не найдена </response>
        [Authorize]
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<CommandReadDto> UpdateCommand(int id, CommandUpdateDto commandUpdateDto)
        {
            var commandModelFromRepo = _repository.GetCommandById(id);
            if (commandModelFromRepo == null)
            {
                return NotFound();
            }
            _mapper.Map(commandUpdateDto, commandModelFromRepo);
            
            _repository.UpdateCommand(commandModelFromRepo);

            _repository.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Частично обновить данные команды по идентификатору.
        /// </summary>
        /// <remarks>
        /// Пример тела запроса:
        /// 
        ///     [
        ///         {
        ///             "op": "replace",
        ///             "path": "/howTo",
        ///             "value": "Run a .NET Core App"
        ///         }
        ///     ]
        ///     
        /// </remarks>
        /// <param name="id"></param>
        /// <param name="patchDoc"></param>
        /// <returns> 204 No Content </returns>
        /// <response code="204"> Команда успешно изменена </response>
        /// <response code="400"> Команда не прошла валидацию </response>
        /// <response code="404"> Команда с указанным Id не найдена </response>
        [Authorize]
        [HttpPatch("{id}")]
        public ActionResult PartialCommandUpdate(int id, JsonPatchDocument<CommandUpdateDto> patchDoc)
        {
            var commandModelFromRepo = _repository.GetCommandById(id);
            if (commandModelFromRepo == null)
            {
                return NotFound();
            }

            var commandToPatch = _mapper.Map<CommandUpdateDto>(commandModelFromRepo);
            patchDoc.ApplyTo(commandToPatch, ModelState);

            if (!TryValidateModel(commandToPatch))
            {
                return ValidationProblem(ModelState);
            }

            _mapper.Map(commandToPatch, commandModelFromRepo);

            _repository.UpdateCommand(commandModelFromRepo);

            _repository.SaveChanges();

            return NoContent();
        }

        /// <summary>
        /// Удалить команду по идентификатору.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <response code="204"> Команда успешно удалена </response>
        /// <response code="404"> Команда с указанным Id не найдена </response>
        [Authorize]
        [HttpDelete("{id}")]
        public ActionResult DeleteCommand(int id)
        {
            var commandModelFromRepo = _repository.GetCommandById(id);
            if (commandModelFromRepo == null)
            {
                return NotFound();
            }
            _repository.DeleteCommand(commandModelFromRepo);
            _repository.SaveChanges();
            return NoContent();
        }
    }
}