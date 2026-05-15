using Helix.Data.Entities;
using Helix.Service.DTOs.LabTestResultDTOs;
using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Helix.Data.Enums;
using Helix.Service.DTOs.LabOrderDTOs;

namespace Helix.Service.Services.LabOrderService
{
    public class LabOrderService(IUnitOfWork unitOfWork) : ILabOrderService
    {

        public async Task<Guid> CreateLabOrderAsync(CreateLabOrderDto dto)
        {
            // Generate a secure, 10-character random token for the QR code
            string qrToken = $"ORD-{Guid.NewGuid().ToString("N").Substring(0, 7).ToUpper()}";

            var labOrder = new LabOrder
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                TerminologyCodeId = dto.TerminologyCodeId,
                QrToken = qrToken,
                Status = EnLabOrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.Repository<LabOrder>().Add(labOrder);
            unitOfWork.Complete();

            return labOrder.Id;
        }

        public async Task<bool> DeleteLabOrderAsync(Guid id)
        {
            var order = await unitOfWork.Repository<LabOrder>().Get(id);
            if (order == null) return false;

            unitOfWork.Repository<LabOrder>().Delete(order);
            return unitOfWork.Complete() > 0;
        }

        public async Task<List<LabOrderDto>> GetAllLabOrdersAsync()
        {
            // Assuming your UnitOfWork has a way to expose IQueryable, like GetAll() or GetQueryable()
            return await unitOfWork.Repository<LabOrder>().Find(o => true).Result
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.TerminologyCode)
                .Select(o => new LabOrderDto
                {
                    OrderId = o.Id,
                    PatientName = $"{o.Patient.AppUser.FirstName} {o.Patient.AppUser.LastName}",
                    TestCode = o.TerminologyCode.Code,
                    TestName = o.TerminologyCode.Display
                })
                .ToListAsync();
        }

        public async Task<LabOrderDto> GetLabOrderByIdAsync(Guid id)
        {
            var order = await unitOfWork.Repository<LabOrder>().Find(o => true).Result
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.TerminologyCode)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) throw new KeyNotFoundException($"LabOrder with ID '{id}' was not found.");

            return new LabOrderDto
            {
                OrderId = order.Id,
                PatientName = $"{order.Patient.AppUser.FirstName} {order.Patient.AppUser.LastName}",
                TestCode = order.TerminologyCode.Code,
                TestName = order.TerminologyCode.Display
            };
        }

        public async Task<List<LabOrderDto>> GetOrdersByDoctorAsync(Guid doctorId)
        {
            return await unitOfWork.Repository<LabOrder>().Find(o => o.DoctorId == doctorId).Result
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.TerminologyCode)
                .Select(o => new LabOrderDto
                {
                    OrderId = o.Id,
                    PatientName = $"{o.Patient.AppUser.FirstName} {o.Patient.AppUser.LastName}",
                    TestCode = o.TerminologyCode.Code,
                    TestName = o.TerminologyCode.Display
                })
                .ToListAsync();
        }

        public async Task<List<PendingLabOrderDto>> GetPendingOrdersAsync(Guid patientId)
        {
            var pendingOrders = await unitOfWork.Repository<LabOrder>().Find(o => o.PatientId == patientId && o.Status == EnLabOrderStatus.Pending).Result
                .Include(o => o.TerminologyCode)
                .Select(o => new PendingLabOrderDto
                {
                    OrderId = o.Id,
                    TestName = o.TerminologyCode.Display,
                    QrToken = o.QrToken,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();

            return pendingOrders;
        }

        public async Task<LabOrderDto> ScanLabOrderAsync(string qrToken)
        {
            var order = await unitOfWork.Repository<LabOrder>().Find(o => o.QrToken == qrToken && o.Status == EnLabOrderStatus.Pending).Result
                .Include(o => o.Patient)
                    .ThenInclude(p => p.AppUser)
                .Include(o => o.TerminologyCode)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                // Changed from DllNotFoundException (which is for Windows System files) to KeyNotFoundException
                throw new KeyNotFoundException($"Pending LabOrder with QR Token '{qrToken}' was not found or is expired.");
            }

            return new LabOrderDto
            {
                OrderId = order.Id,
                PatientName = $"{order.Patient.AppUser.FirstName} {order.Patient.AppUser.LastName}",
                TestCode = order.TerminologyCode.Code,
                TestName = order.TerminologyCode.Display
            };
        }

        public async Task<bool> UpdateLabOrderAsync(Guid id, UpdateLabOrderDto dto)
        {
            var order = await unitOfWork.Repository<LabOrder>().Get(id);

            if (order == null) throw new KeyNotFoundException($"LabOrder with ID '{id}' was not found.");
            if (order.Status != EnLabOrderStatus.Pending) throw new InvalidOperationException("Only pending orders can be updated.");

            order.TerminologyCode = dto.TerminologyCodeId;

            unitOfWork.Repository<LabOrder>().Update(order);
            return unitOfWork.Complete() > 0;
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid id, string newStatus)
        {
            var order = await unitOfWork.Repository<LabOrder>().Get(id);
            if (order == null) throw new KeyNotFoundException($"LabOrder with ID '{id}' was not found.");

            if (Enum.TryParse<EnLabOrderStatus>(newStatus, true, out var parsedStatus))
            {
                order.Status = parsedStatus;
                unitOfWork.Repository<LabOrder>().Update(order);
                return unitOfWork.Complete() > 0;
            }

            throw new ArgumentException($"'{newStatus}' is not a valid lab order status.");
        }

        public async Task<bool> UploadLabResultAsync(LabTestResultDto dto)
        {
            // Assuming your LabTestResultDto contains the OrderId it belongs to
            var order = await unitOfWork.Repository<LabOrder>().Get(dto.OrderId);

            if (order == null || order.Status != EnLabOrderStatus.Pending)
            {
                throw new KeyNotFoundException($"Pending LabOrder with ID '{dto.OrderId}' was not found.");
            }

            // 1. Create the new lab result based on the DTO properties
            var result = new LabTestResult
            {
                PatientId = order.PatientId,
                DoctorId = order.DoctorId,
                TerminologyCode = order.TerminologyCode, // Inherits the test 
                ResultDate = DateTime.UtcNow
                // EncounterId can be mapped here if applicable
            };

            await unitOfWork.Repository<LabTestResult>().Add(result);

            // Note: We have to save once here if you need the LabTestResult.Id to assign back to the order
            unitOfWork.Complete();

            // 2. Update order status and link the result
            order.Status = EnLabOrderStatus.Completed;
            order.LabResultId = result.Id; // Linking the newly created result to the order

            unitOfWork.Repository<LabOrder>().Update(order);

            // 3. Commit transaction
            return unitOfWork.Complete() > 0;
        }
    }
}