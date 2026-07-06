# Helix API - REST Reference Guide

This document provides a pure REST API reference for frontend developers integrating with the Helix backend. It details authentication requirements, endpoint specifications, and data shapes for common clinical workflows.

**Base URL:** `https://localhost:7193`

---

## 1. Global Authentication & Headers

All authenticated API requests must include a JSON Web Token (JWT) provided upon user login. Certain endpoints that access protected patient data require an additional consent token.

### Standard Authentication

For most endpoints, the login JWT must be sent in the `Authorization` header.

- **Header:** `Authorization`
- **Value:** `Bearer {loginJwt}`

### Consent-Based Access

When a doctor views specific patient data (e.g., clinical timelines, records), a temporary consent token is required. This token is obtained when the doctor redeems a patient-generated QR code.

- **Header:** `X-Consent-Token`
- **Value:** `{consentJwt}`

This header must be sent **in addition to** the standard `Authorization` header.

---

## 2. Endpoint Reference

### 2.1 Get Doctor's Appointment List

Retrieves a list of a doctor's appointments, filterable by time period.

- **Method:** `GET`
- **URL:** `/api/appointments/dashboard/list?filter={filter}`
- **URL Parameters:**
    - `filter` (string, required): The time period to filter appointments. Valid values: `today`, `tomorrow`, `week`, `past`, `urgent`.
- **Headers:**
    - `Authorization: Bearer {loginJwt}`

#### Success Response

- **Code:** `200 OK`
- **Payload:**
    ```json
    [
        {
            "id": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
            "patientId": "p1a2b3c4-d5e6-f789-0123-456789abcdef",
            "patientName": "Sarah Johnson",
            "patientInitials": "SJ",
            "appointmentType": "Cardiology Follow-up",
            "displayTime": "09:30 AM",
            "status": "Booked",
            "priority": "Routine"
        },
        {
            "id": "b2c3d4e5-f6a7-8901-2345-67890abcdef1",
            "patientId": "p2b3c4d5-e6f7-a890-1234-56789abcdef0",
            "patientName": "Michael Chen",
            "patientInitials": "MC",
            "appointmentType": "Annual Physical Exam",
            "displayTime": "10:00 AM",
            "status": "Arrived",
            "priority": "Routine"
        }
    ]
    ```

#### Error Responses

- `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
- `403 Forbidden`: The user's role is not `Doctor`.
- `500 Internal Server Error`: An unexpected server error occurred.

### 2.2 Get High-Priority Patients

Retrieves a list of the doctor's high-priority (Urgent or Stat) appointments for the current day.

- **Method:** `GET`
- **URL:** `/api/appointments/dashboard/high-priority`
- **Headers:**
    - `Authorization: Bearer {loginJwt}`

#### Success Response

- **Code:** `200 OK`
- **Payload:**
    ```json
    [
        {
            "id": "c3d4e5f6-a7b8-9012-3456-7890abcdef12",
            "patientId": "p3c4d5e6-f7a8-b901-2345-67890abcdef1",
            "patientName": "Emily Rodriguez",
            "patientInitials": "ER",
            "appointmentType": "Urgent - Post-operative check",
            "displayTime": "11:00 AM",
            "status": "Booked",
            "priority": "Urgent"
        }
    ]
    ```

#### Error Responses

- `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
- `403 Forbidden`: The user's role is not `Doctor`.
- `500 Internal Server Error`: An unexpected server error occurred.

### 2.3 Get Daily Schedule Summary

Retrieves a summary of the doctor's appointment statistics for the current day.

- **Method:** `GET`
- **URL:** `/api/appointments/dashboard/summary`
- **Headers:**
    - `Authorization: Bearer {loginJwt}`

#### Success Response

- **Code:** `200 OK`
- **Payload:**
    ```json
    {
        "totalAppointments": 15,
        "confirmed": 8,
        "waiting": 2,
        "urgent": 1
    }
    ```

#### Error Responses

