<script setup lang="ts">
import { computed } from "vue";

import AppTags from "@/components/tags/AppTags.vue";
import FieldValueDescription from "./FieldValueDescription.vue";
import FieldValueLabel from "./FieldValueLabel.vue";
import type { FieldDefinition } from "@/types/fields";

const props = defineProps<{
  definition: FieldDefinition;
  modelValue?: string;
}>();

const descriptionId = computed<string>(() => `${props.definition.id}-description`);
const tags = computed<string[]>(() => (props.modelValue ? JSON.parse(props.modelValue) : []));

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function onModelValueUpdate(tags: string[]): void {
  emit("update:model-value", tags.length ? JSON.stringify(tags) : "");
}
</script>

<template>
  <AppTags :id="`tags-${definition.id}`" :label="definition.displayName ?? definition.uniqueName" :model-value="tags" @update:model-value="onModelValueUpdate">
    <template #label-override>
      <FieldValueLabel :definition="definition" />
    </template>
    <template #after v-if="definition.description">
      <FieldValueDescription :definition="definition" :id="descriptionId" />
    </template>
  </AppTags>
</template>
