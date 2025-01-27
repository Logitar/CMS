<script setup lang="ts">
import { computed } from "vue";

import ContentMultiselect from "@/components/contents/ContentMultiselect.vue";
import ContentSelect from "@/components/contents/ContentSelect.vue";
import FieldValueDescription from "./FieldValueDescription.vue";
import FieldValueLabel from "./FieldValueLabel.vue";
import type { FieldDefinition, FieldType } from "@/types/fields";
import type { Language } from "@/types/languages";

const props = defineProps<{
  definition: FieldDefinition;
  language?: Language;
  modelValue?: string;
}>();

const contentIds = computed<string[] | undefined>(() => (props.modelValue ? JSON.parse(props.modelValue) : undefined));
const descriptionId = computed<string>(() => `${props.definition.id}-description`);
const fieldType = computed<FieldType>(() => props.definition.fieldType);

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function onContentIdsUpdate(contentIds: string[]): void {
  emit("update:model-value", JSON.stringify(contentIds));
}
</script>

<template>
  <ContentMultiselect
    v-if="fieldType.relatedContent?.isMultiple"
    :content-type-id="fieldType.relatedContent?.contentTypeId"
    :described-by="descriptionId"
    :id="definition.id"
    :label="definition.displayName ?? definition.uniqueName"
    :language-id="language?.id"
    :model-value="contentIds"
    :name="definition.uniqueName"
    :placeholder="definition.placeholder ?? definition.displayName ?? definition.uniqueName"
    raw
    @update:model-value="onContentIdsUpdate"
  >
    <template #label-override>
      <FieldValueLabel :definition="definition" />
    </template>
    <template #after v-if="definition.description">
      <FieldValueDescription :definition="definition" :id="descriptionId" />
    </template>
  </ContentMultiselect>
  <ContentSelect
    v-else
    :content-type-id="fieldType.relatedContent?.contentTypeId"
    :described-by="descriptionId"
    :id="definition.id"
    :label="definition.displayName ?? definition.uniqueName"
    :language-id="language?.id"
    :model-value="modelValue"
    :name="definition.uniqueName"
    :placeholder="definition.placeholder ?? definition.displayName ?? definition.uniqueName"
    raw
    @update:model-value="$emit('update:model-value', $event ?? '')"
  >
    <template #label-override>
      <FieldValueLabel :definition="definition" />
    </template>
    <template #after v-if="definition.description">
      <FieldValueDescription :definition="definition" :id="descriptionId" />
    </template>
  </ContentSelect>
</template>
