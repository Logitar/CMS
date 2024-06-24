import { describe, it, expect } from "vitest";
import { nanoid } from "nanoid";

import type { Actor } from "@/types/actor";
import type { ContentItem } from "@/types/contents";
import type { ContentType } from "@/types/contentTypes";
import type { FieldType } from "@/types/fieldTypes";
import type { Language } from "@/types/languages";
import { formatContentItem, formatContentType, formatFieldType, formatLanguage } from "../displayUtils";

const actor: Actor = {
  id: nanoid(),
  type: "User",
  isDeleted: false,
  displayName: "admin",
};
const now: string = new Date().toISOString();

describe("formatContent", () => {
  it.concurrent("should return the formatted content item", () => {
    const contentItem: ContentItem = {
      id: nanoid(),
      version: 1,
      createdBy: actor,
      updatedBy: actor,
      createdOn: now,
      updatedOn: now,
      contentType: {
        id: nanoid(),
        version: 1,
        createdBy: actor,
        createdOn: now,
        updatedBy: actor,
        updatedOn: now,
        isInvariant: false,
        uniqueName: "BlogArticle",
      },
      invariant: {
        uniqueName: "prolongez-lete-avec-une-acura-nsx-coupe",
        item: {} as unknown as ContentItem,
        createdBy: actor,
        createdOn: now,
        updatedBy: actor,
        updatedOn: now,
      },
      locales: [],
    };
    contentItem.invariant.item = contentItem;
    expect(formatContentItem(contentItem)).toBe(contentItem.invariant.uniqueName);
  });
});

describe("formatContentType", () => {
  it.concurrent("should return the formatted content type without display name", () => {
    const contentType: ContentType = {
      id: nanoid(),
      version: 1,
      createdBy: actor,
      updatedBy: actor,
      createdOn: now,
      updatedOn: now,
      isInvariant: true,
      uniqueName: "BlogAuthor",
      displayName: undefined,
    };
    expect(formatContentType(contentType)).toBe(contentType.uniqueName);
  });

  it.concurrent("should return the formatted content type with display name", () => {
    const contentType: ContentType = {
      id: nanoid(),
      version: 1,
      createdBy: actor,
      updatedBy: actor,
      createdOn: now,
      updatedOn: now,
      isInvariant: false,
      uniqueName: "BlogArticle",
      displayName: "Blog Article",
    };
    expect(formatContentType(contentType)).toBe("Blog Article (BlogArticle)");
  });
});

describe("formatFieldType", () => {
  it.concurrent("should return the formatted field type without display name", () => {
    const fieldType: FieldType = {
      id: nanoid(),
      version: 1,
      createdBy: actor,
      updatedBy: actor,
      createdOn: now,
      updatedOn: now,
      uniqueName: "SubTitle",
      displayName: undefined,
      dataType: "String",
      stringProperties: {},
    };
    expect(formatFieldType(fieldType)).toBe(fieldType.uniqueName);
  });

  it.concurrent("should return the formatted field type with display name", () => {
    const fieldType: FieldType = {
      id: nanoid(),
      version: 1,
      createdBy: actor,
      updatedBy: actor,
      createdOn: now,
      updatedOn: now,
      uniqueName: "SubTitle",
      displayName: "Sub-title",
      dataType: "String",
      stringProperties: {},
    };
    expect(formatFieldType(fieldType)).toBe("Sub-title (SubTitle)");
  });
});

describe("formatLanguage", () => {
  it.concurrent("should return the formatted language", () => {
    const language: Language = {
      id: nanoid(),
      version: 1,
      createdBy: actor,
      updatedBy: actor,
      createdOn: now,
      updatedOn: now,
      isDefault: true,
      locale: {
        id: 12,
        code: "fr",
        displayName: "French",
        englishName: "French",
        nativeName: "fran\u00E7ais",
      },
    };
    expect(formatLanguage(language)).toBe("fran\u00E7ais (fr)");
  });
});
