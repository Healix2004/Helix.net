# Frontend Endpoint Usage Guide

This document is a practical reference for frontend developers. It explains how each backend endpoint should be used, what data to send, and which roles are allowed to call it.

## 1. Important rules for the frontend

- Use the base URL from the environment:
    - Development HTTP: http://localhost:5181
    - Development HTTPS: https://localhost:7193
- For every authenticated endpoint, send:
    - `Authorization: Bearer {loginJwt}`
- For patient consent-protected doctor views, also send:
    - `X-Consent-Token: {consentJwt}`
- Some radiology/lab endpoints require the consent token to be used as the bearer token instead of the regular login token. Check the endpoint notes below.
- Always check the response wrapper:
    - `succeeded`
    - `message`
    - `data`
- If the backend returns a permission error, handle it as a role/consent issue even if the HTTP code is not `403`.

## 2. Roles used by the backend

- `Patient`
- `Doctor`
- `Admin`
- `Radiologist`
- `LabSpecialist`
- `Pharmaciest`

## 3. Authentication and account endpoints

| Method | Endpoint                              | Frontend use                     | Allowed roles       | Notes                                                                                                 |
| ------ | ------------------------------------- | -------------------------------- | ------------------- | ----------------------------------------------------------------------------------------------------- |
| POST   | `/api/auth/login`                     | Sign in the user                 | Public              | Send `emailAddress` and `password`. Store the returned access token.                                  |
| POST   | `/api/auth/register`                  | Create a new account             | Public              | Use `multipart/form-data`. Required fields include `Username`, `Email`, `Password`, and `RegisterAs`. |
| GET    | `/api/auth/profile/{userId}`          | Load current user profile        | Authenticated users | Use the logged-in token.                                                                              |
| GET    | `/api/auth/user/{email}`              | Lookup a user by email           | Authenticated users | Mainly for internal/admin support flows.                                                              |
| POST   | `/api/auth/forgot-password`           | Start password reset             | Public              | Send the email address.                                                                               |
| POST   | `/api/auth/reset-password`            | Complete password reset          | Public              | Use the reset token from the email flow.                                                              |
| POST   | `/api/auth/change-password`           | Change current password          | Authenticated users | The backend reads the user ID from the JWT.                                                           |
| POST   | `/api/auth/confirm-email`             | Confirm email after registration | Public              | Send `userEmail` and the OTP code.                                                                    |
| POST   | `/api/auth/resend-confirmation-email` | Resend verification code         | Public              | Send the email as query parameter or form value.                                                      |

## 4. Patient endpoints

| Method | Endpoint                         | Frontend use                       | Allowed roles     | Notes                                           |
| ------ | -------------------------------- | ---------------------------------- | ----------------- | ----------------------------------------------- |
| POST   | `/api/patients/register-patient` | Complete patient onboarding        | Public            | Use after account registration.                 |
| GET    | `/api/patients/my-profile`       | Load the logged-in patient profile | `Patient`         | Use the patient JWT.                            |
| GET    | `/api/patients/{id}`             | View a specific patient record     | `Admin`, `Doctor` | Doctors need valid consent or emergency access. |
| GET    | `/api/patients/all`              | List all patients                  | `Admin`           | Admin-only data view.                           |
| POST   | `/api/patients`                  | Create a patient record manually   | `Admin`           | Admin-only system management.                   |
| PUT    | `/api/patients/{id}`             | Update a patient record            | `Admin`           | Admin-only management route.                    |
| DELETE | `/api/patients/{id}`             | Delete a patient record            | `Admin`           | Admin-only destructive action.                  |

## 5. Doctor endpoints

| Method | Endpoint                        | Frontend use                      | Allowed roles        | Notes                                            |
| ------ | ------------------------------- | --------------------------------- | -------------------- | ------------------------------------------------ |
| POST   | `/api/doctors/register-doctor`  | Complete doctor onboarding        | Public               | Use for doctor registration and document upload. |
| GET    | `/api/doctors/my-profile`       | Load the logged-in doctor profile | `Doctor`             | Use the doctor JWT.                              |
| GET    | `/api/doctors/all`              | List doctors                      | Authenticated users  | Any logged-in user can view the list.            |
| GET    | `/api/doctors/{id}`             | View a doctor public profile      | Authenticated users  | Any logged-in user can access.                   |
| POST   | `/api/doctors`                  | Create a doctor record manually   | `Admin`              | System management only.                          |
| PUT    | `/api/doctors/{id}`             | Update doctor record              | `Admin`              | Admin-only update route.                         |
| DELETE | `/api/doctors/{id}`             | Delete doctor record              | `Admin`              | Admin-only destructive action.                   |
| GET    | `/api/doctors/search/name`      | Search doctors by name            | Public/Authenticated | Use query parameter `name`.                      |
| GET    | `/api/doctors/search/specialty` | Search doctors by specialty       | Public/Authenticated | Use query parameter `specialty`.                 |

