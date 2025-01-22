<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { Language, SearchLanguagesPayload } from "@/types/languages";
import type { SearchResults } from "@/types/search";
import { formatLanguage } from "@/helpers/format";
import { searchLanguages } from "@/api/languages";

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
    id: "language",
    label: "languages.select.label",
    placeholder: "languages.select.placeholder",
  },
);

const languages = ref<Language[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    languages.value.map((language) => ({
      text: formatLanguage(language),
      value: language.id,
    })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value: Language | undefined): void;
  (e: "update:model-value", value: string | undefined): void;
}>();

function onSelected(id: string | undefined): void {
  emit("update:model-value", id);

  const index: number = languages.value.findIndex((language) => language.id === id);
  if (index >= 0) {
    emit("selected", languages.value[index]);
  }
}

onMounted(async () => {
  try {
    const payload: SearchLanguagesPayload = {
      ids: [],
      search: { operator: "And", terms: [] },
      sort: [{ field: "DisplayName", isDescending: false }],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<Language> = await searchLanguages(payload);
    languages.value = results.items;
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
