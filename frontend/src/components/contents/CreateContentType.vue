<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import IdentifierInput from "@/components/shared/IdentifierInput.vue";
import InvariantCheckbox from "./InvariantCheckbox.vue";
import type { ContentType } from "@/types/contents";
import { createContentType } from "@/api/contents";

const { t } = useI18n();

const isInvariant = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const uniqueName = ref<string>("");

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: ContentType): void;
  (e: "error", value: unknown): void;
}>();

const { handleSubmit, isSubmitting, resetForm } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const contentType = await createContentType({
      isInvariant: isInvariant.value,
      uniqueName: uniqueName.value,
    });
    emit("created", contentType);
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
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" :data-bs-target="`#create-content-type`" />
    <TarModal :close="t('actions.close')" id="create-content-type" ref="modalRef" :title="t('contents.types.title.new')">
      <form @submit.prevent="onSubmit">
        <InvariantCheckbox class="mb-3" v-model="isInvariant" />
        <IdentifierInput required v-model="uniqueName" />
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
