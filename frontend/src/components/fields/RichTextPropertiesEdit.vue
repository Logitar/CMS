<script setup lang="ts">
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

import AppInput from "@/components/shared/AppInput.vue";
import type { RichTextProperties } from "@/types/fields";

const { parseNumber } = parsingUtils;
const { t } = useI18n();

const props = defineProps<{
  modelValue: RichTextProperties;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: RichTextProperties): void;
}>();

function setMaximumLength(maximumLength: number | string | undefined): void {
  if (typeof maximumLength === "string") {
    maximumLength = maximumLength ? parseNumber(maximumLength) : undefined;
  }
  const value: RichTextProperties = { ...props.modelValue, maximumLength };
  emit("update:model-value", value);
}
function setMinimumLength(minimumLength: number | string | undefined): void {
  if (typeof minimumLength === "string") {
    minimumLength = minimumLength ? parseNumber(minimumLength) : undefined;
  }
  const value: RichTextProperties = { ...props.modelValue, minimumLength };
  emit("update:model-value", value);
}
</script>

<template>
  <div>
    <h3>{{ t("fields.types.properties", { type: t("fields.types.dataType.options.RichText") }) }}</h3>
    <div class="row">
      <AppInput
        class="col"
        floating
        id="minimum-length"
        label="fields.types.richText.minimumLength"
        min="0"
        :max="modelValue.maximumLength"
        :model-value="modelValue.minimumLength?.toString()"
        placeholder="fields.types.richText.minimumLength"
        type="number"
        @update:model-value="setMinimumLength"
      />
      <AppInput
        class="col"
        floating
        id="maximum-length"
        label="fields.types.richText.maximumLength"
        :min="modelValue.minimumLength ?? 0"
        :model-value="modelValue.maximumLength?.toString()"
        placeholder="fields.types.richText.maximumLength"
        type="number"
        @update:model-value="setMaximumLength"
      />
    </div>
  </div>
</template>
