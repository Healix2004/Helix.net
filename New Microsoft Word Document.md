# Healix Healthcare Platform - Screen Descriptions 

## Patient Journey Screens 

### 1. Landing/Hero Page 

**Description**: The main entry point showcasing the platform's value proposition, featuring medical records management, smart prescriptions, and doctor notes. 

**Data Needed**: 

- None (static marketing content) 

- Call-to-action buttons (Get Started, Sign Up Now) 

--- 

### 2. Feature Highlights Section 

**Description**: Displays three core features - Medical Records, Smart Prescriptions, and Doctor Notes with visual icons. 

**Data Needed**: 

- Feature title 

- Feature description text 

- Feature icon/image reference 

--- 

### 3. User Role Selection 

**Description**: Allows users to select their role in the healthcare ecosystem (Patient, Doctor, Pharmacy/Lab). 

**Data Needed**: 

- User role options (Patient, Doctor, Pharmacy/Lab) 

--- 

### 4. Patient Sign Up / Create Account 

**Description**: Registration form for new patients with email, password, and profile setup. 

**Data Needed**: 

- Email address 

- Password 

- Full name 

- Date of birth (implied) 

- Phone number (implied) 

--- 

### 5. Email Verification 

**Description**: OTP/6-digit code verification screen for confirming user email address. 

**Data Needed**: 

- User email address (displayed) 

- 6-digit verification code (input) 

- Resend timer countdown 

--- 

### 6. Welcome/Profile Setup 

**Description**: Post-verification welcome screen prompting users to complete their profile. 

**Data Needed**: 

- User name 

- Verification status 

--- 

### 7. Patient Onboarding - Medical Information 

**Description**: Collection of patient medical history for personalized care. 

**Data Needed**: 

- Medical conditions (past and current) 

- Known allergies 

- Current medications 

- Past surgeries 

- Family medical history 

- Primary care physician information 

--- 

### 8. Patient Onboarding - Emergency & Insurance 

**Description**: Collection of emergency contact and insurance information. 

**Data Needed**: 

- Emergency contact name 

- Emergency contact phone number 

- Emergency contact relationship 

- Insurance provider name 

- Insurance policy ID 

- Insurance group number 

--- 

### 9. Patient Onboarding - Preferences 

**Description**: User customization for pharmacy, doctor preferences, and notification settings. 

**Data Needed**: 

- Preferred pharmacy 

- Preferred doctor name 

- Consultation preferences (in-person, virtual, both) 

- Notification preferences (appointment reminders, medication reminders, health tips, lab results, prescription refills) 

--- 

### 10. Patient Dashboard 

**Description**: Main patient dashboard showing health summary, recent activity, active prescriptions, and quick actions. 

**Data Needed**: 

- User greeting (Good Morning, Maryam) 

- Health score (85/100) 

- Total medical records count (24) 

- Active prescriptions count (3) 

- Upcoming appointments count (2) 

- Recent activity items: 

- Lab results (date, description) 

- Prescription refills (medication name, status, date) 

- Appointment confirmations (doctor name, date) 

- Health checkup completions 

- Quick action buttons (Schedule Appointment, Request Prescription, Upload Document, Generate QR Code) 

- Health tips (Stay Hydrated, Daily Exercise, Monitor Blood Sugar) 

- AI Assistant chat prompt 

--- 

### 11. Patient Profile Sidebar 

**Description**: Navigation sidebar with user profile and settings options. 

**Data Needed**: 

- User name (MM Maryam) 

- User role (Patient) 

- Navigation items (Dashboard, Medical Records, Lab Tests, Radiology, Prescriptions, Uploaded Documents) 

- User menu options (My Profile, Account Settings, Help & Support, Sign Out) 

--- 

### 12. Patient Access QR Code 

**Description**: Temporary QR code for healthcare providers to access patient medical history. 

**Data Needed**: 

- Patient full name (John Doe) 

- Patient ID (PT-2026-001) 

- Access Code (MED-8X067AJK) 

- QR code generation 

- Access expiry timer (2 hours) 

--- 

### 13. Schedule Appointment 

**Description**: Appointment booking interface with doctor search, date selection, and time slot availability. 

**Data Needed**: 

- Doctor/Specialty search results 

- Visit type options (In-person, Follow-up) 

- Calendar with available dates 

- Available time slots (with booked slots crossed out) 

- Appointment reason/notes 

- Patient ID 

- Selected date and time 

- Doctor availability data 

--- 

### 14. Medical Records (Lab Tests) 

**Description**: Comprehensive lab test results list with filtering capabilities. 

**Data Needed**: 

- Test name 

