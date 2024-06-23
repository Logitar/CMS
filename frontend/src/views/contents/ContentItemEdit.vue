<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useRoute, useRouter } from "vue-router";

import AppBackButton from "@/components/shared/AppBackButton.vue";
import AppDelete from "@/components/shared/AppDelete.vue";
import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import type { ApiError } from "@/types/api";
import type { ContentItem } from "@/types/contents";
import { handleErrorKey } from "@/inject/App";
import { readContent } from "@/api/contents";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();

const content = ref<ContentItem>();
const isDeleting = ref<boolean>(false);

const formatted = computed<string>(() => /*content.value ? formatContent(content.value) :*/ ""); // TODO(fpion): implement
const hasChanges = computed<boolean>(() => Boolean(content.value) && false); // TODO(fpion): implement

function setModel(model: ContentItem): void {
  content.value = model;
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (content.value) {
    try {
      // const updatedContent = await replaceContent(content.value.id, {}, content.value.version);
      // setModel(updatedContent); // TODO(fpion): implement
      toasts.success("contentTypes.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

function onDelete(hideModal: () => void): void {
  alert("Deleting contents is not implemented.");
  hideModal();
}

onMounted(async () => {
  try {
    const id: string = route.params.id.toString();
    const contentType = await readContent(id);
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
    <template v-if="content">
      <h1>{{ formatted }}</h1>
      <StatusDetail :aggregate="content" />
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
        <!-- TODO(fpion): implement -->
      </form>
    </template>
  </main>
</template>
