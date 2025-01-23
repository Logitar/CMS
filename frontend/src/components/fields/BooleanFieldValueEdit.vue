<script setup lang="ts">
import { parsingUtils } from "logitar-js";

import AppCheckbox from "@/components/shared/AppCheckbox.vue";
import type { FieldDefinition } from "@/types/fields";

const { parseBoolean } = parsingUtils;

defineProps<{
  definition: FieldDefinition;
  modelValue?: string;
}>();

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <AppCheckbox
    :id="definition.id"
    :label="definition.displayName ?? definition.uniqueName"
    :model-value="parseBoolean(modelValue)"
    :name="definition.uniqueName"
    raw
    @update:model-value="$emit('update:model-value', ($event as boolean).toString())"
  />
</template>
