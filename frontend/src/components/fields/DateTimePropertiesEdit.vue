<script setup lang="ts">
import { computed } from "vue";
import { useI18n } from "vue-i18n";

import DateTimeInput from "@/components/shared/DateTimeInput.vue";
import type { DateTimeProperties } from "@/types/fields";

const { t } = useI18n();

const props = defineProps<{
  modelValue: DateTimeProperties;
}>();

const maximumValue = computed<Date | undefined>(() => (props.modelValue.maximumValue ? new Date(props.modelValue.maximumValue) : undefined));
const minimumValue = computed<Date | undefined>(() => (props.modelValue.minimumValue ? new Date(props.modelValue.minimumValue) : undefined));

const emit = defineEmits<{
  (e: "update:model-value", value: DateTimeProperties): void;
}>();

function setMaximumValue(maximumValue: Date | undefined): void {
  const value: DateTimeProperties = { ...props.modelValue, maximumValue: maximumValue?.toISOString() };
  emit("update:model-value", value);
}
function setMinimumValue(minimumValue: Date | undefined): void {
  const value: DateTimeProperties = { ...props.modelValue, minimumValue: minimumValue?.toISOString() };
  emit("update:model-value", value);
}
</script>

<template>
  <div>
    <h3>{{ t("fields.types.properties", { type: t("fields.types.dataType.options.DateTime") }) }}</h3>
    <div class="row">
      <DateTimeInput
        class="col"
        floating
        id="minimum-value"
        label="fields.types.dateTime.minimumValue"
        :max="maximumValue"
        :model-value="minimumValue"
        placeholder="fields.types.dateTime.minimumValue"
        @update:model-value="setMinimumValue"
      />
      <DateTimeInput
        class="col"
        floating
        id="maximum-value"
        label="fields.types.dateTime.maximumValue"
        :min="minimumValue"
        :model-value="maximumValue"
        placeholder="fields.types.dateTime.maximumValue"
        @update:model-value="setMaximumValue"
      />
    </div>
  </div>
</template>
