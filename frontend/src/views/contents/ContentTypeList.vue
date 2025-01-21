<script setup lang="ts">
import { TarBadge, TarButton, parsingUtils, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateContentType from "@/components/contents/CreateContentType.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import YesNoSelect from "@/components/shared/YesNoSelect.vue";
import type { ContentType, ContentTypeSort, SearchContentTypesPayload } from "@/types/contents";
import { handleErrorKey } from "@/inject/App";
import { searchContentTypes } from "@/api/contents";
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
const invariant = computed<boolean | undefined>(() => parseBoolean(route.query.invariant?.toString()));
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("contents.types.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchContentTypesPayload = {
    ids: [],
    isInvariant: invariant.value,
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    sort: sort.value ? [{ field: sort.value as ContentTypeSort, isDescending: isDescending.value }] : [],
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

function onCreated(contentType: ContentType) {
  toasts.success("contents.types.created");
  router.push({ name: "ContentTypeEdit", params: { id: contentType.id } });
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
    <h1>{{ t("contents.types.list") }}</h1>
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
      <CreateContentType class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="row">
      <YesNoSelect
        class="col-lg-3"
        id="is-invariant"
        label="contents.types.isInvariant"
        :model-value="invariant"
        @update:model-value="setQuery('invariant', $event?.toString() ?? '')"
      />
      <SearchInput class="col-lg-3" :model-value="search" @update:model-value="setQuery('search', $event ?? '')" />
      <SortSelect
        class="col-lg-3"
        :descending="isDescending"
        :model-value="sort"
        #
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
            <th scope="col">{{ t("contents.types.sort.options.UniqueName") }}</th>
            <th scope="col">{{ t("contents.types.sort.options.DisplayName") }}</th>
            <th scope="col">{{ t("contents.types.fieldCount") }}</th>
            <th scope="col">{{ t("contents.types.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="contentType in contentTypes" :key="contentType.id">
            <td>
              <RouterLink :to="{ name: 'ContentTypeEdit', params: { id: contentType.id } }">
                <font-awesome-icon icon="fas fa-edit" /> {{ contentType.uniqueName }}
              </RouterLink>
              <template v-if="contentType.isInvariant">
                <br />
                <TarBadge variant="info">{{ t("contents.types.invariant") }}</TarBadge>
              </template>
            </td>
            <td>{{ contentType.displayName ?? "—" }}</td>
            <td>{{ contentType.fieldCount }}</td>
            <td><StatusBlock :actor="contentType.updatedBy" :date="contentType.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("contents.types.empty") }}</p>
  </main>
</template>
