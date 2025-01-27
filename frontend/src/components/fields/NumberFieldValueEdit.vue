<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import type { FieldDefinition, FieldType } from "@/types/fields";

const { t } = useI18n();

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
  <AppInput
    floating
    :id="definition.id"
    :label="definition.displayName ?? definition.uniqueName"
    :min="fieldType.number?.minimumValue"
    :max="fieldType.number?.maximumValue"
    :model-value="modelValue"
    :name="definition.uniqueName"
    :placeholder="definition.placeholder ?? definition.displayName ?? definition.uniqueName"
    raw
    :step="fieldType.number?.step"
    type="number"
    @update:model-value="$emit('update:model-value', $event)"
  >
    <template #label-override>
      <label :for="definition.id">
        {{ definition.displayName ?? definition.uniqueName }}
        <i v-if="definition.isRequired" class="text-secondary">({{ t("fields.definitions.required") }})</i>
      </label>
    </template>
  </AppInput>
</template>
