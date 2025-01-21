import type { DateTimeProperties, NumberProperties, RelatedContentProperties, RichTextProperties, SelectProperties, StringProperties } from "@/types/fields";

export function compareDateTimeProperties(left: DateTimeProperties | undefined, right: DateTimeProperties | undefined): boolean {
  if (!left || !right) {
    return !left && !right;
  }
  return (left.minimumValue ?? undefined) === (right.minimumValue ?? undefined) && (left.maximumValue ?? undefined) === (right.maximumValue ?? undefined);
}

export function compareNumberProperties(left: NumberProperties | undefined, right: NumberProperties | undefined): boolean {
  if (!left || !right) {
    return !left && !right;
  }
  return (
    (left.minimumValue ?? undefined) === (right.minimumValue ?? undefined) &&
    (left.maximumValue ?? undefined) === (right.maximumValue ?? undefined) &&
    (left.step ?? undefined) === (right.step ?? undefined)
  );
}

export function compareRelatedContentProperties(left: RelatedContentProperties | undefined, right: RelatedContentProperties | undefined): boolean {
  if (!left || !right) {
    return !left && !right;
  }
  return left.contentTypeId === right.contentTypeId && left.isMultiple === right.isMultiple;
}

export function compareRichTextProperties(left: RichTextProperties | undefined, right: RichTextProperties | undefined): boolean {
  if (!left || !right) {
    return !left && !right;
  }
  return (
    left.contentType === right.contentType &&
    (left.minimumLength ?? undefined) === (right.minimumLength ?? undefined) &&
    (left.maximumLength ?? undefined) === (right.maximumLength ?? undefined)
  );
}

export function compareSelectProperties(left: SelectProperties | undefined, right: SelectProperties | undefined): boolean {
  if (!left || !right) {
    return !left && !right;
  }
  return left.isMultiple === right.isMultiple && JSON.stringify(left.options) === JSON.stringify(right.options);
}

export function compareStringProperties(left: StringProperties | undefined, right: StringProperties | undefined): boolean {
  if (!left || !right) {
    return !left && !right;
  }
  return (
    (left.minimumLength ?? undefined) === (right.minimumLength ?? undefined) &&
    (left.maximumLength ?? undefined) === (right.maximumLength ?? undefined) &&
    (left.pattern ?? undefined) === (right.pattern ?? undefined)
  );
}
