using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Service.DTOs.RadiologyOrderDto;
using Helix.Service.DTOs.RadiologyTestResultDto;
using Helix.Service.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helix.Service.Services.RadiologyOrderService
{
    public class RadiologyOrderService(IUnitOfWork unitOfWork, IFileService fileService) : IRadiologyOrderService
    {
        public async Task<Guid> CreateRadiologyOrderAsync(CreateRadiologyOrderDto dto)
        {
            string qrToken = $"ORD-{Guid.NewGuid().ToString("N").Substring(0, 7).ToUpper()}";

            var medicalConceptQuery = await unitOfWork.Repository<MedicalConcept>().FindAsQueryable(m => m.Code == dto.TerminologyCode);
            var newMedicalConcept = await medicalConceptQuery.FirstOrDefaultAsync();

            if (newMedicalConcept == null) throw new KeyNotFoundException("Terminology code not found.");

            var radioOrder = new RadiologyOrder
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                MedicalConceptId = newMedicalConcept.Id,
                QrToken = qrToken,
                Status = EnRadiologyOrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.Repository<RadiologyOrder>().AddAsync(radioOrder);
            await unitOfWork.CompleteAsync();

            return radioOrder.Id;
        }

        public async Task<bool> DeleteRadiologyOrderAsync(Guid id)
        {
            var order = await unitOfWork.Repository<RadiologyOrder>().GetByIdAsync(id);
            if (order == null) return false;

            await unitOfWork.Repository<RadiologyOrder>().DeleteAsync(order);
            return await unitOfWork.CompleteAsync() > 0;
        }

        public async Task<List<RadiologyOrderDto>> GetAllRadiologyOrdersAsync()
        {
            var query = await unitOfWork.Repository<RadiologyOrder>().FindAsQueryable(r => true);

            return await query
                .Include(o => o.MedicalConcept)
                .Include(t => t.Doctor).ThenInclude(d => d.AppUser)
                .Include(r => r.Report)
                .Select(o => new RadiologyOrderDto
                {
                    Id = o.Id,
                    PatientId = o.PatientId,
                    DoctorId = o.DoctorId,
                    DoctorName = o.Doctor.FullName,
                    TerminologyCodeId = o.MedicalConceptId,
                    TerminologyDisplay = o.MedicalConcept.Display,
                    TerminologyCode = o.MedicalConcept.Code,
                    QrToken = o.QrToken,
                    Status = o.Status,
                    RadiologyResultId = o.Report != null ? o.Report.Id : Guid.Empty,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<List<RadiologyOrderDto>> GetOrdersByDoctorAsync(Guid doctorId)
        {
            var query = await unitOfWork.Repository<RadiologyOrder>().FindAsQueryable(o => o.DoctorId == doctorId);

            return await query
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.MedicalConcept)
                .Select(o => new RadiologyOrderDto
                {
                    Id = o.Id,
                    PatientId = o.PatientId,
                    PatientName = o.Patient.FullName,
                    DoctorId = o.DoctorId,
                    DoctorName = o.Doctor.FullName,
                    TerminologyCodeId = o.MedicalConceptId,
                    TerminologyDisplay = o.MedicalConcept.Display,
                    TerminologyCode = o.MedicalConcept.Code,
                    QrToken = o.QrToken,
                    Status = o.Status,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<List<RadiologyOrderDto>> GetOrdersByPatientAsync(Guid patientId)
        {
            var query = await unitOfWork.Repository<RadiologyOrder>()
                .FindAsQueryable(o => o.PatientId == patientId);

            return await query
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.MedicalConcept)
                .Include(o => o.Report)
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new RadiologyOrderDto
                {
                    Id = o.Id,
                    PatientName = o.Patient.FullName,
                    TerminologyCode = o.MedicalConcept.Code,
                    TerminologyDisplay = o.MedicalConcept.Display,
                    Status = o.Status,
                    RadiologyResultId = o.Report != null ? o.Report.Id : Guid.Empty,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<List<PendingRadiologyOrderDto>> GetPendingOrdersAsync(Guid patientId)
        {
            var query = await unitOfWork.Repository<RadiologyOrder>().FindAsQueryable(o => o.PatientId == patientId && o.Status == EnRadiologyOrderStatus.Pending);

            return await query
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.MedicalConcept)
                .Select(o => new PendingRadiologyOrderDto
                {
                    Id = o.Id,
                    PatientName = o.Patient.FullName,
                    TerminologyDisplay = o.MedicalConcept.Display,
                    QrToken = o.QrToken,
                    CreatedAt = o.CreatedAt,
                    RequestingDoctorName = o.Doctor.FullName,
                })
                .ToListAsync();
        }

        public async Task<RadiologyOrderDto> GetRadiologyOrderByIdAsync(Guid id)
        {
            var query = await unitOfWork.Repository<RadiologyOrder>().FindAsQueryable(o => o.Id == id);

            var order = await query
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.Doctor).ThenInclude(d => d.AppUser)
                .Include(o => o.MedicalConcept)
                .FirstOrDefaultAsync();

            if (order == null)
                throw new KeyNotFoundException($"RadiologyOrder with ID '{id}' was not found.");

            return new RadiologyOrderDto
            {
                Id = order.Id,
                PatientId = order.PatientId,
                PatientName = order.Patient.FullName,
                DoctorId = order.DoctorId,
                DoctorName = order.Doctor.FullName,
                TerminologyCodeId = order.MedicalConceptId,
                QrToken = order.QrToken,
                Status = order.Status,
                CreatedAt = order.CreatedAt
            };
        }

        public async Task<RadiologyOrderDto> ScanRadiologyOrderAsync(string qrToken)
        {
            var query = await unitOfWork.Repository<RadiologyOrder>().FindAsQueryable(o => o.QrToken == qrToken && o.Status == EnRadiologyOrderStatus.Pending);

            var order = await query
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.MedicalConcept)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                throw new KeyNotFoundException($"Pending RadiologyOrder with QR Token '{qrToken}' was not found or is expired.");
            }

            return new RadiologyOrderDto
            {
                Id = order.Id,
                PatientName = order.Patient.FullName,
                TerminologyCode = order.MedicalConcept.Code,
                TerminologyDisplay = order.MedicalConcept.Display,
                CreatedAt = order.CreatedAt
            };
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid id, string newStatus)
        {
            var order = await unitOfWork.Repository<RadiologyOrder>().GetByIdAsync(id);

            if (order == null)
                throw new KeyNotFoundException($"RadiologyOrder with ID '{id}' was not found.");

            if (Enum.TryParse<EnRadiologyOrderStatus>(newStatus, true, out var parsedStatus))
            {
                order.Status = parsedStatus;
                await unitOfWork.Repository<RadiologyOrder>().UpdateAsync(order);
                return await unitOfWork.CompleteAsync() > 0;
            }
            throw new ArgumentException($"'{newStatus}' is not a valid order status.");
        }

        public async Task<bool> UpdateRadiologyOrderAsync(Guid id, UpdateRadiologyOrderDto dto)
        {
            var order = await unitOfWork.Repository<RadiologyOrder>().GetByIdAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"RadiologyOrder with ID '{id}' was not found.");

            if (order.Status != EnRadiologyOrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be updated.");

            order.MedicalConceptId = (Guid)dto.TerminologyCodeId;

            await unitOfWork.Repository<RadiologyOrder>().UpdateAsync(order);
            return await unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UploadResultAsync(CreateRadiologyTestResultDto dto)
        {
            var query = await unitOfWork.Repository<RadiologyOrder>().FindAsQueryable(o => o.Id == dto.OrderId);
            var order = await query
                .Include(o => o.Patient)
                    .ThenInclude(p => p.AppUser)
                .FirstOrDefaultAsync();

            if (order == null || order.Status != EnRadiologyOrderStatus.Pending)
            {
                throw new KeyNotFoundException($"Pending RadiologyOrder with ID '{dto.OrderId}' was not found.");
            }

            var patientName = order.Patient.FullName;

            var uploadedImages = await fileService.UploadMultipleFilesAsync(dto.Images, patientName, order.PatientId);

            var result = new RadiologyReport
            {
                RadiologyOrderId = order.Id,
                ExternalRadiologistName = dto.ExternalRadiologistName,
                Findings = dto.Findings,
                Conclusion = dto.Impression,
                ReportDate = DateTime.UtcNow,
                ImageUrls = uploadedImages.Select(i => i.FilePath).ToList()
            };

            await unitOfWork.Repository<RadiologyReport>().AddAsync(result);
            await unitOfWork.CompleteAsync();

            order.Status = EnRadiologyOrderStatus.Completed;
            order.Report = result;

            await unitOfWork.Repository<RadiologyOrder>().UpdateAsync(order);
            return await unitOfWork.CompleteAsync() > 0;
        }
    }
}