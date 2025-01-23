<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { computed } from "vue";
import { parsingUtils } from "logitar-js";

import AppInput from "@/components/shared/AppInput.vue";
import AppTextarea from "@/components/shared/AppTextarea.vue";
import DateTimeInput from "@/components/shared/DateTimeInput.vue";
import type { FieldDefinition, FieldType } from "@/types/fields";

const { parseBoolean } = parsingUtils;

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
  <div>
    <div v-if="fieldType.dataType === 'Boolean'" class="mb-3">
      <TarCheckbox
        :id="definition.id"
        :label="definition.displayName ?? definition.uniqueName"
        :model-value="parseBoolean(modelValue)"
        :name="definition.uniqueName"
        @update:model-value="$emit('update:model-value', ($event as boolean).toString())"
      />
    </div>
    <DateTimeInput
      v-else-if="fieldType.dataType === 'DateTime'"
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
    <AppInput
      v-else-if="fieldType.dataType === 'Number'"
      floating
      :id="definition.id"
      :label="definition.displayName ?? definition.uniqueName"
      :min="fieldType.number?.minimumValue"
      :max="fieldType.number?.maximumValue"
      :model-value="modelValue"
      :name="definition.uniqueName"
      :placeholder="definition.placeholder ?? definition.displayName ?? definition.uniqueName"
      raw
      :required="definition.isRequired"
      :step="fieldType.number?.step"
      type="number"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <AppTextarea
      v-else-if="fieldType.dataType === 'RichText'"
      floating
      :id="definition.id"
      :label="definition.displayName ?? definition.uniqueName"
      :min="fieldType.string?.minimumLength"
      :max="fieldType.string?.maximumLength"
      :model-value="modelValue"
      :name="definition.uniqueName"
      :placeholder="definition.placeholder ?? definition.displayName ?? definition.uniqueName"
      raw
      rows="5"
      :required="definition.isRequired"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <AppInput
      v-else-if="fieldType.dataType === 'String'"
      floating
      :id="definition.id"
      :label="definition.displayName ?? definition.uniqueName"
      :min="fieldType.string?.minimumLength"
      :max="fieldType.string?.maximumLength"
      :model-value="modelValue"
      :name="definition.uniqueName"
      :pattern="fieldType.string?.pattern"
      :placeholder="definition.placeholder ?? definition.displayName ?? definition.uniqueName"
      raw
      :required="definition.isRequired"
      @update:model-value="$emit('update:model-value', $event)"
    />
    <!-- TODO(fpion): FieldDefinition.Description -->
    <!-- TODO(fpion): RelatedContent -->
    <!-- TODO(fpion): Select -->
    <!-- TODO(fpion): Tags -->
  </div>
</template>
