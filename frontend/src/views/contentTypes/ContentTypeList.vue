<script setup lang="ts">
import { TarButton, parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateContentType from "@/components/contentTypes/CreateContentType.vue";
import InvariantSelect from "@/components/contentTypes/InvariantSelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { ContentType, ContentTypeSort, SearchContentTypesPayload } from "@/types/contentTypes";
import { handleErrorKey } from "@/inject/App";
import { searchContentTypes } from "@/api/contentTypes";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { isEmpty } = objectUtils;
const { orderBy } = arrayUtils;
const { parseBoolean, parseNumber } = parsingUtils;
const { rt, t, tm } = useI18n();

const contentTypes = ref<ContentType[]>([]);
const isLoading = ref<boolean>(false);
const timestamp = ref<number>(0);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const isInvariant = computed<boolean | undefined>(() => parseBoolean(route.query.invariant?.toString()));
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("contentTypes.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchContentTypesPayload = {
    isInvariant: isInvariant.value,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as ContentTypeSort, isDescending: isDescending.value }] : undefined,
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchContentTypes(payload);
    if (now === timestamp.value) {
      contentTypes.value = results.items;
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

function onCreated(contentType: ContentType): void {
  toasts.success("contentTypes.created");
  router.replace({ name: "ContentTypeEdit", params: { id: contentType.id } });
}

function setQuery(key: string, value: string): void {
  const query = { ...route.query, [key]: value };
  switch (key) {
    case "invariant":
    case "search":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

watch(
  () => route,
  (route) => {
    if (route.name === "ContentTypeList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
                invariant: "",
                type: "",
                search: "",
                sort: "DisplayName",
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
    <h1>{{ t("contentTypes.title.list") }}</h1>
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
      <CreateContentType @created="onCreated" @error="handleError" />
    </div>
    <div class="row">
      <InvariantSelect class="col-lg-3" :model-value="isInvariant?.toString()" @update:model-value="setQuery('invariant', $event ?? '')" />
      <SearchInput class="col-lg-3" :model-value="search" @update:model-value="setQuery('search', $event ?? '')" />
      <SortSelect
        class="col-lg-3"
        :descending="isDescending"
        :model-value="sort"
        :options="sortOptions"
        @descending="setQuery('isDescending', $event.toString())"
        @update:model-value="setQuery('sort', $event ?? '')"
      />
      <CountSelect class="col-lg-3" :model-value="count" @update:model-value="setQuery('count', ($event ?? 10).toString())" />
    </div>
    <template v-if="contentTypes.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("contentTypes.sort.options.UniqueName") }}</th>
            <th scope="col">{{ t("contentTypes.sort.options.DisplayName") }}</th>
            <th scope="col">{{ t("contentTypes.isInvariant") }}</th>
            <th scope="col">{{ t("contentTypes.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="contentType in contentTypes" :key="contentType.id">
            <td>
              <RouterLink :to="{ name: 'ContentTypeEdit', params: { id: contentType.id } }">
                <font-awesome-icon icon="fas fa-edit" />{{ contentType.uniqueName }}
              </RouterLink>
            </td>
            <td>{{ contentType.displayName ?? "—" }}</td>
            <td>
              <template v-if="contentType.isInvariant"><font-awesome-icon icon="fas fa-check" /> {{ t("yes") }}</template>
              <template v-else><font-awesome-icon icon="fas fa-times" /> {{ t("no") }}</template>
            </td>
            <td><StatusBlock :actor="contentType.updatedBy" :date="contentType.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("contentTypes.empty") }}</p>
  </main>
</template>