## 6. Appointment endpoints

| Method | Endpoint                                    | Frontend use                 | Allowed roles       | Notes                                                                               |
| ------ | ------------------------------------------- | ---------------------------- | ------------------- | ----------------------------------------------------------------------------------- |
| GET    | `/api/appointments/dashboard/today`         | Show today’s doctor schedule | `Doctor`            | Used for doctor dashboard.                                                          |
| GET    | `/api/appointments/dashboard/summary`       | Show doctor day summary      | `Doctor`            | Used for dashboard summary cards.                                                   |
| POST   | `/api/appointments`                         | Schedule an appointment      | `Doctor`, `Admin`   | Doctors normally create their own slots; admins can manage system-level scheduling. |
| PUT    | `/api/appointments/{id}/status`             | Update appointment status    | `Doctor`, `Admin`   | Use query param `newStatus`.                                                        |
| GET    | `/api/appointments/doctor/{doctorId}/slots` | Load available time slots    | Authenticated users | Any logged-in user may view doctor availability.                                    |

## 7. Consent and QR access endpoints

| Method | Endpoint                    | Frontend use                  | Allowed roles | Notes                                                                                                            |
| ------ | --------------------------- | ----------------------------- | ------------- | ---------------------------------------------------------------------------------------------------------------- |
| POST   | `/api/consents/generate-qr` | Generate a patient consent QR | `Patient`     | The patient creates a QR token for the doctor.                                                                   |
| POST   | `/api/consents/redeem-qr`   | Redeem a patient QR token     | `Doctor`      | The doctor scans the QR and receives a consent JWT; send it as `X-Consent-Token` or bearer token where required. |
| GET    | `/api/consents/all`         | List consents                 | `Admin`       | Admin dashboard use only.                                                                                        |
| GET    | `/api/consents/{id}`        | Get a consent by ID           | `Admin`       | Admin-only.                                                                                                      |
| POST   | `/api/consents`             | Create a consent record       | `Admin`       | Internal/admin management.                                                                                       |
| PUT    | `/api/consents/{id}`        | Update a consent              | `Admin`       | Admin-only.                                                                                                      |
| DELETE | `/api/consents/{id}`        | Delete a consent              | `Admin`       | Admin-only.                                                                                                      |

## 8. Lab order endpoints

| Method | Endpoint                                  | Frontend use                                      | Allowed roles                | Notes                                                                                                     |
| ------ | ----------------------------------------- | ------------------------------------------------- | ---------------------------- | --------------------------------------------------------------------------------------------------------- |
| GET    | `/api/lab-orders/my-pending`              | Show pending lab orders for the logged-in patient | `Patient`                    | Patient view.                                                                                             |
| GET    | `/api/lab-orders/my-history`              | Show full lab history for patient                 | `Patient`                    | Patient view.                                                                                             |
| GET    | `/api/lab-orders/scan/{qrToken}`          | Scan a lab QR token                               | Public/Authenticated         | Usually used by lab staff or a kiosk workflow.                                                            |
| POST   | `/api/lab-orders/{orderId}/results`       | Upload a lab result                               | Public/Authenticated         | In the current backend it is not role-locked in the code. Use carefully and only for trusted staff flows. |
| GET    | `/api/lab-orders/patient/{patientId}`     | View a patient’s lab orders                       | `Doctor`                     | Requires consent via `X-Consent-Token`.                                                                   |
| GET    | `/api/lab-orders/my-orders`               | View lab orders for the logged-in doctor          | `Doctor`                     | Doctor workspace view.                                                                                    |
| POST   | `/api/lab-orders`                         | Create a lab order                                | `Doctor`                     | Doctor creates an order for a patient.                                                                    |
| PUT    | `/api/lab-orders/{id}`                    | Update a lab order                                | `Doctor`                     | Used for editing existing lab orders.                                                                     |
| GET    | `/api/lab-orders/{id}`                    | Get a specific lab order                          | `Admin`, `Doctor`, `Patient` | Patient can view their own order.                                                                         |
| GET    | `/api/lab-orders/all`                     | List all lab orders                               | `Admin`                      | Admin-only management.                                                                                    |
| PUT    | `/api/lab-orders/{id}/status/{newStatus}` | Update lab status                                 | `Admin`                      | Admin management action.                                                                                  |
| DELETE | `/api/lab-orders/{id}`                    | Delete a lab order                                | `Admin`                      | Admin-only destructive action.                                                                            |

## 9. Lab result endpoints

