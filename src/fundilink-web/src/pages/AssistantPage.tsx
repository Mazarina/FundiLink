import { useState } from 'react'
import { Link } from 'react-router-dom'
import { askAssistant, ASSISTANT_INTENTS } from '../features/assistant/assistantApi'
import { AssistantDisclaimerBanner } from '../features/assistant/AssistantDisclaimerBanner'
import type { AssistantIntent, AssistantResponse } from '../types'

export default function AssistantPage() {
  const [response, setResponse] = useState<AssistantResponse | null>(null)
  const [loading, setLoading] = useState(false)
  const [error, setError] = useState('')
  const [activeIntent, setActiveIntent] = useState<AssistantIntent | null>(null)

  async function handleAsk(intent: AssistantIntent) {
    setActiveIntent(intent)
    setLoading(true)
    setError('')
    setResponse(null)
    try {
      const result = await askAssistant(intent)
      setResponse(result)
    } catch {
      setError('Sorry, the assistant could not answer right now. Please try again.')
    } finally {
      setLoading(false)
    }
  }

  return (
    <main className="min-h-screen bg-brand-light p-4">
      <div className="max-w-2xl mx-auto space-y-4">
        <header className="flex justify-between items-center">
          <div>
            <h1 className="text-2xl font-bold text-brand-primary">Ask FundiLink</h1>
            <p className="text-gray-500 text-sm">Guidance based on your own profile</p>
          </div>
          <Link to="/profile" className="text-sm text-gray-500 hover:underline">Back to profile</Link>
        </header>

        <AssistantDisclaimerBanner />

        <div className="bg-white rounded-xl p-5 shadow-sm">
          <p className="text-sm text-gray-700 mb-3">Choose a question:</p>
          <div className="flex flex-wrap gap-2">
            {ASSISTANT_INTENTS.map((option) => (
              <button
                key={option.intent}
                onClick={() => handleAsk(option.intent)}
                disabled={loading}
                className={`text-sm px-3 py-2 rounded-full border transition disabled:opacity-50 ${
                  activeIntent === option.intent
                    ? 'bg-brand-primary text-white border-brand-primary'
                    : 'bg-white text-brand-primary border-brand-primary hover:bg-brand-light'
                }`}
              >
                {option.label}
              </button>
            ))}
          </div>
        </div>

        {error && <p className="text-red-600 text-sm">{error}</p>}

        {loading && <p className="text-gray-500 text-center py-6">Thinking...</p>}

        {response && !loading && (
          <div className="bg-white rounded-xl p-5 shadow-sm space-y-3">
            <p className="text-gray-800 whitespace-pre-line">{response.answer}</p>

            {response.sources.length > 0 && (
              <div className="text-xs text-gray-500">
                <p className="font-semibold">Based on:</p>
                <ul className="list-disc list-inside">
                  {response.sources.map((source) => <li key={source}>{source}</li>)}
                </ul>
              </div>
            )}

            <p className="text-xs text-amber-700 border-t border-gray-100 pt-3">{response.disclaimer}</p>
          </div>
        )}

        <div className="bg-white rounded-xl p-4 shadow-sm text-sm text-gray-600">
          Need more help? <Link to="/notifications/preferences" className="text-brand-primary hover:underline">
            Talk to a FundiLink support agent
          </Link>.
        </div>
      </div>
    </main>
  )
}
