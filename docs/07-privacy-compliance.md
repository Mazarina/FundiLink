# 07 — Privacy and POPIA Compliance

## Title
FundiLink — Privacy and POPIA Compliance Record

## Purpose
Document FundiLink's approach to POPIA compliance, data flows, and privacy-by-design decisions. This document must be updated whenever data collection, processing, or storage practices change.

---

## Applicable Legislation
- **POPIA** — Protection of Personal Information Act, Act 4 of 2013 (South Africa)
- **PAIA** — Promotion of Access to Information Act (right of access requests)
- **Children's Act** — Guardian consent for minors

---

## Data Controller
**ZulTek** (trading as FundiLink)
Information Officer: [To be appointed]
Contact: [To be added]

---

## Personal Information We Process

| Category | Examples | Legal Basis |
|---|---|---|
| Identity | Full name, ID number, DOB | Contractual necessity (service delivery) |
| Contact | Email, mobile number, address | Contractual necessity |
| Academic | Subjects, marks, school name, grade | Contractual necessity (core feature) |
| Documents | ID copy, birth certificate, matric results | Contractual necessity |
| Guardian/Parent | Name, contact details | Contractual necessity (minors) |
| Account | Email, hashed password | Contractual necessity |
| Usage | Login timestamps, actions | Legitimate interest (security, audit) |
| Financial | Proof of income (for bursary matching) | Consent |

---

## Consent

- Explicit consent obtained at registration
- Learner shown plain-language privacy notice before accepting
- Guardian consent required for learners under 18
- Consent version and timestamp recorded in database
- Consent can be withdrawn; withdrawal triggers a data deletion process

---

## Data Minimisation
We collect only data necessary for the service. Optional fields are clearly marked.
Profile sections the learner hasn't filled in are not required unless needed for a specific feature.

---

## Data Access Controls
- Role-based access (Student, SchoolAdmin, SupportAgent, Admin, SuperAdmin)
- Students access only their own data
- SupportAgent access to learner profile requires audit log entry
- SchoolAdmin sees aggregate data only; not individual documents without learner consent
- Admins cannot access SuperAdmin audit logs
- All access logged

---

## Data Retention
| Data Type | Retention Period | Basis |
|---|---|---|
| Active learner profiles | Until deletion requested | Service delivery |
| Inactive accounts (no login 2+ years) | Flagged for review; notification sent | Legitimate interest |
| Audit logs | 3 years minimum | Legal compliance |
| Documents | Until deleted by learner or retention policy | Contractual |
| Application records | 3 years post-application | Legal / dispute resolution |

---

## Data Deletion (Right to Erasure)
- Learners can request erasure of their account and data from "My data & privacy" (implemented in Phase 10)
- A request is raised by the learner (status Requested); deletion itself is a deliberate, audited admin action
- An admin reviews the request (Approve / Reject) and, on fulfilment, the learner's personal data is anonymised/soft-deleted
- Erasure fulfilment **preserves append-only audit and consent records** — these are POPIA-minimal and retained as legally required proof of lawful processing and consent; they do not contain sensitive personal payloads
- Anonymised aggregate data (e.g., APS statistics) may be retained as it is not personal information
- All export generation and all erasure request/approve/reject/fulfil actions are recorded in the append-only audit log

See "Data Subject Rights — Export & Erasure Data Flows (Phase 10)" below for detail.

---

## Data Breach Response
1. Identify and contain the breach
2. Assess impact (what data, how many learners, how accessed)
3. Notify affected learners within 72 hours where feasible
4. Notify the Information Regulator within 72 hours if there is a risk of harm
5. Document the breach and response in the incident log

---

## Third-Party Data Sharing
- No personal data shared with third parties without consent and legal basis
- If a third-party integration is added (e.g., WhatsApp API), a data processing agreement (DPA) must be in place before launch
- Document all third-party data flows in this file when they are added

---

## Children's Data
- Learners under 18 require guardian consent recorded in the system
- Guardian name, contact, and consent timestamp stored
- No marketing to under-18 learners without guardian consent

---

## Guardian Consent & Co-Access Data Flows (Phase 9)

FundiLink processes minors' data (learners under 18). Phase 9 makes guardian consent explicit,
recorded, and auditable, and provides a consent-gated, minimised guardian co-access view.

