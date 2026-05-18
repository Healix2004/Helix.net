using Helix.Data.Entities;
using Helix.Service.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Helix.Service.Services.EmergencyAccessService
{
    public class EmergencyAccessService(IUnitOfWork unitOfWork) : IEmergencyAccessService
    {
        public async Task<bool> ActivateEmergencyOverrideAsync(Guid doctorId, Guid patientId, string reason)
        {
            if (string.IsNullOrWhiteSpace(reason) || reason.Length < 15)
                throw new Exception("A detailed clinical justification (minimum 15 characters) is legally required to break the glass.");

            var overrideLog = new EmergencyOverrideLog
            {
                DoctorId = doctorId,
                PatientId = patientId,
                JustificationReason = reason,
                OverrideTimestamp = DateTime.UtcNow,
                ExpirationTimestamp = DateTime.UtcNow.AddHours(12) // Grants 12 hours of emergency access
            };

            await unitOfWork.Repository<EmergencyOverrideLog>().AddAsync(overrideLog);
            await unitOfWork.CompleteAsync();

            // TODO: Trigger an email or push notification to the Hospital Administrator here!

            return true;
        }

        public async Task<bool> HasActiveEmergencyAccessAsync(Guid doctorId, Guid patientId)
        {
            // Checks if there is a valid, unexpired emergency override for this specific doctor and patient
            var activeOverrides = await unitOfWork.Repository<EmergencyOverrideLog>()
                .FindAsync(e => e.DoctorId == doctorId&& e.PatientId == patientId&& e.ExpirationTimestamp > DateTime.UtcNow);
            return activeOverrides.Any();
        }
    }
}
