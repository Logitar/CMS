<script setup lang="ts">
import { nanoid } from "nanoid";
import { parsingUtils } from "logitar-js";

import AppInput from "@/components/shared/AppInput.vue";
import type { NumberProperties } from "@/types/fieldTypes";

const { parseNumber } = parsingUtils;

const props = withDefaults(
  defineProps<{
    id?: string;
    modelValue: NumberProperties;
  }>(),
  {
    id: () => nanoid(),
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: NumberProperties): void;
}>();

function setProperty(key: keyof NumberProperties, value: string | undefined): void {
  const properties: NumberProperties = { ...props.modelValue };
  switch (key) {
    case "maximumValue":
      properties.maximumValue = value === "" ? undefined : parseNumber(value);
      break;
    case "minimumValue":
      properties.minimumValue = value === "" ? undefined : parseNumber(value);
      break;
    case "step":
      properties.step = value === "" ? undefined : parseNumber(value);
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
        :id="`${id}_minimum-value`"
        label="fieldTypes.properties.number.minimumValue"
        :max="modelValue.maximumValue ?? 1.7976931348623157e308"
        :model-value="modelValue.minimumValue?.toString()"
        placeholder="fieldTypes.properties.number.minimumValue"
        type="number"
        @update:model-value="setProperty('minimumValue', $event)"
      />
      <AppInput
        class="col-lg-6"
        floating
        :id="`${id}_maximum-value`"
        label="fieldTypes.properties.number.maximumValue"
        :max="1.7976931348623157e308"
        :model-value="modelValue.maximumValue?.toString()"
        placeholder="fieldTypes.properties.number.maximumValue"
        type="number"
        @update:model-value="setProperty('maximumValue', $event)"
      />
    </div>
    <AppInput
      floating
      :id="`${id}step`"
      label="fieldTypes.properties.number.step"
      :min="0"
      :max="modelValue.maximumValue ?? 1.7976931348623157e308"
      :model-value="modelValue.step?.toString()"
      placeholder="fieldTypes.properties.number.step"
      type="number"
      @update:model-value="setProperty('step', $event)"
    />
  </div>
</template>
