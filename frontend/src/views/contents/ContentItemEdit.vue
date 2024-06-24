<script setup lang="ts">
import { TarTab, TarTabs, type TabOptions, type TabContainerOptions } from "logitar-vue3-ui";
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useRoute, useRouter } from "vue-router";

import AppBackButton from "@/components/shared/AppBackButton.vue";
import AppDelete from "@/components/shared/AppDelete.vue";
import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import ContentTypeSelect from "@/components/contentTypes/ContentTypeSelect.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { ApiError } from "@/types/api";
import type { ContentItem } from "@/types/contents";
import type { ContentType } from "@/types/contentTypes";
import { formatContentItem } from "@/helpers/displayUtils";
import { handleErrorKey } from "@/inject/App";
import { readContent } from "@/api/contents";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();

const content = ref<ContentItem>();
const isDeleting = ref<boolean>(false);
const uniqueName = ref<string>("");

const contentType = computed<ContentType | undefined>(() => content.value?.contentType);
const formatted = computed<string>(() => (content.value ? formatContentItem(content.value) : ""));
const hasChanges = computed<boolean>(() => Boolean(content.value) && uniqueName.value !== (content.value?.invariant.uniqueName ?? ""));

function setModel(model: ContentItem): void {
  content.value = model;
  uniqueName.value = model.invariant.uniqueName;
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
  if (content.value && !isDeleting.value) {
    isDeleting.value = true;
    try {
      // await deleteContent(content.value.id); // ISSUE: https://github.com/Logitar/CMS/issues/26
      hideModal();
      toasts.success("contents.deleted");
      router.push({ name: "ContentItemList" });
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
    <template v-if="content && contentType">
      <h1>{{ formatted }}</h1>
      <StatusDetail :aggregate="content" />
      <ContentTypeSelect disabled :model-value="contentType.id" />
      <div v-if="contentType.isInvariant">
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
          <UniqueNameInput required v-model="uniqueName" />
        </form>
      </div>
      <div v-else>
        <!-- TODO(fpion): implement -->
      </div>
    </template>
  </main>
</template>
