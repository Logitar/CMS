<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { ContentItem, CreateContentPayload } from "@/types/contents";
import { createContentItem } from "@/api/contents";

const { t } = useI18n();

const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const payload = ref<CreateContentPayload>({ contentTypeId: "", uniqueName: "" });

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: ContentItem): void;
  (e: "error", value: unknown): void;
}>();

const { handleSubmit, isSubmitting, resetForm } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const contentItem: ContentItem = await createContentItem(payload.value);
    emit("created", contentItem);
    hide();
  } catch (e: unknown) {
    emit("error", e);
  }
});

function onCancel(): void {
  resetForm();
  hide();
}
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" :data-bs-target="`#create-content-item`" />
    <TarModal :close="t('actions.close')" id="create-content-item" ref="modalRef" :title="t('contents.items.title.new')">
      <form @submit.prevent="onSubmit">
        <!-- TODO(fpion): ContentTypeSelect -->
        <!-- TODO(fpion): LanguageSelect -->
        <UniqueNameInput required v-model="payload.uniqueName" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isSubmitting"
          icon="fas fa-plus"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t('actions.create')"
          variant="success"
          @click="onSubmit"
        />
      </template>
    </TarModal>
  </span>
</template>
