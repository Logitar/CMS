<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { ref } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import ContentTypeSelect from "@/components/contents/ContentTypeSelect.vue";
import DataTypeSelect from "./DataTypeSelect.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { ContentType } from "@/types/contents";
import type { CreateOrReplaceFieldTypePayload, DataType, FieldType } from "@/types/fields";
import { FIELD_TYPE_UNIQUE_NAME_CHARACTERS } from "@/helpers/constants";
import { createFieldType } from "@/api/fieldTypes";

const { t } = useI18n();

const contentType = ref<ContentType>();
const dataType = ref<DataType>();
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const uniqueName = ref<string>("");

function hide(): void {
  modalRef.value?.hide();
}

function reset(): void {
  uniqueName.value = "";
  dataType.value = undefined;
  contentType.value = undefined;
}

const emit = defineEmits<{
  (e: "created", value: FieldType): void;
  (e: "error", value: unknown): void;
}>();

function onCancel(): void {
  reset();
  hide();
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  try {
    const payload: CreateOrReplaceFieldTypePayload = {
      uniqueName: uniqueName.value,
    };
    switch (dataType.value) {
      case "Boolean":
        payload.boolean = {};
        break;
      case "DateTime":
        payload.dateTime = {};
        break;
      case "Number":
        payload.number = {};
        break;
      case "RelatedContent":
        payload.relatedContent = { contentTypeId: contentType.value?.id ?? "", isMultiple: false };
        break;
      case "RichText":
        payload.richText = { contentType: "text/plain" };
        break;
      case "Select":
        payload.select = { isMultiple: false, options: [] };
        break;
      case "String":
        payload.string = {};
        break;
      case "Tags":
        payload.tags = {};
        break;
    }
    const fieldType: FieldType = await createFieldType(payload);
    emit("created", fieldType);
    reset();
    hide();
  } catch (e: unknown) {
    emit("error", e);
  }
});
</script>

<template>
  <span>
    <TarButton icon="fas fa-plus" :text="t('actions.create')" variant="success" data-bs-toggle="modal" data-bs-target="#create-field-type" />
    <TarModal :close="t('actions.close')" id="create-field-type" ref="modalRef" size="large" :title="t('fields.types.create')">
      <form>
        <UniqueNameInput :allowed-characters="FIELD_TYPE_UNIQUE_NAME_CHARACTERS" required v-model="uniqueName" />
        <DataTypeSelect required v-model="dataType" />
        <ContentTypeSelect v-if="dataType === 'RelatedContent'" :model-value="contentType?.id" required @selected="contentType = $event" />
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
