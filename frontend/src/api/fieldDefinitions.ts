import { urlUtils } from "logitar-js";

import type { ContentType } from "@/types/contents";
import type { CreateOrReplaceFieldDefinitionPayload } from "@/types/fields";
import { post, put } from ".";

export async function createFieldDefinition(contentTypeId: string, payload: CreateOrReplaceFieldDefinitionPayload): Promise<ContentType> {
  const url: string = new urlUtils.UrlBuilder({ path: "api/contents/types/{contentTypeId}/fields" })
    .setParameter("contentTypeId", contentTypeId)
    .buildRelative();
  return (await post<CreateOrReplaceFieldDefinitionPayload, ContentType>(url, payload)).data;
}

export async function replaceFieldDefinition(contentTypeId: string, fieldId: string, payload: CreateOrReplaceFieldDefinitionPayload): Promise<ContentType> {
  const url: string = new urlUtils.UrlBuilder({ path: "api/contents/types/{contentTypeId}/fields/{fieldId}" })
    .setParameter("contentTypeId", contentTypeId)
    .setParameter("fieldId", fieldId)
    .buildRelative();
  return (await put<CreateOrReplaceFieldDefinitionPayload, ContentType>(url, payload)).data;
}
