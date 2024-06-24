import { urlUtils } from "logitar-js";

import type { ContentItem, ContentLocale, CreateContentPayload, SearchContentsPayload } from "@/types/contents";
import type { SearchResults } from "@/types/search";
import { get, post } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/contents/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/contents" });
}

export async function createContentItem(payload: CreateContentPayload): Promise<ContentItem> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateContentPayload, ContentItem>(url, payload)).data;
}

export async function readContent(id: string): Promise<ContentItem> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<ContentItem>(url)).data;
}

export async function searchContentItems(payload: SearchContentsPayload): Promise<SearchResults<ContentLocale>> {
  const url: string = createUrlBuilder()
    .setQuery("type", payload.contentTypeId ?? "")
    .setQuery("language", payload.languageId ?? "")
    .setQuery("ids", payload.ids ?? [])
    .setQuery("search_terms", payload.search?.terms.map(({ value }) => value) ?? [])
    .setQuery("search_operator", payload.search?.operator ?? "")
    .setQuery("sort", payload.sort?.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)) ?? [])
    .setQuery("skip", payload.skip?.toString() ?? "")
    .setQuery("limit", payload.limit?.toString() ?? "")
    .buildRelative();
  return (await get<SearchResults<ContentLocale>>(url)).data;
}
