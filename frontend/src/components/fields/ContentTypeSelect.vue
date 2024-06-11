<script setup lang="ts">
import { TarSelect, type SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import type { ContentType } from "@/types/fields";

const { orderBy } = arrayUtils;
const { rt, t, tm } = useI18n();

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
    Object.entries(tm(rt("fields.types.properties.text.contentType.options"))).map(([value, text]) => ({ text, value }) as SelectOption),
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value?: ContentType): void;
}>();
</script>

<template>
  <TarSelect
    floating
    :id="id"
    :label="t('fields.types.properties.text.contentType.label')"
    :model-value="modelValue"
    :options="options"
    :placeholder="t('fields.types.properties.text.contentType.placeholder')"
    :required="required"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
