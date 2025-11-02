using LeRayBookingSystem.Data;
using LeRayBookingSystem.Models;
using LeRayBookingSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml; // ✅ Added for Excel export
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LeRayBookingSystem.Controllers
{
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AuditLogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _emailService;

        public AuditLogsController(ApplicationDbContext context, IEmailService emailService)
        {
            _context = context;
            _emailService = emailService;
        }

        // GET: AuditLogs
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.AuditLogs.Include(a => a.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: AuditLogs/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
                return NotFound();

            var auditLog = await _context.AuditLogs
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (auditLog == null)
                return NotFound();

            return View(auditLog);
        }

        // GET: AuditLogs/Create
        public IActionResult Create()
        {
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            return View();
        }

        // POST: AuditLogs/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Action,UserId,Timestamp,EntityType,EntityId,Details")] AuditLog auditLog)
        {
            if (ModelState.IsValid)
            {
                _context.Add(auditLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", auditLog.UserId);
            return View(auditLog);
        }

        // GET: AuditLogs/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
                return NotFound();

            var auditLog = await _context.AuditLogs.FindAsync(id);
            if (auditLog == null)
                return NotFound();

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", auditLog.UserId);
            return View(auditLog);
        }

        // POST: AuditLogs/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,Action,UserId,Timestamp,EntityType,EntityId,Details")] AuditLog auditLog)
        {
            if (id != auditLog.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(auditLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AuditLogExists(auditLog.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", auditLog.UserId);
            return View(auditLog);
        }

        // GET: AuditLogs/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
                return NotFound();

            var auditLog = await _context.AuditLogs
                .Include(a => a.User)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (auditLog == null)
                return NotFound();

            return View(auditLog);
        }

        // POST: AuditLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var auditLog = await _context.AuditLogs.FindAsync(id);
            if (auditLog != null)
            {
                _context.AuditLogs.Remove(auditLog);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool AuditLogExists(string id)
        {
            return _context.AuditLogs.Any(e => e.Id == id);
        }

        // ✅ STEP 3 — EXPORT TO EXCEL
        [HttpGet]
        public async Task<IActionResult> ExportToExcel()
        {
            var logs = await _context.AuditLogs
                .Include(a => a.User)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();

            // Disambiguate LicenseContext to OfficeOpenXml
            ExcelPackage.LicenseContext = OfficeOpenXml.LicenseContext.NonCommercial;
            using var package = new ExcelPackage();
            var worksheet = package.Workbook.Worksheets.Add("Audit Logs");

            // Header
            string[] headers = { "Action", "User", "Entity Type", "Entity ID", "Details", "Timestamp" };
            for (int i = 0; i < headers.Length; i++)
            {
                worksheet.Cells[1, i + 1].Value = headers[i];
                worksheet.Cells[1, i + 1].Style.Font.Bold = true;
                worksheet.Cells[1, i + 1].Style.Fill.PatternType = ExcelFillStyle.Solid;
                worksheet.Cells[1, i + 1].Style.Fill.BackgroundColor.SetColor(Color.LightGray);
            }

            // Rows
            int row = 2;
            foreach (var log in logs)
            {
                worksheet.Cells[row, 1].Value = log.Action;
                worksheet.Cells[row, 2].Value = log.User?.FullName ?? "Unknown";
                worksheet.Cells[row, 3].Value = log.EntityType;
                worksheet.Cells[row, 4].Value = log.EntityId;
                worksheet.Cells[row, 5].Value = log.Details;
                worksheet.Cells[row, 6].Value = log.Timestamp.ToString("g");
                row++;
            }

            worksheet.Cells.AutoFitColumns();

            // Add filter row
            worksheet.Cells["A1:F1"].AutoFilter = true;

            // Stream back to user
            var stream = new MemoryStream();
            await package.SaveAsAsync(stream);
            stream.Position = 0;

            var fileName = $"AuditLogs_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";
            return File(stream,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }
    }
}
