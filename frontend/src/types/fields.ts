import type { Aggregate } from "./aggregate";
import type { SearchPayload, SortOption } from "./search";

export type BooleanProperties = {};

export type CreateOrReplaceFieldDefinitionPayload = {
  fieldTypeId?: string;
  isInvariant: boolean;
  isRequired: boolean;
  isIndexed: boolean;
  isUnique: boolean;
  uniqueName: string;
  displayName?: string;
  description?: string;
  placeholder?: string;
};

export type CreateOrReplaceFieldTypePayload = {
  uniqueName: string;
  displayName?: string;
  description?: string;
  boolean?: BooleanProperties;
  dateTime?: DateTimeProperties;
  number?: NumberProperties;
  relatedContent?: RelatedContentProperties;
  richText?: RichTextProperties;
  select?: SelectProperties;
  string?: StringProperties;
  tags?: TagsProperties;
};

export type DataType = "String" | "RichText" | "Boolean" | "Number" | "DateTime" | "Select" | "Tags" | "RelatedContent";

export type DateTimeProperties = {
  minimumValue?: string;
  maximumValue?: string;
};

export type FieldDefinition = {
  id: string;
  order: number;
  fieldType: FieldType;
  isInvariant: boolean;
  isRequired: boolean;
  isIndexed: boolean;
  isUnique: boolean;
  uniqueName: string;
  displayName?: string;
  description?: string;
  placeholder?: string;
};

export type FieldType = Aggregate & {
  uniqueName: string;
  displayName?: string;
  description?: string;
  dataType: DataType;
  boolean?: BooleanProperties;
  dateTime?: DateTimeProperties;
  number?: NumberProperties;
  relatedContent?: RelatedContentProperties;
  richText?: RichTextProperties;
  select?: SelectProperties;
  string?: StringProperties;
  tags?: TagsProperties;
};

export type FieldTypeSort = "CreatedOn" | "DisplayName" | "UniqueName" | "UpdatedOn";

export type FieldTypeSortOption = SortOption & {
  field: FieldTypeSort;
};

export type NumberProperties = {
  minimumValue?: number;
  maximumValue?: number;
  step?: number;
};

export type RelatedContentProperties = {
  contentTypeId: string;
  isMultiple: boolean;
};

export type RichTextProperties = {
  contentType: string;
  minimumLength?: number;
  maximumLength?: number;
};

export type SearchFieldTypesPayload = SearchPayload & {
  dataType?: DataType;
  sort: FieldTypeSortOption[];
};

export type SelectOption = {
  isDisabled: boolean;
  isSelected: boolean;
  text: string;
  label?: string;
  value?: string;
};

export type SelectProperties = {
  isMultiple: boolean;
  options: SelectOption[];
};

export type StringProperties = {
  minimumLength?: number;
  maximumLength?: number;
  pattern?: string;
};

export type TagsProperties = {};
