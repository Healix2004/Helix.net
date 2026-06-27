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
            // Generate a unique 7-character order token for the barcode/QR
            string qrToken = $"ORD-{Guid.NewGuid().ToString("N").Substring(0, 7).ToUpper()}";

            var medicalConceptQuery = await unitOfWork.Repository<MedicalConcept>().FindAsQueryable(m => m.Code == dto.TerminologyCode);
            var newMedicalConcept = await medicalConceptQuery.FirstOrDefaultAsync();

            if (newMedicalConcept == null) throw new KeyNotFoundException("Terminology code not found.");

            var labOrder = new LabOrder
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                MedicalConceptId = newMedicalConcept.Id,
                QrToken = qrToken,
                Status = EnLabOrderStatus.Pending,
                CreatedAt = DateTime.UtcNow
            };

            // FHIR PANEL EXPANSION LOGIC
            var panelComponentsQuery = await unitOfWork.Repository<LoincPanelComponent>()
                .FindAsQueryable(p => p.ParentLoincConceptId == newMedicalConcept.Id);

            var childIds = await panelComponentsQuery.Select(p => p.ChildLoincConceptId).ToListAsync();

            if (childIds.Any())
            {
                // It is a Panel: Create an empty slot for every child test
                foreach (var childId in childIds)
                {
                    labOrder.Results.Add(new LabTestResult
                    {
                        MedicalConceptId = childId,
                        PatientId = dto.PatientId,
                        Status = EnLabOrderStatus.Pending,
                        ResultDate = null
                    });
                }
            }
            else
            {
                // It is a Single Test: Create one empty slot
                labOrder.Results.Add(new LabTestResult
                {
                    MedicalConceptId = newMedicalConcept.Id,
                    PatientId = dto.PatientId,
                    Status = EnLabOrderStatus.Pending,
                    ResultDate = null
                });
            }

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
            var query = await unitOfWork.Repository<LabOrder>().FindAsQueryable(o => true);

            return await query
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.MedicalConcept)
                .Select(o => new LabOrderDto
                {
                    OrderId = o.Id,
                    PatientName = o.Patient.FullName,
                    TestCode = o.MedicalConcept.Code,
                    TestName = o.MedicalConcept.Display,               
                    Status = o.Status.ToString() // MAP THE STATUS HERE

                })
                .ToListAsync();
        }
        public async Task<LabOrderDto> GetLabOrderByIdAsync(Guid id)
        {
            var query = await unitOfWork.Repository<LabOrder>().FindAsQueryable(o => o.Id == id);

            var order = await query
                .Include(o => o.Patient) // Ensure the patient data is loaded
                .Include(o => o.MedicalConcept)
                .Include(o => o.Results)
                    .ThenInclude(r => r.MedicalConcept) // Ensure child names are loaded
                .FirstOrDefaultAsync();

            if (order == null)
                throw new KeyNotFoundException($"LabOrder with ID '{id}' was not found.");

            return new LabOrderDto
            {
                OrderId = order.Id,
                // Added a fallback string just in case your database dummy data is missing a name
                PatientName = string.IsNullOrWhiteSpace(order.Patient?.FullName) ? "Unknown Patient" : order.Patient.FullName,
                TestCode = order.MedicalConcept?.Code,
                TestName = order.MedicalConcept?.Display,
                Status = order.Status.ToString(), // MAP THE STATUS 
                // THE FIX: Map the child slots into the DTO
                Results = order.Results.Select(r => new LabTestResultDto
                {
                    ResultId = r.Id,
                    MedicalConceptId = r.MedicalConceptId,
                    TestCode = r.MedicalConcept?.Code,
                    TestName = r.MedicalConcept?.Display,
                    Status = r.Status,
                    NumericValue = r.NumericValue,
                    StringValue = r.StringValue,
                    Unit = r.Unit,
                    ReferenceRange = r.ReferenceRange,
                    InterpretationFlag = r.InterpretationFlag,
                    ResultDate = r.ResultDate
                }).ToList()
            };
        }
        public async Task<List<LabOrderDto>> GetOrdersByDoctorAsync(Guid doctorId)
        {
            var query = await unitOfWork.Repository<LabOrder>().FindAsQueryable(o => o.DoctorId == doctorId);

            return await query
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.MedicalConcept)
                .Select(o => new LabOrderDto
                {
                    OrderId = o.Id,
                    PatientName = o.Patient.FullName,
                    TestCode = o.MedicalConcept.Code,
                    Status = o.Status.ToString(), // MAP THE STATUS HERE
                    TestName = o.MedicalConcept.Display
                })
                .ToListAsync();
        }
        public async Task<List<PendingLabOrderDto>> GetPendingOrdersAsync(Guid patientId)
        {
            var query = await unitOfWork.Repository<LabOrder>().FindAsQueryable(o => o.PatientId == patientId && o.Status == EnLabOrderStatus.Pending);

            return await query
                .Include(o => o.MedicalConcept)
                .Select(o => new PendingLabOrderDto
                {
                    OrderId = o.Id,
                    TestName = o.MedicalConcept.Display,
                    QrToken = o.QrToken,
                    Status = o.Status.ToString(), // MAP THE STATUS HERE
                    CreatedAt = o.CreatedAt
                })
                .ToListAsync();
        }
        public async Task<LabOrderDto> ScanLabOrderAsync(string qrToken)
        {
            var query = await unitOfWork.Repository<LabOrder>().FindAsQueryable(o => o.QrToken == qrToken && o.Status == EnLabOrderStatus.Pending);

            var order = await query
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.MedicalConcept)
                .Include(o => o.Results)
                    .ThenInclude(r => r.MedicalConcept)
                .FirstOrDefaultAsync();

            if (order == null)
            {
                throw new KeyNotFoundException($"Pending LabOrder with QR Token '{qrToken}' was not found or is expired.");
            }

            return new LabOrderDto
            {
                OrderId = order.Id,
                PatientName = order.Patient.FullName,
                TestCode = order.MedicalConcept.Code,
                Status = order.Status.ToString(), // MAP THE STATUS HERE
                TestName = order.MedicalConcept.Display
            };
        }
        public async Task<bool> UpdateLabOrderAsync(Guid id, UpdateLabOrderDto dto)
        {
            var query = await unitOfWork.Repository<LabOrder>().FindAsQueryable(o => o.Id == id);
            var order = await query.Include(o => o.Results).FirstOrDefaultAsync();

            if (order == null) throw new KeyNotFoundException($"LabOrder with ID '{id}' was not found.");
            if (order.Status != EnLabOrderStatus.Pending) throw new InvalidOperationException("Only pending orders can be updated.");

            var medicalConceptQuery = await unitOfWork.Repository<MedicalConcept>().FindAsQueryable(m => m.Code == dto.TerminologyCode);
            var newMedicalConcept = await medicalConceptQuery.FirstOrDefaultAsync();

            if (newMedicalConcept == null) throw new KeyNotFoundException("Terminology code not found.");

            // If the doctor changed the test type entirely, cleanly recreate the required child slots
            if (order.MedicalConceptId != newMedicalConcept.Id)
            {
                order.MedicalConceptId = newMedicalConcept.Id;

                foreach (var oldResult in order.Results.ToList())
                {
                    await unitOfWork.Repository<LabTestResult>().DeleteAsync(oldResult);
                }
                order.Results.Clear();

                var panelComponentsQuery = await unitOfWork.Repository<LoincPanelComponent>()
                    .FindAsQueryable(p => p.ParentLoincConceptId == newMedicalConcept.Id);

                var childIds = await panelComponentsQuery.Select(p => p.ChildLoincConceptId).ToListAsync();

                if (childIds.Any())
                {
                    foreach (var childId in childIds)
                    {
                        order.Results.Add(new LabTestResult
                        {
                            MedicalConceptId = childId,
                            PatientId = order.PatientId,
                            Status = EnLabOrderStatus.Pending
                        });
                    }
                }
                else
                {
                    order.Results.Add(new LabTestResult
                    {
                        MedicalConceptId = newMedicalConcept.Id,
                        PatientId = order.PatientId,
                        Status = EnLabOrderStatus.Pending
                    });
                }
            }

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
            var query = await unitOfWork.Repository<LabOrder>().FindAsQueryable(o => o.Id == dto.OrderId);
            var order = await query.Include(o => o.Results).FirstOrDefaultAsync();

            if (order == null || order.Status != EnLabOrderStatus.Pending)
            {
                throw new KeyNotFoundException($"Pending LabOrder with ID '{dto.OrderId}' was not found.");
            }

            foreach (var resultInput in dto.Results)
            {
                var targetSlot = order.Results.FirstOrDefault(r => r.MedicalConceptId == resultInput.MedicalConceptId);

                if (targetSlot != null)
                {
                    // Map the updated DTO values to the entity
                    targetSlot.NumericValue = resultInput.NumericValue;
                    targetSlot.StringValue = resultInput.StringValue;
                    targetSlot.ReferenceRange = resultInput.ReferenceRange;
                    targetSlot.InterpretationFlag = resultInput.InterpretationFlag;
                    targetSlot.Unit = resultInput.Unit;

                    targetSlot.Status = EnLabOrderStatus.Completed;
                    targetSlot.ResultDate = DateTime.UtcNow;

                    await unitOfWork.Repository<LabTestResult>().UpdateAsync(targetSlot);
                }
            }

            order.Status = EnLabOrderStatus.Completed;
            await unitOfWork.Repository<LabOrder>().UpdateAsync(order);

            return await unitOfWork.CompleteAsync() > 0;
        }
        public async Task<List<LabOrderDto>> GetOrdersByPatientAsync(Guid patientId)
        {
            var query = await unitOfWork.Repository<LabOrder>()
                .FindAsQueryable(o => o.PatientId == patientId);

            return await query
                .Include(o => o.Patient).ThenInclude(p => p.AppUser)
                .Include(o => o.MedicalConcept)
                // Sort by newest first!
                .OrderByDescending(o => o.CreatedAt)
                .Select(o => new LabOrderDto
                {
                    OrderId = o.Id,
                    PatientName = o.Patient.FullName,
                    TestCode = o.MedicalConcept.Code,
                    TestName = o.MedicalConcept.Display,
                    Status = o.Status.ToString() // MAP THE STATUS HERE
                })
                .ToListAsync();
        }
    }
}