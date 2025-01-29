<script setup lang="ts">
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import AppCheckbox from "@/components/shared/AppCheckbox.vue";
import FieldValueDescription from "./FieldValueDescription.vue";
import type { FieldDefinition } from "@/types/fields";

const { parseBoolean } = parsingUtils;

const props = defineProps<{
  definition: FieldDefinition;
  modelValue?: string;
}>();

const descriptionId = computed<string>(() => `${props.definition.id}-description`);

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
  >
    <template #after v-if="definition.description">
      <FieldValueDescription :definition="definition" :id="descriptionId" />
    </template>
  </AppCheckbox>
</template>
