import { urlUtils } from "logitar-js";

import type { CreateLanguagePayload, Language, SearchLanguagesPayload } from "@/types/languages";
import type { SearchResults } from "@/types/search";
import { get, patch, post } from ".";

function createUrlBuilder(id?: string): urlUtils.IUrlBuilder {
  if (id) {
    return new urlUtils.UrlBuilder({ path: "/api/languages/{id}" }).setParameter("id", id);
  }
  return new urlUtils.UrlBuilder({ path: "/api/languages" });
}

export async function createLanguage(payload: CreateLanguagePayload): Promise<Language> {
  const url: string = createUrlBuilder().buildRelative();
  return (await post<CreateLanguagePayload, Language>(url, payload)).data;
}

export async function readLanguage(id: string): Promise<Language> {
  const url: string = createUrlBuilder(id).buildRelative();
  return (await get<Language>(url)).data;
}

export async function searchLanguages(payload: SearchLanguagesPayload): Promise<SearchResults<Language>> {
  const url: string = createUrlBuilder()
    .setQuery("ids", payload.ids ?? [])
    .setQuery("search_terms", payload.search?.terms.map(({ value }) => value) ?? [])
    .setQuery("search_operator", payload.search?.operator ?? "")
    .setQuery("sort", payload.sort?.map(({ field, isDescending }) => (isDescending ? `DESC.${field}` : field)) ?? [])
    .setQuery("skip", payload.skip?.toString() ?? "")
    .setQuery("limit", payload.limit?.toString() ?? "")
    .buildRelative();
  return (await get<SearchResults<Language>>(url)).data;
}

export async function setDefaultLanguage(id: string): Promise<Language> {
  const url: string = createUrlBuilder(id).setPath("/api/languages/{id}/default").buildRelative();
  return (await patch<undefined, Language>(url)).data;
}
