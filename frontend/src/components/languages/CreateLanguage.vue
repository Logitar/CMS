<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import LocaleSelect from "@/components/users/LocaleSelect.vue";
import type { CreateLanguagePayload, Language } from "@/types/languages";
import { createLanguage } from "@/api/languages";

const { t } = useI18n();

const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const payload = ref<CreateLanguagePayload>({ locale: "" });

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: Language): void;
  (e: "error", value: unknown): void;
}>();

const { handleSubmit, isSubmitting, resetForm } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const language: Language = await createLanguage(payload.value);
    emit("created", language);
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
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" :data-bs-target="`#create-language`" />
    <TarModal :close="t('actions.close')" id="create-language" ref="modalRef" :title="t('languages.title.new')">
      <form @submit.prevent="onSubmit">
        <LocaleSelect label="languages.locale.label" placeholder="languages.locale.placeholder" required v-model="payload.locale" />
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
