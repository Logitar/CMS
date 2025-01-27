import { urlUtils } from "logitar-js";

import type { CreateOrReplaceContentPayload, Content, ContentLocale, SearchContentsPayload } from "@/types/contents";
import type { SearchResults } from "@/types/search";
import { get, patch, post, put } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  return id ? new urlUtils.UrlBuilder({ path: "/api/contents/{id}" }).setParameter("id", id) : new urlUtils.UrlBuilder({ path: "/api/contents" });
}

export async function createContent(languageId: string | undefined, payload: CreateOrReplaceContentPayload): Promise<Content> {
  const url: string = createUrlBuilder()
    .setQuery("language", languageId ?? "")
    .buildRelative();
  return (await post<CreateOrReplaceContentPayload, Content>(url, payload)).data;
}

export async function publishContent(id: string, languageId?: string): Promise<Content> {
  const url: string = new urlUtils.UrlBuilder({ path: "/api/contents/{id}/publish" })
    .setParameter("id", id)
    .setQuery("language", languageId ?? "")
    .buildRelative();
  return (await patch<void, Content>(url)).data;
}

export async function readContent(id: string): Promise<Content> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<Content>(url)).data;
}

export async function replaceContent(contentId: string, languageId: string | undefined, payload: CreateOrReplaceContentPayload): Promise<Content> {
  const url: string = createUrlBuilder(contentId)
    .setQuery("language", languageId ?? "")
    .buildRelative();
  return (await put<CreateOrReplaceContentPayload, Content>(url, payload)).data;
}

export async function searchContents(payload: SearchContentsPayload): Promise<SearchResults<ContentLocale>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids)
    .setQuery("language", payload.languageId ?? "")
    .setQuery(
      "search",
      payload.search.terms.map(({ value }) => value),
    )
    .setQuery("search_operator", payload.search.operator)
    .setQuery("type", payload.contentTypeId ?? "")
    .setQuery(
      "sort",
      payload.sort.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)),
    )
    .setQuery("skip", payload.skip.toString())
    .setQuery("limit", payload.limit.toString())
    .buildRelative();
  return (await get<SearchResults<ContentLocale>>(url)).data;
}

export async function unpublishContent(id: string, languageId?: string): Promise<Content> {
  const url: string = new urlUtils.UrlBuilder({ path: "/api/contents/{id}/unpublish" })
    .setParameter("id", id)
    .setQuery("language", languageId ?? "")
    .buildRelative();
  return (await patch<void, Content>(url)).data;
}
