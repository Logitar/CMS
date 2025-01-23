<script setup lang="ts">
import { computed } from "vue";

import BooleanFieldValueEdit from "./BooleanFieldValueEdit.vue";
import DateTimeFieldValueEdit from "./DateTimeFieldValueEdit.vue";
import NumberFieldValueEdit from "./NumberFieldValueEdit.vue";
import RichTextFieldValueEdit from "./RichTextFieldValueEdit.vue";
import SelectFieldValueEdit from "./SelectFieldValueEdit.vue";
import StringFieldValueEdit from "./StringFieldValueEdit.vue";
import type { DataType, FieldDefinition } from "@/types/fields";

const props = defineProps<{
  definition: FieldDefinition;
  modelValue?: string;
}>();

const dataType = computed<DataType>(() => props.definition.fieldType.dataType);

defineEmits<{
  (e: "update:model-value", value: string): void;
}>();
</script>

<template>
  <div>
    <BooleanFieldValueEdit
      v-if="dataType === 'Boolean'"
      :definition="definition"
      :model-value="modelValue"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <DateTimeFieldValueEdit
      v-else-if="dataType === 'DateTime'"
      :definition="definition"
      :model-value="modelValue"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <NumberFieldValueEdit
      v-else-if="dataType === 'Number'"
      :definition="definition"
      :model-value="modelValue"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <RichTextFieldValueEdit
      v-else-if="dataType === 'RichText'"
      :definition="definition"
      :model-value="modelValue"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <SelectFieldValueEdit
      v-else-if="dataType === 'Select'"
      :definition="definition"
      :model-value="modelValue"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <StringFieldValueEdit
      v-else-if="dataType === 'String'"
      :definition="definition"
      :model-value="modelValue"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <!-- TODO(fpion): FieldDefinition.Description -->
    <!-- TODO(fpion): RelatedContent -->
    <!-- TODO(fpion): Tags -->
  </div>
</template>
