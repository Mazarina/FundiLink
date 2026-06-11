export function ConsentDisclaimerBanner({ text }: { text?: string }) {
  return (
    <div className="bg-amber-50 border border-amber-200 text-amber-800 text-xs rounded-lg p-3">
      {text ??
        "Where a learner is under 18, a parent or guardian's consent is required before FundiLink processes or shares personal information. Consent is recorded, can be withdrawn at any time, and a guardian's co-access is limited to exactly what consent permits."}
    </div>
  )
}

export function ConsentBadge({ granted }: { granted: boolean }) {
  return granted ? (
    <span className="inline-block rounded-full bg-green-100 text-green-800 text-xs px-2 py-0.5 font-medium">
      Consent granted
    </span>
  ) : (
    <span className="inline-block rounded-full bg-red-100 text-red-800 text-xs px-2 py-0.5 font-medium">
      No consent
    </span>
  )
}
