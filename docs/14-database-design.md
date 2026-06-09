# 14 — Database Design

## Title
FundiLink — Database Design

## Purpose
Document the planned database schema for FundiLink SmartApply MVP.

---

## Database Engine
PostgreSQL 15+
ORM: Entity Framework Core (code-first)
Naming convention: PascalCase table names, PascalCase column names (EF Core default)

---

## Core Tables

### Users (ASP.NET Identity)
Standard ASP.NET Identity table — do not modify the base schema.
Extensions via the `Learners` table linked by `UserId`.

---

### Learners
Links the Identity user to the FundiLink learner profile.

| Column | Type | Notes |
|---|---|---|
| Id | UUID | PK |
| UserId | string | FK to AspNetUsers.Id |
| FirstName | varchar(100) | |
| Surname | varchar(100) | |
| DateOfBirth | date | |
| IdNumber | varchar(13) | SA ID; encrypted at rest |
| PassportNumber | varchar(50) | If no SA ID |
| Gender | varchar(20) | Optional |
| HomeLanguage | varchar(50) | Optional |
| Nationality | varchar(50) | |
| MobileNumber | varchar(20) | |
| Province | varchar(50) | |
| Municipality | varchar(100) | |
| Suburb | varchar(100) | |
| SchoolName | varchar(200) | |
| SchoolProvince | varchar(50) | |
| GradeLevel | varchar(20) | Grade11, Grade12, PostMatric |
| GuardianName | varchar(200) | Required if under 18 |
| GuardianPhone | varchar(20) | Required if under 18 |
| GuardianEmail | varchar(200) | Required if under 18 |
| ConsentAccepted | bool | POPIA consent |
| ConsentTimestamp | timestamptz | When consent was given |
| ConsentVersion | varchar(20) | T&Cs version |
| GuardianConsentAccepted | bool | For under-18 |
| GuardianConsentTimestamp | timestamptz | |
| ProfileCompleteness | int | 0–100 percentage |
| CreatedAt | timestamptz | |
| UpdatedAt | timestamptz | |
| DeletedAt | timestamptz | Soft delete |

---

### AcademicProfiles
One per learner. Contains the learner's academic context.

| Column | Type | Notes |
|---|---|---|
| Id | UUID | PK |
| LearnerId | UUID | FK to Learners.Id |
| Year | int | Year of results |
| ResultType | varchar(20) | Grade11Prelim, Grade12Final, etc. |
| ApsScore | int | Calculated APS |
| ApsCalculatedAt | timestamptz | |
| CreatedAt | timestamptz | |
| UpdatedAt | timestamptz | |

---

### NscSubjectResults
One row per subject per academic profile.

| Column | Type | Notes |
|---|---|---|
| Id | UUID | PK |
| AcademicProfileId | UUID | FK |
| SubjectName | varchar(100) | From approved NSC subject list |
| SubjectCode | varchar(20) | NSC subject code |
| Percentage | int | 0–100 |
| ApsPoints | int | Calculated from percentage |
| IsHomeLanguage | bool | |
| IsLifeOrientation | bool | LO has special APS rules |
| CreatedAt | timestamptz | |

---

### Institutions
Universities, TVETs, private colleges.

| Column | Type | Notes |
|---|---|---|
| Id | UUID | PK |
| Name | varchar(200) | |
| ShortName | varchar(50) | E.g., UCT, Wits, UNISA |
| InstitutionType | varchar(50) | University, TVET, Private |
| Province | varchar(50) | |
| City | varchar(100) | |
| Website | varchar(500) | |
| ApplicationPortalUrl | varchar(500) | Official application URL |
| ApplicationOpenDate | date | Typical; may vary by programme |
| ApplicationCloseDate | date | Typical; may vary by programme |
| IsActive | bool | |
| CreatedAt | timestamptz | |
| UpdatedAt | timestamptz | |

---

### Programmes

| Column | Type | Notes |
|---|---|---|
| Id | UUID | PK |
| InstitutionId | UUID | FK |
| Name | varchar(200) | |
| Code | varchar(50) | Internal code |
| NqfLevel | int | 5, 6, 7, 8, 9, 10 |
| QualificationType | varchar(50) | Degree, Diploma, Certificate |
| FieldOfStudy | varchar(100) | |
| Faculty | varchar(100) | |
| MinimumAps | int | Minimum APS required |
| DurationYears | int | |
| ApplicationOpenDate | date | |
| ApplicationCloseDate | date | |
| FeesPerYear | decimal | Approximate; disclaimer needed |
| IsActive | bool | |
| CreatedAt | timestamptz | |
| UpdatedAt | timestamptz | |

---

### ProgrammeSubjectRequirements

| Column | Type | Notes |
|---|---|---|
| Id | UUID | PK |
| ProgrammeId | UUID | FK |
| SubjectName | varchar(100) | Required subject |
| MinimumPercentage | int | Minimum % required |
| IsRequired | bool | Required vs recommended |

---

### Applications
Learner applications tracked in FundiLink.

| Column | Type | Notes |
|---|---|---|
| Id | UUID | PK |
| LearnerId | UUID | FK |
| ProgrammeId | UUID | FK; nullable if custom |
| InstitutionId | UUID | FK |
| CustomProgrammeName | varchar(200) | If not in database |
| Status | varchar(50) | Draft/Submitted/UnderReview/Accepted/Rejected/Waitlisted/Deferred |
| ApplicationDate | date | |
| Deadline | date | |
| OfficialPortalUrl | varchar(500) | Where learner submits officially |
| Notes | text | Learner notes |
| CreatedAt | timestamptz | |
| UpdatedAt | timestamptz | |

---

### Documents

| Column | Type | Notes |
|---|---|---|
| Id | UUID | PK |
| LearnerId | UUID | FK |
| DocumentType | varchar(100) | ID, BirthCertificate, MatricResults, etc. |
| FileName | varchar(200) | Original filename |
| StorageKey | varchar(500) | Non-guessable storage path |
| MimeType | varchar(100) | |
| SizeBytes | long | |
| IsCertified | bool | |
| CertifiedDate | date | |
| ExpiryDate | date | Certified copies expire |
| Status | varchar(50) | Uploaded/Verified/Rejected |
| UploadedAt | timestamptz | |
| DeletedAt | timestamptz | Soft delete |

---

### Schools

| Column | Type | Notes |
|---|---|---|
| Id | UUID | PK |
| Name | varchar(200) | |
| Province | varchar(50) | |
| District | varchar(100) | |
| EmisNumber | varchar(50) | EMIS number if available |
| AdminUserId | string | FK to AspNetUsers.Id |
| IsActive | bool | |
| CreatedAt | timestamptz | |

---

### AuditLogs

| Column | Type | Notes |
|---|---|---|
| Id | UUID | PK |
| Action | varchar(100) | E.g., ViewLearnerProfile |
| PerformedByUserId | string | FK to AspNetUsers.Id |
| TargetEntityType | varchar(100) | E.g., Learner |
| TargetEntityId | varchar(100) | |
| Details | text | JSON or description |
| IPAddress | varchar(50) | |
| Timestamp | timestamptz | |

---

## Indexing Strategy (initial)
- `Learners.UserId` — unique index
- `Applications.LearnerId` — composite with status
- `NscSubjectResults.AcademicProfileId`
- `Documents.LearnerId`
- `AuditLogs.PerformedByUserId`, `AuditLogs.Timestamp`

---

## Next Actions
- Implement as EF Core entity classes in Phase 1/2
- Generate initial migration
- Seed institutions and programmes from public data
- Add indexes after profiling query patterns
