/** Converts a PascalCase enum value (e.g. "TuitionOnly") into a readable label ("Tuition Only"). */
export function humanizeEnum(value: string): string {
  return value.replace(/([A-Z])/g, ' $1').trim().replace(/\bId\b/g, 'ID')
}

const GRADE_LEVEL_LABELS: Record<string, string> = {
  Grade11: 'Grade 11',
  Grade12: 'Grade 12',
  PostMatric: 'Post-Matric',
}

/** Converts a GradeLevel enum value (e.g. "Grade12") into a readable label ("Grade 12"). */
export function gradeLevelLabel(value: string): string {
  return GRADE_LEVEL_LABELS[value] ?? value
}
