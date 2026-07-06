# Helix API — Pharmacy Portal REST Reference

This document provides a pure REST API reference for frontend developers integrating with the Helix Pharmacy Portal. It details authentication, endpoint specifications, and data shapes for pharmacy-specific workflows.

**Base URL:** `https://localhost:7193` (or your configured environment)

---

## 1. Global Authentication & Headers

All authenticated API requests must include a JSON Web Token (JWT) provided upon user login. The claims within this token must identify the user's role as `Pharmaciest`.

### Standard Authentication

- **Header:** `Authorization`
- **Value:** `Bearer {loginJwt}`

### Standard Response Wrapper

All endpoints return data within a standard wrapper. Always check the `succeeded` flag before using the `data` payload.

```json
{
    "statusCode": 200,
    "succeeded": true,
    "message": "A descriptive success or error message.",
    "data": {},
    "errors": null,
    "meta": null
}
```

---

## 2. Endpoint Reference

### 2.1 Register a New Pharmacy Facility

Registers the details for a new pharmacy, including document uploads for verification. This endpoint is typically called after the pharmacist user has created their initial account.

- **Method:** `POST`
- **URL:** `/api/pharmacies/register`
- **Headers:**
    - `Authorization: Bearer {loginJwt}`
    - `Content-Type: multipart/form-data`

#### Request Payload (`multipart/form-data`)

| Form Field Name      | Type   | Required | Description                                        |
| -------------------- | ------ | -------- | -------------------------------------------------- |
| `NationalId`         | string | Yes      | The pharmacist's national identification number.   |
| `PharmacyName`       | string | Yes      | The legal name of the pharmacy.                    |
| `Address`            | string | Yes      | The full street address of the pharmacy.           |
| `LicenseNumber`      | string | Yes      | The official pharmacy license number.              |
| `ProfileImageFile`   | File   | No       | An optional profile image for the pharmacist.      |
| `PrimaryLicenseFile` | File   | Yes      | The primary pharmacy license document (e.g., PDF). |
| `NationalIdFile`     | File   | Yes      | A document scan of the pharmacist's National ID.   |

#### Success Response

- **Code:** `201 Created`
- **Payload:**
    ```json
    {
        "statusCode": 201,
        "succeeded": true,
        "message": "Pharmacy registration submitted successfully and is now under review.",
        "data": {
            "pharmacyId": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
            "verificationStatus": "UnderReview"
        },
        "errors": null,
        "meta": null
    }
    ```

#### Error Responses

- `400 Bad Request`: Required form fields are missing or invalid.
- `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
- `403 Forbidden`: The user's role is not `Pharmaciest`.
- `500 Internal Server Error`: An unexpected server error occurred, possibly during file upload processing.

### 2.2 Get Pharmacy Dashboard

Retrieves a high-level summary of the pharmacy's daily operations, including key metrics and a list of prescriptions to be processed.

- **Method:** `GET`
- **URL:** `/api/pharmacies/dashboard`
- **Headers:**
    - `Authorization: Bearer {loginJwt}`

#### Success Response

- **Code:** `200 OK`
- **Payload:**
    ```json
    {
        "statusCode": 200,
        "succeeded": true,
        "message": "Dashboard data retrieved successfully.",
        "data": {
            "prescriptionsDispensedToday": 42,
            "pendingOrders": 15,
            "criticalAlerts": 2,
            "revenueToday": 5430.5,
            "stockAlerts": [
                {
                    "medicationName": "Metformin 500mg",
                    "alertLevel": "Critical",
                    "message": "Stock is critically low. Immediate restock required."
                }
            ],
            "todaysPrescriptions": [
                {
                    "prescriptionId": "px-987654321",
                    "patientName": "Sarah Johnson",
                    "department": "Cardiology",
                    "receivedAt": "2026-07-06T09:15:00Z",
                    "status": "Pending"
                },
                {
                    "prescriptionId": "px-123456789",
                    "patientName": "Michael Chen",
                    "department": "Endocrinology",
                    "receivedAt": "2026-07-06T09:30:00Z",
                    "status": "Active"
                }
            ]
        },
        "errors": null,
        "meta": null
    }
    ```

#### Error Responses

- `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
- `403 Forbidden`: The user's role is not `Pharmaciest`.
- `500 Internal Server Error`: An unexpected server error occurred.

