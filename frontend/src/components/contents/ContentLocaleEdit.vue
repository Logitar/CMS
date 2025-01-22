<script setup lang="ts">
import { computed, ref, watch } from "vue";
import { useForm } from "vee-validate";

import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { Content, ContentLocale, CreateOrReplaceContentPayload } from "@/types/contents";
import { CONTENT_UNIQUE_NAME_ALLOWED_CHARACTERS } from "@/constants/allowedCharacters";
import { ErrorCodes } from "@/enums/errorCodes";
import { StatusCodes } from "@/enums/statusCodes";
import { isError } from "@/helpers/errors";
import { replaceContent } from "@/api/contents";

const props = defineProps<{
  contentId: string;
  locale: ContentLocale;
}>();

const description = ref<string>("");
const displayName = ref<string>("");
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const hasChanges = computed<boolean>(
  () =>
    uniqueName.value !== props.locale.uniqueName ||
    displayName.value !== (props.locale.displayName ?? "") ||
    description.value !== (props.locale.description ?? ""),
);

function reset(): void {
  uniqueNameAlreadyUsed.value = false;
  uniqueName.value = props.locale.uniqueName;
  displayName.value = props.locale.displayName ?? "";
  description.value = props.locale.description ?? "";
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
      fieldValues: [],
    };
    const content: Content = await replaceContent(props.contentId, props.locale.language?.id, payload);
    emit("saved", content);
  } catch (e: unknown) {
    if (isError(e, StatusCodes.Conflict, ErrorCodes.ContentUniqueNameAlreadyUsed)) {
      uniqueNameAlreadyUsed.value = true;
    } else {
      emit("error", e);
    }
  }
});

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
    <!-- TODO(fpion): Field Values -->
  </form>
</template>
