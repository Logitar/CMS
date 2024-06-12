import type { Actor } from "./actor";
import type { Aggregate } from "./aggregate";
import type { Language } from "./localization";
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

export type ContentType = Aggregate & {
  isInvariant: boolean;
  uniqueName: string;
  displayName?: string;
  description?: string;
};

export type ContentTypeSort = "DisplayName" | "UniqueName" | "UpdatedOn";

export type ContentTypeSortOption = SortOption & {
  field: ContentTypeSort;
};

export type CreateContentPayload = {
  contentTypeId: string;
  languageId?: string;
  uniqueName: string;
};

export type CreateContentTypePayload = {
  isInvariant: boolean;
  uniqueName: string;
  displayName?: string;
  description?: string;
};

export type ReplaceContentTypePayload = {
  uniqueName: string;
  displayName?: string;
  description?: string;
};

export type SearchContentItemsPayload = SearchPayload & {
  // TODO(fpion): implement
};

export type SearchContentTypesPayload = SearchPayload & {
  isInvariant?: boolean;
  sort?: ContentTypeSortOption[];
};
