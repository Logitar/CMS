import { urlUtils } from "logitar-js";

import type { CreateFieldTypePayload, FieldType, ReplaceFieldTypePayload, SearchFieldTypesPayload } from "@/types/fields";
import type { SearchResults } from "@/types/search";
import { get, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/fields/types/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/fields/types" });
}

export async function createFieldType(payload: CreateFieldTypePayload): Promise<FieldType> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateFieldTypePayload, FieldType>(url, payload)).data;
}

export async function readFieldType(id: string): Promise<FieldType> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<FieldType>(url)).data;
}

export async function replaceFieldType(id: string, payload: ReplaceFieldTypePayload, version?: number): Promise<FieldType> {
  const url: string = createUrlBuilder(id).setQueryString(`?version=${version}`).buildRelative();
  return (await put<ReplaceFieldTypePayload, FieldType>(url, payload)).data;
}

export async function searchFieldTypes(payload: SearchFieldTypesPayload): Promise<SearchResults<FieldType>> {
  const url: string = createUrlBuilder()
    .setQuery("type", payload.dataType ?? "")
    .setQuery("ids", payload.ids ?? [])
    .setQuery("search_terms", payload.search?.terms.map(({ value }) => value) ?? [])
    .setQuery("search_operator", payload.search?.operator ?? "")
    .setQuery("sort", payload.sort?.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)) ?? [])
    .setQuery("skip", payload.skip?.toString() ?? "")
    .setQuery("limit", payload.limit?.toString() ?? "")
    .buildRelative();
  return (await get<SearchResults<FieldType>>(url)).data;
}
