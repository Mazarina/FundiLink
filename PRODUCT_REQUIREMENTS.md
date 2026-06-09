# PRODUCT_REQUIREMENTS.md — FundiLink SmartApply MVP

## Roles

| Role | Description |
|---|---|
| **Student** | The primary user. A learner creating a profile and tracking opportunities. |
| **SchoolAdmin** | A teacher or guidance counsellor at a partner school, with visibility over their learners. |
| **SupportAgent** | A FundiLink staff member who assists learners remotely. Can view and support learner profiles with consent. |
| **Admin** | A FundiLink operations staff member. Manages users, institutions, programmes, and platform settings. |
| **SuperAdmin** | Full platform access. ZulTek technical and executive team only. |

---

## Student Profile

**Purpose:** One unified learner profile that powers all FundiLink features.

### Personal Information
- Full name (first name, surname)
- Date of birth
- South African ID number or passport number
- Gender (optional)
- Home language (optional)
- Nationality
- Contact: mobile number, email address
- Residential address (province, municipality, suburb/area)
- School name and province
- Grade level (Grade 11, Grade 12, Post-matric)
- Guardian/parent contact (name, phone, email) — required for learners under 18

### Account
- Email-based registration
- Email verification
- Password with minimum security standards
- Consent acknowledgement at registration
- POPIA notice and acceptance

---

## Academic Profile

**Purpose:** Capture the learner's academic record to enable APS calculation and programme matching.

### NSC (Matric) Results
- Enter subjects and marks (percentage)
- Support for Grade 11 preliminary results and Grade 12 final results
- Distinction tracking
- Results can be entered manually; document upload for official results

### School Record
- Grade 11 results (for pre-matric learners)
- Previous academic history (optional)

---

## APS Calculator

**Purpose:** Automatically calculate a learner's Admission Point Score from their NSC results.

### Rules
- Standard APS calculation using NSC percentage-to-point conversion
- Handle Life Orientation correctly (not counted at all institutions; configurable per institution)
- Display APS score prominently on the profile
- Recalculate automatically when results are updated
- Explain APS to the learner in plain language

### APS Scale (standard NSC)
| Percentage | APS Points |
|---|---|
| 80–100% | 7 |
| 70–79% | 6 |
| 60–69% | 5 |
| 50–59% | 4 |
| 40–49% | 3 |
| 30–39% | 2 |
| 0–29% | 1 |

---

## Programme Matching

**Purpose:** Show the learner what programmes they may qualify for based on their APS and subjects.

### Features
- Match learner profile against a database of programmes
- Filter by: qualification type (degree, diploma, certificate), institution type (university, TVET, private college), field of study, province
- Show minimum APS requirements
- Show subject requirements
- Show application open/close dates
- Flag programmes where the learner is a strong, borderline, or unlikely match
- Allow learner to save programmes to their tracker

### Data Requirements
- Programme database (institution, programme name, NQF level, minimum APS, subject requirements, application dates, fees placeholder)
- Institution database (name, type, province, website, application portal URL)
- Sourced from public information; not an official integration unless documented

---

## Document Checklist and Vault

**Purpose:** Help learners prepare, upload, and organise the documents they need for applications.

### Document Types
- Certified copy of ID
- Birth certificate
- Matric certificate / Grade 11 results
- Proof of residence
- Passport photo
- Parent/guardian ID
- Proof of income / financial documents
- SASSA grant letter (if applicable)
- Additional documents per institution

### Features
- Per-application document checklist (configurable per institution/programme)
- Upload documents (PDF, JPG, PNG)
- Mark documents as certified / uncertified
- Expiry tracking (certified copies expire)
- Document status: Not uploaded / Uploaded / Verified
- Secure, access-controlled document storage
- Documents accessible only by the learner and authorised agents

---

## Application Tracker

**Purpose:** Give the learner one view of all their applications and their current status.

### Features
- Add an application (institution, programme, application date)
- Application status: Draft / Submitted / Under Review / Accepted / Rejected / Waitlisted / Deferred
- Notes field per application
- Attach documents to applications
- Application deadline tracking with alerts
- Link to official institution portal (for submission — FundiLink does not submit on their behalf unless officially integrated)

---

## Notifications

**Purpose:** Keep learners informed about deadlines, status changes, and required actions.

### Channels
- In-app notifications (notification bell)
- Email notifications

### Triggers
- Application deadline approaching (30 days, 7 days, 1 day)
- Application status change
- Document expiry approaching
- New matching programme available
- Admin/support messages

### Future
- WhatsApp notifications (Phase 5)
- SMS fallback (Phase 5+)

---

## Admin Portal

**Purpose:** Allow FundiLink staff to manage the platform and support learners.

### Support Agent Capabilities
- Search for learners (by name, ID, school)
- View a learner's profile (with consent/audit log)
- Add notes to a learner's support record
- Flag a learner for follow-up
- Send in-app messages to learners

### Admin Capabilities
- All Support Agent capabilities
- Manage institution and programme data
- Manage school accounts and SchoolAdmin users
- View platform usage reports
- Manage notification templates
- Manage content (FAQs, guidance content)

### SuperAdmin Capabilities
- All Admin capabilities
- Manage admin accounts and roles
- View audit logs
- Platform configuration
- Data export (with POPIA compliance)

---

## School Dashboard Foundation

**Purpose:** Give partner school admins visibility over their learners' progress.

### Features (MVP — foundational only)
- School registration and admin account setup
- View list of learners associated with the school
- See aggregate progress (profiles complete, documents uploaded, applications submitted)
- Flag learners who need support
- School admin cannot view individual learner documents without explicit learner consent

---

## Exclusions from MVP

The following are explicitly out of scope for the MVP:

- WhatsApp Business API integration
- SMS gateway
- Automated application submission to any institution
- Payment processing / fee collection
- Bursary partner portal
- AI guidance chatbot
- Student accommodation search
- Skills development / learnerships module
- Mentorship matching
- Employer / early career portal
- Official NSFAS API integration
- Official CAO / university API integration
- Mobile native app (iOS / Android)
- Multi-language support (English only for MVP)

These will be considered in later phases. See `ROADMAP.md`.
