import { useEffect, useState } from 'react'
import { Link } from 'react-router-dom'
import {
  getConsentState,
  recordConsent,
  revokeConsent,
} from '../features/consent/consentApi'
import { ConsentDisclaimerBanner, ConsentBadge } from '../features/consent/ConsentDisclaimerBanner'
import type { ConsentScope, ConsentState, ConsentType } from '../types'

const CONSENT_LABELS: Record<ConsentType, string> = {
  DataProcessing: 'Process my personal information',
  GuardianCoAccess: 'Allow my guardian read-only co-access',
  SharingWithInstitutions: 'Share my information with institutions I choose',
}

export default function ConsentPage() {
  const [state, setState] = useState<ConsentState | null>(null)
  const [error, setError] = useState('')
  const [guardianName, setGuardianName] = useState('')
  const [guardianContact, setGuardianContact] = useState('')
  const [scope, setScope] = useState<ConsentScope>('ProfileBasic')

  function load() {
    getConsentState()
      .then(setState)
      .catch(() => setError('Could not load consent settings. Please try again.'))
  }

  useEffect(load, [])

  async function handleGrant(consentType: ConsentType) {
    await recordConsent({ consentType, scope, guardianName, guardianContact })
    load()
  }

  async function handleRevoke(consentType: ConsentType) {
    await revokeConsent(consentType)
    load()
  }

  if (error) return <Centered text={error} className="text-red-600" />
  if (!state) return <Centered text="Loading consent settings..." className="text-gray-500" />

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-2xl mx-auto">
        <header className="mb-6">
          <Link to="/profile" className="text-sm text-brand-primary hover:underline">&larr; Back to profile</Link>
          <h1 className="text-2xl font-bold text-brand-primary mt-2">Guardian Consent</h1>
        </header>

        <div className="mb-4">
          <ConsentDisclaimerBanner text={state.disclaimer} />
        </div>

        {!state.isMinor && (
          <div className="bg-white rounded-xl p-5 mb-4 shadow-sm text-sm text-gray-600">
            You are 18 or older, so guardian consent is not required for your profile.
          </div>
        )}

        {state.isMinor && (
          <div className="bg-white rounded-xl p-5 mb-4 shadow-sm">
            <h2 className="font-semibold text-gray-800 mb-3">Guardian details &amp; co-access scope</h2>
            <label className="block text-sm text-gray-600 mb-1" htmlFor="guardianName">Guardian name</label>
            <input
              id="guardianName"
              className="w-full border rounded-lg p-2 mb-3 text-sm"
              value={guardianName}
              onChange={(e) => setGuardianName(e.target.value)}
            />
            <label className="block text-sm text-gray-600 mb-1" htmlFor="guardianContact">Guardian contact</label>
            <input
              id="guardianContact"
              className="w-full border rounded-lg p-2 mb-3 text-sm"
              value={guardianContact}
              onChange={(e) => setGuardianContact(e.target.value)}
            />
            <label className="block text-sm text-gray-600 mb-1" htmlFor="scope">Co-access scope</label>
            <select
              id="scope"
              className="w-full border rounded-lg p-2 text-sm"
              value={scope}
              onChange={(e) => setScope(e.target.value as ConsentScope)}
            >
              <option value="ProfileBasic">Basic profile only</option>
              <option value="ProfileAndApplications">Profile and application tracking</option>
            </select>
          </div>
        )}

        <div className="bg-white rounded-xl p-5 shadow-sm">
          <h2 className="font-semibold text-gray-800 mb-3">Consent settings</h2>
          <ul className="space-y-3">
            {state.consents.map((c) => (
              <li key={c.consentType} className="flex items-center justify-between gap-3">
                <div>
                  <p className="text-sm text-gray-800">{CONSENT_LABELS[c.consentType]}</p>
                  <ConsentBadge granted={c.isGranted} />
                </div>
                {c.isGranted ? (
                  <button
                    aria-label={`Revoke ${c.consentType}`}
                    onClick={() => handleRevoke(c.consentType)}
                    className="text-sm text-red-600 hover:underline"
                    disabled={c.consentType === 'GuardianCoAccess' && !state.isMinor}
                  >
                    Revoke
                  </button>
                ) : (
                  <button
                    aria-label={`Grant ${c.consentType}`}
                    onClick={() => handleGrant(c.consentType)}
                    className="text-sm text-brand-primary hover:underline"
                    disabled={c.consentType === 'GuardianCoAccess' && !state.isMinor}
                  >
                    Grant
                  </button>
                )}
              </li>
            ))}
          </ul>
        </div>
      </div>
    </main>
  )
}

function Centered({ text, className }: { text: string; className: string }) {
  return (
    <main className="min-h-screen bg-brand-light flex items-center justify-center p-4">
      <p className={className}>{text}</p>
    </main>
  )
}