### Data collected
- **GuardianConsent** (append-only): learner id, consent type (DataProcessing, GuardianCoAccess,
  SharingWithInstitutions), scope (ProfileBasic, ProfileAndApplications), status (Granted/Revoked),
  recorded guardian name and contact, timestamp. POPIA-minimal — no sensitive learner detail.
- **GuardianLink**: learner id, guardian's authenticated user id, guardian name/contact.

### Lawfulness & consent
- Guardian consent is required before sensitive processing/sharing where the learner is a minor.
  Recording or revoking consent is only permitted for learners under 18.
- Consent is captured at the learner's request, recorded immutably, and may be withdrawn at any
  time (POPIA right to withdraw). A revocation is appended as a new record; grants are never
  mutated or deleted, preserving a full auditable consent history.

### Data minimisation (guardian co-access)
- A guardian sees only what consent permits. The co-access view is gated by BOTH a guardian link
  and a current `GuardianCoAccess` consent, and is limited to the consented scope:
  - `ProfileBasic`: name, grade, school, province, profile completeness.
  - `ProfileAndApplications`: the above plus application status summaries (status only).
- The guardian view NEVER exposes the learner's ID number, uploaded documents, notes, or contact
  details. Access is read-only — guardians cannot edit or download.

### Auditability
- All consent grants, revocations, guardian links, and every guardian co-access read are written
  to the append-only audit log (actor, action, target, timestamp). Audit records are never updated
  or deleted.

### Processors / third parties
- No external identity-verification or e-signature provider is integrated in this phase. Consent
  checks are deterministic and local (behind `IConsentCheckService`). A real provider may be wired
  later behind the same interface, with any key supplied via environment variables only.

---

## Data Subject Rights — Export & Erasure Data Flows (Phase 10)

Phase 10 implements the POPIA right of access (data export) and right to erasure.

### Right of access — data export
- A learner downloads a structured, typed export of their own FundiLink data (profile, academic
  profile, applications, bursary applications, document metadata, accommodation/career interests,
  consent history) from "My data & privacy".
- The export is **owner-scoped** (resolved from the authenticated user id) and generated **in-process**
  (typed DTO / JSON). No third-party storage, email, or delivery provider is used in this phase.
- Each export generation is recorded in the append-only audit log (`ExportData`).

### Right to erasure — request and fulfilment
- The learner raises an erasure request (`ErasureRequest`, status Requested). Only one open request
  is allowed at a time.
- An admin reviews the request (RBAC: SupportAgent/Admin/SuperAdmin) and approves or rejects it.
- Fulfilment (RBAC: Admin/SuperAdmin) invokes the deterministic `IErasureService`, which:
  - **anonymises** the learner profile (redacts name, ID/passport, contact, school, guardian fields)
    and soft-deletes it as a tombstone, and
  - **removes** the learner's academic profile and subject results, application and bursary-application
    tracking, document metadata, and accommodation/career interests.
- The erasure service **never touches** the append-only audit (`AuditLogEntries`) or consent
  (`GuardianConsents`) records. These are POPIA-minimal and retained as proof of lawful processing/
  consent (the retention rule). The request is then marked Fulfilled.
- Every request, approval, rejection, and fulfilment is recorded in the append-only audit log
  (`RequestErasure`, `ApproveErasureRequest`, `RejectErasureRequest`, `FulfilErasureRequest`).

### Processors / third parties
- No external storage/email/delivery provider is integrated in this phase. The export is generated
  in-process and erasure is a deterministic in-process service behind `IErasureService`. A real
  delivery channel may be wired later behind the same interface, with any key supplied via
  environment variables only.

---

## Admin Reporting & POPIA Operations Dashboard (Phase 11)

The staff reporting feature is **read-only and aggregate-first**. It introduces **no new way to
read learner sensitive fields**:

- The operations dashboard and POPIA operations summary return **aggregate counts and grouped
  totals only** (e.g. learners by province, applications by status, pending document
  verifications, pending erasure requests, consent grants vs revocations). No personal
  information is contained in these responses.
- The audit activity report is a **filtered, paged view over the existing append-only audit log**
  (by action, actor role, date range). It surfaces only the fields the audit log already records;
  it never mutates audit data.