| Method | Endpoint                               | Frontend use                      | Allowed roles     | Notes                                  |
| ------ | -------------------------------------- | --------------------------------- | ----------------- | -------------------------------------- |
| GET    | `/api/lab-results/my-labs`             | Show the patient’s lab results    | `Patient`         | Patient self-service view.             |
| GET    | `/api/lab-results/patient/{patientId}` | View patient results for a doctor | `Doctor`          | Requires consent.                      |
| GET    | `/api/lab-results/all`                 | List all lab results              | `Admin`           | Admin-only.                            |
| GET    | `/api/lab-results/{id}`                | Get a specific lab result         | `Doctor`, `Admin` | Use only when you already know the ID. |
| POST   | `/api/lab-results`                     | Create a lab result               | `Admin`           | Strictly admin-managed.                |
| PUT    | `/api/lab-results/{id}`                | Update a lab result               | `Admin`           | Admin-only.                            |
| DELETE | `/api/lab-results/{id}`                | Delete a lab result               | `Admin`           | Admin-only.                            |

## 10. Radiology order endpoints

| Method | Endpoint                                        | Frontend use                                  | Allowed roles          | Notes                                                               |
| ------ | ----------------------------------------------- | --------------------------------------------- | ---------------------- | ------------------------------------------------------------------- |
| GET    | `/api/radiology-orders/my-pending`              | Show pending radiology orders for the patient | `Patient`              | Patient-facing.                                                     |
| GET    | `/api/radiology-orders/scan/{qrToken}`          | Scan a radiology QR token                     | `Admin`, `Radiologist` | Staff workflow.                                                     |
| POST   | `/api/radiology-orders/{orderId}/results`       | Upload radiology result                       | `Admin`, `Radiologist` | Staff workflow.                                                     |
| GET    | `/api/radiology-orders/patient/{patientId}`     | View a patient’s radiology orders             | `Doctor`               | Requires consent and valid doctor identity.                         |
| GET    | `/api/radiology-orders/my-orders`               | View orders for the logged-in doctor          | `Doctor`               | Doctor view.                                                        |
| POST   | `/api/radiology-orders`                         | Create a radiology order                      | `Doctor`               | Doctor creates the order.                                           |
| PUT    | `/api/radiology-orders/{id}`                    | Update a radiology order                      | `Doctor`               | Doctor editing workflow.                                            |
| GET    | `/api/radiology-orders/{id}`                    | Get a specific radiology order                | `Admin`                | Admin management route.                                             |
| GET    | `/api/radiology-orders/all`                     | List all radiology orders                     | `Admin`                | Admin-only.                                                         |
| PUT    | `/api/radiology-orders/{id}/status/{newStatus}` | Update radiology status                       | `Admin`, `Radiologist` | Staff action.                                                       |
| DELETE | `/api/radiology-orders/{id}`                    | Delete a radiology order                      | `Admin`                | Admin-only.                                                         |
| GET    | `/api/radiology-orders/order/{orderId}`         | Get radiology report by order ID              | Authenticated users    | Use carefully; the backend currently allows any authenticated user. |

## 11. Radiology result endpoints

| Method | Endpoint                                               | Frontend use                                    | Allowed roles       | Notes                                |
| ------ | ------------------------------------------------------ | ----------------------------------------------- | ------------------- | ------------------------------------ |
| GET    | `/api/radiology-results/order/{orderId}`               | Get results for a specific order                | Authenticated users | Use after an order is created.       |
| GET    | `/api/radiology-results/my-radiology`                  | Show the patient’s radiology results            | `Patient`           | Patient self-service.                |
| GET    | `/api/radiology-results/patient/{patientId}/radiology` | View a patient’s radiology results for a doctor | `Doctor`            | Requires consent.                    |
| GET    | `/api/radiology-results/all`                           | List all radiology results                      | `Admin`             | Admin-only.                          |
| PUT    | `/api/radiology-results/{id}`                          | Update a radiology result                       | `Doctor`, `Admin`   | Use when editing an existing result. |
| DELETE | `/api/radiology-results/{id}`                          | Delete a radiology result                       | `Admin`             | Admin-only.                          |

## 12. Medication, observation, prescription, encounter, facility, and emergency endpoints

