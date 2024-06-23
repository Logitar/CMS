<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useRoute, useRouter } from "vue-router";

import AppBackButton from "@/components/shared/AppBackButton.vue";
import AppDelete from "@/components/shared/AppDelete.vue";
import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import InvariantCheckbox from "@/components/contentTypes/InvariantCheckbox.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { ApiError } from "@/types/api";
import type { ContentType } from "@/types/contentTypes";
import { formatContentType } from "@/helpers/displayUtils";
import { handleErrorKey } from "@/inject/App";
import { readContentType, replaceContentType } from "@/api/contentTypes";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();

const contentType = ref<ContentType>();
const description = ref<string>("");
const displayName = ref<string>("");
const isDeleting = ref<boolean>(false);
const uniqueName = ref<string>("");

const formatted = computed<string>(() => (contentType.value ? formatContentType(contentType.value) : ""));
const hasChanges = computed<boolean>(
  () =>
    Boolean(contentType.value) &&
    (uniqueName.value !== contentType.value?.uniqueName ||
      displayName.value !== (contentType.value.displayName ?? "") ||
      description.value !== (contentType.value.description ?? "")),
);

function setModel(model: ContentType): void {
  contentType.value = model;
  description.value = model.description ?? "";
  displayName.value = model.displayName ?? "";
  uniqueName.value = model.uniqueName;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (contentType.value) {
    try {
      const updatedContentType = await replaceContentType(
        contentType.value.id,
        {
          uniqueName: uniqueName.value,
          displayName: displayName.value,
          description: description.value,
        },
        contentType.value.version,
      );
      setModel(updatedContentType);
      toasts.success("contentTypes.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

function onDelete(hideModal: () => void): void {
  if (contentType.value && !isDeleting.value) {
    isDeleting.value = true;
    try {
      // await deleteContentType(contentType.value.id); // ISSUE: https://github.com/Logitar/CMS/issues/23
      hideModal();
      toasts.success("contentTypes.deleted");
      router.push({ name: "ContentTypeList" });
    } catch (e: unknown) {
      handleError(e);
    } finally {
      isDeleting.value = false;
    }
  }
}

onMounted(async () => {
  try {
    const id: string = route.params.id.toString();
    const contentType = await readContentType(id);
    setModel(contentType);
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
    <template v-if="contentType">
      <h1>{{ formatted }}</h1>
      <StatusDetail :aggregate="contentType" />
      <form @submit.prevent="onSubmit">
        <div class="mb-3">
          <AppSaveButton class="me-1" :disabled="isSubmitting || !hasChanges" exists :loading="isSubmitting" />
          <AppBackButton class="mx-1" :has-changes="hasChanges" />
          <AppDelete
            class="ms-1"
            confirm="contentTypes.delete.confirm"
            :displayName="formatted"
            :loading="isDeleting"
            title="contentTypes.delete.title"
            @confirmed="onDelete"
          />
        </div>
        <InvariantCheckbox class="mb-3" disabled :model-value="contentType.isInvariant" />
        <div class="row">
          <UniqueNameInput class="col-lg-6" required v-model="uniqueName" />
          <DisplayNameInput class="col-lg-6" v-model="displayName" />
        </div>
        <DescriptionTextarea v-model="description" />
      </form>
    </template>
  </main>
</template>
