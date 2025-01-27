<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, ref, watch } from "vue";
import { useForm } from "vee-validate";

import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import ContentFieldValueConflicts from "./ContentFieldValueConflicts.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import FieldValueEdit from "@/components/fields/FieldValueEdit.vue";
import MissingFieldValues from "./MissingFieldValues.vue";
import PublishButton from "./PublishButton.vue";
import StatusInfo from "@/components/shared/StatusInfo.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import UnpublishButton from "./UnpublishButton.vue";
import type { ApiError, ProblemDetails } from "@/types/api";
import type { Content, ContentLocale, CreateOrReplaceContentPayload } from "@/types/contents";
import type { FieldDefinition, FieldValue } from "@/types/fields";
import { CONTENT_UNIQUE_NAME_ALLOWED_CHARACTERS } from "@/constants/allowedCharacters";
import { ErrorCodes } from "@/enums/errorCodes";
import { StatusCodes } from "@/enums/statusCodes";
import { isError } from "@/helpers/errors";
import { publishContent, replaceContent, unpublishContent } from "@/api/contents";

const { orderBy } = arrayUtils;

const props = defineProps<{
  content: Content;
  locale: ContentLocale;
}>();

const contentFieldValueConflicts = ref<string[]>([]);
const description = ref<string>("");
const displayName = ref<string>("");
const fieldValues = ref<Map<string, string>>(new Map<string, string>());
const isPublishing = ref<boolean>(false);
const missingFieldValues = ref<string[]>([]);
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const canPublish = computed<boolean>(() => !hasChanges.value && props.locale.revision !== props.locale.publishedRevision);
const canUnpublish = computed<boolean>(() => !hasChanges.value && props.locale.isPublished);
const fieldDefinitions = computed<FieldDefinition[]>(() =>
  orderBy(
    props.content.contentType.fields.filter(({ isInvariant }) => isInvariant === !props.locale.language),
    "order",
  ),
);
const hasChanges = computed<boolean>(
  () =>
    uniqueName.value !== props.locale.uniqueName ||
    displayName.value !== (props.locale.displayName ?? "") ||
    description.value !== (props.locale.description ?? "") ||
    JSON.stringify([...fieldValues.value.entries()].map(([id, value]) => ({ id, value }) as FieldValue)) !== JSON.stringify(props.locale.fieldValues),
);

function reset(): void {
  uniqueNameAlreadyUsed.value = false;
  missingFieldValues.value = [];
  contentFieldValueConflicts.value = [];
  uniqueName.value = props.locale.uniqueName;
  displayName.value = props.locale.displayName ?? "";
  description.value = props.locale.description ?? "";

  fieldValues.value.clear();
  props.locale.fieldValues.forEach(({ id, value }) => fieldValues.value.set(id, value));
}

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "published", value: Content): void;
  (e: "unpublished", value: Content): void;
  (e: "saved", value: Content): void;
}>();

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  uniqueNameAlreadyUsed.value = false;
  contentFieldValueConflicts.value = [];
  try {
    const payload: CreateOrReplaceContentPayload = {
      uniqueName: uniqueName.value,
      displayName: displayName.value,
      description: description.value,
      fieldValues: [...fieldValues.value.entries()].map(([id, value]) => ({ id, value }) as FieldValue),
    };
    const content: Content = await replaceContent(props.content.id, props.locale.language?.id, payload);
    emit("saved", content);
  } catch (e: unknown) {
    if (isError(e, StatusCodes.Conflict, ErrorCodes.ContentUniqueNameAlreadyUsed)) {
      uniqueNameAlreadyUsed.value = true;
    } else if (isError(e, StatusCodes.Conflict, ErrorCodes.ContentFieldValueConflict)) {
      const apiError = e as ApiError;
      const problemDetails = apiError.data as ProblemDetails;
      Object.keys(problemDetails.error?.data.ConflictIds).forEach((fieldId) => {
        const field: FieldDefinition | undefined = props.content.contentType.fields.find(({ id }) => id === fieldId);
        if (field) {
          contentFieldValueConflicts.value.push(field.displayName ?? field.uniqueName);
        }
      });
    } else {
      emit("error", e);
    }
  }
});

