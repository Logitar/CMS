<script setup lang="ts">
import { TarButton } from "logitar-vue3-ui";
import { nanoid } from "nanoid";

import AppCheckbox from "@/components/shared/AppCheckbox.vue";
import AppInput from "@/components/shared/AppInput.vue";
import type { SelectOption } from "@/types/fields";

const props = withDefaults(
  defineProps<{
    id?: string;
    modelValue: SelectOption;
  }>(),
  {
    id: () => nanoid(),
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: SelectOption): void;
  (e: "removed"): void;
}>();

function setIsDisabled(isDisabled: boolean): void {
  const option: SelectOption = { ...props.modelValue, isDisabled };
  emit("update:model-value", option);
}
function setIsSelected(isSelected: boolean): void {
  const option: SelectOption = { ...props.modelValue, isSelected };
  emit("update:model-value", option);
}
function setLabel(label: string): void {
  const option: SelectOption = { ...props.modelValue, label };
  emit("update:model-value", option);
}
function setText(text: string): void {
  const option: SelectOption = { ...props.modelValue, text };
  emit("update:model-value", option);
}
function setValue(value: string): void {
  const option: SelectOption = { ...props.modelValue, value };
  emit("update:model-value", option);
}
</script>

<template>
  <div class="row">
    <AppInput
      class="col"
      floating
      :id="`${id}-text`"
      label="fields.types.select.options.text"
      :model-value="modelValue.text"
      placeholder="fields.types.select.options.text"
      required
      @update:model-value="setText"
    />
    <AppInput
      class="col"
      floating
      :id="`${id}-value`"
      label="fields.types.select.options.value"
      :model-value="modelValue.value"
      placeholder="fields.types.select.options.value"
      @update:model-value="setValue"
    />
    <AppInput
      class="col"
      floating
      :id="`${id}-label`"
      label="fields.types.select.options.label"
      :model-value="modelValue.label"
      placeholder="fields.types.select.options.label"
      @update:model-value="setLabel"
    />
    <div class="col">
      <div class="float-start">
        <AppCheckbox
          :id="`${id}-disabled`"
          label="fields.types.select.options.isDisabled"
          :model-value="modelValue.isDisabled"
          tight
          @update:model-value="setIsDisabled"
        />
        <AppCheckbox
          :id="`${id}-selected`"
          label="fields.types.select.options.isSelected"
          :model-value="modelValue.isSelected"
          tight
          @update:model-value="setIsSelected"
        />
      </div>
      <div class="float-end">
        <TarButton icon="fas fa-trash-can" variant="danger" @click="$emit('removed')" />
      </div>
    </div>
  </div>
</template>
