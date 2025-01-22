<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useRoute, useRouter } from "vue-router";

import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import DateTimePropertiesEdit from "@/components/fields/DateTimePropertiesEdit.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import NumberPropertiesEdit from "@/components/fields/NumberPropertiesEdit.vue";
import RelatedContentPropertiesEdit from "@/components/fields/RelatedContentPropertiesEdit.vue";
import RichTextPropertiesEdit from "@/components/fields/RichTextPropertiesEdit.vue";
import SelectPropertiesEdit from "@/components/fields/SelectPropertiesEdit.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import StringPropertiesEdit from "@/components/fields/StringPropertiesEdit.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { ApiError } from "@/types/api";
import type {
  CreateOrReplaceFieldTypePayload,
  DateTimeProperties,
  FieldType,
  NumberProperties,
  RelatedContentProperties,
  RichTextProperties,
  SelectProperties,
  StringProperties,
} from "@/types/fields";
import { FIELD_TYPE_UNIQUE_NAME_CHARACTERS } from "@/helpers/constants";
import {
  compareDateTimeProperties,
  compareNumberProperties,
  compareRelatedContentProperties,
  compareRichTextProperties,
  compareSelectProperties,
  compareStringProperties,
} from "@/helpers/fields";
import { handleErrorKey } from "@/inject/App";
import { readFieldType, replaceFieldType } from "@/api/fieldTypes";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();

const dateTime = ref<DateTimeProperties>({});
const description = ref<string>("");
const displayName = ref<string>("");
const fieldType = ref<FieldType>();
const number = ref<NumberProperties>({});
const relatedContent = ref<RelatedContentProperties>({ contentTypeId: "", isMultiple: false });
const richText = ref<RichTextProperties>({ contentType: "text/plain" });
const select = ref<SelectProperties>({ isMultiple: false, options: [] });
const string = ref<StringProperties>({});
const uniqueName = ref<string>("");

const hasChanges = computed<boolean>(() =>
  Boolean(
    fieldType.value &&
      (uniqueName.value !== fieldType.value.uniqueName ||
        displayName.value !== (fieldType.value.displayName ?? "") ||
        description.value !== (fieldType.value.description ?? "") ||
        (fieldType.value.dataType === "DateTime" && !compareDateTimeProperties(dateTime.value, fieldType.value.dateTime)) ||
        (fieldType.value.dataType === "Number" && !compareNumberProperties(number.value, fieldType.value.number)) ||
        (fieldType.value.dataType === "RelatedContent" && !compareRelatedContentProperties(relatedContent.value, fieldType.value.relatedContent)) ||
        (fieldType.value.dataType === "RichText" && !compareRichTextProperties(richText.value, fieldType.value.richText)) ||
        (fieldType.value.dataType === "Select" && !compareSelectProperties(select.value, fieldType.value.select)) ||
        (fieldType.value.dataType === "String" && !compareStringProperties(string.value, fieldType.value.string))),
  ),
);

function setModel(model: FieldType): void {
  fieldType.value = model;
  dateTime.value = model.dateTime ? { ...model.dateTime } : {};
  description.value = model.description ?? "";
  displayName.value = model.displayName ?? "";
  number.value = model.number ? { ...model.number } : {};
  relatedContent.value = model.relatedContent ? { ...model.relatedContent } : { contentTypeId: "", isMultiple: false };
  richText.value = model.richText ? { ...model.richText } : { contentType: "text/plain" };
  select.value = model.select ? { ...model.select, options: [...model.select.options] } : { isMultiple: false, options: [] };
  string.value = model.string ? { ...model.string } : {};
  uniqueName.value = model.uniqueName;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (fieldType.value) {
    try {
      const payload: CreateOrReplaceFieldTypePayload = {
        uniqueName: uniqueName.value,
        displayName: displayName.value,
        description: description.value,
      };
      switch (fieldType.value.dataType) {
        case "Boolean":
          payload.boolean = {};
          break;
        case "DateTime":
          payload.dateTime = dateTime.value;
          break;
        case "Number":
          payload.number = number.value;
          break;
        case "RelatedContent":
          payload.relatedContent = relatedContent.value;
          break;
        case "RichText":
          payload.richText = {
            contentType: richText.value.contentType,
            minimumLength: richText.value.minimumLength || undefined,
            maximumLength: richText.value.maximumLength || undefined,
          };
          break;
        case "Select":
          payload.select = select.value;
          break;
        case "String":
          payload.string = {
            minimumLength: string.value.minimumLength || undefined,
            maximumLength: string.value.maximumLength || undefined,
            pattern: string.value.pattern?.trim() ? string.value.pattern : undefined,
          };
          break;
        case "Tags":
          payload.tags = {};
          break;
      }
      const updatedFieldType: FieldType = await replaceFieldType(fieldType.value.id, payload, fieldType.value.version);
      setModel(updatedFieldType);
      toasts.success("fields.types.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const language = await readFieldType(id);
      setModel(language);
    }
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
      <h1>{{ fieldType.displayName ?? fieldType.uniqueName }}</h1>
      <StatusDetail :aggregate="fieldType" />
      <form @submit.prevent="onSubmit">
        <div class="row">
          <UniqueNameInput :allowed-characters="FIELD_TYPE_UNIQUE_NAME_CHARACTERS" class="col" required v-model="uniqueName" />
          <DisplayNameInput class="col" v-model="displayName" />
        </div>
        <DescriptionTextarea v-model="description" />
        <DateTimePropertiesEdit v-if="fieldType.dataType === 'DateTime'" v-model="dateTime" />
        <NumberPropertiesEdit v-else-if="fieldType.dataType === 'Number'" v-model="number" />
        <RelatedContentPropertiesEdit v-else-if="fieldType.dataType === 'RelatedContent'" v-model="relatedContent" />
        <RichTextPropertiesEdit v-else-if="fieldType.dataType === 'RichText'" v-model="richText" />
        <SelectPropertiesEdit v-else-if="fieldType.dataType === 'Select'" v-model="select" />
        <StringPropertiesEdit v-else-if="fieldType.dataType === 'String'" v-model="string" />
        <div class="mb-3">
          <AppSaveButton :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
        </div>
      </form>
    </template>
  </main>
</template>
