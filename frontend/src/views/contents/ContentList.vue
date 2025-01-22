<script setup lang="ts">
import { TarButton, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppPagination from "@/components/shared/AppPagination.vue";
import ContentTypeSelect from "@/components/contents/ContentTypeSelect.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateContent from "@/components/contents/CreateContent.vue";
import LanguageSelect from "@/components/languages/LanguageSelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { Content, ContentLocale, ContentSort, SearchContentsPayload } from "@/types/contents";
import { formatContentType } from "@/helpers/format";
import { handleErrorKey } from "@/inject/App";
import { searchContents } from "@/api/contents";
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
const locales = ref<ContentLocale[]>([]);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const language = computed<string>(() => route.query.language?.toString() ?? "");
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const type = computed<string>(() => route.query.type?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("contents.items.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchContentsPayload = {
    ids: [],
    languageId: language.value,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    contentTypeId: type.value,
    sort: sort.value ? [{ field: sort.value as ContentSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchContents(payload);
    if (now === timestamp.value) {
      locales.value = results.items;
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

function setQuery(key: string, value: string): void {
  const query = { ...route.query, [key]: value };
  switch (key) {
    case "language":
    case "search":
    case "type":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

function onCreated(content: Content) {
  toasts.success("contents.items.created");
  router.push({ name: "ContentEdit", params: { id: content.id } });
}

watch(
  () => route,
  (route) => {
    if (route.name === "ContentList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                language: "",
                search: "",
                type: "",
                sort: "UpdatedOn",
                isDescending: "true",
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
    <h1>{{ t("contents.items.list") }}</h1>
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
      <CreateContent class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="row">
      <ContentTypeSelect class="col-lg-6" :model-value="type" no-status placeholder="any" @update:model-value="setQuery('type', $event ?? '')" />
      <LanguageSelect
        class="col-lg-6"
        :model-value="language"
        no-status
        placeholder="contents.items.invariant"
        @update:model-value="setQuery('language', $event ?? '')"
      />
    </div>
    <div class="row">
      <SearchInput class="col-lg-4" :model-value="search" @update:model-value="setQuery('search', $event ?? '')" />
      <SortSelect
        class="col-lg-4"
        :descending="isDescending"
        :model-value="sort"
        #
        :options="sortOptions"
        @descending="setQuery('isDescending', $event.toString())"
        @update:model-value="setQuery('sort', $event ?? '')"
      />
      <CountSelect class="col-lg-4" :model-value="count" @update:model-value="setQuery('count', ($event ?? 10).toString())" />
    </div>
    <template v-if="locales.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("contents.items.sort.options.UniqueName") }}</th>
            <th scope="col">{{ t("contents.items.sort.options.DisplayName") }}</th>
            <th scope="col">{{ t("contents.types.select.label") }}</th>
            <th scope="col">{{ t("contents.items.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="locale in locales" :key="locale.content.id">
            <td>
              <RouterLink :to="{ name: 'ContentEdit', params: { id: locale.content.id } }">
                <font-awesome-icon icon="fas fa-edit" /> {{ locale.uniqueName }}
              </RouterLink>
            </td>
            <td>{{ locale.displayName ?? "—" }}</td>
            <td>
              <RouterLink :to="{ name: 'ContentTypeEdit', params: { id: locale.content.contentType.id } }" target="_blank">
                {{ formatContentType(locale.content.contentType) }}
              </RouterLink>
            </td>
            <td><StatusBlock :actor="locale.updatedBy" :date="locale.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("contents.items.empty") }}</p>
  </main>
</template>
