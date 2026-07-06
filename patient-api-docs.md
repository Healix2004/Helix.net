# Helix Patient Portal - REST API Reference

This document provides a pure REST API reference for frontend developers integrating with the Helix Patient Portal. It details authentication requirements, endpoint specifications, and data shapes for the patient-facing dashboard modules.

**Base URL:** `<https://localhost:7193>` (or your configured environment)

---

## 1. Global Authentication & Headers

All API requests to the patient portal must include a JSON Web Token (JWT) provided upon user login. The claims within this token must identify the user's role as `Patient`.

### Standard Authentication

-   **Header:** `Authorization`
-   **Value:** `Bearer {loginJwt}`

---

## 2. Endpoint Reference

### 2.1 Get Main Patient Dashboard

Retrieves a high-level summary of the patient's health status, including metric cards, recent activities, and health tips.

-   **Method:** `GET`
-   **URL:** `/api/patients/dashboard`
-   **Headers:**
    -   `Authorization: Bearer {loginJwt}`

#### Success Response

-   **Code:** `200 OK`
-   **Payload:**
    ```json
    {
      "totalRecords": 24,
      "activePrescriptions": 3,
      "upcomingAppointments": 2,
      "recentActivity": [
        {
          "activityType": "Lab",
          "title": "Lab Results Available",
          "description": "Blood test results for Complete Blood Count",
          "date": "2026-07-05T10:30:00Z",
          "timeAgo": "2 hours ago"
        },
        {
          "activityType": "Radiology",
          "title": "Radiology Results Available",
          "description": "Radiology results for Chest X-Ray",
          "date": "2026-07-04T15:00:00Z",
          "timeAgo": "1 day ago"
        },
        {
          "activityType": "Appointment",
          "title": "Appointment Confirmed",
          "description": "Dr. Mustafa - Jul 10, 2026",
          "date": "2026-07-10T14:00:00Z",
          "timeAgo": "In 4 days"
        },
        {
          "activityType": "Prescription",
          "title": "Prescription Updated",
          "description": "Medication: Atorvastatin",
          "date": "2026-07-01T09:00:00Z",
          "timeAgo": "4 days ago"
        }
      ],
      "healthScore": 85,
      "healthScoreMessage": "Your health metrics are looking good!",
      "healthTips": [
        {
          "iconType": "Heart",
          "title": "Stay Hydrated",
          "description": "Drink at least 8 glasses of water daily"
        },
        {
          "iconType": "Activity",
          "title": "Daily Exercise",
          "description": "30 minutes of moderate activity"
        }
      ]
    }
    ```

#### Error Responses

-   `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
-   `403 Forbidden`: The user's role is not `Patient`.
-   `500 Internal Server Error`: An unexpected server error occurred.

### 2.2 Get Patient Lab Dashboard

Retrieves a summary of the patient's lab test history, including counts, alerts, and a list of all tests.

-   **Method:** `GET`
-   **URL:** `/api/patients/lab-dashboard`
-   **Headers:**
    -   `Authorization: Bearer {loginJwt}`

#### Success Response

-   **Code:** `200 OK`
-   **Payload:**
    ```json
    {
      "totalTests": 5,
      "pendingResults": 1,
      "abnormalResults": 1,
      "activeAlert": {
        "title": "Abnormal Result Alert",
        "description": "Your Lipid Panel shows results outside the normal range. Please contact your provider.",
        "timeAgo": "3 days ago"
      },
      "recentActivity": [
        {
          "title": "CBC ordered",
          "timeAgo": "1 day ago",
          "statusColor": "yellow"
        },
        {
          "title": "Lipid Panel results uploaded",
          "timeAgo": "3 days ago",
          "statusColor": "red"
        },
        {
          "title": "Metabolic Panel completed",
          "timeAgo": "1 month ago",
          "statusColor": "green"
        }
      ],
      "labTests": [
        {
          "id": "a1b2c3d4-e5f6-7890-1234-567890abcdef",
          "testName": "Complete Blood Count (CBC)",
          "date": "2026-07-04T11:00:00Z",
          "requestedBy": "Dr. Emily Carter",
          "status": "Pending"
        },
        {
          "id": "b2c3d4e5-f6a7-8901-2345-67890abcdef1",
          "testName": "Lipid Panel",
          "date": "2026-07-02T09:15:00Z",
          "requestedBy": "Dr. Emily Carter",
          "status": "Abnormal"
        },
        {
          "id": "c3d4e5f6-a7b8-9012-3456-7890abcdef12",
          "testName": "Comprehensive Metabolic Panel",
          "date": "2026-06-05T08:00:00Z",
          "requestedBy": "Dr. Ben Adams",
          "status": "Completed"
        }
      ]
    }
    ```

#### Error Responses

-   `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
-   `403 Forbidden`: The user's role is not `Patient`.
-   `500 Internal Server Error`: An unexpected server error occurred.

### 2.3 Get Lab Test Details

Retrieves the detailed results for a single lab order, including patient info, doctor comments, and a breakdown of each parameter.

