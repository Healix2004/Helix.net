using Helix.Data.Entities;
using Helix.Service.DTOs.RadiologyTestResultDto;
using Helix.Service.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;

namespace Helix.Service.Services.RadiologyResultService
{
    public class RadiologyResultService(IUnitOfWork unitOfWork, IWebHostEnvironment env) : IRadiologyResultService
    {
        //===========================================
        // PATIENT WORKFLOW
        //===========================================

        public async Task<RadiologyTestResultDto> GetRadiologyResultByOrderIdAsync(Guid orderId)
        {
            // 1. Optimized: Filter directly in FindAsQueryable
            var query = await unitOfWork.Repository<RadiologyResult>().FindAsQueryable(r => r.OrderId == orderId);

            var result = await query
                .Include(r => r.Order)
                    .ThenInclude(o => o.Patient)
                        .ThenInclude(p => p.AppUser) // To get the patient's name
                .Select(r => new RadiologyTestResultDto
                {
                    Id = r.Id,
                    OrderId = r.OrderId,
                    PatientId = r.PatientId,
                    PatientName = r.Order.Patient.AppUser.FullName,
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
            var query = await unitOfWork.Repository<RadiologyResult>().FindAsQueryable(r => r.PatientId == patientId);

            return await query
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
            var query = await unitOfWork.Repository<RadiologyResult>().FindAsQueryable(r => r.Id == id);

            var result = await query
                .Include(r => r.Images) // Must include images to get their file paths
                .FirstOrDefaultAsync();

            if (result == null) return false;

            // Delete the physical files from the server
            if (result.Images != null && result.Images.Any())
            {
                var webRootPath = env.WebRootPath ?? System.IO.Directory.GetCurrentDirectory();
                foreach (var img in result.Images)
                {
                    var fullPath = System.IO.Path.Combine(webRootPath, img.FilePath.TrimStart('/'));
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
            }

            // Remove the database record
            await unitOfWork.Repository<RadiologyResult>().DeleteAsync(result);
            return await unitOfWork.CompleteAsync() > 0;
        }

        public async Task UpdateRadiologyResultAsync(UpdateRadiologyTestResultDto dto)
        {
            var result = await unitOfWork.Repository<RadiologyResult>().GetByIdAsync(dto.Id);

            if (result == null)
                throw new KeyNotFoundException("Radiology result not found.");

            // Only update the fields that the doctor actually changed
            if (!string.IsNullOrWhiteSpace(dto.Findings))
                result.Findings = dto.Findings;

            if (!string.IsNullOrWhiteSpace(dto.Impression))
                result.Impression = dto.Impression;

            await unitOfWork.Repository<RadiologyResult>().UpdateAsync(result);
            await unitOfWork.CompleteAsync();
        }

        public async Task<List<RadiologyTestResultDto>> GetAllRadiologyResultsAsync()
        {
            var query = await unitOfWork.Repository<RadiologyResult>().FindAsQueryable(r => true);

            return await query
                .Include(r => r.Order)
                    .ThenInclude(o => o.Patient)
                        .ThenInclude(p => p.AppUser) // To get the patient's name
                .Select(r => new RadiologyTestResultDto
                {
                    Id = r.Id,
                    OrderId = r.OrderId,
                    PatientName = r.Order.Patient.AppUser.FullName,
                    TerminologyDisplay = r.Order.TerminologyCode.Display,
                    PerformedDate = r.PerformedDate,
                })
                .OrderByDescending(r => r.PerformedDate)
                .ToListAsync();
        }
    }
}