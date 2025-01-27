<script setup lang="ts">
import { computed } from "vue";

import AppTextarea from "@/components/shared/AppTextarea.vue";
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
  <AppTextarea
    floating
    :described-by="descriptionId"
    :id="definition.id"
    :label="definition.displayName ?? definition.uniqueName"
    :min="fieldType.string?.minimumLength"
    :max="fieldType.string?.maximumLength"
    :model-value="modelValue"
    :name="definition.uniqueName"
    :placeholder="definition.placeholder ?? definition.displayName ?? definition.uniqueName"
    raw
    rows="5"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #label-override>
      <FieldValueLabel :definition="definition" />
    </template>
    <template #after v-if="definition.description">
      <FieldValueDescription :definition="definition" :id="descriptionId" />
    </template>
  </AppTextarea>
</template>