-   **Method:** `GET`
-   **URL:** `/api/patients/lab-dashboard/{orderId}`
-   **Headers:**
    -   `Authorization: Bearer {loginJwt}`

#### Success Response

-   **Code:** `200 OK`
-   **Payload:**
    ```json
    {
      "orderId": "b2c3d4e5-f6a7-8901-2345-67890abcdef1",
      "panelName": "Lipid Panel",
      "patientInfo": {
        "patientName": "Maryam",
        "email": "maryam@example.com",
        "contact": "01234567890",
        "dateOfBirth": "January 1, 1980 (46 years)",
        "testDate": "Jul 2, 2026",
        "patientIdDisplay": "PT-A1B2C3D4"
      },
      "doctorComment": {
        "doctorName": "Dr. Emily Carter",
        "specialty": "Cardiology",
        "commentDate": "Jul 2, 2026 at 3:45 PM",
        "overallComment": "No additional comments provided.",
        "recommendations": []
      },
      "results": [
        {
          "parameterName": "Cholesterol, Total",
          "resultValue": "210",
          "normalRange": "<200",
          "unit": "mg/dL",
          "status": "High"
        },
        {
          "parameterName": "Triglycerides",
          "resultValue": "160",
          "normalRange": "<150",
          "unit": "mg/dL",
          "status": "High"
        },
        {
          "parameterName": "HDL Cholesterol",
          "resultValue": "38",
          "normalRange": ">40",
          "unit": "mg/dL",
          "status": "Low"
        },
        {
          "parameterName": "LDL Cholesterol (Calculated)",
          "resultValue": "140",
          "normalRange": "<100",
          "unit": "mg/dL",
          "status": "High"
        }
      ]
    }
    ```

#### Error Responses

-   `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
-   `403 Forbidden`: The user's role is not `Patient`.
-   `404 Not Found`: The `orderId` does not exist or does not belong to the logged-in patient.
-   `500 Internal Server Error`: An unexpected server error occurred.

### 2.4 Get Patient Radiology Dashboard

Retrieves a summary of the patient's radiology history, including scan counts, previews, and a list of all studies.

-   **Method:** `GET`
-   **URL:** `/api/patients/radiology-dashboard`
-   **Headers:**
    -   `Authorization: Bearer {loginJwt}`

#### Success Response

-   **Code:** `200 OK`
-   **Payload:**
    ```json
    {
      "totalScans": 4,
      "pendingReview": 1,
      "abnormalFindings": 1,
      "recentUploads": 2,
      "latestScanPreview": {
        "scanId": "d4e5f6a7-b8c9-0123-4567-890abcdef123",
        "scanName": "MRI Brain without contrast",
        "patientName": "Maryam"
      },
      "latestNote": {
        "doctorName": "Dr. Sarah Jenkins",
        "timeAgo": "15 days ago",
        "notePreview": "Minor degenerative changes noted in the lumbar spine."
      },
      "scans": [
        {
          "id": "e5f6a7b8-c9d0-1234-5678-90abcdef1234",
          "scanType": "CT Chest with contrast",
          "category": "CT",
          "date": "2026-07-05T14:00:00Z",
          "requestedBy": "Dr. Ben Adams",
          "status": "Pending"
        },
        {
          "id": "d4e5f6a7-b8c9-0123-4567-890abcdef123",
          "scanType": "MRI Brain without contrast",
          "category": "MRI",
          "date": "2026-06-25T11:30:00Z",
          "requestedBy": "Dr. Emily Carter",
          "status": "Completed"
        },
        {
          "id": "f6a7b8c9-d0e1-2345-6789-0abcdef12345",
          "scanType": "X-Ray Lumbar Spine",
          "category": "X-Ray",
          "date": "2026-06-20T10:00:00Z",
          "requestedBy": "Dr. Ben Adams",
          "status": "Abnormal"
        }
      ]
    }
    ```

#### Error Responses

-   `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
-   `403 Forbidden`: The user's role is not `Patient`.
-   `500 Internal Server Error`: An unexpected server error occurred.

### 2.5 Get Radiology Study Details

Retrieves the detailed report for a single radiology order, including metadata, findings, and physician notes.

-   **Method:** `GET`
-   **URL:** `/api/patients/radiology-dashboard/{orderId}`
-   **Headers:**
    -   `Authorization: Bearer {loginJwt}`

#### Success Response

