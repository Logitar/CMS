import { urlUtils } from "logitar-js";

import type { CreateOrReplaceContentTypePayload, ContentType, SearchContentTypesPayload } from "@/types/contents";
import type { SearchResults } from "@/types/search";
import { get, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/api/contents/types/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/api/contents/types" });
}

export async function createContentType(payload: CreateOrReplaceContentTypePayload): Promise<ContentType> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateOrReplaceContentTypePayload, ContentType>(url, payload)).data;
}

export async function readContentType(id: string): Promise<ContentType> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<ContentType>(url)).data;
}

export async function replaceContentType(id: string, payload: CreateOrReplaceContentTypePayload, version?: number): Promise<ContentType> {
  const url: string = createUrlBuilder(id)
    .setQuery("version", version?.toString() ?? "")
    .buildRelative();
  return (await put<CreateOrReplaceContentTypePayload, ContentType>(url, payload)).data;
}

export async function searchContentTypes(payload: SearchContentTypesPayload): Promise<SearchResults<ContentType>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery("invariant", payload.isInvariant?.toString() ?? "")
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<ContentType>>(url)).data;
}
