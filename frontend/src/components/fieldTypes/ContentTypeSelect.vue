<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { ContentType } from "@/types/fieldTypes";

const { orderBy } = arrayUtils;
const { rt, tm } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    modelValue?: ContentType;
    required?: boolean | string;
  }>(),
  {
    id: "content-type",
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    Object.entries(tm(rt("fieldTypes.properties.text.contentType.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value?: ContentType): void;
}>();
</script>

<template>
  <AppSelect
    floating
    :id="id"
    label="fieldTypes.properties.text.contentType.label"
    :model-value="modelValue"
    :options="options"
    placeholder="fieldTypes.properties.text.contentType.placeholder"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
