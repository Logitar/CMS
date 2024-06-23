<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { DataType } from "@/types/fieldTypes";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

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
    Object.entries(tm(rt("fields.types.dataType.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value?: DataType): void;
}>();
</script>

<template>
  <TarSelect
    floating
    :id="id"
    :label="t('fields.types.dataType.label')"
    :model-value="modelValue"
    :options="options"
    :placeholder="t('fields.types.dataType.placeholder')"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
