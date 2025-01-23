<script setup lang="ts">
import AppCheckbox from "@/components/shared/AppCheckbox.vue";

defineProps<{
  modelValue?: boolean | string;
}>();

defineEmits<{
  (e: "update:model-value", value: boolean | undefined): void;
}>();
</script>

<template>
  <AppCheckbox id="invariant" label="contents.types.invariant" :model-value="modelValue" @update:model-value="$emit('update:model-value', $event)" />
</template>
