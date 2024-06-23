<script setup lang="ts">
import { nanoid } from "nanoid";
import { objectUtils, parsingUtils } from "logitar-js";

import AppInput from "@/components/shared/AppInput.vue";
import type { StringProperties } from "@/types/fieldTypes";

const { assign } = objectUtils;
const { parseNumber } = parsingUtils;

const props = withDefaults(
  defineProps<{
    id?: string;
    modelValue: StringProperties;
  }>(),
  {
    id: () => nanoid(),
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: StringProperties): void;
}>();

function setProperty(key: keyof StringProperties, value: string | undefined): void {
  const properties: StringProperties = { ...props.modelValue };
  switch (key) {
    case "maximumLength":
      properties.maximumLength = value === "" ? undefined : parseNumber(value);
      break;
    case "minimumLength":
      properties.minimumLength = value === "" ? undefined : parseNumber(value);
      break;
    default:
      assign(properties, key, value || undefined);
      break;
  }
  emit("update:model-value", properties);
}
</script>

<template>
  <div>
    <div class="row">
      <AppInput
        class="col-lg-6"
        floating
        :id="`${id}_minimum-length`"
        label="fields.types.properties.string.minimumLength"
        min="0"
        :max="modelValue.maximumLength ?? 0x7fffffff"
        :model-value="modelValue.minimumLength?.toString() ?? '0'"
        placeholder="fields.types.properties.string.minimumLength"
        type="number"
        @update:model-value="setProperty('minimumLength', $event)"
      />
      <AppInput
        class="col-lg-6"
        floating
        :id="`${id}_maximum-length`"
        label="fields.types.properties.string.maximumLength"
        min="0"
        :model-value="modelValue.maximumLength?.toString() ?? '0'"
        placeholder="fields.types.properties.string.maximumLength"
        type="number"
        @update:model-value="setProperty('maximumLength', $event)"
      />
    </div>
    <AppInput
      floating
      :id="`${id}_pattern`"
      label="fields.types.properties.string.pattern"
      :model-value="modelValue.pattern"
      placeholder="fields.types.properties.string.pattern"
      @update:model-value="setProperty('pattern', $event)"
    />
  </div>
</template>