-   **Code:** `200 OK`
-   **Payload:**
    ```json
    {
      "orderId": "f6a7b8c9-d0e1-2345-6789-0abcdef12345",
      "imageUrls": [
        "/path/to/image1.dcm.jpg",
        "/path/to/image2.dcm.jpg"
      ],
      "metadata": {
        "patientName": "Maryam",
        "gender": "Female",
        "email": "maryam@example.com",
        "contact": "01234567890",
        "dateOfBirth": "January 1, 1980 (46 years)",
        "patientIdDisplay": "PT-A1B2C3D4",
        "studyDate": "Jun 20, 2026",
        "studyTime": "10:00 AM",
        "referringPhysician": "Dr. Ben Adams",
        "modality": "X-Ray Lumbar Spine",
        "bodyPart": "XR",
        "institution": "Helix Memorial Hospital"
      },
      "findings": {
        "status": "Abnormal",
        "radiologistName": "Dr. Sarah Jenkins",
        "clinicalIndication": "Follow-up examination for lower back pain.",
        "findingsText": "Mild narrowing of the L4-L5 disc space. No evidence of acute fracture or dislocation.",
        "impression": "Minor degenerative changes noted in the lumbar spine.",
        "reportDate": "Reported on Jun 21, 2026 at 11:00 AM"
      },
      "physicianNotes": {
        "physicianName": "Dr. Ben Adams, MD",
        "notes": "Discussed findings with patient. Will proceed with physical therapy."
      }
    }
    ```

#### Error Responses

-   `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
-   `403 Forbidden`: The user's role is not `Patient`.
-   `404 Not Found`: The `orderId` does not exist or does not belong to the logged-in patient.
-   `500 Internal Server Error`: An unexpected server error occurred.

### 2.6 Get Patient Prescription Dashboard

Retrieves a summary of the patient's prescription history, including counts, renewal reminders, and a list of all medications.

-   **Method:** `GET`
-   **URL:** `/api/patients/prescription-dashboard`
-   **Headers:**
    -   `Authorization: Bearer {loginJwt}`

#### Success Response

-   **Code:** `200 OK`
-   **Payload:**
    ```json
    {
      "totalMedications": 5,
      "activePrescriptions": 3,
      "expiringSoon": 1,
      "renewedThisMonth": 2,
      "renewalReminders": [
        {
          "medicationId": "med-guid-1",
          "medicationName": "Lisinopril",
          "expiresInText": "Expires in 12 days"
        }
      ],
      "medicationTips": [
        {
          "title": "Take with Food",
          "description": "Certain medications should be taken with meals to reduce side effects. Check your bottle labels."
        },
        {
          "title": "Morning Dose",
          "description": "Try taking your once-daily pills at the same time each morning to build a routine."
        }
      ],
      "medications": [
        {
          "id": "med-guid-1",
          "prescriptionId": "presc-guid-a",
          "medicationName": "Lisinopril",
          "prescribedBy": "Dr. Emily Carter",
          "date": "2026-06-18T10:00:00Z",
          "duration": "30 Days",
          "status": "Expiring"
        },
        {
          "id": "med-guid-2",
          "prescriptionId": "presc-guid-b",
          "medicationName": "Atorvastatin",
          "prescribedBy": "Dr. Emily Carter",
          "date": "2026-07-01T11:00:00Z",
          "duration": "90 Days",
          "status": "Active"
        },
        {
          "id": "med-guid-3",
          "prescriptionId": "presc-guid-c",
          "medicationName": "Metformin",
          "prescribedBy": "Dr. Ben Adams",
          "date": "2026-04-15T09:00:00Z",
          "duration": "90 Days",
          "status": "Completed"
        }
      ]
    }
    ```

#### Error Responses

-   `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
-   `403 Forbidden`: The user's role is not `Patient`.
-   `500 Internal Server Error`: An unexpected server error occurred.

### 2.7 Get Prescription Details

Retrieves the detailed information for a single prescription, including patient summary, doctor's notes, and a list of prescribed medications.

-   **Method:** `GET`
-   **URL:** `/api/patients/prescription-dashboard/{prescriptionId}`
-   **Headers:**
    -   `Authorization: Bearer {loginJwt}`

#### Success Response

-   **Code:** `200 OK`
-   **Payload:**
    ```json
    {
      "prescriptionId": "presc-guid-b",
      "summary": {
        "patientName": "Maryam",
        "gender": "Female",
        "email": "maryam@example.com",
        "contact": "01234567890",
        "dateOfBirth": "January 1, 1980 (46 years)",
        "patientIdDisplay": "PT-A1B2C3D4",
        "issueDate": "Jul 1, 2026",
        "prescribingDoctor": "Dr. Emily Carter",
        "validUntil": "Sep 29, 2026"
      },
      "doctorInstructions": "Take all medications exactly as prescribed. Contact your doctor if you experience unusual side effects.",
      "medications": [
        {
          "medicationName": "ATORVASTATIN",
          "dosage": "20mg",
          "frequency": "Once daily at bedtime",
          "duration": "90 Days"
        }
      ],
      "refillHistory": [],
      "warnings": [
        {
          "warningType": "Drug Interaction",
          "description": "Avoid grapefruit and grapefruit juice while taking Atorvastatin as it may increase the risk of side effects."
        },
        {
          "warningType": "Side Effects",
          "description": "May cause muscle pain. Report any unusual muscle soreness to your doctor immediately."
        }
      ]
    }
    ```

#### Error Responses

-   `401 Unauthorized`: The `loginJwt` is missing, invalid, or expired.
-   `403 Forbidden`: The user's role is not `Patient`.
-   `404 Not Found`: The `prescriptionId` does not exist or does not belong to the logged-in patient.
-   `500 Internal Server Error`: An unexpected server error occurred.
