<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";
import { useRoute, useRouter } from "vue-router";

import AppBackButton from "@/components/shared/AppBackButton.vue";
import AppDelete from "@/components/shared/AppDelete.vue";
import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import BooleanPropertiesEdit from "@/components/fields/BooleanPropertiesEdit.vue";
import DataTypeSelect from "@/components/fields/DataTypeSelect.vue";
import DateTimePropertiesEdit from "@/components/fields/DateTimePropertiesEdit.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import NumberPropertiesEdit from "@/components/fields/NumberPropertiesEdit.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import StringPropertiesEdit from "@/components/fields/StringPropertiesEdit.vue";
import TextPropertiesEdit from "@/components/fields/TextPropertiesEdit.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { ApiError } from "@/types/api";
import type { BooleanProperties, DateTimeProperties, FieldType, NumberProperties, StringProperties, TextProperties } from "@/types/fields";
import { formatFieldType } from "@/helpers/displayUtils";
import { handleErrorKey } from "@/inject/App";
import { readFieldType, replaceFieldType } from "@/api/fields";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();
const { t } = useI18n();

const booleanProperties = ref<BooleanProperties>({});
const dateTimeProperties = ref<DateTimeProperties>({});
const description = ref<string>("");
const displayName = ref<string>("");
const fieldType = ref<FieldType>();
const isDeleting = ref<boolean>(false);
const numberProperties = ref<NumberProperties>({});
const stringProperties = ref<StringProperties>({});
const textProperties = ref<TextProperties>({ contentType: "text/plain" });
const uniqueName = ref<string>("");

const formatted = computed<string>(() => (fieldType.value ? formatFieldType(fieldType.value) : ""));
const hasChanges = computed<boolean>(
  () =>
    Boolean(fieldType.value) &&
    (uniqueName.value !== fieldType.value?.uniqueName ||
      displayName.value !== (fieldType.value.displayName ?? "") ||
      description.value !== (fieldType.value.description ?? "") ||
      hasPropertyChanges.value),
);
const hasPropertyChanges = computed<boolean>(() => {
  if (fieldType.value) {
    switch (fieldType.value.dataType) {
      case "DateTime": {
        const properties: DateTimeProperties = fieldType.value.dateTimeProperties ?? {};
        return (
          (properties.minimumValue ?? "") !== (dateTimeProperties.value.minimumValue ?? "") ||
          (properties.maximumValue ?? "") !== (dateTimeProperties.value.maximumValue ?? "")
        );
      }
      case "Number": {
        const properties: NumberProperties = fieldType.value.numberProperties ?? {};
        return (
          (properties.minimumValue ?? 0) !== (numberProperties.value.minimumValue ?? 0) ||
          (properties.maximumValue ?? 0) !== (numberProperties.value.maximumValue ?? 0)
        );
      }
      case "String": {
        const properties: StringProperties = fieldType.value.stringProperties ?? {};
        return (
          (properties.minimumLength ?? 0) !== (stringProperties.value.minimumLength ?? 0) ||
          (properties.maximumLength ?? 0) !== (stringProperties.value.maximumLength ?? 0) ||
          (properties.pattern ?? "") !== (stringProperties.value.pattern ?? "")
        );
      }
      case "Text": {
        const properties: TextProperties = fieldType.value.textProperties ?? { contentType: "text/plain" };
        return (
          properties.contentType !== textProperties.value.contentType ||
          (properties.minimumLength ?? 0) !== (textProperties.value.minimumLength ?? 0) ||
          (properties.maximumLength ?? 0) !== (textProperties.value.maximumLength ?? 0)
        );
      }
    }
  }
  return false;
});

function setModel(model: FieldType): void {
  fieldType.value = model;
  description.value = model.description ?? "";
  displayName.value = model.displayName ?? "";
  uniqueName.value = model.uniqueName;
  booleanProperties.value = { ...model.booleanProperties };
  dateTimeProperties.value = { ...model.dateTimeProperties };
  numberProperties.value = { ...model.numberProperties };
  stringProperties.value = { ...model.stringProperties };
  textProperties.value = { ...model.textProperties, contentType: model.textProperties?.contentType ?? "text/plain" };
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (fieldType.value) {
    try {
      const updatedFieldType = await replaceFieldType(
        fieldType.value.id,
        {
          uniqueName: uniqueName.value,
          displayName: displayName.value,
          description: description.value,
          booleanProperties: fieldType.value.dataType === "Boolean" ? booleanProperties.value : undefined,
          dateTimeProperties: fieldType.value.dataType === "DateTime" ? dateTimeProperties.value : undefined,
          numberProperties: fieldType.value.dataType === "Number" ? numberProperties.value : undefined,
          stringProperties: fieldType.value.dataType === "String" ? stringProperties.value : undefined,
          textProperties: fieldType.value.dataType === "Text" ? textProperties.value : undefined,
        },
        fieldType.value.version,
      );
      setModel(updatedFieldType);
      toasts.success("fields.types.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

function onDelete(hideModal: () => void): void {
  alert("Deleting field types is not implemented.");
  hideModal();
}

onMounted(async () => {
  try {
    const id: string = route.params.id.toString();
    const fieldType = await readFieldType(id);
    setModel(fieldType);
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === 404) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <main class="container">
    <template v-if="fieldType">
      <h1>{{ formatted }}</h1>
      <StatusDetail :aggregate="fieldType" />
      <form @submit.prevent="onSubmit">
        <div class="mb-3">
          <AppSaveButton class="me-1" :disabled="isSubmitting || !hasChanges" exists :loading="isSubmitting" />
          <AppBackButton class="mx-1" :has-changes="hasChanges" />
          <AppDelete
            class="ms-1"
            confirm="fields.types.delete.confirm"
            :displayName="formatted"
            :loading="isDeleting"
            title="fields.types.delete.title"
            @confirmed="onDelete"
          />
        </div>
        <div class="row">
          <DataTypeSelect disabled :model-value="fieldType.dataType" />
          <UniqueNameInput class="col-lg-6" required v-model="uniqueName" />
          <DisplayNameInput class="col-lg-6" v-model="displayName" />
        </div>
        <DescriptionTextarea v-model="description" />
        <h3>{{ t("fields.types.properties.title") }}</h3>
        <BooleanPropertiesEdit v-if="fieldType.dataType === 'Boolean'" v-model="booleanProperties" />
        <DateTimePropertiesEdit v-else-if="fieldType.dataType === 'DateTime'" v-model="dateTimeProperties" />
        <NumberPropertiesEdit v-else-if="fieldType.dataType === 'Number'" v-model="numberProperties" />
        <StringPropertiesEdit v-else-if="fieldType.dataType === 'String'" v-model="stringProperties" />
        <TextPropertiesEdit v-else-if="fieldType.dataType === 'Text'" v-model="textProperties" />
      </form>
    </template>
  </main>
</template>
