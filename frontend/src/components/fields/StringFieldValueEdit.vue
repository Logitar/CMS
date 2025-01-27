<script setup lang="ts">
import { computed } from "vue";

import AppInput from "@/components/shared/AppInput.vue";
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
  <AppInput
    :described-by="descriptionId"
    floating
    :id="definition.id"
    :min="fieldType.string?.minimumLength"
    :max="fieldType.string?.maximumLength"
    :model-value="modelValue"
    :name="definition.uniqueName"
    :pattern="fieldType.string?.pattern"
    :placeholder="definition.placeholder ?? definition.displayName ?? definition.uniqueName"
    raw
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #label-override>
      <FieldValueLabel :definition="definition" />
    </template>
    <template #after v-if="definition.description">
      <FieldValueDescription :definition="definition" :id="descriptionId" />
    </template>
  </AppInput>
</template>
