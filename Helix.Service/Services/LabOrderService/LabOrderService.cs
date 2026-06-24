using Helix.Data.Entities;
using Helix.Service.DTOs.LabTestResultDTOs;
using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Helix.Data.Enums;
using Helix.Service.DTOs.LabOrderDTOs;

namespace Helix.Service.Services.LabOrderService
{
    public class LabOrderService(IUnitOfWork unitOfWork, ITerminologyCodeLookupService terminologyService) : ILabOrderService
    {
        public async Task<Guid> CreateLabOrderAsync(CreateLabOrderDto dto)
        {
            string qrToken = $"ORD-{Guid.NewGuid().ToString("N").Substring(0, 7).ToUpper()}";

            var terminologyLookup = await terminologyService.GetTerminologyCodeLooKupByCodeAsync(dto.TerminologyCode);

            var labOrder = new LabOrder
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                TerminologyCodeId = terminologyLookup.Id,
                QrToken = qrToken,
                Status = EnLabOrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            await unitOfWork.Repository<LabOrder>().AddAsync(labOrder);
            await unitOfWork.CompleteAsync();

            return labOrder.Id;
        }

        public async Task<bool> DeleteLabOrderAsync(Guid id)
        {
            var order = await unitOfWork.Repository<LabOrder>().GetByIdAsync(id);
            if (order == null) return false;

            await unitOfWork.Repository<LabOrder>().DeleteAsync(order);
            return await unitOfWork.CompleteAsync() > 0;
        }

        public async Task<List<LabOrderDto>> GetAllLabOrdersAsync()
        {
            // 1. Await the Queryable safely without locking the thread
            var query = await unitOfWork.Repository<LabOrder>().FindAsQueryable(o => true);

            // 2. Await the final execution (.ToListAsync)
            return await query
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.TerminologyCode)
                .Select(o => new LabOrderDto
                {
                    OrderId = o.Id,
                    PatientName = o.Patient.FullName,
                    TestCode = o.TerminologyCode.Code,
                    TestName = o.TerminologyCode.Display
                })
                .ToListAsync();
        }

        public async Task<LabOrderDto> GetLabOrderByIdAsync(Guid id)
        {
            // Filter directly in the FindAsQueryable for maximum database efficiency
            var query = await unitOfWork.Repository<LabOrder>().FindAsQueryable(o => o.Id == id);

            var order = await query
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.TerminologyCode)
                .FirstOrDefaultAsync();

            if (order == null)
                throw new KeyNotFoundException($"LabOrder with ID '{id}' was not found.");

            return new LabOrderDto
            {
                OrderId = order.Id,
                PatientName = order.Patient.FullName,
                TestCode = order.TerminologyCode.Code,
                TestName = order.TerminologyCode.Display
            };
        }

        public async Task<List<LabOrderDto>> GetOrdersByDoctorAsync(Guid doctorId)
        {
            var query = await unitOfWork.Repository<LabOrder>().FindAsQueryable(o => o.DoctorId == doctorId);

            return await query
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.TerminologyCode)
                .Select(o => new LabOrderDto
                {
                    OrderId = o.Id,
                    PatientName = o.Patient.FullName,
                    TestCode = o.TerminologyCode.Code,
                    TestName = o.TerminologyCode.Display
                })
                .ToListAsync();
        }

        public async Task<List<PendingLabOrderDto>> GetPendingOrdersAsync(Guid patientId)
        {
            var query = await unitOfWork.Repository<LabOrder>().FindAsQueryable(o => o.PatientId == patientId && o.Status == EnLabOrderStatus.Pending);

            return await query
                .Include(o => o.TerminologyCode)
                .Select(o => new PendingLabOrderDto
                {
                    OrderId = o.Id,
                    TestName = o.TerminologyCode.Display,
                    QrToken = o.QrToken,
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();
        }

        public async Task<LabOrderDto> ScanLabOrderAsync(string qrToken)
        {
            var query = await unitOfWork.Repository<LabOrder>().FindAsQueryable(o => o.QrToken == qrToken && o.Status == EnLabOrderStatus.Pending);

            var order = await query
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.TerminologyCode)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                throw new KeyNotFoundException($"Pending LabOrder with QR Token '{qrToken}' was not found or is expired.");
            }

            return new LabOrderDto
            {
                OrderId = order.Id,
                PatientName = order.Patient.FullName,
                TestCode = order.TerminologyCode.Code,
                TestName = order.TerminologyCode.Display
            };
        }

        public async Task<bool> UpdateLabOrderAsync(Guid id, UpdateLabOrderDto dto)
        {
            var order = await unitOfWork.Repository<LabOrder>().GetByIdAsync(id);

            if (order == null) throw new KeyNotFoundException($"LabOrder with ID '{id}' was not found.");
            if (order.Status != EnLabOrderStatus.Pending) throw new InvalidOperationException("Only pending orders can be updated.");

            var terminologyLookup = await terminologyService.GetTerminologyCodeLooKupByCodeAsync(dto.TerminologyCode);
            order.TerminologyCodeId = terminologyLookup.Id;

            await unitOfWork.Repository<LabOrder>().UpdateAsync(order);
            return await unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid id, string newStatus)
        {
            var order = await unitOfWork.Repository<LabOrder>().GetByIdAsync(id);
            if (order == null) throw new KeyNotFoundException($"LabOrder with ID '{id}' was not found.");

            if (Enum.TryParse<EnLabOrderStatus>(newStatus, true, out var parsedStatus))
            {
                order.Status = parsedStatus;
                await unitOfWork.Repository<LabOrder>().UpdateAsync(order);
                return await unitOfWork.CompleteAsync() > 0;
            }

            throw new ArgumentException($"'{newStatus}' is not a valid lab order status.");
        }

        public async Task<bool> UploadLabResultAsync(UploadLabResultDto dto)
        {
            var order = await unitOfWork.Repository<LabOrder>().GetByIdAsync(dto.OrderId);

            if (order == null || order.Status != EnLabOrderStatus.Pending)
            {
                throw new KeyNotFoundException($"Pending LabOrder with ID '{dto.OrderId}' was not found.");
            }

            var result = new LabTestResult
            {
                OrderId = dto.OrderId,
                PatientId = order.PatientId,
                TerminologyCodeId = order.TerminologyCodeId,
                ResultDate = DateTime.UtcNow,
                Status = EnLabOrderStatus.Completed,
                Value = dto.Value,
                Unit = dto.Unit
            };

            await unitOfWork.Repository<LabTestResult>().AddAsync(result);

            // 3. Double Await Strategy: First save generates the Guid/ID for the LabTestResult
            await unitOfWork.CompleteAsync();

            order.Status = EnLabOrderStatus.Completed;
            order.LabResultId = result.Id;

            await unitOfWork.Repository<LabOrder>().UpdateAsync(order);

            // 4. Second save commits the updated order
            return await unitOfWork.CompleteAsync() > 0;
        }
    }
}