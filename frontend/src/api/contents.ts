import { urlUtils } from "logitar-js";

import type { ContentType, CreateContentTypePayload, ReplaceContentTypePayload, SearchContentTypesPayload } from "@/types/contents";
import type { SearchResults } from "@/types/search";
import { get, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/contents/types/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/contents/types" });
}

export async function createContentType(payload: CreateContentTypePayload): Promise<ContentType> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateContentTypePayload, ContentType>(url, payload)).data;
}

export async function readContentType(id: string): Promise<ContentType> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<ContentType>(url)).data;
}

export async function replaceContentType(id: string, payload: ReplaceContentTypePayload, version?: number): Promise<ContentType> {
  const url: string = createUrlBuilder(id).setQueryString(`?version=${version}`).buildRelative();
  return (await put<ReplaceContentTypePayload, ContentType>(url, payload)).data;
}

export async function searchContentTypes(payload: SearchContentTypesPayload): Promise<SearchResults<ContentType>> {
  const url: string = createUrlBuilder()
    .setQuery("invariant", payload.isInvariant?.toString() ?? "")
    .setQuery("ids", payload.ids ?? [])
    .setQuery("search_terms", payload.search?.terms.map(({ value }) => value) ?? [])
    .setQuery("search_operator", payload.search?.operator ?? "")
    .setQuery("sort", payload.sort?.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)) ?? [])
    .setQuery("skip", payload.skip?.toString() ?? "")
    .setQuery("limit", payload.limit?.toString() ?? "")
    .buildRelative();
  return (await get<SearchResults<ContentType>>(url)).data;
}
