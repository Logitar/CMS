<script setup lang="ts">
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import type { NumberProperties } from "@/types/fields";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  modelValue: NumberProperties;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: NumberProperties): void;
}>();

function setMaximumValue(maximumValue: number | string | undefined): void {
  if (typeof maximumValue === "string") {
    maximumValue = maximumValue ? parseNumber(maximumValue) : undefined;
  }
  const value: NumberProperties = { ...props.modelValue, maximumValue };
  emit("update:model-value", value);
}
function setMinimumValue(minimumValue: number | string | undefined): void {
  if (typeof minimumValue === "string") {
    minimumValue = minimumValue ? parseNumber(minimumValue) : undefined;
  }
  const value: NumberProperties = { ...props.modelValue, minimumValue };
  emit("update:model-value", value);
}
function setStep(step: number | string | undefined): void {
  if (typeof step === "string") {
    step = step ? parseNumber(step) : undefined;
  }
  const value: NumberProperties = { ...props.modelValue, step };
  emit("update:model-value", value);
}
</script>

<template>
  <div>
    <h3>{{ t("fields.types.properties", { type: t("fields.types.dataType.options.Number") }) }}</h3>
    <div class="row">
      <AppInput
        class="col"
        floating
        id="minimum-value"
        label="fields.types.number.minimumValue"
        :max="modelValue.maximumValue"
        :model-value="modelValue.minimumValue?.toString()"
        placeholder="fields.types.number.minimumValue"
        type="number"
        @update:model-value="setMinimumValue"
      />
      <AppInput
        class="col"
        floating
        id="maximum-value"
        label="fields.types.number.maximumValue"
        :min="modelValue.minimumValue"
        :model-value="modelValue.maximumValue?.toString()"
        placeholder="fields.types.number.maximumValue"
        type="number"
        @update:model-value="setMaximumValue"
      />
      <AppInput
        class="col"
        floating
        id="step"
        label="fields.types.number.step"
        min="0"
        :max="modelValue.maximumValue"
        :model-value="modelValue.step?.toString()"
        placeholder="fields.types.number.step"
        step="any"
        type="number"
        @update:model-value="setStep"
      />
    </div>
  </div>
</template>
