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
    :min="fieldType.string?.minimumLength"
    :max="fieldType.string?.maximumLength"
    :model-value="modelValue"
    :name="definition.uniqueName"
    :pattern="fieldType.string?.pattern"
    :placeholder="definition.placeholder ?? definition.displayName ?? definition.uniqueName"
    raw
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
