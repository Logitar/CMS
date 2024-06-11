<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import DataTypeSelect from "./DataTypeSelect.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { DataType, FieldType } from "@/types/fields";
import { createFieldType } from "@/api/fields";

const { t } = useI18n();

const dataType = ref<DataType>("String");
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const uniqueName = ref<string>("");

function hide(): void {
  modalRef.value?.hide();
}

const emit = defineEmits<{
  (e: "created", value: FieldType): void;
  (e: "error", value: unknown): void;
}>();

const { handleSubmit, isSubmitting, resetForm } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const fieldType = await createFieldType({
      uniqueName: uniqueName.value,
      booleanProperties: dataType.value === "Boolean" ? {} : undefined,
      dateTimeProperties: dataType.value === "DateTime" ? {} : undefined,
      numberProperties: dataType.value === "Number" ? {} : undefined,
      stringProperties: dataType.value === "String" ? {} : undefined,
      textProperties: dataType.value === "Text" ? { contentType: "text/plain" } : undefined,
    });
    emit("created", fieldType);
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
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" :data-bs-target="`#create-field-type`" />
    <TarModal :close="t('actions.close')" id="create-field-type" ref="modalRef" :title="t('fields.types.title.new')">
      <form @submit.prevent="onSubmit">
        <DataTypeSelect required v-model="dataType" />
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
          variant="success"
          @click="onSubmit"
        />
      </template>
    </TarModal>
  </span>
</template>
