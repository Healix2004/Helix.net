# Helix API — Doctor Calendar & Appointments

This document provides the technical specifications for two key endpoints in the Doctor Portal: fetching monthly calendar availability and retrieving the appointment schedule for a specific day.

**Base URL:** `https://localhost:7193` (or your configured environment)

---

## 1. Authentication

Both endpoints are designed for the currently logged-in doctor. The system identifies the doctor based on the JWT sent in the `Authorization` header.

- **Header:** `Authorization`
- **Value:** `Bearer {doctorLoginJwt}`

A valid JWT for a user with an associated `Doctor` role is required. If the token is missing, invalid, or does not correspond to a doctor, the request will fail.

---

## 2. Get Monthly Calendar Flags

This endpoint retrieves an overview of a specific month for the logged-in doctor. It returns a list of days within that month, indicating which days have scheduled appointments. This is ideal for rendering a calendar view in the UI and highlighting active days.

- **Method:** `GET`
- **URL:** `/api/doctors/calendar`
- **Query Parameters:**
    - `year` (integer, required): The full year (e.g., `2026`).
    - `month` (integer, required): The month number, from 1 (January) to 12 (December).

### Example Request

```http
GET https://localhost:7193/api/doctors/calendar?year=2026&month=7
Authorization: Bearer {doctorLoginJwt}
```

### Success Response (`200 OK`)

The `data` payload contains a `CalendarMonthDto` object, which includes the year, month, and an array of `CalendarDayDto` objects.

```json
{
    "statusCode": 200,
    "succeeded": true,
    "message": null,
    "data": {
        "year": 2026,
        "month": 7,
        "days": [
            {
                "day": 5,
                "hasAppointments": true
            },
            {
                "day": 6,
                "hasAppointments": false
            },
            {
                "day": 12,
                "hasAppointments": true
            }
        ]
    },
    "errors": null
}
```

### Error Responses

- **`400 Bad Request`**: If the `year` or `month` parameters are missing or invalid.
- **`404 Not Found`**: If the doctor's profile associated with the JWT cannot be found.
- **`500 Internal Server Error`**: For unexpected server-side issues.

---

## 3. Get Daily Appointments

This endpoint retrieves a detailed list of all appointments scheduled for the logged-in doctor on a specific date. This is used to display the doctor's agenda for the day.

- **Method:** `GET`
- **URL:** `/api/doctors/appointments/day-date`
- **Query Parameters:**
    - `date` (string, required): The specific date in `YYYY-MM-DD` format.

### Example Request

```http
GET https://localhost:7193/api/doctors/appointments/day-date?date=2026-07-05
Authorization: Bearer {doctorLoginJwt}
```

### Success Response (`200 OK`)

The `data` payload contains an array of `AppointmentListDto` objects.

```json
{
    "statusCode": 200,
    "succeeded": true,
    "message": "Appointments retrieved for 2026-07-05",
    "data": [
        {
            "appointmentId": "a1b2c3d4-e5f6-...",
            "patientName": "John Doe",
            "appointmentTime": "2026-07-05T09:00:00",
            "status": "Confirmed"
        }
    ],
    "errors": null
}
```

### Error Responses

- **`400 Bad Request`**: If the `date` parameter is missing or in an invalid format.
- **`404 Not Found`**: If the doctor's profile associated with the JWT cannot be found.
- **`500 Internal Server Error`**: For unexpected server-side issues.
