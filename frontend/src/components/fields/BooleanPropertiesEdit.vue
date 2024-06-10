<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { BooleanProperties } from "@/types/fields";

const { t } = useI18n();

defineProps<{
  modelValue: BooleanProperties;
}>();
</script>

<template>
  <div>{{ t("fields.types.properties.boolean.none") }}</div>
</template>
