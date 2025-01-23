import type { ContentLocale, ContentType } from "@/types/contents";
import type { FieldType } from "@/types/fields";
import type { Language } from "@/types/languages";

export function formatContentLocale(contentLocale: ContentLocale): string {
  return contentLocale.displayName ? `${contentLocale.displayName} (${contentLocale.uniqueName})` : contentLocale.uniqueName;
}

export function formatContentType(contentType: ContentType): string {
  return contentType.displayName ? `${contentType.displayName} (${contentType.uniqueName})` : contentType.uniqueName;
}

export function formatFieldType(fieldType: FieldType): string {
  return fieldType.displayName ? `${fieldType.displayName} (${fieldType.uniqueName})` : fieldType.uniqueName;
}

export function formatLanguage(language: Language): string {
  return `${language.locale.displayName} | ${language.locale.code}`;
}
