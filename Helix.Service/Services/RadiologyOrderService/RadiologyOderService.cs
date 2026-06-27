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
                MedicalConceptId = terminologyLookup.Id,
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
            // 1. Asynchronous Queryable
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
                    RadiologyResultId = o.Report.Id,
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
            var query = await unitOfWork.Repository<RadiologyOrder>().FindAsQueryable(o => o.Id == id && o.Status == EnRadiologyOrderStatus.Pending);

            var order = await query
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.Doctor).ThenInclude(d => d.AppUser) // Added Doctor Include to prevent null refs on DoctorName
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
            // 2. Optimization: Eagerly load the Patient and AppUser immediately so we don't have to query the database twice!
            var query = await unitOfWork.Repository<RadiologyOrder>().FindAsQueryable(o => o.Id == dto.OrderId);
            var order = await query
                .Include(o => o.Patient)
                    .ThenInclude(p => p.AppUser)
                .FirstOrDefaultAsync();

            if (order == null || order.Status != EnRadiologyOrderStatus.Pending)
            {
                throw new KeyNotFoundException($"Pending RadiologyOrder with ID '{dto.OrderId}' was not found.");
            }

            // The patient is already guaranteed to be loaded here thanks to the .Include() above
            var patientName = order.Patient.FullName;

            // 3. Upload files using your FileService
            var uploadedImages = await fileService.UploadMultipleFilesAsync(dto.Images, patientName, order.PatientId);

            var result = new RadiologyReport
            {
                RadiologyOrderId = order.Id,
                ExternalRadiologistName = dto.ExternalRadiologistName, // If the DTO includes the name of the external doctor
                Findings = dto.Findings,
                Conclusion = dto.Impression, // Mapped from your DTO's "Impression" or "Conclusion"
                ReportDate = DateTime.UtcNow,
                //ImageUrls = uploadedImages // Assuming uploadedImages is a List<string> of URLs from Azure/S3/Local storage
            };

            await unitOfWork.Repository<RadiologyReport>().AddAsync(result);

            // 4. Double Await Strategy: Generate the Result ID first
            await unitOfWork.CompleteAsync();

            order.Status = EnRadiologyOrderStatus.Completed;
            order.Report = result;

            await unitOfWork.Repository<RadiologyOrder>().UpdateAsync(order);

            // Commit the updated order
            return await unitOfWork.CompleteAsync() > 0;
        }
    }
}