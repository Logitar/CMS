import type { Aggregate } from "./aggregate";
import type { SearchPayload, SortOption } from "./search";

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

export type SearchContentTypesPayload = SearchPayload & {
  isInvariant?: boolean;
  sort?: ContentTypeSortOption[];
};
