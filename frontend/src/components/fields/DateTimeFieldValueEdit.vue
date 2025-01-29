<script setup lang="ts">
import { computed } from "vue";

import DateTimeInput from "@/components/shared/DateTimeInput.vue";
import FieldValueDescription from "./FieldValueDescription.vue";
import FieldValueLabel from "./FieldValueLabel.vue";
import type { FieldDefinition, FieldType } from "@/types/fields";

const props = defineProps<{
  definition: FieldDefinition;
  modelValue?: string;
}>();

const descriptionId = computed<string>(() => `${props.definition.id}-description`);
const fieldType = computed<FieldType>(() => props.definition.fieldType);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <DateTimeInput
    :described-by="descriptionId"
    floating
    :id="definition.id"
    :label="definition.displayName ?? definition.uniqueName"
    :min="fieldType.dateTime?.minimumValue ? new Date(fieldType.dateTime.minimumValue) : undefined"
    :max="fieldType.dateTime?.maximumValue ? new Date(fieldType.dateTime.maximumValue) : undefined"
    :model-value="modelValue ? new Date(modelValue) : undefined"
    :name="definition.uniqueName"
    raw
    @update:model-value="$emit('update:model-value', $event?.toISOString() ?? '')"
  >
    <template #label-override>
      <FieldValueLabel :definition="definition" />
    </template>
    <template #after v-if="definition.description">
      <FieldValueDescription :definition="definition" :id="descriptionId" />
    </template>
  </DateTimeInput>
</template>