- `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
- `403 Forbidden`: The user's role is not `Doctor`.
- `500 Internal Server Error`: An unexpected server error occurred.

### 2.4 Get Patient Dashboard for an Appointment

Retrieves a comprehensive clinical summary dashboard for the patient associated with a specific appointment.

- **Method:** `GET`
- **URL:** `/api/appointments/{id}/patient-dashboard`
- **Headers:**
    - `Authorization: Bearer {loginJwt}`

#### Success Response

- **Code:** `200 OK`
- **Payload:**
    ```json
    {
        "patientId": "p1a2b3c4-d5e6-f789-0123-456789abcdef",
        "fullName": "Sarah Johnson",
        "initials": "SJ",
        "displayId": "A1B2C3D",
        "age": 45,
        "gender": "Female",
        "phone": "+15551234567",
        "email": "sarah.j@example.com",
        "insuranceProvider": "United Health",
        "patientStatus": "Active Patient",
        "knownAllergies": ["Penicillin", "Shellfish"],
        "chronicConditions": ["Hypertension", "Type 2 Diabetes"],
        "currentMedications": [
            {
                "rxcui": "860975",
                "name": "Metformin 500 MG Oral Tablet",
                "dosage": "500mg",
                "startDate": "2023-01-15",
                "endDate": null
            }
        ],
        "lastVisit": {
            "appointmentTitle": "Routine Checkup",
            "appointmentDate": "01-15-2024",
            "prescriptionSummary": "View active medications list",
            "clinicalNoteSummary": "Routine visit completed.",
            "clinicalNoteDate": "6 months ago",
            "labResultSummary": "No recent labs",
            "labResultDate": ""
        },
        "isMedicalHistoryLocked": true
    }
    ```

#### Error Responses

- `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
- `403 Forbidden`: The logged-in user is not the doctor for this appointment.
- `404 Not Found`: The appointment with the specified `id` does not exist.
- `500 Internal Server Error`: An unexpected server error occurred.

### 2.5 Get Patient Clinical Timeline (Doctor View)

Retrieves a patient's chronological medical history. This is a protected endpoint requiring explicit patient consent.

- **Method:** `GET`
- **URL:** `/api/patients/{patientId}/timeline`
- **Headers:**
    - `Authorization: Bearer {loginJwt}`
    - `X-Consent-Token: {consentJwt}`

#### Success Response

- **Code:** `200 OK`
- **Payload:**
    ```json
    [
        {
            "eventType": "LabResult",
            "eventDate": "2024-06-20T10:00:00Z",
            "title": "Lab Results Received",
            "summary": "Complete Blood Count (CBC) - All values within normal range.",
            "detailsUrl": "/api/lab-results/res123"
        },
        {
            "eventType": "Prescription",
            "eventDate": "2024-05-15T14:30:00Z",
            "title": "New Prescription Issued",
            "summary": "Lisinopril 10mg, 1 tablet daily.",
            "detailsUrl": "/api/prescriptions/px456"
        },
        {
            "eventType": "Appointment",
            "eventDate": "2024-05-15T14:00:00Z",
            "title": "Consultation with Dr. Adams",
            "summary": "Follow-up for hypertension management.",
            "detailsUrl": "/api/appointments/apt789"
        }
    ]
    ```

#### Error Responses

- `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
- `403 Forbidden`: The `X-Consent-Token` is missing, invalid, expired, or does not grant access to this patient's records.
- `404 Not Found`: The patient with the specified `patientId` does not exist.
- `500 Internal Server Error`: An unexpected server error occurred.

### 2.6 Get My Clinical Timeline (Patient View)

Retrieves the logged-in patient's own chronological medical history.

- **Method:** `GET`
- **URL:** `/api/patients/my-timeline`
- **Headers:**
    - `Authorization: Bearer {loginJwt}`

#### Success Response

- **Code:** `200 OK`
- **Payload:** (Same structure as 2.5 Get Patient Clinical Timeline)

#### Error Responses

- `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
- `403 Forbidden`: The user's role is not `Patient`.
- `500 Internal Server Error`: An unexpected server error occurred.

### 2.7 Create a New Prescription

Creates a new prescription for a patient, containing one or more medications.

- **Method:** `POST`
- **URL:** `/api/prescriptions`
- **Headers:**
    - `Authorization: Bearer {loginJwt}`
    - `Content-Type: application/json`

#### Request Payload

```json
{
    "patientId": "p1a2b3c4-d5e6-f789-0123-456789abcdef",
    "appointmentId": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
    "doctorNotes": "Take with food to avoid stomach upset. Monitor blood pressure weekly.",
    "medications": [
        {
            "terminologyRxcui": "315442",
            "dosage": "10mg",
            "frequency": "Once daily",
            "duration": "30 days"
        },
        {
            "terminologyRxcui": "860975",
            "dosage": "500mg",
            "frequency": "Twice daily with meals",
            "duration": "90 days"
        }
    ]
}
```

#### Success Response

- **Code:** `201 Created`
- **Payload:**
    ```json
    {
        "prescriptionId": "px-987654321",
        "message": "Prescription created successfully."
    }
    ```

#### Error Responses

- `400 Bad Request`: The request body is missing required fields or contains invalid data (e.g., invalid `patientId`, malformed medication array).
- `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
- `403 Forbidden`: The user's role is not `Doctor`.
- `404 Not Found`: The specified `patientId` or `appointmentId` does not exist.
- `500 Internal Server Error`: An unexpected server error occurred, possibly during a drug interaction check.
