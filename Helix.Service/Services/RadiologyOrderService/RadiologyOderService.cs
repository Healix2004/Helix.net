using Helix.Data.Entities;
using Helix.Data.Enums;
using Helix.Service.DTOs.RadiologyOrderDto;
using Helix.Service.DTOs.RadiologyTestResultDto;
using Helix.Service.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Helix.Service.Services.RadiologyOrderService // Fixed typo in namespace
{
    public class RadiologyOrderService(IUnitOfWork unitOfWork, ITerminologyCodeLookupService terminologyService, IFileService fileService) : IRadiologyOrderService
    {
        public async Task<Guid> CreateRadiologyOrderAsync(CreateRadiologyOrderDto dto)
        {
            string qrToken = $"ORD-{Guid.NewGuid().ToString("N").Substring(0, 7).ToUpper()}";

            var terminologyLookup = await terminologyService.GetTerminologyCodeLooKupByCodeAsync(dto.TerminologyCode);

            var radioOrder = new RadiologyOrder
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                TerminologyCodeId = terminologyLookup.Id,
                QrToken = qrToken,
                Status = EnLabOrderStatus.Pending,
                CreateDate = DateTime.UtcNow
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
            // 1. Asynchronous Queryable
            var query = await unitOfWork.Repository<RadiologyOrder>().FindAsQueryable(r => true);

            return await query
                .Include(o => o.TerminologyCode)
                .Include(t => t.Doctor).ThenInclude(d => d.AppUser)
                .Include(r => r.Result)
                .Select(o => new RadiologyOrderDto
                {
                    Id = o.Id,
                    PatientId = o.PatientId,
                    DoctorId = o.DoctorId,
                    DoctorName = o.Doctor.FullName,
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

        public async Task<List<RadiologyOrderDto>> GetOrdersByDoctorAsync(Guid doctorId)
        {
            var query = await unitOfWork.Repository<RadiologyOrder>().FindAsQueryable(o => o.DoctorId == doctorId);

            return await query
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.TerminologyCode)
                .Select(o => new RadiologyOrderDto
                {
                    Id = o.Id,
                    PatientId = o.PatientId,
                    PatientName = o.Patient.FullName,
                    DoctorId = o.DoctorId,
                    DoctorName = o.Doctor.FullName,
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
            var query = await unitOfWork.Repository<RadiologyOrder>().FindAsQueryable(o => o.PatientId == patientId && o.Status == EnLabOrderStatus.Pending);

            return await query
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.TerminologyCode)
                .Select(o => new PendingRadiologyOrderDto
                {
                    Id = o.Id,
                    PatientName = o.Patient.FullName,
                    TerminologyDisplay = o.TerminologyCode.Display,
                    QrToken = o.QrToken,
                    CreatedAt = o.CreateDate,
                    RequestingDoctorName = o.Doctor.FullName,
                })
                .ToListAsync();
        }

        public async Task<RadiologyOrderDto> GetRadiologyOrderByIdAsync(Guid id)
        {
            var query = await unitOfWork.Repository<RadiologyOrder>().FindAsQueryable(o => o.Id == id && o.Status == EnLabOrderStatus.Pending);

            var order = await query
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.Doctor).ThenInclude(d => d.AppUser) // Added Doctor Include to prevent null refs on DoctorName
                .Include(o => o.TerminologyCode)
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
                TerminologyCodeId = order.TerminologyCodeId,
                QrToken = order.QrToken,
                Status = order.Status,
                CreatedAt = order.CreateDate
            };
        }

        public async Task<RadiologyOrderDto> ScanRadiologyOrderAsync(string qrToken)
        {
            var query = await unitOfWork.Repository<RadiologyOrder>().FindAsQueryable(o => o.QrToken == qrToken && o.Status == EnLabOrderStatus.Pending);

            var order = await query
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.TerminologyCode)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                throw new KeyNotFoundException($"Pending RadiologyOrder with QR Token '{qrToken}' was not found or is expired.");
            }

            return new RadiologyOrderDto
            {
                Id = order.Id,
                PatientName = order.Patient.FullName,
                TerminologyCode = order.TerminologyCode.Code,
                TerminologyDisplay = order.TerminologyCode.Display,
                CreatedAt = order.CreateDate
            };
        }

        public async Task<bool> UpdateOrderStatusAsync(Guid id, string newStatus)
        {
            var order = await unitOfWork.Repository<RadiologyOrder>().GetByIdAsync(id);

            if (order == null)
                throw new KeyNotFoundException($"RadiologyOrder with ID '{id}' was not found.");

            if (Enum.TryParse<EnLabOrderStatus>(newStatus, true, out var parsedStatus))
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

            if (order.Status != EnLabOrderStatus.Pending)
                throw new InvalidOperationException("Only pending orders can be updated.");

            order.TerminologyCodeId = (Guid)dto.TerminologyCodeId;

            await unitOfWork.Repository<RadiologyOrder>().UpdateAsync(order);
            return await unitOfWork.CompleteAsync() > 0;
        }

        public async Task<bool> UploadResultAsync(CreateRadiologyTestResultDto dto)
        {
            // 2. Optimization: Eagerly load the Patient and AppUser immediately so we don't have to query the database twice!
            var query = await unitOfWork.Repository<RadiologyOrder>().FindAsQueryable(o => o.Id == dto.OrderId);
            var order = await query
                .Include(o => o.Patient)
                    .ThenInclude(p => p.AppUser)
                .FirstOrDefaultAsync();

            if (order == null || order.Status != EnLabOrderStatus.Pending)
            {
                throw new KeyNotFoundException($"Pending RadiologyOrder with ID '{dto.OrderId}' was not found.");
            }

            // The patient is already guaranteed to be loaded here thanks to the .Include() above
            var patientName = order.Patient.FullName;

            // 3. Upload files using your FileService
            var uploadedImages = await fileService.UploadMultipleFilesAsync(dto.UploadedFilePaths, patientName, order.PatientId);

            var result = new RadiologyResult
            {
                PatientId = order.PatientId,
                OrderId = order.Id,
                Findings = dto.Findings,
                Impression = dto.Impression,
                PerformedDate = DateTime.UtcNow,
                StudyType = "Some Study Type", // Note: Consider passing this in from the DTO!
                Images = new List<RadiologyImage>(uploadedImages)
            };

            await unitOfWork.Repository<RadiologyResult>().AddAsync(result);

            // 4. Double Await Strategy: Generate the Result ID first
            await unitOfWork.CompleteAsync();

            order.Status = EnLabOrderStatus.Completed;
            order.Result = result;

            await unitOfWork.Repository<RadiologyOrder>().UpdateAsync(order);

            // Commit the updated order
            return await unitOfWork.CompleteAsync() > 0;
        }
    }
}