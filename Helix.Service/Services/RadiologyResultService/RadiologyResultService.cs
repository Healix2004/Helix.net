using Helix.Data.Entities;
using Helix.Service.DTOs.RadiologyTestResultDto;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Service.Services.RadiologyResultService
{
    public class RadiologyResultService(IUnitOfWork unitOfWork, IWebHostEnvironment env) : IRadiologyResultService
    {
        //===========================================
        // PATIENT WORKFLOW
        //===========================================
        public async Task<RadiologyTestResultDto> GetRadiologyResultByOrderIdAsync(Guid orderId)
        {
            var result = await unitOfWork.Repository<RadiologyResult>()
                .Find(r => true).Result
                .Where(r => r.OrderId == orderId)
                .Include(r => r.Order)
                    .ThenInclude(o => o.Patient)
                        .ThenInclude(p => p.AppUser) // To get the patient's name
                .Select(r => new RadiologyTestResultDto
                {
                    Id = r.Id,
                    OrderId = r.OrderId,
                    PatientId = r.PatientId,
                    PatientName = $"{r.Order.Patient.AppUser.FirstName} {r.Order.Patient.AppUser.LastName}",
                    TerminologyDisplay = r.Order.TerminologyCode.Display,
                    Findings = r.Findings,
                    Impression = r.Impression,
                    PerformedDate = r.PerformedDate,
                    Images = r.Images.Select(img => new RadiologyImageDto
                    {
                        Id = img.Id,
                        FilePath = img.FilePath,
                        FileName = img.FileName,
                        Label = img.Label
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if (result == null)
                throw new KeyNotFoundException($"No radiology result found for Order ID {orderId}");

            return result;
        }

        public async Task<List<RadiologyTestResultDto>> GetRadiologyResultsByPatientIdAsync(Guid patientId)
        {
            return await unitOfWork.Repository<RadiologyResult>()
                .Find(r => true).Result
                .Where(r => r.PatientId == patientId)
                .Include(r => r.Order)
                .Select(r => new RadiologyTestResultDto
                {
                    Id = r.Id,
                    OrderId = r.OrderId,
                    TerminologyDisplay = r.Order.TerminologyCode.Display,
                    Findings = r.Findings,
                    Impression = r.Impression,
                    PerformedDate = r.PerformedDate,
                    Images = r.Images.Select(img => new RadiologyImageDto
                    {
                        Id = img.Id,
                        FilePath = img.FilePath,
                        FileName = img.FileName
                    }).ToList()
                })
                .OrderByDescending(r => r.PerformedDate) // Newest scans first
                .ToListAsync();
        }

        //===========================================
        // ADMIN WORKFLOW
        //===========================================

        public async Task<bool> DeleteRadiologyResultAsync(Guid id)
        {
            var result = await unitOfWork.Repository<RadiologyResult>()
                .Find(r => true).Result
                .Include(r => r.Images) // Must include images to get their file paths
                .FirstOrDefaultAsync(r => r.Id == id);

            if (result == null) return false;

            // Optional but Highly Recommended: Delete the physical files from the server
            // so your hard drive doesn't fill up with orphaned X-Rays!
            if (result.Images != null && result.Images.Any())
            {
                var webRootPath = env.WebRootPath ?? System.IO.Directory.GetCurrentDirectory();
                foreach (var img in result.Images)
                {
                    // Convert the relative URL ("/uploads/...") to a physical C:\ path
                    var fullPath = System.IO.Path.Combine(webRootPath, img.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }

            // Remove the database record
            unitOfWork.Repository<RadiologyResult>().Delete(result);
            return unitOfWork.Complete() > 0;
        }

        public async Task UpdateRadiologyResultAsync(UpdateRadiologyTestResultDto dto) // <-- Tweaked the DTO here!
        {
            var result = await unitOfWork.Repository<RadiologyResult>().Get(dto.Id);

            if (result == null)
                throw new KeyNotFoundException("Radiology result not found.");

            // Only update the fields that the doctor actually changed
            if (!string.IsNullOrWhiteSpace(dto.Findings))
                result.Findings = dto.Findings;

            if (!string.IsNullOrWhiteSpace(dto.Impression))
                result.Impression = dto.Impression;

            unitOfWork.Repository<RadiologyResult>().Update(result);
            unitOfWork.Complete() ;
        }

        public async Task<List<RadiologyTestResultDto>> GetAllRadiologyResultsAsync()
        {
            return await unitOfWork.Repository<RadiologyResult>()
                .Find(r => true).Result
                .Include(r => r.Order)
                    .ThenInclude(o => o.Patient)
                        .ThenInclude(p => p.AppUser) // To get the patient's name
                .Select(r => new RadiologyTestResultDto
                {
                    Id = r.Id,
                    OrderId = r.OrderId,
                    PatientName = $"{r.Order.Patient.AppUser.FirstName} {r.Order.Patient.AppUser.LastName}",
                    TerminologyDisplay = r.Order.TerminologyCode.Display,
                    PerformedDate = r.PerformedDate,
                })
                .OrderByDescending(r => r.PerformedDate)
                .ToListAsync();
        }
    }
}
