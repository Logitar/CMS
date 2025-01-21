import type { Aggregate } from "./aggregate";
import type { FieldDefinition } from "./fields";
import type { SearchPayload, SortOption } from "./search";

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

export type CreateOrReplaceContentTypePayload = {
  isInvariant: boolean;
  uniqueName: string;
  displayName?: string;
  description?: string;
};

export type SearchContentTypesPayload = SearchPayload & {
  isInvariant?: boolean;
  sort: ContentTypeSortOption[];
};
