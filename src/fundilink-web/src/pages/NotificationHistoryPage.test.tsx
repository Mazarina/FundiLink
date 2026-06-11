import { render, screen } from '@testing-library/react'
import { describe, it, expect, vi, beforeEach } from 'vitest'
import NotificationHistoryPage from './NotificationHistoryPage'
import * as api from '../features/notifications/notificationsApi'
import type { NotificationLogEntry } from '../types'

vi.mock('../features/notifications/notificationsApi')

const mockedApi = vi.mocked(api)

describe('NotificationHistoryPage', () => {
  beforeEach(() => vi.clearAllMocks())

  it('renders notification history entries', async () => {
    const items: NotificationLogEntry[] = [
      {
        id: '1',
        notificationType: 'DeadlineReminder',
        channel: 'Email',
        status: 'Sent',
        sentAt: '2026-06-10T09:00:00Z',
        errorMessage: null,
      },
    ]
    mockedApi.getNotificationHistory.mockResolvedValue(items)

    render(<NotificationHistoryPage />)

    expect(await screen.findByText('Deadline reminder')).toBeTruthy()
    expect(screen.getByText('Sent')).toBeTruthy()
  })

  it('renders the empty state when there are no notifications', async () => {
    mockedApi.getNotificationHistory.mockResolvedValue([])

    render(<NotificationHistoryPage />)

    expect(await screen.findByText('You have no notifications yet.')).toBeTruthy()
  })

  it('shows an error message when loading fails', async () => {
    mockedApi.getNotificationHistory.mockRejectedValue(new Error('boom'))

    render(<NotificationHistoryPage />)

    expect(await screen.findByText('Could not load your notification history.')).toBeTruthy()
  })
})
