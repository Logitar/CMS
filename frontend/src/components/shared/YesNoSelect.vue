<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { nanoid } from "nanoid";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import AppSelect from "./AppSelect.vue";
import { computed } from "vue";

const { orderBy } = arrayUtils;
const { parseBoolean } = parsingUtils;
const { t } = useI18n();

withDefaults(
  defineProps<{
    id?: string;
    label: string;
    modelValue?: boolean | string;
    placeholder?: string;
  }>(),
  {
    id: () => nanoid(),
  },
);

const options = computed<SelectOption[]>(() =>
  orderBy(
    [
      { text: t("yes"), value: "true" },
      { text: t("no"), value: false },
    ] as SelectOption[],
    "text",
  ),
);

defineEmits<{
  (e: "update:model-value", value: boolean | undefined): void;
}>();
</script>

<template>
  <AppSelect
    floating
    :id="id"
    :label="label"
    :model-value="modelValue?.toString()"
    :options="options"
    :placeholder="placeholder ?? 'any'"
    validation="server"
    @update:model-value="$emit('update:model-value', parseBoolean($event))"
  />
</template>
