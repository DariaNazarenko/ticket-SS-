﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DBContextLibrary.Domain;
using DBContextLibrary.Domain.Entities;
using DBContextLibrary.Domain.Repositories;
using ETicketAdmin.Models;
using ETicketAdmin.Services;
using ETicketAdmin.Common;

namespace ETicketAdmin.Controllers
{
    public class UserController : Controller
    {
        private readonly ETicketDataContext _context;
        private readonly ETicketData _repository;

        public UserController(ETicketDataContext context)
        {
            _context = context;
            _repository = new ETicketData(_context);
        }

        // GET: User
        public async Task<IActionResult> Index(int? pageNumber, string sortOrder)
        {
            ViewBag.LastNameSortParm = String.IsNullOrEmpty(sortOrder) ? "LastName_desc" : "";
            ViewBag.FirstNameSortParm = sortOrder == "FirstName" ? "FirstName_desc" : "FirstName";
            var eTicketDataContext = _context.Users.Include(u => u.Document).Include(u => u.Privilege).Include(u => u.Role);
            IOrderedQueryable<User> users;

            switch (sortOrder)
            {
                case "LastName_desc":
                    users = eTicketDataContext.OrderByDescending(s=>s.LastName);
                    break;
                case "FirstName_desc":
                    users = eTicketDataContext.OrderByDescending(s => s.FirstName);
                    break;
                case "FirstName":
                    users = eTicketDataContext.OrderBy(s => s.FirstName);
                    break;
                default:
                    users = eTicketDataContext.OrderBy(s => s.LastName);
                    break;
            }
 
            if (!pageNumber.HasValue)
            {
                pageNumber = 1;
            }  
            var pageSize = CommonSettings.DefaultPageSize;

            return View(await PaginatedList<User>.CreateAsync(users, pageNumber.Value, pageSize));
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Document)
                .Include(u => u.Privilege)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id); 

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            ViewData["DocumentId"] = new SelectList(_context.Documents, "Id", "Number");
            ViewData["PrivilegeId"] = new SelectList(_context.Privileges, "Id", "Name");
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name");

            return View();
        }

        // POST: User/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FirstName,LastName,Phone,Email,RoleId,PrivilegeId,DocumentId")] User user)
        {
            if (ModelState.IsValid)
            {
                user.Id = Guid.NewGuid();
                _context.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            ViewData["DocumentId"] = new SelectList(_context.Documents, "Id", "Number", user.DocumentId);
            ViewData["PrivilegeId"] = new SelectList(_context.Privileges, "Id", "Name", user.PrivilegeId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);

            return View(user);
        }

        // GET: User/SendMessage/5
        public async Task<IActionResult> SendMessage(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/SendMessage
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendMessage(Guid id, string message)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null)
                {
                    return NotFound();
                } 

                MailService emailService = new MailService();
                await emailService.SendEmailAsync(user.Email, message);

                return RedirectToAction(nameof(Index));
            } 
            return View(message);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            ViewData["DocumentId"] = new SelectList(_context.Documents, "Id", "Number", user.DocumentId);
            ViewData["PrivilegeId"] = new SelectList(_context.Privileges, "Id", "Name", user.PrivilegeId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);

            return View(user);
        }

        // POST: User/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("Id,FirstName,LastName,Phone,Email,RoleId,PrivilegeId,DocumentId")] User user)
        {
            if (id != user.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index));
            }

            ViewData["DocumentId"] = new SelectList(_context.Documents, "Id", "Number", user.DocumentId);
            ViewData["PrivilegeId"] = new SelectList(_context.Privileges, "Id", "Name", user.PrivilegeId);
            ViewData["RoleId"] = new SelectList(_context.Roles, "Id", "Name", user.RoleId);

            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.Users
                .Include(u => u.Document)
                .Include(u => u.Privilege)
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            var user = await _context.Users.FindAsync(id);
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(Guid id)
        {
            return _context.Users.Any(e => e.Id == id);
        }
    }
}
