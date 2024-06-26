<script setup lang="ts">
import { TarButton, parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppPagination from "@/components/shared/AppPagination.vue";
import ContentTypeSelect from "@/components/contentTypes/ContentTypeSelect.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateContentItem from "@/components/contents/CreateContentItem.vue";
import LanguageSelect from "@/components/languages/LanguageSelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { ContentItem, ContentLocale, ContentSort, SearchContentsPayload } from "@/types/contents";
import { formatContentType } from "@/helpers/displayUtils";
import { handleErrorKey } from "@/inject/App";
import { searchContentItems } from "@/api/contents";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const contentLocales = ref<ContentLocale[]>([]);
const isLoading = ref<boolean>(false);
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
    Object.entries(tm(rt("contents.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchContentsPayload = {
    contentTypeId: type.value,
    languageId: language.value,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as ContentSort, isDescending: isDescending.value }] : undefined,
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchContentItems(payload);
    if (now === timestamp.value) {
      contentLocales.value = results.items;
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

function onCreated(content: ContentItem): void {
  toasts.success("contents.created");
  router.replace({ name: "ContentItemEdit", params: { id: content.id } });
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

watch(
  () => route,
  (route) => {
    if (route.name === "ContentItemList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                type: "",
                language: "",
                search: "",
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
    <h1>{{ t("contents.title.list") }}</h1>
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
      <CreateContentItem @created="onCreated" @error="handleError" />
    </div>
    <div class="row">
      <ContentTypeSelect class="col-lg-6" :model-value="type" @update:model-value="setQuery('type', $event ?? '')" />
      <LanguageSelect class="col-lg-6" :model-value="language" placeholder="languages.invariant" @update:model-value="setQuery('language', $event ?? '')" />
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
    <template v-if="contentLocales.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("contents.sort.options.UniqueName") }}</th>
            <th scope="col">{{ t("contentTypes.select.label") }}</th>
            <th scope="col">{{ t("contents.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="contentLocale in contentLocales" :key="contentLocale.item.id">
            <td>
              <RouterLink :to="{ name: 'ContentItemEdit', params: { id: contentLocale.item.id } }">
                <font-awesome-icon icon="fas fa-edit" /> {{ contentLocale.uniqueName }}
              </RouterLink>
            </td>
            <td>
              <RouterLink :to="{ name: 'ContentTypeEdit', params: { id: contentLocale.item.contentType.id } }" target="_blank">
                {{ formatContentType(contentLocale.item.contentType) }}
                <font-awesome-icon icon="fas fa-arrow-up-right-from-square" />
              </RouterLink>
            </td>
            <td><StatusBlock :actor="contentLocale.updatedBy" :date="contentLocale.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("contents.empty") }}</p>
  </main>
</template>
