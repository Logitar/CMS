<script setup lang="ts">
import { TarCheckbox, type CheckboxOptions } from "logitar-vue3-ui";
import { computed } from "vue";
import { nanoid } from "nanoid";
import { parsingUtils } from "logitar-js";
import { useI18n } from "vue-i18n";

const { parseBoolean } = parsingUtils;
const { t } = useI18n();

const props = withDefaults(
  defineProps<
    CheckboxOptions & {
      raw?: boolean | string;
      tight?: boolean | string;
    }
  >(),
  {
    id: () => nanoid(),
  },
);

const isRaw = computed<boolean>(() => parseBoolean(props.raw) ?? false);
const isTight = computed<boolean>(() => parseBoolean(props.tight) ?? false);

defineEmits<{
  (e: "update:model-value", value: boolean): void;
}>();
</script>

<template>
  <div :class="{ 'mb-3': !isTight }">
    <TarCheckbox
      :aria-label="ariaLabel"
      :disabled="disabled"
      :id="id"
      :inline="inline"
      :label="label ? (isRaw ? label : t(label)) : undefined"
      :model-value="modelValue"
      :name="name"
      :required="required"
      :reverse="reverse"
      :role="role"
      :switch="props.switch"
      :value="value"
      @update:model-value="$emit('update:model-value', $event)"
    />
  </div>
</template>
