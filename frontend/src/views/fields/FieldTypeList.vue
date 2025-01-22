<script setup lang="ts">
import { TarButton, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils, objectUtils } from "logitar-js";
import { computed, inject, ref, watch } from "vue";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppPagination from "@/components/shared/AppPagination.vue";
import CountSelect from "@/components/shared/CountSelect.vue";
import CreateFieldType from "@/components/fields/CreateFieldType.vue";
import DataTypeSelect from "@/components/fields/DataTypeSelect.vue";
import SearchInput from "@/components/shared/SearchInput.vue";
import SortSelect from "@/components/shared/SortSelect.vue";
import StatusBlock from "@/components/shared/StatusBlock.vue";
import type { DataType, FieldType, FieldTypeSort, SearchFieldTypesPayload } from "@/types/fields";
import { handleErrorKey } from "@/inject/App";
import { searchFieldTypes } from "@/api/fieldTypes";
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
const timestamp = ref<number>(0);
const fieldTypes = ref<FieldType[]>([]);
const total = ref<number>(0);

const count = computed<number>(() => parseNumber(route.query.count?.toString()) || 10);
const isDescending = computed<boolean>(() => parseBoolean(route.query.isDescending?.toString()) ?? false);
const page = computed<number>(() => parseNumber(route.query.page?.toString()) || 1);
const search = computed<string>(() => route.query.search?.toString() ?? "");
const sort = computed<string>(() => route.query.sort?.toString() ?? "");
const type = computed<DataType>(() => (route.query.type?.toString() ?? "") as DataType);

const sortOptions = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("fields.types.sort.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

async function refresh(): Promise<void> {
  const payload: SearchFieldTypesPayload = {
    ids: [],
    search: {
      terms: search.value
        .split(" ")
        .filter((term) => Boolean(term))
        .map((term) => ({ value: `%${term}%` })),
      operator: "And",
    },
    dataType: type.value,
    sort: sort.value ? [{ field: sort.value as FieldTypeSort, isDescending: isDescending.value }] : [],
    skip: (page.value - 1) * count.value,
    limit: count.value,
  };
  isLoading.value = true;
  const now = Date.now();
  timestamp.value = now;
  try {
    const results = await searchFieldTypes(payload);
    if (now === timestamp.value) {
      fieldTypes.value = results.items;
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
    case "search":
    case "type":
    case "count":
      query.page = "1";
      break;
  }
  router.replace({ ...route, query });
}

function onCreated(fieldType: FieldType) {
  toasts.success("fields.types.created");
  router.push({ name: "FieldTypeEdit", params: { id: fieldType.id } });
}

watch(
  () => route,
  (route) => {
    if (route.name === "FieldTypeList") {
      const { query } = route;
      if (!query.page || !query.count) {
        router.replace({
          ...route,
          query: isEmpty(query)
            ? {
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
    <h1>{{ t("fields.types.list") }}</h1>
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
      <CreateFieldType class="ms-1" @created="onCreated" @error="handleError" />
    </div>
    <div class="row">
      <DataTypeSelect class="col-lg-3" :model-value="type" no-status @update:model-value="setQuery('type', $event ?? '')" />
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
    <template v-if="fieldTypes.length">
      <table class="table table-striped">
        <thead>
          <tr>
            <th scope="col">{{ t("fields.types.sort.options.UniqueName") }}</th>
            <th scope="col">{{ t("fields.types.sort.options.DisplayName") }}</th>
            <th scope="col">{{ t("fields.types.dataType.label") }}</th>
            <th scope="col">{{ t("fields.types.sort.options.UpdatedOn") }}</th>
          </tr>
        </thead>
        <tbody>
          <tr v-for="fieldType in fieldTypes" :key="fieldType.id">
            <td>
              <RouterLink :to="{ name: 'FieldTypeEdit', params: { id: fieldType.id } }">
                <font-awesome-icon icon="fas fa-edit" /> {{ fieldType.uniqueName }}
              </RouterLink>
            </td>
            <td>{{ fieldType.displayName ?? "—" }}</td>
            <td>{{ t(`fields.types.dataType.options.${fieldType.dataType}`) }}</td>
            <td><StatusBlock :actor="fieldType.updatedBy" :date="fieldType.updatedOn" /></td>
          </tr>
        </tbody>
      </table>
      <AppPagination :count="count" :model-value="page" :total="total" @update:model-value="setQuery('page', $event.toString())" />
    </template>
    <p v-else>{{ t("fields.types.empty") }}</p>
  </main>
</template>
