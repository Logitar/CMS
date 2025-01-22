<script setup lang="ts">
import type { SelectOption } from "logitar-vue3-ui";
import { arrayUtils } from "logitar-js";
import { computed, onMounted, ref } from "vue";

import AppSelect from "@/components/shared/AppSelect.vue";
import type { FieldType, SearchFieldTypesPayload } from "@/types/fields";
import type { SearchResults } from "@/types/search";
import { formatFieldType } from "@/helpers/format";
import { searchFieldTypes } from "@/api/fieldTypes";

const { orderBy } = arrayUtils;

withDefaults(
  defineProps<{
    disabled?: boolean | string;
    id?: string;
    label?: string;
    modelValue?: string;
    placeholder?: string;
    required?: boolean | string;
  }>(),
  {
    id: "field-type",
    label: "fields.types.select.label",
    placeholder: "fields.types.select.placeholder",
  },
);

const fieldTypes = ref<FieldType[]>([]);

const options = computed<SelectOption[]>(() =>
  orderBy(
    fieldTypes.value.map((fieldType) => ({
      text: formatFieldType(fieldType),
      value: fieldType.id,
    })),
    "text",
  ),
);

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "selected", value: FieldType | undefined): void;
  (e: "update:model-value", value: string | undefined): void;
}>();

function onSelected(id: string | undefined): void {
  emit("update:model-value", id);

  const index: number = fieldTypes.value.findIndex((fieldType) => fieldType.id === id);
  if (index >= 0) {
    emit("selected", fieldTypes.value[index]);
  }
}

onMounted(async () => {
  try {
    const payload: SearchFieldTypesPayload = {
      ids: [],
      search: { operator: "And", terms: [] },
      sort: [{ field: "DisplayName", isDescending: false }],
      skip: 0,
      limit: 0,
    };
    const results: SearchResults<FieldType> = await searchFieldTypes(payload);
    fieldTypes.value = results.items;
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
    :label="label"
    :model-value="modelValue?.toString()"
    :options="options"
    :placeholder="placeholder ?? 'any'"
    :required="required"
    @update:model-value="onSelected"
  />
</template>
