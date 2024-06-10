<script setup lang="ts">
import { nanoid } from "nanoid";
import { objectUtils } from "logitar-js";

import DateTimeInput from "@/components/shared/DateTimeInput.vue";
import type { DateTimeProperties } from "@/types/fields";

const defaultMaximum = new Date(9999, 11, 31, 23, 59, 59);
const { assign } = objectUtils;

const props = withDefaults(
  defineProps<{
    id?: string;
    modelValue: DateTimeProperties;
  }>(),
  {
    id: () => nanoid(),
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: DateTimeProperties): void;
}>();

function setProperty(key: keyof DateTimeProperties, value: Date | undefined): void {
  const properties: DateTimeProperties = { ...props.modelValue };
  assign(properties, key, value?.toISOString());
  emit("update:model-value", properties);
}
</script>

<template>
  <div>
    <div class="row">
      <DateTimeInput
        class="col-lg-6"
        floating
        :id="`${id}_minimum-value`"
        label="fields.types.properties.dateTime.minimumValue"
        :max="modelValue.maximumValue ? new Date(modelValue.maximumValue) : defaultMaximum"
        :model-value="modelValue.minimumValue ? new Date(modelValue.minimumValue) : undefined"
        @update:model-value="setProperty('minimumValue', $event)"
      />
      <DateTimeInput
        class="col-lg-6"
        floating
        :id="`${id}_maximum-value`"
        label="fields.types.properties.dateTime.maximumValue"
        :max="defaultMaximum"
        :model-value="modelValue.maximumValue ? new Date(modelValue.maximumValue) : undefined"
        @update:model-value="setProperty('maximumValue', $event)"
      />
    </div>
  </div>
</template>
