<script setup lang="ts">
import { computed, inject, onMounted, ref } from "vue";
import { useForm } from "vee-validate";
import { useRoute, useRouter } from "vue-router";

import AppSaveButton from "@/components/shared/AppSaveButton.vue";
import DescriptionTextarea from "@/components/shared/DescriptionTextarea.vue";
import DisplayNameInput from "@/components/shared/DisplayNameInput.vue";
import NumberPropertiesEdit from "@/components/fields/NumberPropertiesEdit.vue";
import StatusDetail from "@/components/shared/StatusDetail.vue";
import StringPropertiesEdit from "@/components/fields/StringPropertiesEdit.vue";
import UniqueNameInput from "@/components/shared/UniqueNameInput.vue";
import type { ApiError } from "@/types/api";
import type { CreateOrReplaceFieldTypePayload, FieldType, NumberProperties, StringProperties } from "@/types/fields";
import { FIELD_TYPE_UNIQUE_NAME_CHARACTERS } from "@/helpers/constants";
import { handleErrorKey } from "@/inject/App";
import { readFieldType, replaceFieldType } from "@/api/fields";
import { useToastStore } from "@/stores/toast";

const handleError = inject(handleErrorKey) as (e: unknown) => void;
const route = useRoute();
const router = useRouter();
const toasts = useToastStore();

const description = ref<string>("");
const displayName = ref<string>("");
const fieldType = ref<FieldType>();
const number = ref<NumberProperties>({});
const string = ref<StringProperties>({});
const uniqueName = ref<string>("");

const hasChanges = computed<boolean>(() =>
  Boolean(
    fieldType.value &&
      (uniqueName.value !== fieldType.value.uniqueName ||
        displayName.value !== (fieldType.value.displayName ?? "") ||
        description.value !== (fieldType.value.description ?? "") ||
        (fieldType.value.dataType === "Number" &&
          (number.value.minimumValue !== (fieldType.value.number?.minimumValue ?? undefined) ||
            number.value.maximumValue !== (fieldType.value.number?.maximumValue ?? undefined) ||
            number.value.step !== (fieldType.value.number?.step ?? undefined))) ||
        (fieldType.value.dataType === "String" && string.value.minimumLength !== (fieldType.value.string?.minimumLength ?? undefined)) ||
        string.value.maximumLength !== (fieldType.value.string?.maximumLength ?? undefined) ||
        (string.value.pattern ?? "") !== (fieldType.value.string?.pattern ?? "")),
  ),
);

function setModel(model: FieldType): void {
  fieldType.value = model;
  uniqueName.value = model.uniqueName;
  displayName.value = model.displayName ?? "";
  description.value = model.description ?? "";
  number.value = model.number ? { ...model.number } : {};
  string.value = model.string ? { ...model.string } : {};
}

const { handleSubmit, isSubmitting } = useForm();
const onSubmit = handleSubmit(async () => {
  if (fieldType.value) {
    try {
      const payload: CreateOrReplaceFieldTypePayload = {
        uniqueName: uniqueName.value,
        displayName: displayName.value,
        description: description.value,
      };
      switch (fieldType.value.dataType) {
        case "Boolean":
          payload.boolean = {};
          break;
        case "Number":
          payload.number = number.value;
          break;
        case "String":
          payload.string = string.value;
          break;
        case "Tags":
          payload.tags = {};
          break;
      }
      const updatedFieldType: FieldType = await replaceFieldType(fieldType.value.id, payload, fieldType.value.version);
      setModel(updatedFieldType);
      toasts.success("fields.types.updated");
    } catch (e: unknown) {
      handleError(e);
    }
  }
});

onMounted(async () => {
  try {
    const id = route.params.id?.toString();
    if (id) {
      const language = await readFieldType(id);
      setModel(language);
    }
  } catch (e: unknown) {
    const { status } = e as ApiError;
    if (status === 404) {
      router.push({ path: "/not-found" });
    } else {
      handleError(e);
    }
  }
});
</script>

<template>
  <main class="container">
    <template v-if="fieldType">
      <h1>{{ fieldType.displayName ?? fieldType.uniqueName }}</h1>
      <StatusDetail :aggregate="fieldType" />
      <form @submit.prevent="onSubmit">
        <div class="row">
          <UniqueNameInput :allowed-characters="FIELD_TYPE_UNIQUE_NAME_CHARACTERS" class="col" required v-model="uniqueName" />
          <DisplayNameInput class="col" v-model="displayName" />
        </div>
        <DescriptionTextarea v-model="description" />
        <NumberPropertiesEdit v-if="fieldType.dataType === 'Number'" v-model="number" />
        <StringPropertiesEdit v-if="fieldType.dataType === 'String'" v-model="string" />
        <div class="mb-3">
          <AppSaveButton :disabled="isSubmitting || !hasChanges" :loading="isSubmitting" />
        </div>
      </form>
    </template>
  </main>
</template>
