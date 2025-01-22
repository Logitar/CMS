<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import ContentTypeSelect from "./ContentTypeSelect.vue";
import LanguageSelect from "@/components/languages/LanguageSelect.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { CreateOrReplaceContentPayload, Content, ContentType } from "@/types/contents";
import type { Language } from "@/types/languages";
import { CONTENT_UNIQUE_NAME_CHARACTERS } from "@/helpers/constants";
import { createContent } from "@/api/contents";

const { t } = useI18n();

const contentType = ref<ContentType>();
const language = ref<Language>();
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const uniqueName = ref<string>("");

function hide(): void {
  modalRef.value?.hide();
}

function reset(): void {
  contentType.value = undefined;
  language.value = undefined;
  uniqueName.value = "";
}

const emit = defineEmits<{
  (e: "created", value: Content): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  reset();
  hide();
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (contentType.value) {
    try {
      const payload: CreateOrReplaceContentPayload = {
        contentTypeId: contentType.value.id,
        uniqueName: uniqueName.value,
        fieldValues: [],
      };
      const content: Content = await createContent(language.value?.id, payload);
      emit("created", content);
      reset();
      hide();
    } catch (e: unknown) {
      emit("error", e);
    }
  }
});

function setContentType(value?: ContentType): void {
  contentType.value = value;
  language.value = undefined;
}
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-content" />
    <TarModal :close="t('actions.close')" id="create-content" ref="modalRef" :title="t('contents.items.create')">
      <form>
        <ContentTypeSelect :model-value="contentType?.id" required @selected="setContentType" />
        <LanguageSelect
          :disabled="!contentType || contentType.isInvariant"
          :model-value="language?.id"
          :required="Boolean(contentType && !contentType.isInvariant)"
          @selected="language = $event"
        />
        <UniqueNameInput :allowed-characters="CONTENT_UNIQUE_NAME_CHARACTERS" required v-model="uniqueName" />
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isSubmitting"
          icon="fas fa-plus"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t('actions.create')"
          type="submit"
          variant="success"
          @click="onSubmit"
        />
      </template>
    </TarModal>
  </span>
</template>
