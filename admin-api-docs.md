# Helix API — Admin Portal REST Reference

This document provides a pure REST API reference for frontend developers integrating with the Helix Admin Portal. It details authentication, endpoint specifications, and data shapes for administrative workflows.

**Base URL:** `https://localhost:7193` (or your configured environment)

---

## 1. Global Authentication & Headers

All API requests to the admin portal are protected and require a JSON Web Token (JWT) provided upon user login. The claims within this token must identify the user's role as `Admin`.

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

### 2.1 Get System Overview Dashboard

Retrieves a high-level summary of key metrics across the entire Helix system.

- **Method:** `GET`
- **URL:** `/api/admin/dashboard`

#### Success Response

- **Code:** `200 OK`
- **Payload:**
    ```json
    {
        "statusCode": 200,
        "succeeded": true,
        "message": "System overview metrics retrieved successfully.",
        "data": {
            "patients": {
                "totalCount": 1250,
                "trendText": "+12% from last month"
            },
            "doctors": {
                "totalCount": 350,
                "trendText": "+8% from last month"
            },
            "facilities": {
                "totalCount": 45,
                "trendText": "+5% from last month"
            },
            "labOrders": {
                "totalCount": 8430,
                "trendText": "+18% from last month"
            },
            "radiologyOrders": {
                "totalCount": 4120,
                "trendText": "+6% from last month"
            },
            "drugs": {
                "totalCount": 5600,
                "trendText": "+3% from last month"
            }
        },
        "errors": null,
        "meta": null
    }
    ```

#### Error Responses

- `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
- `403 Forbidden`: The user's role is not `Admin`.

### 2.2 Get Patients Management

Retrieves metrics and a searchable list of all patients in the system.

- **Method:** `GET`
- **URL:** `/api/admin/patients`

#### Query Parameters

| Parameter    | Type   | Required | Description                                                 |
| ------------ | ------ | -------- | ----------------------------------------------------------- |
| `searchTerm` | string | No       | A string to search by patient name, email, or phone number. |

#### Success Response

- **Code:** `200 OK`
- **Payload:**
    ```json
    {
        "statusCode": 200,
        "succeeded": true,
        "message": "Patients management data retrieved successfully.",
        "data": {
            "totalPatients": 1250,
            "activePatients": 1200,
            "inactivePatients": 50,
            "newThisMonth": 75,
            "patientsList": [
                {
                    "id": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
                    "patientIdDisplay": "P-1001",
                    "initials": "SJ",
                    "name": "Sarah Johnson",
                    "email": "sarah.j@example.com",
                    "phone": "01234567890",
                    "gender": "Female",
                    "bloodType": "O_Positive",
                    "status": "Active",
                    "registeredDate": "Jun 5, 2026"
                }
            ]
        },
        "errors": null,
        "meta": null
    }
    ```

#### Error Responses

- `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
- `403 Forbidden`: The user's role is not `Admin`.
- `500 Internal Server Error`: An unexpected error occurred while fetching data.

### 2.3 Get Doctors Management

Retrieves metrics and a searchable list of all doctors in the system.

- **Method:** `GET`
- **URL:** `/api/admin/doctors`

#### Query Parameters

| Parameter    | Type   | Required | Description                                                |
| ------------ | ------ | -------- | ---------------------------------------------------------- |
| `searchTerm` | string | No       | A string to search by doctor name, email, or phone number. |

#### Success Response

- **Code:** `200 OK`
- **Payload:**
    ```json
    {
        "statusCode": 200,
        "succeeded": true,
        "message": "Doctors management data retrieved successfully.",
        "data": {
            "totalDoctors": 350,
            "verifiedDoctors": 320,
            "pendingVerification": 25,
            "rejectedDoctors": 5,
            "doctorsList": [
                {
                    "id": "d1e2f3a4-b5c6-7890-1234-567890abcdef",
                    "doctorIdDisplay": "D-2001",
                    "initials": "EC",
                    "name": "Dr. Emily Carter",
                    "email": "emily.c@example.com",
                    "phone": "01123456789",
                    "specialty": "Cardiology",
                    "experience": "15 years",
                    "consultation": "Online / Clinic",
                    "status": "Verified",
                    "registeredDate": "May 15, 2026"
                }
            ]
        },
        "errors": null,
        "meta": null
    }
    ```

#### Error Responses

- `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
- `403 Forbidden`: The user's role is not `Admin`.
- `500 Internal Server Error`: An unexpected error occurred while fetching data.

### 2.4 Get Facilities Management

Retrieves metrics and a searchable list of all healthcare facilities (hospitals, clinics, etc.).

- **Method:** `GET`
- **URL:** `/api/admin/facilities`

#### Query Parameters

| Parameter    | Type   | Required | Description                              |
| ------------ | ------ | -------- | ---------------------------------------- |
| `searchTerm` | string | No       | A string to search by the facility name. |

#### Success Response

- **Code:** `200 OK`
- **Payload:**
    ```json
    {
        "statusCode": 200,
        "succeeded": true,
        "message": "Facilities management data retrieved successfully.",
        "data": {
            "totalFacilities": 45,
            "activeFacilities": 42,
            "inactiveFacilities": 3,
            "hospitalsCount": 15,
            "facilitiesList": [
                {
                    "id": "f1a2b3c4-d5e6-7890-1234-567890abcdef",
                    "facilityIdDisplay": "F-3001",
                    "name": "Helix Memorial Hospital",
                    "locationSubtitle": "123 Health St, Cairo",
                    "type": "Hospital",
                    "email": "contact@helixmemorial.com",
                    "phone": "0223456789",
                    "city": "Cairo",
                    "status": "Active",
                    "createdDate": "Jan 10, 2025"
                }
            ]
        },
        "errors": null,
        "meta": null
    }
    ```

#### Error Responses

- `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
- `403 Forbidden`: The user's role is not `Admin`.
- `500 Internal Server Error`: An unexpected error occurred while fetching data.

### 2.5 Get Drugs Catalog Management

Retrieves metrics and a searchable list of all drugs in the medication catalog.

- **Method:** `GET`
- **URL:** `/api/admin/drugs`

#### Query Parameters

| Parameter    | Type   | Required | Description                                                        |
| ------------ | ------ | -------- | ------------------------------------------------------------------ |
| `searchTerm` | string | No       | A string to search by drug name, generic name, RxCUI, or category. |

#### Success Response

- **Code:** `200 OK`
- **Payload:**
    ```json
    {
        "statusCode": 200,
        "succeeded": true,
        "message": "Drugs catalog management data retrieved successfully.",
        "data": {
            "totalDrugs": 5600,
            "availableDrugs": 5500,
            "unavailableDrugs": 100,
            "categoriesCount": 150,
            "drugsList": [
                {
                    "id": "860975",
                    "drugIdDisplay": "DR-4001",
                    "drugName": "Metformin 500 MG Oral Tablet",
                    "createdSubtitle": "Created: Apr 20, 2026",
                    "genericName": "Metformin",
                    "category": "Oral Hypoglycemic",
                    "form": "Tablet",
                    "strength": "500 mg",
                    "manufacturer": "Pharma Inc.",
                    "status": "Available"
                }
            ]
        },
        "errors": null,
        "meta": null
    }
    ```

#### Error Responses

- `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
- `403 Forbidden`: The user's role is not `Admin`.
- `500 Internal Server Error`: An unexpected error occurred while fetching data.
