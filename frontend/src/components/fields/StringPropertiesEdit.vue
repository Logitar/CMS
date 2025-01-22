<script setup lang="ts">
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import type { StringProperties } from "@/types/fields";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  modelValue: StringProperties;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: StringProperties): void;
}>();

function setMaximumLength(maximumLength: number | undefined): void {
  const value: StringProperties = { ...props.modelValue, maximumLength };
  emit("update:model-value", value);
}
function setMinimumLength(minimumLength: number | undefined): void {
  const value: StringProperties = { ...props.modelValue, minimumLength };
  emit("update:model-value", value);
}
function setPattern(pattern: string | undefined): void {
  const value: StringProperties = { ...props.modelValue, pattern };
  emit("update:model-value", value);
}
</script>

<template>
  <div>
    <h3>{{ t("fields.types.properties", { type: t("fields.types.dataType.options.String") }) }}</h3>
    <div class="row">
      <AppInput
        class="col"
        floating
        id="minimum-length"
        label="fields.types.string.minimumLength"
        min="0"
        :max="modelValue.maximumLength"
        :model-value="modelValue.minimumLength?.toString()"
        placeholder="fields.types.string.minimumLength"
        type="number"
        @update:model-value="setMinimumLength(parseNumber($event))"
      />
      <AppInput
        class="col"
        floating
        id="maximum-length"
        label="fields.types.string.maximumLength"
        :min="modelValue.minimumLength ?? 0"
        :model-value="modelValue.maximumLength?.toString()"
        placeholder="fields.types.string.maximumLength"
        type="number"
        @update:model-value="setMaximumLength(parseNumber($event))"
      />
    </div>
    <AppInput
      floating
      id="step"
      label="fields.types.string.pattern"
      :model-value="modelValue?.pattern?.toString()"
      placeholder="fields.types.string.pattern"
      @update:model-value="setPattern"
    />
  </div>
</template>
