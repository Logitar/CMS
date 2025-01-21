<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils, parsingUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";

const { orderBy } = arrayUtils;
const { parseBoolean } = parsingUtils;
const { rt, tm } = useI18n();

withDefaults(
  defineProps<{
    label?: string;
    modelValue?: string;
    noStatus?: boolean | string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    label: "fields.types.dataType.label",
    placeholder: "fields.types.dataType.placeholder",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("fields.types.dataType.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppSelect
    floating
    id="data-type"
    :label="label"
    :model-value="modelValue"
    :options="options"
    :placeholder="placeholder"
    :required="required"
    :validation="parseBoolean(noStatus) ? 'server' : undefined"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
