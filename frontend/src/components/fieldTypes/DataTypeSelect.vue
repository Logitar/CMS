<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { DataType } from "@/types/fieldTypes";

const { orderBy } = arrayUtils;
const { rt, tm } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    modelValue?: DataType;
    required?: boolean | string;
  }>(),
  {
    id: "data-type",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("fieldTypes.dataType.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value?: DataType): void;
}>();
</script>

<template>
  <AppSelect
    floating
    :id="id"
    label="fieldTypes.dataType.label"
    :model-value="modelValue"
    :options="options"
    placeholder="fieldTypes.dataType.placeholder"
    :required="required"
    :show-status="required ? 'touched' : 'never'"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
