<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { ContentType } from "@/types/contentTypes";
import { formatContentType } from "@/helpers/displayUtils";
import { searchContentTypes } from "@/api/contentTypes";

const { orderBy } = arrayUtils;

withDefaults(
  defineProps<{
    disabled?: boolean | string;
    id?: string;
    modelValue?: string;
    required?: boolean | string;
  }>(),
  {
    id: "content-type",
  },
);

const contentTypes = ref<ContentType[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    contentTypes.value.map((contentType) => ({
      text: formatContentType(contentType),
      value: contentType.id,
    })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value?: ContentType): void;
  (e: "update:model-value", value?: string): void;
}>();

function onModelValueUpdate(id: string): void {
  emit("update:model-value", id);
  const contentType: ContentType | undefined = contentTypes.value.find((contentType) => contentType.id === id);
  emit("selected", contentType);
}

onMounted(async () => {
  try {
    const results = await searchContentTypes({});
    contentTypes.value = results.items;
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <AppSelect
    :disabled="disabled"
    floating
    :id="id"
    label="contentTypes.select.label"
    :model-value="modelValue"
    :options="options"
    placeholder="contentTypes.select.placeholder"
    :required="required"
    :show-status="required ? 'touched' : 'never'"
    @update:model-value="onModelValueUpdate"
  />
</template>
