import { Link } from 'react-router-dom'

function PrivacyPolicyPage() {
  return (
    <main className="min-h-screen bg-brand-light p-6">
      <div className="max-w-3xl mx-auto bg-white rounded-lg shadow-sm p-6 sm:p-10">
        <Link to="/" className="text-sm text-brand-primary font-semibold hover:underline">
          &larr; Back to FundiLink
        </Link>

        <h1 className="text-3xl font-bold text-brand-primary mt-4 mb-1">Privacy Policy</h1>
        <p className="text-sm text-gray-500 mb-6">Last updated: June 2026</p>

        <div className="prose prose-sm max-w-none text-gray-700 space-y-4">
          <p className="bg-yellow-50 border border-yellow-200 text-yellow-800 rounded-md p-3 text-sm">
            This Privacy Policy is an MVP template provided for transparency to early users of
            FundiLink SmartApply. It is not a substitute for formal legal advice and must be
            reviewed by a qualified legal professional before any large-scale public launch.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">1. Who we are</h2>
          <p>
            FundiLink by ZulTek ("FundiLink", "we", "us") is operated by Nhloso Ndlovu / ZulTek.
            FundiLink SmartApply helps South African learners create a unified education profile,
            understand what programmes and funding they may qualify for, prepare supporting
            documents, and track applications.
          </p>
          <p>
            <strong>FundiLink is not an official government, university, TVET, NSFAS, or bursary
            admissions platform.</strong> We help learners prepare, organise, understand, and
            track applications. Final submissions happen through the official institution or
            funder portals unless a documented, formal integration exists.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">2. Information we collect</h2>
          <ul className="list-disc pl-5 space-y-1">
            <li><strong>Account information:</strong> name, email address, and a securely hashed password.</li>
            <li><strong>Profile information:</strong> contact details, school, grade, province, and (where provided) ID/passport number.</li>
            <li><strong>Academic information:</strong> subjects, marks, and APS-related data you enter.</li>
            <li><strong>Application information:</strong> programmes, bursaries, accommodation and career opportunities you express interest in or track.</li>
            <li><strong>Documents:</strong> files you upload (e.g. ID copy, certificates, proof of income) for your own use in preparing applications.</li>
            <li><strong>Guardian information:</strong> where you are under 18, your guardian's name and contact details and consent records.</li>
            <li><strong>Usage information:</strong> login activity and actions taken in the platform, used for security and audit purposes.</li>
          </ul>

          <h2 className="text-xl font-semibold text-gray-900">3. How we use your information</h2>
          <p>We use your information to:</p>
          <ul className="list-disc pl-5 space-y-1">
            <li>Provide your FundiLink account and education profile;</li>
            <li>Calculate APS scores and match you to programmes, bursaries, accommodation and career opportunities you may qualify for;</li>
            <li>Help you prepare, organise and track supporting documents and applications;</li>
            <li>Send notifications and reminders about deadlines you are tracking, where you have opted in;</li>
            <li>Maintain the security, integrity and auditability of the platform; and</li>
            <li>Comply with our legal obligations.</li>
          </ul>

          <h2 className="text-xl font-semibold text-gray-900">4. Access and storage</h2>
          <p>
            Access to your personal information is restricted by role — staff only see what they
            need to perform their role, and sensitive admin access is recorded in an append-only
            audit log. Uploaded documents are stored securely and are not publicly accessible;
            only you (and authorised staff where required for verification) can access them.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">5. Consent</h2>
          <p>
            We ask for your explicit consent at registration before processing your personal
            information. If you are under 18, we require your guardian's consent before
            processing certain categories of data or enabling guardian co-access. You can review
            and manage consent from your account's "Consent" page, and you may withdraw consent
            at any time, which may limit or end certain features.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">6. Retention</h2>
          <p>
            We retain your information only for as long as necessary to provide the service, or
            as required by law (for example, audit logs are retained for a minimum period for
            legal compliance). Documents are retained until you delete them or your account is
            erased.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">7. Your rights</h2>
          <p>
            Under the Protection of Personal Information Act (POPIA), you have the right to
            access, correct, and request deletion ("erasure") of your personal information.
            You can download a copy of your data and submit an erasure request from the
            "My data &amp; privacy" page in your account. Erasure requests are reviewed and
            actioned by an authorised administrator and recorded in our audit log.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">8. Sharing of information</h2>
          <p>
            We do not sell or share your personal information with third parties without your
            consent and a valid legal basis. Should we introduce a new third-party integration
            (for example, a notification provider), we will document this and update this policy
            before it goes live.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">9. Security</h2>
          <p>
            We use industry-standard measures to protect your information, including encrypted
            connections (HTTPS), role-based access control, and audit logging of sensitive
            actions. No system is completely secure, and we encourage you to use a strong,
            unique password for your account.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">10. Your responsibilities</h2>
          <p>
            Please ensure the information you provide is accurate and that you keep your login
            details confidential. You are responsible for the documents you upload and for
            checking the official institution or funder portal for the authoritative status of
            any application.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">11. Limitation of liability</h2>
          <p>
            FundiLink provides preparation, organisation and tracking tools "as is" and does not
            guarantee admission, funding, accommodation or employment outcomes. We are not liable
            for decisions made by institutions, funders, or other third parties.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">12. Contact us</h2>
          <p>
            If you have questions about this Privacy Policy, or wish to exercise your rights
            under POPIA, please contact us at{' '}
            <a className="text-brand-primary font-semibold hover:underline" href="mailto:nhlsndlv@gmail.com">
              nhlsndlv@gmail.com
            </a>.
          </p>
        </div>
      </div>
    </main>
  )
}

export default PrivacyPolicyPage
