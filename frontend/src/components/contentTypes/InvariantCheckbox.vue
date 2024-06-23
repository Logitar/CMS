<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

withDefaults(
  defineProps<{
    disabled?: boolean | string;
    id?: string;
    label?: string;
    modelValue?: boolean | string;
  }>(),
  {
    id: "is-invariant",
    label: "contentTypes.isInvariant",
  },
);

defineEmits<{
  (e: "update:model-value", value: boolean): void;
}>();
</script>

<template>
  <TarCheckbox :disabled="disabled" :id="id" :label="t(label)" :model-value="modelValue" switch @update:model-value="$emit('update:model-value', $event)" />
</template>
