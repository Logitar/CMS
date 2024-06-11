import type { FieldType } from "@/types/fields";
import type { Language } from "@/types/languages";

export function formatFieldType(fieldType: FieldType): string {
  return fieldType.displayName ? `${fieldType.displayName} (${fieldType.uniqueName})` : fieldType.uniqueName;
}

export function formatLanguage(language: Language): string {
  return `${language.locale.nativeName} (${language.locale.code})`;
}
