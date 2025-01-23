<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { ContentType, SearchContentTypesPayload } from "@/types/contents";
import type { SearchResults } from "@/types/search";
import { formatContentType } from "@/helpers/format";
import { searchContentTypes } from "@/api/contentTypes";

const { orderBy } = arrayUtils;
const { parseBoolean } = parsingUtils;

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
    noStatus?: boolean | string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "content-type",
    label: "contents.types.select.label",
    placeholder: "contents.types.select.placeholder",
  },
);

const contentTypes = ref<ContentType[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    contentTypes.value.map((contentType) => ({
      text: formatContentType(contentType),
      value: contentType.id,
    })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value: ContentType | undefined): void;
  (e: "update:model-value", value: string | undefined): void;
}>();

function onSelected(id: string | undefined): void {
  emit("update:model-value", id);

  const index: number = contentTypes.value.findIndex((contentType) => contentType.id === id);
  emit("selected", contentTypes.value[index]);
}

onMounted(async () => {
  try {
    const payload: SearchContentTypesPayload = {
      ids: [],
      search: { operator: "And", terms: [] },
      sort: [{ field: "DisplayName", isDescending: false }],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<ContentType> = await searchContentTypes(payload);
    contentTypes.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <AppSelect
    floating
    :id="id"
    :label="label"
    :model-value="modelValue?.toString()"
    :options="options"
    :placeholder="placeholder ?? 'any'"
    :required="required"
    :validation="parseBoolean(noStatus) ? 'server' : undefined"
    @update:model-value="onSelected"
  />
</template>
