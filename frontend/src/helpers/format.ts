import type { ContentType } from "@/types/contents";
import type { Language } from "@/types/languages";

export function formatContentType(contentType: ContentType): string {
  return contentType.displayName ? `${contentType.displayName} (${contentType.uniqueName})` : contentType.uniqueName;
}

export function formatLanguage(language: Language): string {
  return `${language.locale.displayName} | ${language.locale.code}`;
}
