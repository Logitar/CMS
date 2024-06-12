import { urlUtils } from "logitar-js";

import type {
  ContentItem,
  ContentType,
  CreateContentPayload,
  CreateContentTypePayload,
  ReplaceContentTypePayload,
  SearchContentItemsPayload,
  SearchContentTypesPayload,
} from "@/types/contents";
import type { SearchResults } from "@/types/search";
import { get, post, put } from ".";

function createItemUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/contents/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/contents" });
}

function createTypeUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/contents/types/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/contents/types" });
}

export async function createContentItem(payload: CreateContentPayload): Promise<ContentItem> {
  const url: string = createItemUrlBuilder().buildRelative();
  return (await post<CreateContentPayload, ContentItem>(url, payload)).data;
}

export async function createContentType(payload: CreateContentTypePayload): Promise<ContentType> {
  const url: string = createTypeUrlBuilder().buildRelative();
  return (await post<CreateContentTypePayload, ContentType>(url, payload)).data;
}

export async function readContentType(id: string): Promise<ContentType> {
  const url: string = createTypeUrlBuilder(id).buildRelative();
  return (await get<ContentType>(url)).data;
}

export async function replaceContentType(id: string, payload: ReplaceContentTypePayload, version?: number): Promise<ContentType> {
  const url: string = createTypeUrlBuilder(id).setQueryString(`?version=${version}`).buildRelative();
  return (await put<ReplaceContentTypePayload, ContentType>(url, payload)).data;
}

export async function searchContentItems(payload: SearchContentItemsPayload): Promise<SearchResults<ContentItem>> {
  // const url: string = createUrlBuilder()
  //   .setQuery("invariant", payload.isInvariant?.toString() ?? "")
  //   .setQuery("ids", payload.ids ?? [])
  //   .setQuery("search_terms", payload.search?.terms.map(({ value }) => value) ?? [])
  //   .setQuery("search_operator", payload.search?.operator ?? "")
  //   .setQuery("sort", payload.sort?.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)) ?? [])
  //   .setQuery("skip", payload.skip?.toString() ?? "")
  //   .setQuery("limit", payload.limit?.toString() ?? "")
  //   .buildRelative();
  // return (await get<SearchResults<ContentType>>(url)).data;
  return { items: [], total: 0 }; // TODO(fpion): implement
}

export async function searchContentTypes(payload: SearchContentTypesPayload): Promise<SearchResults<ContentType>> {
  const url: string = createTypeUrlBuilder()
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
