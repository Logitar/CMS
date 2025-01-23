<script setup lang="ts">
import { computed } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { FieldDefinition, FieldType } from "@/types/fields";

const props = defineProps<{
  definition: FieldDefinition;
  modelValue?: string;
}>();

const fieldType = computed<FieldType>(() => props.definition.fieldType);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <AppSelect
    floating
    :id="definition.id"
    :label="definition.displayName ?? definition.uniqueName"
    :model-value="modelValue"
    :name="definition.uniqueName"
    :options="fieldType.select?.options"
    :placeholder="definition.placeholder ?? definition.displayName ?? definition.uniqueName"
    raw
    :required="definition.isRequired"
    @update:model-value="$emit('update:model-value', $event)"
  />
  <!-- TODO(fpion): multiple -->
</template>
