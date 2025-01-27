<script setup lang="ts">
import { computed } from "vue";

import AppTags from "@/components/tags/AppTags.vue";
import FieldValueLabel from "./FieldValueLabel.vue";
import type { FieldDefinition } from "@/types/fields";

const props = defineProps<{
  definition: FieldDefinition;
  modelValue?: string;
}>();

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
  </AppTags>
</template>
