import type { Actor } from "./actor";
import type { Aggregate } from "./aggregate";
import type { ContentType } from "./contentTypes";
import type { Language } from "./languages";
import type { SearchPayload, SortOption } from "./search";

export type ContentItem = Aggregate & {
  contentType: ContentType;
  invariant: ContentLocale;
  locales: ContentLocale[];
};

export type ContentLocale = {
  uniqueName: string;
  item: ContentItem;
  language?: Language;
  createdBy: Actor;
  createdOn: string;
  updatedBy: Actor;
  updatedOn: string;
};

export type ContentSort = "UniqueName" | "UpdatedOn";

export type ContentSortOption = SortOption & {
  field: ContentSort;
};

export type CreateContentPayload = {
  contentTypeId: string;
  languageId?: string;
  uniqueName: string;
};

export type SearchContentsPayload = SearchPayload & {
  contentTypeId?: string;
  languageId?: string;
  sort?: ContentSortOption[];
};
