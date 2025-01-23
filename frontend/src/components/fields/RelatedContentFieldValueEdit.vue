<script setup lang="ts">
import { computed } from "vue";

import ContentSelect from "@/components/contents/ContentSelect.vue";
import type { FieldDefinition, FieldType } from "@/types/fields";
import type { Language } from "@/types/languages";

const props = defineProps<{
  definition: FieldDefinition;
  language?: Language;
  modelValue?: string;
}>();

const fieldType = computed<FieldType>(() => props.definition.fieldType);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <ContentSelect
    :content-type-id="fieldType.relatedContent?.contentTypeId"
    :id="definition.id"
    :label="definition.displayName ?? definition.uniqueName"
    :language-id="language?.id"
    :model-value="modelValue"
    :name="definition.uniqueName"
    :placeholder="definition.placeholder ?? definition.displayName ?? definition.uniqueName"
    raw
    :required="definition.isRequired"
    @update:model-value="$emit('update:model-value', $event ?? '')"
  />
  <!-- TODO(fpion): multiple -->
</template>
