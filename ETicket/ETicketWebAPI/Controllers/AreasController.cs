﻿using System;
using System.Reflection;
using ETicket.ApplicationServices.DTOs;
using ETicket.ApplicationServices.Services.Interfaces;
using log4net;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace ETicket.WebAPI.Controllers
{
    [Route("api/areas")]
    [ApiController]
    [SwaggerTag("Area service")]
    public class AreasController : ControllerBase
    {
        private readonly IAreaService areaService;
        private static readonly ILog logger = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public AreasController(IAreaService areaService)
        {
            this.areaService = areaService;
        }
        
        [HttpGet]
        [SwaggerOperation(Summary = "Get all areas", Description = "Allowed: everyone")]
        [SwaggerResponse(200, "Returns if everything was ok. Contains a list of areas")]
        [SwaggerResponse(400, "Returns if exseption occurred")]
        public IActionResult GetAreas()
        {
            logger.Info(nameof(AreasController.GetAreas));
            
            try
            {
                return Ok(areaService.GetAreas());
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                
                return BadRequest();
            }
        }

        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get area by id", Description = "Allowed: everyone")]
        [SwaggerResponse(200, "Returns if everything was ok. Contains an Area object", typeof(AreaDto))]
        [SwaggerResponse(400, "Returns if exseption occurred")]
        public IActionResult GetArea([SwaggerParameter("Int", Required = true)] int id)
        {
            logger.Info(nameof(AreasController.GetArea));
            
            try
            {
                return Ok(areaService.GetAreaById(id));
            }
            catch (Exception exception)
            {
                logger.Error(exception);
                
                return BadRequest();
            }
        }
    }
}