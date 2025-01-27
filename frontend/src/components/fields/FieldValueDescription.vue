<script setup lang="ts">
import type { FieldDefinition } from "@/types/fields";

defineProps<{
  definition: FieldDefinition;
  id: string;
}>();
</script>

<template>
  <i :id="id"><font-awesome-icon icon="fas fa-circle-info" /> {{ definition.description }}</i>
</template>
