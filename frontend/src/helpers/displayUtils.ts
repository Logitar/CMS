import type { ContentType } from "@/types/contentTypes";
import type { ContentItem } from "@/types/contents";
import type { FieldType } from "@/types/fieldTypes";
import type { Language } from "@/types/languages";

export function formatContentItem(contentItem: ContentItem): string {
  return contentItem.invariant.uniqueName;
}

export function formatContentType(contentType: ContentType): string {
  return contentType.displayName ? `${contentType.displayName} (${contentType.uniqueName})` : contentType.uniqueName;
}

export function formatFieldType(fieldType: FieldType): string {
  return fieldType.displayName ? `${fieldType.displayName} (${fieldType.uniqueName})` : fieldType.uniqueName;
}

export function formatLanguage(language: Language): string {
  return `${language.locale.nativeName} (${language.locale.code})`;
}
