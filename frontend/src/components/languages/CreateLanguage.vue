<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import LocaleSelect from "@/components/shared/LocaleSelect.vue";
import type { CreateOrReplaceLanguagePayload, Language } from "@/types/languages";
import { createLanguage } from "@/api/languages";
const { t } = useI18n();

const locale = ref<string>("");
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

function hide(): void {
  modalRef.value?.hide();
}

function reset(): void {
  locale.value = "";
}

const emit = defineEmits<{
  (e: "created", value: Language): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  reset();
  hide();
}
const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const payload: CreateOrReplaceLanguagePayload = {
      locale: locale.value,
    };
    const language: Language = await createLanguage(payload);
    emit("created", language);
    reset();
    hide();
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-language" />
    <TarModal :close="t('actions.close')" id="create-language" ref="modalRef" :title="t('languages.create')">
      <form>
        <LocaleSelect required v-model="locale" />
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