- All reporting endpoints are **RBAC-gated** (dashboard & POPIA summary: SupportAgent/Admin/
  SuperAdmin; audit activity report: SuperAdmin). Aggregate endpoints surface no individual
  learner, so they add no new per-learner audit entries; existing per-learner admin access
  (e.g. learner search/overview) remains append-only audit-logged as before.
- Aggregation is **deterministic and computed in-process** behind `IReportingRepository`. There
  is **no third-party analytics/telemetry provider integration**; a real provider may be wired
  later behind the same interface, with any key supplied via environment variables only.

---

## Notifications & Deadline Reminders (Phase 12)

Phase 12 activates deterministic, in-process **deadline-reminder generation** plus learner
**notification history** and an admin-triggered **reminder run**. It adds **no new PII exposure**:

- **Guidance only.** Reminders help learners prepare and organise; FundiLink is **not** an official
  admissions or funding portal. Reminder copy explicitly states submissions happen on the official
  institution/funder portal.
- **Preferences and consent respected.** Reminders are dispatched through the existing
  `INotificationService`, which only contacts channels the learner has opted into (email on by
  default; WhatsApp/SMS opt-in). For **minor learners** (under 18), a **current guardian
  data-processing consent** is required before any reminder is sent; otherwise the learner is skipped.
- **Data minimisation.** The deadline query returns only the minimal fields needed to compose a
  reminder (learner id, opportunity name, deadline date) for **active (non-deleted) learners** only.
  The learner notification-history DTO surfaces only existing log fields and **deliberately omits the
  recipient address**.
- **Idempotency.** At most **one deadline reminder per learner per UTC day**, so repeated runs do not
  double-send.
- **Stub delivery only.** No real email/SMS/WhatsApp provider is integrated; a real provider may be
  wired later behind the same interfaces, with any key supplied via environment variables only.
- **Auditability & RBAC.** The admin-triggered run is **RBAC-gated** (SupportAgent/Admin/SuperAdmin)
  and writes an **append-only audit entry** (`TriggerDeadlineReminders`) recording actor, window, and
  aggregate sent/skipped counts. Every channel attempt continues to write an append-only
  `NotificationLog` entry as before. No external scheduler is wired in this phase.

---

## Learner Home Dashboard & Activity Summary (Phase 13)

Phase 13 adds an **owner-scoped, read-only** learner home dashboard (`GET /api/v1/home/summary`)
that composes an at-a-glance summary from the learner's **own existing data only**.

- **Owner-scoped.** The learner is resolved from the authenticated user id; every underlying read
  is filtered to that learner's id. There is no new way to read another learner's data and no new
  cross-learner surface. The Phase 12 deadline query gained an owner-scoped variant
  (`GetUpcomingDeadlinesForLearnerAsync`) used here.
- **Data minimisation.** The response is aggregate/derived figures plus minimal fields the learner
  already owns (profile completeness, application/bursary status counts, pending required-document
  count, upcoming deadline name/date, recent notification type/channel/status/date). **No new PII
  surface** is introduced beyond fields already exposed by existing owner-scoped endpoints; no ID
  number, recipient address, or document contents are returned.
- **Read-only / no new audit surface.** The dashboard performs no writes and introduces no new
  per-learner audit surface — it follows the same access pattern the learner already uses per
  feature.
- **Guidance only.** The dashboard is framed as preparation/tracking guidance; it makes no
  official admissions/funding-portal claims.
- **Processors / third parties.** None. No new third-party integration; any future provider goes
  behind an interface with keys supplied via environment variables only.

---

## Privacy-by-Design Checklist (for developers)
- [ ] Is there a consent notice for new data collection?
- [ ] Is the data minimised to what is needed?
- [ ] Is access restricted by role?
- [ ] Is sensitive admin access audit logged?
- [ ] Is there a retention policy for this data?
- [ ] Can the learner delete or export this data?
- [ ] Is this document updated to reflect the change?

---

## Next Actions
- Appoint a POPIA Information Officer
- Draft full Privacy Policy (T&Cs and Privacy Notice for learners)
- Draft Guardian Consent notice for under-18 learners
- Legal review of privacy policy before public launch
- Register with the Information Regulator if required
