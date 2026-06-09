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
- Learners can request full account and data deletion
- Request triggers a soft-delete, then queued physical deletion within 30 days
- Deletion logged in audit trail
- Anonymised aggregate data (e.g., APS statistics) may be retained as it is not personal information
- Learner notified when deletion is complete

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
