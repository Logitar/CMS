<script setup lang="ts">
import { TarAlert } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

const { t } = useI18n();

withDefaults(
  defineProps<{
    modelValue?: string[];
  }>(),
  {
    modelValue: () => [],
  },
);
</script>

<template>
  <TarAlert
    :close="t('actions.close')"
    dismissible
    :model-value="modelValue.length > 0"
    variant="warning"
    @update:model-value="$emit('update:model-value', [])"
  >
    <p>
      <strong>{{ t("errors.contentFieldValueConflicts.lead") }}</strong> {{ t("errors.contentFieldValueConflicts.help") }}
    </p>
    <ul>
      <li v-for="field in modelValue" :key="field">{{ field }}</li>
    </ul>
  </TarAlert>
</template>
