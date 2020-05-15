using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ETicket.ApplicationServices.DTOs;
using ETicket.ApplicationServices.Services.Interfaces;

namespace ETicket.Admin.Controllers
{
    [Authorize(Roles = "Admin, SuperUser")]
    public class DocumentTypesController : Controller
    {
        private readonly IDocumentTypesService service;

        public DocumentTypesController(IDocumentTypesService service)
        {
            this.service = service;
        }

        // GET: DocumentTypes
        public IActionResult Index()
        {
            var documentTypes = service.GetAll();

            return View(documentTypes);
        }

        // GET: DocumentTypes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: DocumentTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(DocumentTypeDto documentTypeDto)
        {
            if (ModelState.IsValid)
            {
                service.Create(documentTypeDto);

                return RedirectToAction(nameof(Index));
            }

            return View(documentTypeDto);
        }

        // GET: DocumentTypes/Edit/5
        public IActionResult Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentType = service.Get(id.Value);
            if (documentType == null)
            {
                return NotFound();
            }

            return View(documentType);
        }

        // POST: DocumentTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, DocumentTypeDto documentTypeDto)
        {
            if (id != documentTypeDto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    service.Update(documentTypeDto);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!service.Exists(documentTypeDto.Id))
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

            return View(documentTypeDto);
        }

        // GET: DocumentTypes/Delete/5
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documentType = service.Get(id.Value);
            if (documentType == null)
            {
                return NotFound();
            }

            return View(documentType);
        }

        // POST: DocumentTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            service.Delete(id);

            return RedirectToAction(nameof(Index));
        }
    }
}