### 2.3 Get Prescription Details

Retrieves the full details of a specific prescription, including patient information, prescribed medications, and doctor's instructions. This is typically called after scanning a patient's QR code or selecting a prescription from the dashboard.

- **Method:** `GET`
- **URL:** `/api/pharmacies/prescriptions/{prescriptionId}`
- **Headers:**
    - `Authorization: Bearer {loginJwt}`

#### Success Response

- **Code:** `200 OK`
- **Payload:**
    ```json
    {
        "statusCode": 200,
        "succeeded": true,
        "message": "Prescription details retrieved successfully.",
        "data": {
            "prescriptionId": "px-987654321",
            "status": "Pending",
            "patientDetails": {
                "patientId": "p1a2b3c4-d5e6-f789-0123-456789abcdef",
                "fullName": "Sarah Johnson",
                "age": 45,
                "gender": "Female",
                "knownAllergies": ["Penicillin", "Shellfish"]
            },
            "prescribingDoctor": {
                "fullName": "Dr. Emily Carter",
                "specialty": "Cardiology",
                "licenseNumber": "MD-12345"
            },
            "doctorNotes": "Take with food to avoid stomach upset. Monitor blood pressure weekly.",
            "medications": [
                {
                    "medicationId": "med-guid-1",
                    "rxcui": "315442",
                    "name": "Lisinopril",
                    "dosage": "10mg",
                    "frequency": "Once daily",
                    "duration": "30 days"
                }
            ]
        },
        "errors": null,
        "meta": null
    }
    ```

#### Error Responses

- `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
- `403 Forbidden`: The user's role is not `Pharmaciest`.
- `404 Not Found`: The prescription with the specified `prescriptionId` does not exist or is not accessible.
- `500 Internal Server Error`: An unexpected server error occurred.

### 2.4 Dispense a Prescription

Marks a prescription as dispensed. This is a final action that updates the prescription's status in the system.

- **Method:** `POST`
- **URL:** `/api/pharmacies/prescriptions/{prescriptionId}/dispense`
- **Headers:**
    - `Authorization: Bearer {loginJwt}`

#### Success Response

- **Code:** `200 OK`
- **Payload:**
    ```json
    {
        "statusCode": 200,
        "succeeded": true,
        "message": "Prescription px-987654321 has been marked as dispensed.",
        "data": {
            "prescriptionId": "px-987654321",
            "newStatus": "Dispensed",
            "dispensedAt": "2026-07-06T12:30:00Z"
        },
        "errors": null,
        "meta": null
    }
    ```

#### Error Responses

- `400 Bad Request`: The prescription has already been dispensed or is in a non-dispensable state (e.g., flagged, cancelled).
- `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
- `403 Forbidden`: The user's role is not `Pharmaciest`.
- `404 Not Found`: The prescription with the specified `prescriptionId` does not exist.
- `500 Internal Server Error`: An unexpected server error occurred.

### 2.5 Flag a Prescription for Review

Flags a prescription that requires review from the prescribing doctor due to a potential issue (e.g., suspected error, drug interaction concern, clarification needed).

- **Method:** `POST`
- **URL:** `/api/pharmacies/prescriptions/{prescriptionId}/flag`
- **Headers:**
    - `Authorization: Bearer {loginJwt}`
    - `Content-Type: application/json`

#### Request Payload

```json
{
    "reason": "Suspected Drug Interaction",
    "notes": "Patient is currently taking Warfarin. Prescribing Rivaroxaban poses a high risk of bleeding. Please review and provide an alternative if necessary."
}
```

#### Success Response

- **Code:** `200 OK`
- **Payload:**
    ```json
    {
        "statusCode": 200,
        "succeeded": true,
        "message": "Prescription has been flagged for doctor review.",
        "data": {
            "prescriptionId": "px-987654321",
            "newStatus": "Flagged",
            "flaggedAt": "2026-07-06T11:45:00Z"
        },
        "errors": null,
        "meta": null
    }
    ```

#### Error Responses

- `400 Bad Request`: The request body is missing the `reason` or `notes` field.
- `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
- `403 Forbidden`: The user's role is not `Pharmaciest`.
- `404 Not Found`: The prescription with the specified `prescriptionId` does not exist.
- `500 Internal Server Error`: An unexpected server error occurred.
