<script setup lang="ts">
import { computed } from "vue";

import AppInput from "@/components/shared/AppInput.vue";
import type { ValidationRules } from "@/types/validation";

const props = withDefaults(
  defineProps<{
    allowedCharacters?: string;
    id?: string;
    modelValue?: string;
    required?: boolean | string;
  }>(),
  {
    id: "unique-name",
  },
);

const rules = computed<ValidationRules>(() => {
  const rules: ValidationRules = {};
  if (props.allowedCharacters) {
    rules.allowed_characters = props.allowedCharacters;
  } else {
    rules.identifier = true;
  }
  return rules;
});

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppInput
    floating
    :id="id"
    label="uniqueName"
    max="255"
    :model-value="modelValue"
    placeholder="uniqueName"
    :required="required"
    :rules="rules"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
