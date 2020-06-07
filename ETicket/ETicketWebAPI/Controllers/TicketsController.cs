﻿using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using log4net;
using ETicket.ApplicationServices.Services.Interfaces;
using ETicket.WebAPI.Models.TicketVerification;
using ETicket.ApplicationServices.Extensions;
using Swashbuckle.AspNetCore.Annotations;
using ETicket.ApplicationServices.DTOs;

namespace ETicket.WebAPI.Controllers
{
    [Route("api/tickets")]
    [ApiController]
    [SwaggerTag("Ticket service")]
    public class TicketsController : BaseAPIController
    {
        #region Private members

        private readonly ITicketService ticketService;
        private readonly ITicketVerificationService verificationService;
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        #endregion
        public TicketsController(ITicketService ticketService, ITicketVerificationService verificationService)
        {
            this.ticketService = ticketService;
            this.verificationService = verificationService;
        }

        // GET: api/tickets/5
        [HttpGet("{id}")]
        [SwaggerOperation(Summary = "Get ticket by id", Description = "Allowed: authorized user")]
        [SwaggerResponse(200, "Returns if everything is correct. Contains a Ticket object", typeof(TicketDto))]
        [SwaggerResponse(400, "Returns if an exception occurred")]
        [SwaggerResponse(404, "Returns if ticket is not found by id")]
        [SwaggerResponse(401, "Returns if user is unauthorized")]
        public IActionResult GetTicket([SwaggerParameter("Guid", Required = true)] Guid id)
        {
            log.Info(nameof(GetTicket));

            try
            {
                var ticketDto = ticketService.GetTicketById(id);

                if (ticketDto == null)
                {
                    log.Warn(nameof(GetTicket) + " ticketDto is null");

                    return NotFound();
                }

                return Json(ticketDto);
            }
            catch (Exception e)
            {
                log.Error(e);

                return BadRequest();
            }
        }

        [HttpGet("{ticketId}/verification-history")]
        [SwaggerOperation(Summary = "Get ticket verification history by id", Description = "Allowed: authorized user")]
        [SwaggerResponse(200, "Returns if everything is correct. Contains a list of ticket verifications")]
        [SwaggerResponse(400, "Returns if an exception occurred")]
        [SwaggerResponse(401, "Returns if user is unauthorized")]
        public IActionResult GetTicketVerificationHistory(Guid ticketId, [FromQuery]int pageNumber = 1, [FromQuery]int pageSize = 10)
        {
            log.Info(nameof(GetTicketVerificationHistory));

            try
            {
                var ticketVerificationPage = verificationService
                     .GetVerificationHistoryByTicketId(ticketId)
                     .ToPage(pageNumber, pageSize);            

                return Ok(ticketVerificationPage);
            }
            catch (Exception e)
            {
                log.Error(e);

                return BadRequest();
            }
        }

        // PUT: api/tickets/activate
        [HttpPut("/activate/{ticketId}")]
        [SwaggerOperation(Summary = "Update(activate) ticket", Description = "Allowed: authorized user")]
        [SwaggerResponse(204, "Returns if everything is correct, without content")]
        [SwaggerResponse(400, "Returns if an exception occurred")]
        [SwaggerResponse(401, "Returns if user is unauthorized")]
        public IActionResult Activate([SwaggerParameter("Guid", Required = true)] Guid ticketId)
        {
            log.Info(nameof(Activate));

            try
            {
                ticketService.Activate(ticketId);

                return NoContent();
            }
            catch (Exception e)
            {
                log.Error(e);

                return BadRequest();
            }
        }

        [HttpPost("{ticketId}/verify")]
        [SwaggerOperation(Summary = "Validate ticket endpoint", Description = "Allowed: Validator")]
        [SwaggerResponse(200, "Returns if everything is correct. Contains a VerifyTicketResponse", typeof(VerifyTicketResponceDto))]
        [SwaggerResponse(400, "Returns if an exception occurred")]
        [SwaggerResponse(401, "Returns if user is unauthorized")]
        public IActionResult VerifyTicket([SwaggerParameter("Guid", Required = true)] Guid ticketId, [FromBody, SwaggerRequestBody("Verify ticket payload", Required = true)] VerifyTicketRequest request)
        {
            log.Info(nameof(VerifyTicket));

            try
            {
                return Ok(verificationService.VerifyTicket(
                    ticketId, request.TransportId, request.Longitude, request.Latitude));
            }
            catch (Exception e)
            {
                log.Error(e);

                return BadRequest();
            }
        }
    }
}

