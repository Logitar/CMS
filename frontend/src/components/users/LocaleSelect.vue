<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import locales from "@/resources/locales.json";

const { orderBy } = arrayUtils;

withDefaults(
  defineProps<{
    disabled?: boolean | string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    label: "users.locale.label",
    placeholder: "users.locale.placeholder",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    locales.map(({ code, nativeName }) => ({ value: code, text: nativeName })),
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppSelect
    :disabled="disabled"
    floating
    id="locale"
    :label="label"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
