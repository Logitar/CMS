<script setup lang="ts">
import { computed } from "vue";

import AppMultiselect from "@/components/shared/AppMultiselect.vue";
import AppSelect from "@/components/shared/AppSelect.vue";
import type { FieldDefinition, FieldType } from "@/types/fields";

const props = defineProps<{
  definition: FieldDefinition;
  modelValue?: string;
}>();

const fieldType = computed<FieldType>(() => props.definition.fieldType);
const values = computed<string[] | undefined>(() => (props.modelValue ? JSON.parse(props.modelValue) : undefined));

const emit = defineEmits<{
  (e: "update:model-value", value: string): void;
}>();

function onValuesUpdate(values: string[]): void {
  emit("update:model-value", JSON.stringify(values));
}
</script>

<template>
  <AppMultiselect
    v-if="fieldType.select?.isMultiple"
    floating
    :id="definition.id"
    :label="definition.displayName ?? definition.uniqueName"
    :model-value="values"
    :name="definition.uniqueName"
    :options="fieldType.select?.options"
    :placeholder="definition.placeholder ?? definition.displayName ?? definition.uniqueName"
    raw
    :required="definition.isRequired"
    @update:model-value="onValuesUpdate"
  />
  <AppSelect
    v-else
    floating
    :id="definition.id"
    :label="definition.displayName ?? definition.uniqueName"
    :model-value="modelValue"
    :name="definition.uniqueName"
    :options="fieldType.select?.options"
    :placeholder="definition.placeholder ?? definition.displayName ?? definition.uniqueName"
    raw
    :required="definition.isRequired"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
