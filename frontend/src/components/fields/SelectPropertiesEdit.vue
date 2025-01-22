<script setup lang="ts">
import { TarButton, TarCheckbox } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

import SelectOptionEdit from "./SelectOptionEdit.vue";
import type { SelectOption, SelectProperties } from "@/types/fields";

const { t } = useI18n();

const props = defineProps<{
  modelValue: SelectProperties;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: SelectProperties): void;
}>();

function setIsMultiple(isMultiple: boolean): void {
  const value: SelectProperties = { ...props.modelValue, isMultiple };
  emit("update:model-value", value);
}

function addOption(): void {
  const value: SelectProperties = { ...props.modelValue };
  value.options.push({ isDisabled: false, isSelected: false, text: "" });
  emit("update:model-value", value);
}
function removeOption(index: number): void {
  const value: SelectProperties = { ...props.modelValue };
  value.options.splice(index, 1);
  emit("update:model-value", value);
}
function setOption(index: number, option: SelectOption): void {
  const value: SelectProperties = { ...props.modelValue };
  value.options.splice(index, 1, option);
  emit("update:model-value", value);
}
</script>

<template>
  <div>
    <h3>{{ t("fields.types.properties", { type: t("fields.types.dataType.options.Select") }) }}</h3>
    <div class="my-3">
      <TarCheckbox id="multiple" :label="t('fields.types.select.isMultiple')" :model-value="modelValue.isMultiple" @update:model-value="setIsMultiple" />
    </div>
    <h5>{{ t("fields.types.select.options.title") }}</h5>
    <div class="my-3">
      <TarButton icon="fas fa-plus" :text="t('actions.add')" variant="success" @click="addOption" />
    </div>
    <p v-if="modelValue.options.length < 1">{{ t("fields.types.select.options.empty") }}</p>
    <SelectOptionEdit
      v-for="(option, index) in modelValue.options"
      :key="index"
      :id="`option-${index}`"
      :model-value="option"
      @removed="removeOption(index)"
      @update:model-value="setOption(index, $event)"
    />
  </div>
</template>
