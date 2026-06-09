# 16 — UI/UX Screen List

## Title
FundiLink — UI/UX Screen List

## Purpose
Define all screens required for the FundiLink SmartApply MVP frontend.

---

## Design Principles
- Mobile-first (320px minimum width)
- Plain language — Grade 9 reading level target
- Accessible — WCAG 2.1 AA minimum
- Low-bandwidth friendly — minimal images, optimised assets
- Encouraging tone — celebrate progress

---

## Public Screens (No Auth Required)

| Screen | Route | Description |
|---|---|---|
| Landing Page | `/` | Hero, features, how it works, CTA |
| Register | `/register` | Create account form |
| Email Verification | `/verify-email` | Confirm email sent; verify link |
| Login | `/login` | Email + password |
| Forgot Password | `/forgot-password` | Request reset email |
| Reset Password | `/reset-password` | Enter new password via token |
| Privacy Policy | `/privacy` | POPIA-compliant privacy policy |
| Terms of Use | `/terms` | Terms and conditions |

---

## Learner Screens (Student Role)

### Onboarding
| Screen | Route | Description |
|---|---|---|
| Welcome / Onboarding Intro | `/welcome` | First-login welcome; guide to complete profile |
| POPIA Consent (in-flow) | Part of registration | Plain-language consent |

### Profile
| Screen | Route | Description |
|---|---|---|
| My Profile | `/profile` | View and edit personal information |
| Edit Personal Info | `/profile/personal` | Name, DOB, ID, contact, address |
| Edit School Info | `/profile/school` | School name, grade, province |
| Guardian Details | `/profile/guardian` | For under-18 learners |
| Profile Completeness | Part of profile | Progress indicator |

### Academic Profile and APS
| Screen | Route | Description |
|---|---|---|
| Academic Profile | `/academic` | View academic profile and APS |
| Enter NSC Results | `/academic/results` | Subject selector and mark entry |
| APS Score Display | `/academic/aps` | APS score, breakdown, explanation |

### Programme Discovery
| Screen | Route | Description |
|---|---|---|
| Programme Matches | `/programmes` | Matched programmes ranked by strength |
| Programme Search | `/programmes/search` | Search and filter all programmes |
| Programme Detail | `/programmes/:id` | Full programme info, requirements, CTA |

### Applications
| Screen | Route | Description |
|---|---|---|
| Application Tracker | `/applications` | List of all tracked applications |
| Application Detail | `/applications/:id` | Status, documents, notes, deadline |
| Add Application | `/applications/new` | Add institution + programme to tracker |

### Documents
| Screen | Route | Description |
|---|---|---|
| Document Vault | `/documents` | List of all uploaded documents |
| Upload Document | `/documents/upload` | Upload + categorise document |
| Document Detail | `/documents/:id` | View, download, delete |

### Notifications
| Screen | Route | Description |
|---|---|---|
| Notifications | `/notifications` | List of all notifications |
| Notification Preferences | `/settings/notifications` | Choose channels and preferences |

### Settings
| Screen | Route | Description |
|---|---|---|
| Account Settings | `/settings` | Email, password, notification prefs |
| Change Password | `/settings/password` | |
| Delete Account | `/settings/delete` | POPIA data deletion |

---

## Admin / Support Agent Screens

| Screen | Route | Description |
|---|---|---|
| Admin Dashboard | `/admin` | Overview metrics |
| Learner Search | `/admin/learners` | Search learners |
| Learner Profile View | `/admin/learners/:id` | Read-only with support notes |
| Add Support Note | `/admin/learners/:id/notes` | Internal notes |
| Institution List | `/admin/institutions` | Manage institutions |
| Institution Detail | `/admin/institutions/:id` | Edit |
| Programme List | `/admin/programmes` | Manage programmes |
| User Management | `/admin/users` | Manage user accounts and roles |
| Audit Log | `/admin/audit` | SuperAdmin only |

---

## School Admin Screens

| Screen | Route | Description |
|---|---|---|
| School Dashboard | `/school` | Overview of school's learners |
| Learner List | `/school/learners` | All learners at the school |
| Learner Progress | `/school/learners/:id` | Aggregate progress view |

---

## Shared Components (Not Screens but Key)
- Navigation bar (mobile: bottom nav; desktop: side nav)
- Notification bell
- Progress indicator / stepper
- Empty state illustrations
- Error / success toast messages
- Confirmation modals
- Document upload widget
- APS score badge

---

## Next Actions
- Create low-fidelity wireframes for core learner journey screens
- Define component library based on screen list
- Prioritise screens for Phase 2 build
- Validate screen list with user research
