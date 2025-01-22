<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

defineProps<{
  modelValue?: boolean | string;
}>();

defineEmits<{
  (e: "update:model-value", value: boolean | undefined): void;
}>();
</script>

<template>
  <TarCheckbox
    class="mb-3"
    id="invariant"
    :label="t('contents.types.invariant')"
    :model-value="modelValue"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