async function onPublish(): Promise<void> {
  if (!isPublishing.value) {
    isPublishing.value = true;
    missingFieldValues.value = [];
    try {
      const content: Content = await publishContent(props.content.id, props.locale.language?.id);
      emit("published", content);
    } catch (e: unknown) {
      if (isError(e, StatusCodes.BadRequest, ErrorCodes.Validation)) {
        const apiError = e as ApiError;
        const problemDetails = apiError.data as ProblemDetails;
        problemDetails.errors?.forEach(({ attemptedValue }) => {
          if (typeof attemptedValue === "string") {
            const field: FieldDefinition | undefined = props.content.contentType.fields.find(({ id }) => id === attemptedValue);
            if (field) {
              missingFieldValues.value.push(field.displayName ?? field.uniqueName);
            }
          }
        });
      } else {
        emit("error", e);
      }
    } finally {
      isPublishing.value = false;
    }
  }
}
async function onUnpublish(): Promise<void> {
  if (!isPublishing.value) {
    isPublishing.value = true;
    try {
      const content: Content = await unpublishContent(props.content.id, props.locale.language?.id);
      emit("unpublished", content);
    } catch (e: unknown) {
      emit("error", e);
    } finally {
      isPublishing.value = false;
    }
  }
}

function getFieldValue(id: string): string | undefined {
  return fieldValues.value.get(id);
}
function setFieldValue(id: string, value: string | undefined): void {
  fieldValues.value.set(id, value ?? "");
}

watch(() => props.locale, reset, { deep: true, immediate: true });
</script>

<template>
  <form @submit.prevent="onSubmit">
    <p v-if="locale.revision > 0">
      <StatusInfo :actor="locale.createdBy" :date="locale.createdOn" format="status.createdOn" />
      <br />
      <StatusInfo :actor="locale.updatedBy" :date="locale.updatedOn" format="status.updatedOn" :revision="locale.revision" />
      <template v-if="locale.publishedRevision && locale.publishedBy && locale.publishedOn">
        <br />
        <StatusInfo :actor="locale.publishedBy" :date="locale.publishedOn" format="contents.items.publishedOn" :revision="locale.publishedRevision" />
      </template>
    </p>
    <div class="mb-3">
      <AppSaveButton class="me-1" :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
      <template v-if="locale.revision > 0">
        <PublishButton class="mx-1" :disabled="!canPublish" :loading="isPublishing" @click="onPublish" />
        <UnpublishButton class="ms-1" :disabled="!canUnpublish" :loading="isPublishing" @click="onUnpublish" />
      </template>
    </div>
    <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
    <MissingFieldValues v-model="missingFieldValues" />
    <ContentFieldValueConflicts v-model="contentFieldValueConflicts" />
    <div class="row">
      <UniqueNameInput
        class="col"
        :allowed-characters="CONTENT_UNIQUE_NAME_ALLOWED_CHARACTERS"
        :id="`unique-name-${locale.language?.id ?? 'invariant'}`"
        required
        v-model="uniqueName"
      />
      <DisplayNameInput class="col" :id="`display-name-${locale.language?.id ?? 'invariant'}`" v-model="displayName" />
    </div>
    <DescriptionTextarea :id="`description-${locale.language?.id ?? 'invariant'}`" rows="5" v-model="description" />
    <FieldValueEdit
      v-for="fieldDefinition in fieldDefinitions"
      :key="fieldDefinition.id"
      :definition="fieldDefinition"
      :language="locale.language"
      :model-value="getFieldValue(fieldDefinition.id)"
      @update:model-value="setFieldValue(fieldDefinition.id, $event)"
    />
  </form>
</template>