- Test date 

- Ordering physician 

- Test status (Completed, Pending, Abnormal) 

- Test results/data 

- Reference ranges 

- Quick actions (Upload New Test, Request Report) 

--- 

### 15. Radiology Management 

**Description**: Radiology scan management with preview and radiologist notes. 

**Data Needed**: 

- Scan type (MRI Brain, CT Chest, X-Ray Spine, Ultrasound Abdomen) 

- Scan date 

- Requesting physician 

- Status (Completed, Pending, Abnormal) 

- Radiologist notes 

- Scan preview/thumbnail 

- Total scans count (2,847) 

- Pending review count (23) 

- Recent uploads count (156) 

- Quick actions (Upload New Scan, Request Report, Share with Team) 

--- 

### 16. Uploaded Documents 

**Description**: Repository for all uploaded medical documents. 

**Data Needed**: 

- Document name 

- Document type (PDF, JPG, PNG) 

- Upload date 

- Document category 

--- 

### 17. Prescriptions Management 

**Description**: View and manage active prescriptions with medication history. 

**Data Needed**: 

- Medication name 

- Dosage 

- Frequency 

- Duration 

- Prescribing doctor 

- Prescription date 

- Refill status 

- Pharmacy information 

--- 

## Doctor Journey Screens 

### 18. Doctor Onboarding - Personal Information 

**Description**: Initial doctor profile setup. 

**Data Needed**: 

- Doctor full name 

- Date of birth 

- Contact phone number 

- Email address 

- Professional title 

--- 

### 19. Doctor Onboarding - Professional Information 

- **Description**: Medical credentials and practice details. 

**Data Needed**: 

- Medical specialty 

- Subspecialty 

- Country 

- Clinic/Hospital name 

- Clinic address 

- Medical license number 

- Years of experience (implied) 

--- 

### 20. Doctor Onboarding - Availability & Practice 

**Description**: Setting consultation schedule and practice preferences. 

**Data Needed**: 

- Days of availability 

- Working hours 

- Consultation types offered 

- Appointment duration settings 

- Patient capacity 

--- 

### 21. Doctor Onboarding - Verification 

**Description**: Upload credentials for verification. 

**Data Needed**: 

- Medical license (PDF, JPG, PNG, max 10MB) 

- National ID (PDF, JPG, PNG, max 10MB) 

- Additional certifications (optional) 

- Verification checklist status 

--- 

### 22. Doctor Onboarding - Submission Successful 

**Description**: Post-submission confirmation with verification timeline. 

**Data Needed**: 

- Doctor name 

- Application reference number 

- Email address 

- Verification timeline (12-24 hours, 24-48 hours) 

- Support contact information 

- Dashboard navigation button 

--- 

## ### 23. Doctor Dashboard 

**Description**: Main doctor dashboard with patient overview, appointments, and AI insights. 

**Data Needed**: 

- Doctor greeting (Good Morning, Dr. Mustafa) 

- Today's appointments count (24) with trend (+15%) 

- Critical alerts count (3) with trend (-25%) 

- Pending labs count (8) with trend (-33%) 

- Active patients count (142) with trend (+8%) 

- Appointment trend chart data (Jan-May) 

- AI Assistant recommendations (Review Critical Labs, Update Care Plans, Schedule Follow-ups) 

- Today's appointments list (patient name, visit type, time, status) 

- Quick actions (Add Patient, Upload Scan, Generate Report, Request Lab Test, Create Prescription) 

- Weekly patient visits chart 

--- 

### 24. Doctor Appointments View 

**Description**: Complete appointment management interface. 

**Data Needed**: 

- All scheduled appointments 

- Filter options (Today, Upcoming, Completed, Cancelled) 

- Patient information 

- Appointment status 

- Time slots 

- Quick actions 

--- 

### 25. Doctor Patient Profile 

**Description**: Detailed patient view with medical history, current medications, and allergies. 

**Data Needed**: 

- Patient name (Sarah Johnson) 

- Patient ID (45892-A) 

- Age (58) 

- Gender (Male) 

- Contact information 

- Email 

- Insurance details 

- Known allergies (Penicillin, Sulfa drugs) 

- Chronic conditions (Hypertension, Type 2 Diabetes) 

- Current medications (Lisinopril, Aspirin, Vitamin D3) 

- Medication details (dosage, frequency, prescribed date) 

- Compliance rate (94%) 

- Quick actions (Search Medication, Search Lab Test, Search Radiology) 

- Appointment scheduling 

--- 

### 26. Doctor - New Prescription 

**Description**: Prescription creation interface with medication search and patient selection. 

