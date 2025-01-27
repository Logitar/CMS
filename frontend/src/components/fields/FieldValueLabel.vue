<script setup lang="ts">
import { useI18n } from "vue-i18n";

import type { FieldDefinition } from "@/types/fields";

const { t } = useI18n();

defineProps<{
  definition: FieldDefinition;
}>();
</script>

<template>
  <label :for="definition.id">
    {{ definition.displayName ?? definition.uniqueName }}
    <i v-if="definition.isRequired" class="text-secondary">({{ t("fields.definitions.required") }})</i>
  </label>
</template>
