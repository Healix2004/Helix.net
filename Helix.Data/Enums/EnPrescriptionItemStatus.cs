namespace Helix.Data.Enums
{
    public enum EnPrescriptionItemStatus
    {
        Active = 0,         // The patient is currently taking this medication
        Completed = 1,      // The full duration/course has been finished
        Stopped = 2,        // The doctor halted the medication early (e.g., due to side effects)
        OnHold = 3,         // Temporarily paused (e.g., before a surgery)
        Cancelled = 4,      // Cancelled before the patient ever started taking it
        EnteredInError = 5  // A mistaken entry that should be ignored
    }
}
