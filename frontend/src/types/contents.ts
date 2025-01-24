import type { Actor } from "./actor";
import type { Aggregate } from "./aggregate";
import type { FieldDefinition, FieldValue } from "./fields";
import type { Language } from "./languages";
import type { SearchPayload, SortOption } from "./search";

export type Content = Aggregate & {
  contentType: ContentType;
  invariant: ContentLocale;
  locales: ContentLocale[];
};

export type ContentLocale = {
  content: Content;
  language?: Language;
  uniqueName: string;
  displayName?: string;
  description?: string;
  fieldValues: FieldValue[];
  createdBy: Actor;
  createdOn: string;
  updatedBy: Actor;
  updatedOn: string;
};

export type ContentSort = "CreatedOn" | "DisplayName" | "UniqueName" | "UpdatedOn";

export type ContentSortOption = SortOption & {
  field: ContentSort;
};

export type ContentType = Aggregate & {
  isInvariant: boolean;
  uniqueName: string;
  displayName?: string;
  description?: string;
  fieldCount: number;
  fields: FieldDefinition[];
};

export type ContentTypeSort = "CreatedOn" | "DisplayName" | "UniqueName" | "UpdatedOn";

export type ContentTypeSortOption = SortOption & {
  field: ContentTypeSort;
};

export type CreateOrReplaceContentPayload = {
  contentTypeId?: string;
  uniqueName: string;
  displayName?: string;
  description?: string;
  fieldValues: FieldValue[];
};

export type CreateOrReplaceContentTypePayload = {
  isInvariant: boolean;
  uniqueName: string;
  displayName?: string;
  description?: string;
};

export type SearchContentsPayload = SearchPayload & {
  contentTypeId?: string;
  languageId?: string;
  sort: ContentSortOption[];
};

export type SearchContentTypesPayload = SearchPayload & {
  isInvariant?: boolean;
  sort: ContentTypeSortOption[];
};
