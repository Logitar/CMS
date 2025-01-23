<script setup lang="ts">
import { arrayUtils } from "logitar-js";
import { computed, ref, watch } from "vue";
import { useForm } from "vee-validate";

import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import FieldValueEdit from "@/components/fields/FieldValueEdit.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { Content, ContentLocale, CreateOrReplaceContentPayload } from "@/types/contents";
import type { FieldDefinition, FieldValue } from "@/types/fields";
import { CONTENT_UNIQUE_NAME_ALLOWED_CHARACTERS } from "@/constants/allowedCharacters";
import { ErrorCodes } from "@/enums/errorCodes";
import { StatusCodes } from "@/enums/statusCodes";
import { isError } from "@/helpers/errors";
import { replaceContent } from "@/api/contents";

const { orderBy } = arrayUtils;

const props = defineProps<{
  content: Content;
  locale: ContentLocale;
}>();

const description = ref<string>("");
const displayName = ref<string>("");
const fieldValues = ref<Map<string, string>>(new Map<string, string>());
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

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
  uniqueName.value = props.locale.uniqueName;
  displayName.value = props.locale.displayName ?? "";
  description.value = props.locale.description ?? "";

  fieldValues.value.clear();
  props.locale.fieldValues.forEach(({ id, value }) => fieldValues.value.set(id, value));
}

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "saved", value: Content): void;
}>();

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  uniqueNameAlreadyUsed.value = false;
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
    } else {
      emit("error", e);
    }
  }
});

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
    <div class="mb-3">
      <AppSaveButton :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
    </div>
    <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
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
      :model-value="getFieldValue(fieldDefinition.id)"
      @update:model-value="setFieldValue(fieldDefinition.id, $event)"
    />
  </form>
</template>
