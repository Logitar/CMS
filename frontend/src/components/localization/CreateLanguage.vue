<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import LocaleSelect from "@/components/users/LocaleSelect.vue";
import type { Language } from "@/types/localization";
import { createLanguage } from "@/api/localization";

const { t } = useI18n();

const locale = ref<string>("");
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);

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
    const language = await createLanguage({
      locale: locale.value,
    });
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
    <TarModal :close="t('actions.close')" id="create-language" ref="modalRef" :title="t('localization.languages.title.new')">
      <form @submit.prevent="onSubmit">
        <LocaleSelect label="localization.languages.locale.label" placeholder="localization.languages.locale.placeholder" required v-model="locale" />
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