| Method | Endpoint                                         | Frontend use                        | Allowed roles       | Notes                                   |
| ------ | ------------------------------------------------ | ----------------------------------- | ------------------- | --------------------------------------- |
| GET    | `/api/medications/my-medications`                | Show patient medications            | `Patient`           | Patient self-service.                   |
| GET    | `/api/medications/patient/{patientId}`           | View a patient’s medication history | `Doctor`            | Doctor workflow.                        |
| POST   | `/api/medications`                               | Prescribe or add medication         | `Doctor`            | Doctor action.                          |
| PUT    | `/api/medications/{id}`                          | Update medication entry             | `Doctor`, `Admin`   | Doctor/admin editing.                   |
| GET    | `/api/medications/all`                           | List all medications                | `Admin`             | Admin management.                       |
| GET    | `/api/medications/{id}`                          | Get one medication                  | `Admin`, `Doctor`   | Restricted lookup.                      |
| DELETE | `/api/medications/{id}`                          | Delete medication                   | `Admin`, `Doctor`   | Use carefully.                          |
| GET    | `/api/observations/my-observations`              | Show patient observations           | `Patient`           | Patient view.                           |
| GET    | `/api/observations/patient/{patientId}`          | View observations for a patient     | `Doctor`            | Doctor workflow.                        |
| POST   | `/api/observations`                              | Add observations                    | `Doctor`, `Nurse`   | Clinician action.                       |
| PUT    | `/api/observations/{id}`                         | Update an observation               | `Doctor`, `Admin`   | Clinical editing.                       |
| GET    | `/api/observations/all`                          | List all observations               | `Admin`             | Admin-only.                             |
| GET    | `/api/observations/{id}`                         | Get one observation                 | `Admin`, `Doctor`   | Restricted lookup.                      |
| DELETE | `/api/observations/{id}`                         | Delete observation                  | `Admin`             | Admin-only.                             |
| GET    | `/api/prescriptions/patient/{patientId}/summary` | Show prescription summary           | `Doctor`            | Doctor clinical summary.                |
| POST   | `/api/prescriptions`                             | Create a prescription               | `Doctor`            | Doctor action.                          |
| PUT    | `/api/prescriptions/item/{itemId}/status`        | Update prescription item status     | `Doctor`, `Admin`   | Doctor/admin action.                    |
| GET    | `/api/encounters`                                | List encounters                     | `Doctor`            | Doctor workflow.                        |
| GET    | `/api/encounters/{id}`                           | Get one encounter                   | `Doctor`            | Doctor workflow.                        |
| POST   | `/api/encounters`                                | Create an encounter                 | `Doctor`            | Doctor workflow.                        |
| PUT    | `/api/encounters/{id}`                           | Update an encounter                 | `Doctor`            | Doctor workflow.                        |
| DELETE | `/api/encounters/{id}`                           | Delete an encounter                 | `Doctor`            | Doctor workflow.                        |
| GET    | `/api/facilities/all`                            | List facilities                     | Authenticated users | Any logged-in user can view facilities. |
| GET    | `/api/facilities/{id}`                           | Get one facility                    | Authenticated users | Any logged-in user can view.            |
| POST   | `/api/facilities`                                | Create facility                     | `Admin`             | Admin-only management.                  |
| PUT    | `/api/facilities/{id}`                           | Update facility                     | `Admin`             | Admin-only management.                  |
| DELETE | `/api/facilities/{id}`                           | Delete facility                     | `Admin`             | Admin-only management.                  |
| POST   | `/api/emergency-access/break-the-glass`          | Request emergency access            | `Doctor`            | Used for break-the-glass workflows.     |

## 13. Catalog, chatbot, files, and other shared endpoints

| Method | Endpoint                            | Frontend use                       | Allowed roles       | Notes                                |
| ------ | ----------------------------------- | ---------------------------------- | ------------------- | ------------------------------------ |
| GET    | `/api/catalog/search`               | Search catalog terms               | Public              | No auth required.                    |
| GET    | `/api/medicationcatalog/search`     | Search medication catalog          | Public              | No auth required.                    |
| GET    | `/api/allergencatalog/search`       | Search allergy catalog             | Public              | No auth required.                    |
| GET    | `/api/chronicdiseasecatalog/search` | Search chronic disease catalog     | Public              | No auth required.                    |
| GET    | `/api/specialtycatalog/search`      | Search specialty catalog           | Public              | No auth required.                    |
| POST   | `/api/chatbot/ask`                  | Chat with the backend AI assistant | Authenticated users | Use with the login token.            |
| POST   | `/api/files/upload`                 | Upload a file                      | Authenticated users | Use for documents and media uploads. |
| GET    | `/api/files/{fileName}`             | Download or show a file            | Authenticated users | Use the returned file name/path.     |
| POST   | `/api/emails/send`                  | Send an email                      | `Admin`             | Internal/admin action.               |

## 14. Frontend best practices

1. Do not expose admin-only APIs to patient or doctor screens.
2. Use the role-based UI routing so users only see the screens they are allowed to access.
3. When a doctor needs patient records, always request explicit consent first and send the consent token.
4. For file upload forms, use `multipart/form-data`.
5. For all mutation actions, show success and error states from the `message` field.
6. Handle permission responses gracefully and show a friendly “not authorized” message.

## 15. Quick checklist for new frontend screens

- Decide which role can access the screen.
- Pick the correct endpoint.
- Attach the bearer token when required.
- Add the consent token when the backend expects it.
- Show role/consent errors clearly.
- Use the response wrapper fields consistently.
