<script setup lang="ts">
import { TarButton, TarCheckbox, TarModal } from "logitar-vue3-ui";
import { computed, ref, watch } from "vue";
import { useForm } from "vee-validate";
import { useI18n } from "vue-i18n";

import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import FieldTypeSelect from "./FieldTypeSelect.vue";
import PlaceholderInput from "./PlaceholderInput.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { ContentType } from "@/types/contents";
import type { CreateOrReplaceFieldDefinitionPayload, FieldDefinition } from "@/types/fields";
import { createFieldDefinition, replaceFieldDefinition } from "@/api/fields";

const { t } = useI18n();

const props = defineProps<{
  contentTypeId: string;
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

const modalId = computed<string>(() => (props.field ? `edit-field-definition-${props.field.id}` : "create-field-definition"));

function hide(): void {
  modalRef.value?.hide();
}

function reset(): void {
  fieldTypeId.value = props.field?.fieldType.id;
  uniqueName.value = props.field?.uniqueName ?? "";
  displayName.value = props.field?.displayName ?? "";
  description.value = props.field?.description ?? "";
  placeholder.value = props.field?.placeholder ?? "";
  isInvariant.value = props.field?.isInvariant ?? false;
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
      ? await replaceFieldDefinition(props.contentTypeId, props.field.id, payload)
      : await createFieldDefinition(props.contentTypeId, payload);
    emit("saved", contentType);
    reset();
    hide();
  } catch (e: unknown) {
    emit("error", e);
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
        <div class="row">
          <UniqueNameInput class="col" :id="field ? `unique-name-${field.id}` : undefined" required v-model="uniqueName" />
          <DisplayNameInput class="col" :id="field ? `display-name-${field.id}` : undefined" v-model="displayName" />
        </div>
        <PlaceholderInput :id="field ? `placeholder-${field.id}` : undefined" v-model="placeholder" />
        <DescriptionTextarea :id="field ? `description-${field.id}` : undefined" v-model="description" />
        <div>
          <TarCheckbox :id="field ? `invariant-${field.id}` : 'invariant-field  '" :label="t('fields.definitions.invariant')" v-model="isInvariant" />
          <TarCheckbox :id="field ? `required-${field.id}` : 'required'" :label="t('fields.definitions.required')" v-model="isRequired" />
          <TarCheckbox :id="field ? `indexed-${field.id}` : 'indexed'" :label="t('fields.definitions.indexed')" v-model="isIndexed" />
          <TarCheckbox :id="field ? `unique-${field.id}` : 'unique'" :label="t('fields.definitions.unique')" v-model="isUnique" />
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
