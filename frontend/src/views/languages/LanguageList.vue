<script setup lang="ts">
import { TarBadge, TarButton, parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateLanguage from "@/components/languages/CreateLanguage.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { ApiError, Error } from "@/types/api";
import type { Language, LanguageSort, SearchLanguagesPayload } from "@/types/languages";
import { handleErrorKey } from "@/inject/App";
import { searchLanguages } from "@/api/languages";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const isLoading = ref<boolean>(false);
const languages = ref<Language[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("languages.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchLanguagesPayload = {
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as LanguageSort, isDescending: isDescending.value }] : undefined,
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchLanguages(payload);
    if (now === timestamp.value) {
      languages.value = results.items;
      total.value = results.total;
    }
  } catch (e: unknown) {
    handleError(e);
  } finally {
    if (now === timestamp.value) {
      isLoading.value = false;
    }
  }
}

function onCreated(language: Language): void {
  toasts.success("languages.created");
  router.replace({ name: "LanguageEdit", params: { id: language.id } });
}

function setQuery(key: string, value: string): void {
  const query = { ...route.query, [key]: value };
  switch (key) {
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

function onError(e: unknown): void {
  const { data, status } = e as ApiError;
  if (status === 409 && (data as Error)?.code === "LocaleAlreadyUsed") {
    toasts.warning("languages.localeAlreadyUsed");
  } else {
    handleError(e);
  }
}

watch(
  () => route,
  (route) => {
    if (route.name === "LanguageList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                search: "",
                sort: "Code",
                isDescending: "false",
                page: 1,
                count: 10,
              }
            : {
                page: 1,
                count: 10,
                ...query,
              },
        });
      } else {
        refresh();
      }
    }
  },
  { deep: true, immediate: true },
);
</script>

<template>
  <main class="container">
    <h1>{{ t("languages.title.list") }}</h1>
    <div class="my-3">
      <TarButton
        class="me-1"
        :disabled="isLoading"
        icon="fas fa-rotate"
        :loading="isLoading"
        :status="t('loading')"
        :text="t('actions.refresh')"
        @click="refresh()"
      />
      <CreateLanguage @created="onCreated" @error="onError" />
    </div>
    <div class="row">
      <SearchInput class="col-lg-4" :model-value="search" @update:model-value="setQuery('search', $event ?? '')" />
      <SortSelect
        class="col-lg-4"
        :descending="isDescending"
        :model-value="sort"
        :options="sortOptions"
        @descending="setQuery('isDescending', $event.toString())"
        @update:model-value="setQuery('sort', $event ?? '')"
      />
      <CountSelect class="col-lg-4" :model-value="count" @update:model-value="setQuery('count', ($event ?? 10).toString())" />
    </div>
    <template v-if="languages.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("languages.sort.options.Code") }}</th>
            <th scope="col">{{ t("languages.displayNames") }}</th>
            <th scope="col">{{ t("languages.isDefault") }}</th>
            <th scope="col">{{ t("languages.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="language in languages" :key="language.id">
            <td>
              <RouterLink :to="{ name: 'LanguageEdit', params: { id: language.id } }">
                <font-awesome-icon icon="fas fa-edit" />{{ language.locale.code }}
              </RouterLink>
            </td>
            <td>
              {{ language.locale.displayName }}, {{ t("languages.englishFormat", { englishName: language.locale.englishName }) }}
              <br />
              {{ t("languages.nativeFormat", { nativeName: language.locale.nativeName }) }}
            </td>
            <td>
              <TarBadge v-if="language.isDefault" variant="info"><font-awesome-icon icon="fas fa-star" /> {{ t("languages.default") }}</TarBadge>
              <template v-else>{{ "—" }}</template>
            </td>
            <td><StatusBlock :actor="language.updatedBy" :date="language.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("languages.empty") }}</p>
  </main>
</template>
