import { describe, it, expect } from "vitest";
import { nanoid } from "nanoid";

import type { Actor } from "@/types/actor";
import type { FieldType } from "@/types/fields";
import type { Language } from "@/types/languages";
import { formatFieldType, formatLanguage } from "../displayUtils";

const actor: Actor = {
  id: nanoid(),
  type: "User",
  isDeleted: false,
  displayName: "admin",
};
const now: string = new Date().toISOString();

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
