<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppSelect from "@/components/shared/AppSelect.vue";

const { orderBy } = arrayUtils;
const { t } = useI18n();

defineProps<{
  modelValue?: string;
}>();

const options = computed<SelectOption[]>(() =>
  orderBy(
    [
      {
        text: t("no"),
        value: "false",
      },
      {
        text: t("yes"),
        value: "true",
      },
    ],
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
    id="is-done"
    label="contentTypes.invariant.label"
    :model-value="modelValue"
    :options="options"
    placeholder="contentTypes.invariant.placeholder"
    show-status="never"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
