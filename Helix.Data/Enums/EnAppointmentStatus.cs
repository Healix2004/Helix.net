namespace Helix.Data.Enums
{
    public enum EnAppointmentStatus
    {
        Pending = 0,      // Created, but needs confirmation
        Booked = 1,       // Confirmed by both parties (Your UI: "Upcoming" / "Confirmed")
        Arrived = 2,      // Patient is in the waiting room (Your UI: "Waiting")
        Fulfilled = 3,    // Appointment is done (Your UI: "Completed")
        Cancelled = 4,    // Cancelled by patient or doctor
        NoShow = 5        // Patient didn't arrive
    }
}
