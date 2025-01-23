<script setup lang="ts">
import { useI18n } from "vue-i18n";

import AppCheckbox from "@/components/shared/AppCheckbox.vue";
import ContentTypeSelect from "@/components/contents/ContentTypeSelect.vue";
import type { RelatedContentProperties } from "@/types/fields";

const { t } = useI18n();

const props = defineProps<{
  modelValue: RelatedContentProperties;
}>();

const emit = defineEmits<{
  (e: "update:model-value", value: RelatedContentProperties): void;
}>();

function setContentTypeId(contentTypeId: string): void {
  const value: RelatedContentProperties = { ...props.modelValue, contentTypeId };
  emit("update:model-value", value);
}
function setIsMultiple(isMultiple: boolean): void {
  const value: RelatedContentProperties = { ...props.modelValue, isMultiple };
  emit("update:model-value", value);
}
</script>

<template>
  <div>
    <h3>{{ t("fields.types.properties", { type: t("fields.types.dataType.options.RelatedContent") }) }}</h3>
    <AppCheckbox id="multiple" label="fields.types.select.isMultiple" :model-value="modelValue.isMultiple" @update:model-value="setIsMultiple" />
    <ContentTypeSelect :model-value="modelValue.contentTypeId" required @update:model-value="setContentTypeId($event ?? '')" />
  </div>
</template>
