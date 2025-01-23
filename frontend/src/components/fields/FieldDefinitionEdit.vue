<script setup lang="ts">
import { TarButton, TarModal } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import AppCheckbox from "@/components/shared/AppCheckbox.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import FieldTypeSelect from "./FieldTypeSelect.vue";
import PlaceholderInput from "./PlaceholderInput.vue";
import UniqueNameAlreadyUsed from "@/components/shared/UniqueNameAlreadyUsed.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { ContentType } from "@/types/contents";
import type { CreateOrReplaceFieldDefinitionPayload, FieldDefinition } from "@/types/fields";
import { ErrorCodes } from "@/enums/errorCodes";
import { StatusCodes } from "@/enums/statusCodes";
import { createFieldDefinition, replaceFieldDefinition } from "@/api/fieldDefinitions";
import { isError } from "@/helpers/errors";

const { t } = useI18n();

const props = defineProps<{
  contentType: ContentType;
  field?: FieldDefinition;
}>();

const description = ref<string>("");
const displayName = ref<string>("");
const fieldTypeId = ref<string>();
const isIndexed = ref<boolean>(false);
const isInvariant = ref<boolean>(false);
const isRequired = ref<boolean>(false);
const isUnique = ref<boolean>(false);
const modalRef = ref<InstanceType<typeof TarModal> | null>(null);
const placeholder = ref<string>("");
const uniqueName = ref<string>("");
const uniqueNameAlreadyUsed = ref<boolean>(false);

const modalId = computed<string>(() => (props.field ? `edit-field-definition-${props.field.id}` : "create-field-definition"));

function hide(): void {
  modalRef.value?.hide();
}

function reset(): void {
  fieldTypeId.value = props.field?.fieldType.id;
  uniqueNameAlreadyUsed.value = false;
  uniqueName.value = props.field?.uniqueName ?? "";
  displayName.value = props.field?.displayName ?? "";
  description.value = props.field?.description ?? "";
  placeholder.value = props.field?.placeholder ?? "";
  isInvariant.value = props.contentType.isInvariant ? true : props.field?.isInvariant ?? false;
  isRequired.value = props.field?.isRequired ?? false;
  isIndexed.value = props.field?.isIndexed ?? false;
  isUnique.value = props.field?.isUnique ?? false;
}

const emit = defineEmits<{
  (e: "error", value: unknown): void;
  (e: "saved", value: ContentType): void;
}>();

function onCancel(): void {
  reset();
  hide();
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  uniqueNameAlreadyUsed.value = false;
  try {
    const payload: CreateOrReplaceFieldDefinitionPayload = {
      fieldTypeId: fieldTypeId.value,
      isInvariant: isInvariant.value,
      isRequired: isRequired.value,
      isIndexed: isIndexed.value,
      isUnique: isUnique.value,
      uniqueName: uniqueName.value,
      displayName: displayName.value,
      description: description.value,
      placeholder: placeholder.value,
    };
    const contentType: ContentType = props.field
      ? await replaceFieldDefinition(props.contentType.id, props.field.id, payload)
      : await createFieldDefinition(props.contentType.id, payload);
    emit("saved", contentType);
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

watch(() => props.field, reset, { deep: true, immediate: true });
</script>

<template>
  <span>
    <TarButton
      :icon="field ? 'fas fa-edit' : 'fas fa-plus'"
      :text="t(field ? 'actions.edit' : 'actions.add')"
      :variant="field ? 'primary' : 'success'"
      data-bs-toggle="modal"
      :data-bs-target="`#${modalId}`"
    />
    <TarModal :close="t('actions.close')" :id="modalId" ref="modalRef" size="x-large" :title="t(`fields.definitions.${field ? 'update' : 'create'}`)">
      <form>
        <FieldTypeSelect :disabled="Boolean(field)" :id="field ? `field-type-${field.id}` : undefined" :required="!field" v-model="fieldTypeId" />
        <UniqueNameAlreadyUsed v-model="uniqueNameAlreadyUsed" />
        <div class="row">
          <UniqueNameInput class="col" :id="field ? `unique-name-${field.id}` : undefined" required v-model="uniqueName" />
          <DisplayNameInput class="col" :id="field ? `display-name-${field.id}` : undefined" v-model="displayName" />
        </div>
        <PlaceholderInput :id="field ? `placeholder-${field.id}` : undefined" v-model="placeholder" />
        <DescriptionTextarea :id="field ? `description-${field.id}` : undefined" v-model="description" />
        <div>
          <AppCheckbox
            :disabled="contentType.isInvariant"
            :id="field ? `invariant-${field.id}` : 'invariant-field'"
            label="fields.definitions.invariant"
            tight
            v-model="isInvariant"
          />
          <AppCheckbox :id="field ? `required-${field.id}` : 'required'" label="fields.definitions.required" tight v-model="isRequired" />
          <AppCheckbox :id="field ? `indexed-${field.id}` : 'indexed'" label="fields.definitions.indexed" tight v-model="isIndexed" />
          <AppCheckbox :id="field ? `unique-${field.id}` : 'unique'" label="fields.definitions.unique" tight v-model="isUnique" />
        </div>
      </form>
      <template #footer>
        <TarButton icon="fas fa-ban" :text="t('actions.cancel')" variant="secondary" @click="onCancel" />
        <TarButton
          :disabled="isSubmitting"
          :icon="field ? 'fas fa-save' : 'fas fa-plus'"
          :loading="isSubmitting"
          :status="t('loading')"
          :text="t(field ? 'actions.save' : 'actions.add')"
          type="submit"
          :variant="field ? 'primary' : 'success'"
          @click="onSubmit"
        />
      </template>
    </TarModal>
  </span>
</template>
