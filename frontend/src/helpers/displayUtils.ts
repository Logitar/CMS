import type { FieldType } from "@/types/fields";

export function formatFieldType(fieldType: FieldType): string {
  return fieldType.displayName ? `${fieldType.displayName} (${fieldType.uniqueName})` : fieldType.uniqueName;
}