**Data Needed**: 

- Patient information (name, ID, age, gender) 

- Medication search (by drug name, generic, condition) 

- Dosage selection 

- Frequency selection 

- Duration 

- Special instructions 

- Active medications list 

- Known allergies (displayed) 

- Chronic conditions (displayed) 

- Compliance rate 

--- 

### 27. Drug Interaction Alert 

**Description**: AI-powered warning when prescribing medications that may interact with current patient medications. 

**Data Needed**: 

- Drug interaction warning (e.g., Rivaroxaban + Warfarin) 

- AI recommendation (alternative medication: Apixaban) 

- Risk assessment (85% High Risk) 

- Patient risk factors (age 72, current medications, cardiovascular conditions) 

- Safer alternative options (Clopidogrel 75mg) 

- Action buttons (Switch to Recommended Medication, Continue Anyway) 

--- 

### 28. QR Code Scan 

**Description**: Camera interface for scanning patient QR codes from mobile devices. 

**Data Needed**: 

- Camera access 

- QR code scanner 

- Patient information display (after scan) 

- Cancel button 

--- 

## Pharmacy Journey Screens 

### 29. Pharmacy Registration - Account Setup 

- **Description**: Pharmacist account creation. 

**Data Needed**: 

- Full name 

- Email address 

- Password 

- Confirm password 

- Pharmacist license number (implied) 

--- 

### 30. Pharmacy Registration - Pharmacy Details 

- **Description**: Pharmacy location and registration information. 

**Data Needed**: 

- Pharmacy name 

- License number 

- Registration year 

- Pharmacy address 

- City 

- State/ZIP 

- Contact phone number (implied) 

--- 

### 31. Pharmacy Registration - Verification 

**Description**: Upload verification documents for pharmacy approval. 

**Data Needed**: 

- Pharmacy license (PDF, JPG, PNG) 

- Pharmacy registration certificate 

- Business license 

- Pharmacist credentials 

--- 

### 32. Pharmacy Registration - Complete 

**Description**: Application submitted confirmation with verification timeline. 

**Data Needed**: 

- Application status (under review) 

- Estimated verification time (24-48 hours) 

- Verification checklist (License verification, Document review, Account activation) 

- Support contact information 

--- 

## Shared/Common Screens 

### 33. AI Assistant Chat Interface 

**Description**: AI-powered chat interface for health-related questions and insights. 

**Data Needed**: 

- User question input 

- Patient context (Sarah Johnson, 68y, Female, PID: 88421) 

- Chronic conditions 

- Current medications 

- AI-generated responses 

- Contact care team option 

--- 

### 34. Notifications Center 

**Description**: Central notification hub for all system alerts. 

**Data Needed**: 

- Notification list with timestamps 

- Notification types (Lab Results, AI Analysis, Appointment Reminder, Pharmacy Confirmation, Critical Alert) 

- Read/unread status 

- Mark all read functionality 

--- 

### 35. Doctor Directory/Search 

**Description**: Searchable directory of healthcare providers. 

**Data Needed**: 

- Doctor name 

- Specialty 

- Practice/Clinic name 

- Availability status 

- Location 

- Rating (implied) 

--- 

## ## Data Requirements Summary 

## ### User Data 

- Name, email, phone, date of birth 

- Role (Patient, Doctor, Pharmacy) 

- Profile photo (optional) 

- Account settings preferences 

## ### Patient Data 

- Medical history (conditions, allergies, medications, surgeries) 

- Lab test results (name, date, results, reference ranges) 

- Radiology scans (type, date, notes, images) 

- Prescriptions (medication, dosage, frequency, duration, provider) 

- Uploaded documents (name, type, date, file) 

- Emergency contact (name, phone, relationship) 

- Insurance (provider, policy ID) 

## ### Doctor Data 

- Professional credentials (license, specialty, subspecialty) 

- Practice information (clinic name, address) 

- Schedule availability (days, hours) 

- Patient appointments 

- Prescriptions issued 

- Verification documents 

### Pharmacy Data 

- Pharmacy information (name, license, address) 

- Pharmacist credentials 

- Verification documents 

- Prescription processing status 

## ### Appointment Data 

- Patient and doctor information 

- Date and time 

- Type (In-person, Follow-up, Virtual) 

- Status (Scheduled, Completed, Cancelled) 

- Reason/Notes 

## ### Notification Data 

- Message content 

- Timestamp 

- Type (Alert, Reminder, Update) 

- Read/Unread status 

## ### AI Assistant Data 

- User queries 

- Patient context (conditions, medications) 

- AI-generated responses and recommendations 

- Health insights and alerts 

