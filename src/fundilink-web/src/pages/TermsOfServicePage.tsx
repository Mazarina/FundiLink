import { Link } from 'react-router-dom'

function TermsOfServicePage() {
  return (
    <main className="min-h-screen bg-brand-light p-6">
      <div className="max-w-3xl mx-auto bg-white rounded-lg shadow-sm p-6 sm:p-10">
        <Link to="/" className="text-sm text-brand-primary font-semibold hover:underline">
          &larr; Back to FundiLink
        </Link>

        <h1 className="text-3xl font-bold text-brand-primary mt-4 mb-1">Terms of Service</h1>
        <p className="text-sm text-gray-500 mb-6">Last updated: June 2026</p>

        <div className="prose prose-sm max-w-none text-gray-700 space-y-4">
          <p className="bg-yellow-50 border border-yellow-200 text-yellow-800 rounded-md p-3 text-sm">
            These Terms of Service are an MVP template provided for transparency to early users of
            FundiLink SmartApply. They are not a substitute for formal legal advice and must be
            reviewed by a qualified legal professional before any large-scale public launch.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">1. About FundiLink</h2>
          <p>
            FundiLink by ZulTek ("FundiLink", "we", "us") is operated by Nhloso Ndlovu / ZulTek.
            FundiLink SmartApply ("the Service") helps South African learners create a unified
            education profile, calculate APS scores, understand what programmes and funding they
            may qualify for, prepare supporting documents, and track applications to
            universities, TVETs, bursaries, accommodation and skills/career opportunities.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">2. Not an official admissions platform</h2>
          <p>
            <strong>FundiLink is not an official government, university, TVET, NSFAS, or bursary
            admissions platform</strong> unless a formal partnership is documented and
            implemented. The Service helps you prepare, organise, understand, and track
            applications. You are responsible for submitting final applications through the
            relevant official institution or funder portal, and for verifying the status of any
            application directly with that institution or funder.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">3. Eligibility and accounts</h2>
          <p>
            You must provide accurate information when creating your account. If you are under
            18, your guardian's consent is required for certain features, as described in our
            Privacy Policy. You are responsible for keeping your login credentials confidential
            and for all activity under your account.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">4. Acceptable use</h2>
          <p>You agree not to:</p>
          <ul className="list-disc pl-5 space-y-1">
            <li>Provide false or misleading information about yourself or any third party;</li>
            <li>Upload documents that are not your own or that you are not authorised to share;</li>
            <li>Use the Service to harass, defraud, or impersonate any person or institution;</li>
            <li>Attempt to gain unauthorised access to other users' accounts or data; or</li>
            <li>Use the Service in any way that breaches applicable South African law, including POPIA.</li>
          </ul>

          <h2 className="text-xl font-semibold text-gray-900">5. Your content and documents</h2>
          <p>
            You retain ownership of the information and documents you upload. By uploading a
            document, you confirm you have the right to do so. We store your documents securely
            and do not make them publicly accessible. You may delete or replace your documents at
            any time from the Documents section of your account.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">6. No guarantee of outcomes</h2>
          <p>
            FundiLink provides preparation, organisation, matching and tracking tools "as is" and
            "as available". We do not guarantee admission to any programme, receipt of any
            bursary or funding, accommodation placement, or employment/career outcomes. Decisions
            remain entirely with the relevant institutions, funders, and other third parties.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">7. Availability and changes</h2>
          <p>
            We may update, suspend, or discontinue features of the Service at any time. We will
            make reasonable efforts to maintain availability, but do not guarantee uninterrupted
            access.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">8. Limitation of liability</h2>
          <p>
            To the maximum extent permitted by law, FundiLink and ZulTek shall not be liable for
            any indirect, incidental, or consequential loss arising from your use of the Service,
            including missed deadlines, application outcomes, or reliance on information provided
            through the Service. You remain responsible for verifying all deadlines and
            requirements directly with the relevant institution or funder.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">9. Privacy</h2>
          <p>
            Our collection and use of your personal information is described in our{' '}
            <Link to="/privacy" className="text-brand-primary font-semibold hover:underline">
              Privacy Policy
            </Link>, which forms part of these Terms.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">10. Termination</h2>
          <p>
            You may stop using the Service and request deletion of your account and data at any
            time via "My data &amp; privacy". We may suspend or terminate accounts that breach
            these Terms.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">11. Governing law</h2>
          <p>
            These Terms are governed by the laws of the Republic of South Africa.
          </p>

          <h2 className="text-xl font-semibold text-gray-900">12. Contact us</h2>
          <p>
            If you have questions about these Terms, please contact us at{' '}
            <a className="text-brand-primary font-semibold hover:underline" href="mailto:nhlsndlv@gmail.com">
              nhlsndlv@gmail.com
            </a>.
          </p>
        </div>
      </div>
    </main>
  )
}

export default TermsOfServicePage
