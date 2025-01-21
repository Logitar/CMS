<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useRoute, useRouter } from "vue-router";

import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import InvariantCheckbox from "@/components/contents/InvariantCheckbox.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { ApiError } from "@/types/api";
import type { CreateOrReplaceContentTypePayload, ContentType } from "@/types/contents";
import { handleErrorKey } from "@/inject/App";
import { readContentType, replaceContentType } from "@/api/contents";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();

const description = ref<string>("");
const displayName = ref<string>("");
const contentType = ref<ContentType>();
const isInvariant = ref<boolean>(false);
const uniqueName = ref<string>("");

const hasChanges = computed<boolean>(() =>
  Boolean(
    contentType.value &&
      (isInvariant.value !== contentType.value.isInvariant ||
        uniqueName.value !== contentType.value.uniqueName ||
        displayName.value !== (contentType.value.displayName ?? "") ||
        description.value !== (contentType.value.description ?? "")),
  ),
);

function setModel(model: ContentType): void {
  contentType.value = model;
  description.value = model.description ?? "";
  displayName.value = model.displayName ?? "";
  isInvariant.value = model.isInvariant;
  uniqueName.value = model.uniqueName;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (contentType.value) {
    try {
      const payload: CreateOrReplaceContentTypePayload = {
        isInvariant: isInvariant.value,
        uniqueName: uniqueName.value,
        displayName: displayName.value,
        description: description.value,
      };
      const updatedContentType: ContentType = await replaceContentType(contentType.value.id, payload, contentType.value.version);
      setModel(updatedContentType);
      toasts.success("contents.types.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const language = await readContentType(id);
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
    <template v-if="contentType">
      <h1>{{ contentType.displayName ?? contentType.uniqueName }}</h1>
      <StatusDetail :aggregate="contentType" />
      <form @submit.prevent="onSubmit">
        <InvariantCheckbox v-model="isInvariant" />
        <div class="row">
          <UniqueNameInput class="col" required v-model="uniqueName" />
          <DisplayNameInput class="col" v-model="displayName" />
        </div>
        <DescriptionTextarea v-model="description" />
        <div class="mb-3">
          <AppSaveButton :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
        </div>
      </form>
    </template>
  </main>
</template>
