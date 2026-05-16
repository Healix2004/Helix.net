using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Service.DTOs.LabTestResultDTOs;
using Helix.Service.DTOs.RadiologyOrderDto;
using Helix.Service.DTOs.RadiologyTestResultDto;
using Helix.Service.Interfaces;
using Helix.Service.Repositories;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Service.Services.RadiologyOder
{
    public class RadiologyOderService(IUnitOfWork unitOfWork,ITerminologyCodeLookupService terminologyService) : IRadiologyOrderService
    {
        public async Task<Guid> CreateRadiologyOrderAsync(CreateRadiologyOrderDto dto)
        {
            // Generate a secure, 10-character random token for the QR code
            string qrToken = $"ORD-{Guid.NewGuid().ToString("N").Substring(0, 7).ToUpper()}";

            var RadioOrder = new RadiologyOrder
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                TerminologyCodeId = (await terminologyService.GetTerminologyCodeLooKupByCodeAsync(dto.TerminologyCode)).Id,
                QrToken = qrToken,
                Status = EnLabOrderStatus.Pending,
                CreateDate = DateTime.UtcNow
            };

            await unitOfWork.Repository<RadiologyOrder>().Add(RadioOrder);
            unitOfWork.Complete();

            return RadioOrder.Id;
        }

        public async Task<bool> DeleteRadiologyOrderAsync(Guid id)
        {
            var order =await unitOfWork.Repository<RadiologyOrder>().Get(id);
            if (order == null) return false;
            unitOfWork.Repository<RadiologyOrder>().Delete(order);
            return unitOfWork.Complete() > 0;
        }

        public async Task<List<RadiologyOrderDto>> GetAllRadiologyOrdersAsync()
        {
            return await unitOfWork.Repository<RadiologyOrder>().Find(r=> true).Result
                .Include(o => o.TerminologyCode).Include(t => t.Doctor).ThenInclude(d => d.AppUser)
                .Include(r => r.Result)
                .Select(o => new RadiologyOrderDto
                {
                    Id = o.Id,
                    PatientId = o.PatientId,
                    DoctorId = o.DoctorId,
                    DoctorName = o.Doctor.AppUser.FirstName +" "+o.Doctor.AppUser.LastName, // Assuming you have a navigation property for Doctor
                    TerminologyCodeId = o.TerminologyCodeId,
                    TerminologyDisplay = o.TerminologyCode.Display,
                    TerminologyCode = o.TerminologyCode.Code,
                    QrToken = o.QrToken,
                    Status = o.Status,
                    RadiologyResultId = o.Result.Id,
                    CreatedAt = o.CreateDate
                })
                .ToListAsync();
        }

        public Task<List<RadiologyOrderDto>> GetOrdersByDoctorAsync(Guid doctorId)
        {
            return unitOfWork.Repository<RadiologyOrder>().Find(o => o.DoctorId == doctorId).Result
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.TerminologyCode)
                .Select(o => new RadiologyOrderDto
                {
                    Id = o.Id,
                    PatientId = o.PatientId,
                    PatientName = $"{o.Patient.AppUser.FirstName} {o.Patient.AppUser.LastName}",
                    DoctorId = o.DoctorId,
                    DoctorName = $"{o.Doctor.AppUser.FirstName} {o.Doctor.AppUser.LastName}",
                    TerminologyCodeId = o.TerminologyCodeId,
                    TerminologyDisplay = o.TerminologyCode.Display,
                    TerminologyCode = o.TerminologyCode.Code,
                    QrToken = o.QrToken,
                    Status = o.Status,
                    CreatedAt = o.CreateDate
                })
                .ToListAsync();
        }

        public async Task<List<PendingRadiologyOrderDto>> GetPendingOrdersAsync(Guid patientId)
        {
            var pendingOrders = await unitOfWork.Repository<RadiologyOrder>().Find(o => o.PatientId == patientId && o.Status == EnLabOrderStatus.Pending).Result
            .Include(o => o.TerminologyCode)
            .Select(o => new PendingRadiologyOrderDto
            {
                Id = o.Id,
                TerminologyDisplay= o.TerminologyCode.Display,
                QrToken = o.QrToken,
                CreatedAt = o.CreateDate,
            })
            .ToListAsync();

            return pendingOrders;
        }

        public async Task<RadiologyOrderDto> GetRadiologyOrderByIdAsync(Guid id)
        {
            var order = await unitOfWork.Repository<RadiologyOrder>().Get(id);
            if(order == null) throw new KeyNotFoundException($"LabOrder with ID '{id}' was not found.");

            return new RadiologyOrderDto
            {
                Id = order.Id,
                PatientId = order.PatientId,
                PatientName = $"{order.Patient.AppUser.FirstName} {order.Patient.AppUser.LastName}",
                DoctorId = order.DoctorId,
                DoctorName = $"{order.Doctor.AppUser.FirstName} {order.Doctor.AppUser.LastName}",
                TerminologyCodeId = order.TerminologyCodeId,
                QrToken = order.QrToken,
                Status = order.Status,
                CreatedAt = order.CreateDate
            };
        }

        public async Task<RadiologyOrderDto> ScanRadiologyOrderAsync(string qrToken)
        {
            var order = await unitOfWork.Repository<RadiologyOrder>().Find(o => o.QrToken == qrToken && o.Status == EnLabOrderStatus.Pending).Result
            .Include(o => o.Patient).ThenInclude(p => p.AppUser)
            .Include(o => o.TerminologyCode)
            .FirstOrDefaultAsync();

            if (order == null)
            {
                // Changed from DllNotFoundException (which is for Windows System files) to KeyNotFoundException
                throw new KeyNotFoundException($"Pending LabOrder with QR Token '{qrToken}' was not found or is expired.");
            }

            return new RadiologyOrderDto
            {
                Id = order.Id,
                PatientName = $"{order.Patient.AppUser.FirstName} {order.Patient.AppUser.LastName}",
                TerminologyCode = order.TerminologyCode.Code,
                TerminologyDisplay = order.TerminologyCode.Display,
                CreatedAt=order.CreateDate
            };
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid id, string newStatus)
        {
            var order = await unitOfWork.Repository<RadiologyOrder>().Get(id);

            if (order == null) throw new KeyNotFoundException($"LabOrder with ID '{id}' was not found.");

            if (Enum.TryParse<EnLabOrderStatus>(newStatus, true, out var parsedStatus))
            {
                order.Status = parsedStatus;
                unitOfWork.Repository<RadiologyOrder>().Update(order);
                return unitOfWork.Complete() > 0;
            }
            throw new ArgumentException($"'{newStatus}' is not a valid lab order status.");
        }

        public async Task<bool> UpdateRadiologyOrderAsync(Guid id, UpdateRadiologyOrderDto dto)
        {
            var order = await unitOfWork.Repository<RadiologyOrder>().Get(id);
            if (order == null) throw new KeyNotFoundException($"LabOrder with ID '{id}' was not found.");
            if (order.Status != EnLabOrderStatus.Pending) throw new InvalidOperationException("Only pending orders can be updated.");
            order.TerminologyCodeId = (Guid)dto.TerminologyCodeId;
            unitOfWork.Repository<RadiologyOrder>().Update(order);
            return unitOfWork.Complete() > 0;
        }

        public async Task<bool> UploadResultAsync(CreateRadiologyTestResultDto dto)
        {
            // Assuming your LabTestResultDto contains the OrderId it belongs to
            var order = await unitOfWork.Repository<RadiologyOrder>().Get(dto.OrderId);

            if (order == null || order.Status != EnLabOrderStatus.Pending)
            {
                throw new KeyNotFoundException($"Pending LabOrder with ID '{dto.OrderId}' was not found.");
            }

            // 1. Create the new lab result based on the DTO properties
            var result = new RadiologyResult
            {
                PatientId = order.PatientId,
                Findings=dto.Findings,                
                Impression = dto.Impression,
                PerformedDate = DateTime.UtcNow,
                StudyType = "Some Study Type",
                Images= dto.UploadedFilePaths.Select(filePath => new RadiologyImage()
                {
                    FilePath = filePath,
                    FileName = Path.GetFileName(filePath),
                    FileSizeInKB = new FileInfo(filePath).Length / 1024
                }).ToList()
                // EncounterId can be mapped here if applicable
            };

            await unitOfWork.Repository<RadiologyResult>().Add(result);

            // Note: We have to save once here if you need the RadiologyResult.Id to assign back to the order
            unitOfWork.Complete();

            // 2. Update order status and link the result
            order.Status = EnLabOrderStatus.Completed;
            order.Result = result; // Linking the newly created result to the order

            unitOfWork.Repository<RadiologyOrder>().Update(order);
            unitOfWork.Complete();

            // 3. Commit transaction
            return unitOfWork.Complete() > 0;
        }
    }
}
