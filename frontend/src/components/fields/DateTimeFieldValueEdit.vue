<script setup lang="ts">
import { computed } from "vue";

import DateTimeInput from "@/components/shared/DateTimeInput.vue";
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
  <DateTimeInput
    floating
    :id="definition.id"
    :label="definition.displayName ?? definition.uniqueName"
    :min="fieldType.dateTime?.minimumValue ? new Date(fieldType.dateTime.minimumValue) : undefined"
    :max="fieldType.dateTime?.maximumValue ? new Date(fieldType.dateTime.maximumValue) : undefined"
    :model-value="modelValue ? new Date(modelValue) : undefined"
    :name="definition.uniqueName"
    raw
    :required="definition.isRequired"
    @update:model-value="$emit('update:model-value', $event?.toISOString() ?? '')"
  />
</template>
