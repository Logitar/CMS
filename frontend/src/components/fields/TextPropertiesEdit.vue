<script setup lang="ts">
import { nanoid } from "nanoid";
import { parsingUtils } from "logitar-js";

import AppInput from "@/components/shared/AppInput.vue";
import ContentTypeSelect from "./ContentTypeSelect.vue";
import type { ContentType, TextProperties } from "@/types/fields";

const { parseNumber } = parsingUtils;

const props = withDefaults(
  defineProps<{
    id?: string;
    modelValue: TextProperties;
  }>(),
  {
    id: () => nanoid(),
  },
);

const emit = defineEmits<{
  (e: "update:model-value", value: TextProperties): void;
}>();

function setProperty(key: keyof TextProperties, value: string | undefined): void {
  const properties: TextProperties = { ...props.modelValue };
  switch (key) {
    case "contentType":
      properties.contentType = (value as ContentType) ?? "text/plain";
      break;
    case "maximumLength":
      properties.maximumLength = value === "" ? undefined : parseNumber(value);
      break;
    case "minimumLength":
      properties.minimumLength = value === "" ? undefined : parseNumber(value);
      break;
  }
  emit("update:model-value", properties);
}
</script>

<template>
  <div>
    <ContentTypeSelect disabled :model-value="modelValue.contentType" />
    <div class="row">
      <AppInput
        class="col-lg-6"
        floating
        :id="`${id}_minimum-length`"
        label="fields.types.properties.text.minimumLength"
        min="0"
        :max="modelValue.maximumLength ?? 0x7fffffff"
        :model-value="modelValue.minimumLength?.toString() ?? '0'"
        placeholder="fields.types.properties.text.minimumLength"
        type="number"
        @update:model-value="setProperty('minimumLength', $event)"
      />
      <AppInput
        class="col-lg-6"
        floating
        :id="`${id}_maximum-length`"
        label="fields.types.properties.text.maximumLength"
        min="0"
        :model-value="modelValue.maximumLength?.toString() ?? '0'"
        placeholder="fields.types.properties.text.maximumLength"
        type="number"
        @update:model-value="setProperty('maximumLength', $event)"
      />
    </div>
  </div>
</template>
