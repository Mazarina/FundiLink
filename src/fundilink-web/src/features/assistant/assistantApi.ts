import client from '../../api/client'
import type { AssistantIntent, AssistantResponse } from '../../types'

export interface AssistantIntentOption {
  intent: AssistantIntent
  label: string
}

// The constrained set of questions the assistant can answer (matches the backend enum).
export const ASSISTANT_INTENTS: AssistantIntentOption[] = [
  { intent: 'WhatIsMyAps', label: 'What is my APS?' },
  { intent: 'WhatDoIQualifyFor', label: 'What do I qualify for?' },
  { intent: 'WhichBursariesFitMe', label: 'Which bursaries may fit me?' },
  { intent: 'WhatDocumentsDoINeed', label: 'What documents do I still need?' },
]

export async function askAssistant(intent: AssistantIntent): Promise<AssistantResponse> {
  const res = await client.post('/assistant/ask', { intent })
  return res.data
}
