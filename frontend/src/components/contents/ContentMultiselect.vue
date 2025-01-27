<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";

import AppMultiselect from "@/components/shared/AppMultiselect.vue";
import { searchContents } from "@/api/contents";
import type { ContentLocale, SearchContentsPayload } from "@/types/contents";
import type { SearchResults } from "@/types/search";

const { orderBy } = arrayUtils;

const props = defineProps<{
  contentTypeId?: string;
  id?: string;
  label?: string;
  languageId?: string;
  modelValue?: string[];
  name?: string;
  placeholder?: string;
  raw?: boolean | string;
  required?: boolean | string;
}>();

const locales = ref<ContentLocale[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    locales.value.map((locale) => ({ text: locale.displayName ?? locale.uniqueName, value: locale.content.id })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "update:model-value", value: string[]): void;
}>();

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
  <AppMultiselect
    :id="id"
    :label="label"
    :model-value="modelValue"
    :name="name"
    :options="options"
    :placeholder="placeholder"
    :raw="raw"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #label-override>
      <slot name="label-override"></slot>
    </template>
  </AppMultiselect>
</template>
