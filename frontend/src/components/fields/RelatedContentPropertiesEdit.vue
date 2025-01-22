<script setup lang="ts">
import { TarCheckbox } from "logitar-vue3-ui";
import { useI18n } from "vue-i18n";

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
    <div class="my-3">
      <TarCheckbox id="multiple" :label="t('fields.types.select.isMultiple')" :model-value="modelValue.isMultiple" @update:model-value="setIsMultiple" />
    </div>
    <ContentTypeSelect :model-value="modelValue.contentTypeId" required @update:model-value="setContentTypeId($event ?? '')" />
  </div>
</template>
