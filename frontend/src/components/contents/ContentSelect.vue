<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { ContentLocale, SearchContentsPayload } from "@/types/contents";
import type { SearchResults } from "@/types/search";
import { formatContentLocale } from "@/helpers/format";
import { searchContents } from "@/api/contents";

const { orderBy } = arrayUtils;
const { parseBoolean } = parsingUtils;

const props = withDefaults(
  defineProps<{
    contentTypeId?: string;
    id?: string;
    label?: string;
    languageId?: string;
    modelValue?: string;
    noStatus?: boolean | string;
    placeholder?: string;
    raw?: boolean | string;
    required?: boolean | string;
  }>(),
  {
    id: "content",
    label: "contents.items.select.label",
    placeholder: "contents.items.select.placeholder",
  },
);

const locales = ref<ContentLocale[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    locales.value.map((locale) => ({
      text: formatContentLocale(locale),
      value: locale.content.id,
    })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value: ContentLocale | undefined): void;
  (e: "update:model-value", value: string | undefined): void;
}>();

function onSelected(id: string | undefined): void {
  emit("update:model-value", id);

  const index: number = locales.value.findIndex((locale) => locale.content.id === id);
  emit("selected", locales.value[index]);
}

onMounted(async () => {
  try {
    const payload: SearchContentsPayload = {
      contentTypeId: props.contentTypeId,
      ids: [],
      languageId: props.languageId,
      search: { operator: "And", terms: [] },
      sort: [{ field: "DisplayName", isDescending: false }],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<ContentLocale> = await searchContents(payload);
    locales.value = results.items;
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
    :raw="raw"
    :required="required"
    :validation="parseBoolean(noStatus) ? 'server' : undefined"
    @update:model-value="onSelected"
  >
    <template #label-override>
      <slot name="label-override"></slot>
    </template>
  </AppSelect>
</template>
