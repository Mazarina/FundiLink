import { render, screen, fireEvent, waitFor } from '@testing-library/react'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import NotificationPreferencesPage from './NotificationPreferencesPage'
import * as api from '../features/notifications/notificationsApi'

vi.mock('../features/notifications/notificationsApi')

const mockedApi = vi.mocked(api)

describe('NotificationPreferencesPage', () => {
  beforeEach(() => {
    vi.clearAllMocks()
    mockedApi.getNotificationPreferences.mockResolvedValue({
      emailEnabled: true,
      whatsAppEnabled: false,
      smsEnabled: false,
    })
    mockedApi.updateNotificationPreferences.mockResolvedValue(undefined)
  })

  it('renders with initial preferences loaded', async () => {
    render(<NotificationPreferencesPage />)
    const email = (await screen.findByLabelText('Email notifications')) as HTMLInputElement
    const whatsApp = screen.getByLabelText('WhatsApp notifications') as HTMLInputElement
    expect(email.checked).toBe(true)
    expect(whatsApp.checked).toBe(false)
  })

  it('toggling a checkbox changes its checked state', async () => {
    render(<NotificationPreferencesPage />)
    const whatsApp = (await screen.findByLabelText('WhatsApp notifications')) as HTMLInputElement
    expect(whatsApp.checked).toBe(false)
    fireEvent.click(whatsApp)
    expect(whatsApp.checked).toBe(true)
  })

  it('saves preferences and shows success message', async () => {
    render(<NotificationPreferencesPage />)
    const save = await screen.findByText('Save preferences')
    fireEvent.click(save)
    await waitFor(() => expect(mockedApi.updateNotificationPreferences).toHaveBeenCalled())
    expect(await screen.findByText(/preferences have been saved/i)).toBeTruthy()
  })

  it('shows the coming soon note for WhatsApp and SMS', async () => {
    render(<NotificationPreferencesPage />)
    expect(await screen.findByText(/coming soon/i)).toBeTruthy()
  })
})
