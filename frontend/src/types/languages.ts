import type { Aggregate } from "./aggregate";
import type { Locale } from "./i18n";
import type { SearchPayload, SortOption } from "./search";

export type CreateLanguagePayload = {
  locale: string;
};

export type Language = Aggregate & {
  isDefault: boolean;
  locale: Locale;
};

export type LanguageSort = "Code" | "DisplayName" | "EnglishName" | "NativeName" | "UpdatedOn";

export type LanguageSortOption = SortOption & {
  field: LanguageSort;
};

export type SearchLanguagesPayload = SearchPayload & {
  sort?: LanguageSortOption[];
};
