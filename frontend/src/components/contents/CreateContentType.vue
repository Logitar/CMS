<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import InvariantCheckbox from "./InvariantCheckbox.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { CreateOrReplaceContentTypePayload, ContentType } from "@/types/contents";
import { ErrorCodes } from "@/enums/errorCodes";
import { StatusCodes } from "@/enums/statusCodes";
import { createContentType } from "@/api/contentTypes";
import { isError } from "@/helpers/errors";

const { t } = useI18n();

const isInvariant = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

function hide(): void {
  modalRef.value?.hide();
}

function reset(): void {
  isInvariant.value = false;
  uniqueNameAlreadyUsed.value = false;
  uniqueName.value = "";
}

const emit = defineEmits<{
  (e: "created", value: ContentType): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  reset();
  hide();
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  uniqueNameAlreadyUsed.value = false;
  try {
    const payload: CreateOrReplaceContentTypePayload = {
      isInvariant: isInvariant.value,
      uniqueName: uniqueName.value,
    };
    const contentType: ContentType = await createContentType(payload);
    emit("created", contentType);
    reset();
    hide();
  } catch (e: unknown) {
    if (isError(e, StatusCodes.Conflict, ErrorCodes.UniqueNameAlreadyUsed)) {
      uniqueNameAlreadyUsed.value = true;
    } else {
      emit("error", e);
    }
  }
});
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-content-type" />
    <TarModal :close="t('actions.close')" id="create-content-type" ref="modalRef" :title="t('contents.types.create')">
      <form>
        <InvariantCheckbox v-model="isInvariant" />
        <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
        <UniqueNameInput required v-model="uniqueName" />
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
