import type { Aggregate } from "./aggregate";
import type { SearchPayload, SortOption } from "./search";

export type BooleanProperties = {};

export type ContentType = "text/plain";

export type CreateFieldTypePayload = {
  uniqueName: string;
  displayName?: string;
  description?: string;
  booleanProperties?: BooleanProperties;
  dateTimeProperties?: DateTimeProperties;
  numberProperties?: NumberProperties;
  stringProperties?: StringProperties;
  textProperties?: TextProperties;
};

export type DataType = "Boolean" | "DateTime" | "Number" | "String" | "Text";

export type DateTimeProperties = {
  minimumValue?: string;
  maximumValue?: string;
};

export type FieldType = Aggregate & {
  uniqueName: string;
  displayName?: string;
  description?: string;
  dataType: DataType;
  booleanProperties?: BooleanProperties;
  dateTimeProperties?: DateTimeProperties;
  numberProperties?: NumberProperties;
  stringProperties?: StringProperties;
  textProperties?: TextProperties;
};

export type FieldTypeSort = "DisplayName" | "UniqueName" | "UpdatedOn";

export type FieldTypeSortOption = SortOption & {
  field: FieldTypeSort;
};

export type NumberProperties = {
  minimumValue?: number;
  maximumValue?: number;
  step?: number;
};

export type ReplaceFieldTypePayload = {
  uniqueName: string;
  displayName?: string;
  description?: string;
  booleanProperties?: BooleanProperties;
  dateTimeProperties?: DateTimeProperties;
  numberProperties?: NumberProperties;
  stringProperties?: StringProperties;
  textProperties?: TextProperties;
};

export type SearchFieldTypesPayload = SearchPayload & {
  dataType?: DataType;
  sort?: FieldTypeSortOption[];
};

export type StringProperties = {
  minimumLength?: number;
  maximumLength?: number;
  pattern?: string;
};

export type TextProperties = {
  contentType: ContentType;
  minimumLength?: number;
  maximumLength?: number;
};
