<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { Language } from "@/types/languages";
import { formatLanguage } from "@/helpers/displayUtils";
import { searchLanguages } from "@/api/languages";

const { orderBy } = arrayUtils;

withDefaults(
  defineProps<{
    id?: string;
    label?: string;
    modelValue?: string;
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
  (e: "selected", value?: Language): void;
  (e: "update:model-value", value?: string): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);
  const language: Language | undefined = languages.value.find((language) => language.id === id);
  emit("selected", language);
}

onMounted(async () => {
  try {
    const results = await searchLanguages({});
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
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder"
    :required="required"
    :show-status="required ? 'touched' : 'never'"
    @update:model-value="onModelValueUpdate"
  />
</template>
