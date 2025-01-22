<script setup lang="ts">
import AppTextarea from "./AppTextarea.vue";

withDefaults(
  defineProps<{
    id?: string;
    modelValue?: string;
    rows?: number | string;
  }>(),
  {
    id: "description",
    rows: 15,
  },
);

defineEmits<{
  (e: "update:model-value", value?: string): void;
}>();
</script>

<template>
  <AppTextarea
    floating
    :id="id"
    label="description"
    :model-value="modelValue"
    placeholder="description"
    :rows="rows"
    @update:model-value="$emit('update:model-value', $event)"
  />
</template>
