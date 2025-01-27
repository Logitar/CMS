<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { computed } from "vue";

import AppMultiselect from "@/components/shared/AppMultiselect.vue";
import AppSelect from "@/components/shared/AppSelect.vue";
import FieldValueDescription from "./FieldValueDescription.vue";
import FieldValueLabel from "./FieldValueLabel.vue";
import type { FieldDefinition, FieldType } from "@/types/fields";

const props = defineProps<{
  definition: FieldDefinition;
  modelValue?: string;
}>();

const descriptionId = computed<string>(() => `${props.definition.id}-description`);
const fieldType = computed<FieldType>(() => props.definition.fieldType);
const options = computed<SelectOption[]>(
  () =>
    fieldType.value.select?.options.map(
      (option) =>
        ({
          disabled: option.isDisabled,
          label: option.label || undefined,
          text: option.text,
          value: option.value || option.text,
        }) as SelectOption,
    ) ?? [],
);
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
    :described-by="descriptionId"
    floating
    :id="definition.id"
    :label="definition.displayName ?? definition.uniqueName"
    :model-value="values"
    :name="definition.uniqueName"
    :options="options"
    :placeholder="definition.placeholder ?? definition.displayName ?? definition.uniqueName"
    raw
    @update:model-value="onValuesUpdate"
  >
    <template #label-override>
      <FieldValueLabel :definition="definition" />
    </template>
    <template #after v-if="definition.description">
      <FieldValueDescription :definition="definition" :id="descriptionId" />
    </template>
  </AppMultiselect>
  <AppSelect
    v-else
    :described-by="descriptionId"
    floating
    :id="definition.id"
    :label="definition.displayName ?? definition.uniqueName"
    :model-value="modelValue"
    :name="definition.uniqueName"
    :options="options"
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
  </AppSelect>
</template>
